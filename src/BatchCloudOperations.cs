using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AzureTableFramework
{
    public partial class AzureTableContext : IDisposable
    {
        public async Task ParseBatchCloudAction<T>(AzureTableDictionary<T> dictionary)
        {
            var SW = Stopwatch.StartNew();

            if (!dictionary.Items.Any()) return;

            var IndexesAdded = 0; var IndexesDeleted = 0;

            var ItemsToSave = new List<T>();
            var ItemsToSoftDelete = new List<T>();
            var ItemsToHardDelete = new List<T>();

            foreach (var item in dictionary.Items)
                if ((item as AzureTableEntity)._IsSoftDeleted)
                    ItemsToSoftDelete.Add(item);
                else if ((item as AzureTableEntity)._HardDeleteWithBatch)
                    ItemsToHardDelete.Add(item);
                else
                    ItemsToSave.Add(item);

            if (ItemsToSoftDelete.Count > 0 || ItemsToHardDelete.Count > 0)
                NewInsertsOnlyOperation = false;

            if (ItemsToSoftDelete.Any())
                IndexesDeleted = IndexesDeleted + await BatchCloudActionSoftDeleteItems(dictionary, ItemsToSoftDelete);
            if (ItemsToHardDelete.Any())
                IndexesDeleted = IndexesDeleted + await BatchCloudActionHardDeleteItems(dictionary, ItemsToHardDelete);
            if (ItemsToSave.Any())
                IndexesAdded = IndexesAdded + await BatchCloudActionAddItems(dictionary, ItemsToSave);

            Debug.WriteLine(string.Format(
                "=== Items: {3} Added, {4} SoftDeleted, {5} HardDeleted --- Indexes: {6} Added, {7} Deleted \n" +
                "=== End Batch Cloud Action Operations for {0} @ {1} Elapsed {2} \n"
                , typeof(T), DateTime.UtcNow, SW.Elapsed, ItemsToSave.Count, ItemsToSoftDelete.Count, ItemsToHardDelete.Count, IndexesAdded, IndexesDeleted));

            //dictionary.Items = ItemsToSave;
        }

        private async Task<int> UpdateIndexesForListItems<T>(List<T> list, bool delete)
        {
            var IndexChangeCount = 0;
            foreach (var item in list)
                IndexChangeCount = IndexChangeCount + await UpdateObjectIndexes(item, delete);

            return IndexChangeCount;
        }

        private async Task<int> UpdateObjectIndexes(object item, bool delete)
        {
            var IndexChangeCount = 0;

            foreach (var prop in item.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(IndexAttribute), true).Any()))
            {
                var IA = (IndexAttribute)prop.GetCustomAttribute(typeof(IndexAttribute), false);

                var indexVersionOfObj = IA.PartitionKeyOnly ? new AzureTableEntity() : item.Clone() as AzureTableEntity;
                indexVersionOfObj._IsIndexVersion = true;

                var propKeyValue = Utils.GetVal(item, prop.Name).ToString();
                var rowKeyValue = Utils.GetRowKeyValue(item);

                indexVersionOfObj.PartitionKey = propKeyValue.MakeAzureSafe();
                indexVersionOfObj.RowKey = rowKeyValue.MakeAzureSafe();

                if (!IA.PartitionKeyOnly)
                {
                    if (indexVersionOfObj.PartitionKey.Equals(propKeyValue))
                        Utils.SetVal(indexVersionOfObj, prop.Name, null);

                    if (indexVersionOfObj.RowKey.Equals(rowKeyValue))
                        Utils.SetVal(indexVersionOfObj, Utils.GetRowKeyPropertyName(item.GetType()), null);
                }

                var indexedProperties = IA.Properties ?? new List<string>() { prop.Name };
                var indexTableName = Utils.IndexTableName(item, indexedProperties);

                if (delete)
                {
                    var table = await Utils.GetCloudTableAsync(indexTableName, IndexStorageAccount(), false);
                    var filterstring = Utils.FilterString("RowKey", QueryComparisons.Equal, indexVersionOfObj.RowKey);

                    if (indexTableName.EndsWith("LastUpdated"))
                    {
                        if (item.GetType().GetTypeInfo().GetCustomAttributes().Where(x => x.GetType().Name == "BackupAttribute").Any())
                            continue;

                        ///TODO: set a partitionkey filter of ge + le a few seconds from timestamp
                    }
                    else
                    {
                        var partitionKeyFilter = Utils.FilterString("PartitionKey", QueryComparisons.Equal, indexVersionOfObj.PartitionKey);
                        filterstring = Utils.CombineFilterStrings(partitionKeyFilter, filterstring);
                    }

                    var tq = new TableQuery() { FilterString = filterstring };
                    var tqs = await table.ExecuteQuerySegmentedAsync(tq, null);
                    var ATEs = Utils.DynamicResultsToAzureTableEntityList(item.GetType(), item, tqs.Results);

                    IndexChangeCount = IndexChangeCount + (await BatchCloudTableOperation(indexTableName, IndexStorageAccount(), ATEs, delete)).Count;
                }
                else
                    IndexChangeCount = IndexChangeCount + (await BatchCloudTableOperation(indexTableName, IndexStorageAccount(), new List<AzureTableEntity> { indexVersionOfObj }, delete)).Count;
            }

            return IndexChangeCount;
        }

        private async Task<int> DeleteOldRows<T>(AzureTableDictionary<T> dictionary, T item)
        {
            var OperationSW = Stopwatch.StartNew();

            var rowKeyValue = Utils.GetRowKeyValue(item);
            var rowkeyFilter = Utils.FilterString("RowKey", QueryComparisons.Equal, rowKeyValue);
            var timestampFilter = Utils.FilterString("Timestamp", QueryComparisons.LessThan, DateTime.UtcNow.AddMinutes(-2));
            var oldRowKeysFilter = Utils.CombineFilterStrings(new List<string>() { rowkeyFilter, timestampFilter });

            var oldRows = await dictionary.QueryAllAsync(oldRowKeysFilter) ?? new List<T>();
            await BatchCloudTableOperation(typeof(T).Name, PrimaryStorageAccount(), oldRows, true);

            Debug.WriteLine($"== Old Row Delete Operation on {item.GetType().Name} {(item as AzureTableEntity).RowKey} found = {oldRows.Count} {OperationSW.Elapsed.ToString()} =========");
            return oldRows.Count;
        }

        private async Task<int> DeleteOldRows<T>(AzureTableDictionary<T> dictionary, List<T> items)
        {
            Debug.WriteLine($"== Start DeleteOldRows => Old Row Batch Delete Operation on {items.First().GetType().Name} {items.Count} =========");
            var OperationSW = Stopwatch.StartNew();
            int DeletedOldRowsCount = 0;
            foreach (var item in items)
                DeletedOldRowsCount += await DeleteOldRows(dictionary, item);
            Debug.WriteLine($"== End DeleteOldRows   => Old Row Batch Delete Operation on {items.First().GetType().Name} {items.Count} {OperationSW.Elapsed.ToString()} =========");
            return DeletedOldRowsCount;
        }

        private async Task<int> BatchCloudActionAddItems<T>(AzureTableDictionary<T> dictionary, List<T> list)
        {
            if (!list.Any()) return 0;

            foreach (var item in list)
            {
                CheckRequiredFields(item);
                (item as AzureTableEntity).Timestamp = DateTime.UtcNow;
            }

            var results = await BatchCloudTableOperation(typeof(T).Name, PrimaryStorageAccount(),
            Utils.SetRowAndPartitionKeys(list), false);

            if (!NewInsertsOnlyOperation)
                DeleteOldRows(dictionary, results);

            return await UpdateIndexesForListItems(results, false);
        }

        private async Task<int> BatchCloudActionSoftDeleteItems<T>(AzureTableDictionary<T> dictionary, List<T> list)
        {
            if (!list.Any()) return 0;

            //foreach (var item in list)
            //    await Utils.DeleteBlobs(item, PrimaryStorageAccount());

            await BatchCloudTableOperation(list.First().GetType().Name, PrimaryStorageAccount(),
                    Utils.SetRowAndPartitionKeys(list), false);

            return await UpdateIndexesForListItems(list, true);
        }

        private async Task<int> BatchCloudActionHardDeleteItems<T>(AzureTableDictionary<T> dictionary, List<T> list)
        {
            if (!list.Any()) return 0;

            //foreach (var item in list)
            //    await Utils.DeleteBlobs(item, PrimaryStorageAccount());

            await BatchCloudTableOperation(list.First().GetType().Name, PrimaryStorageAccount(),
                    Utils.SetRowAndPartitionKeys(list), true);

            return await UpdateIndexesForListItems(list, true);
        }

        private async Task<List<T>> BatchCloudTableOperation<T>(string TableName, CloudStorageAccount StorageAccount, List<T> items, bool Delete)
        {
            if (items == null || !items.Any()) return null;

            Debug.WriteLine($"\n=== Batch {((Delete) ? "Delete" : "Upsert")} Operation on {TableName} =========");
            var OperationSW = Stopwatch.StartNew();

            var table = await Utils.GetCloudTableAsync(TableName, StorageAccount, true);

            var PartitionSortedList = new SortedList<string, List<T>>();

            foreach (var item in items)
            {
                if (Delete && Utils.GetVal(item, "ETag") == null)
                    (item as AzureTableEntity).ETag = "*";

                var PK = Utils.GetVal(item, "PartitionKey").ToString();
                if (PK == null || String.IsNullOrEmpty(PK))
                    Debug.WriteLine("PK == null");

                if (!PartitionSortedList.ContainsKey(PK)) PartitionSortedList.Add(PK, new List<T>());
                PartitionSortedList[PK].Add(item);
            }

            var results = new List<T>();

            int operations = 0;
            foreach (List<T> ListOfObjectsWithTheSamePartitionKey in PartitionSortedList.Values)
            {
                var EditableList = ListOfObjectsWithTheSamePartitionKey;
                while (EditableList.Any())
                {
                    var BatchSW = Stopwatch.StartNew();
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

                    EditableList = EditableList.Skip(100).ToList();
                }
                EditableList.Clear();
            }

            PartitionSortedList.Clear();

            //=00:00:00.4292620   2 Total on Blog
            Debug.WriteLine($"={OperationSW.Elapsed}  \t\t{items.Count} Total on {TableName} ===");

            return results;
        }

        private void CheckRequiredFields<T>(T obj)
        {
            foreach (var x in typeof(T).GetProperties().Where(x => x.GetCustomAttributes(typeof(RequiredAttribute), false).Any()))
                if (x.GetValue(obj) == null) throw new Exception($"Required field {x.Name} on {typeof(T).Name} with ID of {Utils.GetRowKeyValue(obj)} is Null or Empty");
        }
    }
}