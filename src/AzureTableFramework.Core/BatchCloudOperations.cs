using AzureTableFramework.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    public partial class AzureTablesContext : IDisposable
    {
        public async Task ParseBatchCloudAction<T>(AzureTableDictionary<T> dictionary)
        {
            dictionary.Timestamp = DateTime.UtcNow;
            var SW = new Stopwatch(); SW.Start();

            var allItems = dictionary.Items.Values.ToList();
            if (!allItems.Any()) return;

            var IndexesAdded = 0; var IndexesDeleted = 0;

            var ItemsToSave = new List<T>();
            var ItemsToSoftDelete = new List<T>();
            var ItemsToHardDelete = new List<T>();

            foreach (var item in allItems)
                if ((item as AzureTableEntity)._IsSoftDeleted)
                    ItemsToSoftDelete.Add(item);
                else if ((item as AzureTableEntity)._HardDeleteWithBatch)
                    ItemsToHardDelete.Add(item);
                else
                    ItemsToSave.Add(item);

            //need not be awaited in production
            await deleteObsoleteItems(dictionary, ItemsToSave);

            if (ItemsToSoftDelete.Any())
                IndexesDeleted = IndexesDeleted + await BatchCloudActionSoftDeleteItems(dictionary, ItemsToSoftDelete);
            if (ItemsToHardDelete.Any())
                IndexesDeleted = IndexesDeleted + await BatchCloudActionHardDeleteItems(dictionary, ItemsToHardDelete);
            if (ItemsToSave.Any())
                IndexesAdded = IndexesAdded + await BatchCloudActionAddItems(dictionary, ItemsToSave);

            Debug.WriteLine(string.Format(
                "=== Items: {3} Added, {4} SoftDeleted, {5} HardDeleted --- Indexes: {6} Added, {7} Deleted \n\n" +
                "=== End Batch Cloud Action Operations for {0} @ {1} Elapsed {2} \n"
                , typeof(T), DateTime.UtcNow, SW.Elapsed, ItemsToSave.Count, ItemsToSoftDelete.Count, ItemsToHardDelete.Count, IndexesAdded, IndexesDeleted));
        }

        private async Task deleteObsoleteItems<T>(AzureTableDictionary<T> dictionary, List<T> ItemsToSave)
        {
            foreach (var item in ItemsToSave)
            {
                var primaryItemsToDelete = new List<object>();

                (item as TableEntity).Timestamp = dictionary.Timestamp;
                // if (!string.IsNullOrEmpty((item as AzureTableEntity).PartitionKey))
                // if (!string.IsNullOrEmpty((item as AzureTableEntity).ETag)) //? not sure if we need to look them all up
                var ItemRowKeyValue = Utils.GetRowKeyValue(item);
                string ItemPartitionKeyValue = Utils.GetPartitionKeyValue(item, false);

                //try
                //{
                var primaryItemTable = await Utils.GetCloudTableAsync(item.GetType().Name, PrimaryStorageAccount(), true);

                if (primaryItemTable != null)
                {
                    var otherPrimaryItemsWithTheSameRowKey = await dictionary.GetAllByRowKeyAsync(ItemRowKeyValue);
                    foreach (var found in otherPrimaryItemsWithTheSameRowKey.AsNotNull())
                        if (ItemPartitionKeyValue != Utils.GetPartitionKeyValue(found, false))
                            primaryItemsToDelete.Add(found);

                    if (primaryItemsToDelete.Any())
                        await BatchCloudTableOperation(item.GetType().Name, PrimaryStorageAccount(), primaryItemsToDelete, true);
                }
                //}
                //catch (Exception ex) { Debug.WriteLine(ex.Message); }

                // ^^^  Making sure that all other primary items with the same RowKey are deleted

                var q = Utils.FilterString("RowKey", QueryComparisons.Equal, ItemRowKeyValue);

                foreach (var indexProperty in Utils.GetNonPartitionIndexProperties(item.GetType()))
                {
                    var indexesToDelete = new List<object>();

                    var indexTableName = Utils.IndexTableName(item.GetType().Name, indexProperty.Name);

                    //try
                    //{
                    Debug.WriteLine("Checking for old indexes on " + indexTableName);
                    var table = await Utils.GetCloudTableAsync(indexTableName, IndexStorageAccount(), false);

                    if (table != null)
                    {
                        var tq = new TableQuery { FilterString = q };

                        var data = await table.ExecuteQuerySegmentedAsync(tq, null);

                        if (data != null && data.Results != null && (bool)data?.Results?.Any())
                        {
                            var AllOldIndexes = Utils.DynamicResultsToTypedList<T, DynamicTableEntity>(data.Results.ToList());
                            foreach (var index in AllOldIndexes)
                                if ((index as TableEntity).Timestamp < dictionary.Timestamp)
                                {
                                    indexesToDelete.Add(index);
                                    //Debug.WriteLine(string.Format(" Deleting old index: {0} => {1}", indexTableName, (index as TableEntity).PartitionKey));
                                }

                            await BatchCloudTableOperation(indexTableName, IndexStorageAccount(), indexesToDelete, true);
                        }
                    }
                    //}
                    //catch (Exception ex) { Debug.WriteLine(ex); }
                }

                //GetDynamicIndexProperties
            }
        }

        private async Task<int> BatchCloudActionAddItems<T>(AzureTableDictionary<T> dictionary, List<T> list)
        {
            // if (!list.Any()) return 0;

            foreach (var item in list)
            {
                CheckRequiredFields(item);
                //(item as AzureTableEntity).Timestamp = _timestamp;

                //await Utils.SaveBlobs(item, PrimaryStorageAccount());
            }

            var results = await BatchCloudTableOperation(typeof(T).Name, PrimaryStorageAccount(),
                Utils.SetRowAndPartitionKeyPropertiesFromTypedObjectList(list), false);

            foreach (var item in results)
                dictionary.Items[Utils.GetRowKeyValue(item)] = item;

            var Indexes = Utils.GetIndexes(results);

            var indexCount = 0;

            foreach (var key in Indexes.Keys)
            {
                foreach (var item in Indexes[key]) (item as TableEntity).Timestamp = dictionary.Timestamp;

                await BatchCloudTableOperation(key, IndexStorageAccount(), Indexes[key], false);
                indexCount = indexCount + Indexes[key].Count;
            }

            foreach (var item in results)
                foreach (var dynamicIdxProp in item.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(DynamicIndexAttribute), true).Any()))
                {
                    var props = ((DynamicIndexAttribute)dynamicIdxProp.GetCustomAttribute(typeof(DynamicIndexAttribute), false)).Properties;
                    var DynamicIndexTableName = Utils.IndexTableName(item, props);
                    var DynamicIndexValue = Utils.DynamicIndexValue(item, dynamicIdxProp.Name);
                    var SingleItemList = new List<object> { DynamicIndexValue };

                    await BatchCloudTableOperation(
                        DynamicIndexTableName,
                        IndexStorageAccount(),
                        SingleItemList,
                        false);

                    indexCount = indexCount + 1;
                }

            return indexCount;
        }

        private async Task<int> BatchCloudActionSoftDeleteItems<T>(AzureTableDictionary<T> dictionary, List<T> list)
        {
            // if (!list.Any()) return 0;

            foreach (var item in list)
                await Utils.DeleteBlobs(item, PrimaryStorageAccount());

            //foreach (var item in list)
            //{
            //    //(item as TableEntity).Timestamp = _timestamp;
            //    //TODO: add SoftDelete   ((dynamic)item).IsSoftDeleted = true;
            //}

            var results = await BatchCloudTableOperation(list.First().GetType().Name, PrimaryStorageAccount(),
                Utils.SetRowAndPartitionKeyPropertiesFromTypedObjectList(list), false);

            foreach (var item in results)
                dictionary.Items[Utils.GetRowKeyValue(item)] = item;

            var Indexes = Utils.GetIndexes(results);

            var indexCount = 0;

            foreach (var key in Indexes.Keys)
            {
                await BatchCloudTableOperation(key, IndexStorageAccount(), Indexes[key], true);
                indexCount = indexCount + Indexes[key].Count;
            }

            return indexCount;
        }

        private async Task<int> BatchCloudActionHardDeleteItems<T>(AzureTableDictionary<T> dictionary, List<T> list)
        {
            // if (!list.Any()) return 0;

            var results = await BatchCloudTableOperation(list.First().GetType().Name, PrimaryStorageAccount(),
                Utils.SetRowAndPartitionKeyPropertiesFromTypedObjectList(list), true);

            foreach (var item in list)
            {
                dictionary.Items.Remove(Utils.GetRowKeyValue(item));
                await Utils.DeleteBlobs(item, PrimaryStorageAccount());
            }

            var Indexes = Utils.GetIndexes(results);

            var indexCount = 0;

            foreach (var key in Indexes.Keys)
            {
                await BatchCloudTableOperation(key, IndexStorageAccount(), Indexes[key], true);
                indexCount = indexCount + Indexes[key].Count;
            }

            return indexCount;
        }

        private async Task<List<T>> BatchCloudTableOperation<T>(string TableName, CloudStorageAccount StorageAccount, List<T> list, bool Delete)
        {
            Debug.WriteLine(string.Format("\n Batch {0} Operation on {1} =========", (Delete) ? "Delete" : "Upsert", TableName));
            var OperationSW = new Stopwatch(); OperationSW.Start();

            var table = await Utils.GetCloudTableAsync(TableName, StorageAccount, true);

            var PartitionSortedList = new SortedList<string, List<T>>();

            foreach (var obj in list)
            {
                if (Delete && Utils.GetVal(obj, "ETag") == null)
                    (obj as AzureTableEntity).ETag = "*";

                var PK = Utils.GetVal(obj, "PartitionKey").ToString();
                if (!PartitionSortedList.ContainsKey(PK)) PartitionSortedList.Add(PK, new List<T>());
                PartitionSortedList[PK].Add(obj);
            }

            var results = new List<T>();

            int operations = 0;
            foreach (List<T> ListOfObjectsWithTheSamePartitionKey in PartitionSortedList.Values)
            {
                var EditableList = ListOfObjectsWithTheSamePartitionKey;
                while (EditableList.Any())
                {
                    var BatchSW = new Stopwatch(); BatchSW.Start();
                    var batchOperation = new TableBatchOperation();

                    Action<T> takeAction = o => batchOperation.InsertOrReplace(o as TableEntity);
                    if (Delete) takeAction = o => batchOperation.Delete(o as TableEntity);

                    EditableList.Take(100).ToList().ForEach(takeAction);

                    var batchResultsList = await table.ExecuteBatchAsync(batchOperation);
                    try
                    {
                        var typedBatchResults = Utils.TableResultsToTypedList<T>(batchResultsList);
                        results.AddRange(typedBatchResults);

                        Debug.WriteLine("+" + BatchSW.Elapsed + " #" + operations + " \t" + typedBatchResults.Count
                                            + " item" + (typedBatchResults.Count == 1 ? "" : "s")
                                            + " with PK " + (EditableList.First() as TableEntity).PartitionKey);
                    }
                    catch (Exception EX)
                    {
                        Debug.WriteLine(EX.Message);
                    }
                    operations++;

                    //+00:00:00.1438848 #1  1 item(s) with PK 654564

                    EditableList = EditableList.Skip(100).ToList();
                }
                EditableList.Clear();
            }

            PartitionSortedList.Clear();

            //=00:00:00.4292620   2 Total on Blog
            Debug.WriteLine(string.Format("={0}  \t\t{1} Total on {2} === \n", OperationSW.Elapsed, list.Count, TableName));

            return results;
        }

        private void CheckRequiredFields<T>(T obj)
        {
            foreach (var x in typeof(T).GetProperties().Where(x => x.GetCustomAttributes(typeof(RequiredAttribute), false).Any()))
                if (x.GetValue(obj) == null) throw new Exception($"Required field {x.Name} on {typeof(T).Name} with ID of {Utils.GetRowKeyValue(obj)} is Null or Empty");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //var Dictionaries = GetDictionaries();
                //if (Dictionaries != null)
                //    foreach (var D in Dictionaries)
                //        D.Items.Clear();
            }
        }
    }
}