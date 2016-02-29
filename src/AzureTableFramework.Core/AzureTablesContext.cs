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
    public class AzureTablesContext : IDisposable
    {
        public string PrimaryStorageAccountName { get; set; }
        public string PrimaryStorageAccountKey { get; set; }

        private string _IndexStorageAccountName { get; set; }

        public string IndexStorageAccountName
        {
            get
            {
                return (!string.IsNullOrEmpty(_IndexStorageAccountName)) ?
                    _IndexStorageAccountName : PrimaryStorageAccountName;
            }
            set
            {
                _IndexStorageAccountName = value;
            }
        }

        private string _IndexStorageAccountKey { get; set; }

        public string IndexStorageAccountKey
        {
            get
            {
                return (!string.IsNullOrEmpty(_IndexStorageAccountKey)) ?
                    _IndexStorageAccountKey : PrimaryStorageAccountKey;
            }
            set
            {
                _IndexStorageAccountKey = value;
            }
        }

        private string _EncryptionKey16Chars { get; set; }

        public string EncryptionKey16Chars
        {
            get
            {
                return (!string.IsNullOrEmpty(_EncryptionKey16Chars)) ?
                    _EncryptionKey16Chars : PrimaryStorageAccountKey;
            }
            set
            {
                _EncryptionKey16Chars = value;
            }
        }

        public string SearchServiceName { get; set; }
        public string SearchServiceManagementKey { get; set; }

        public AzureTablesContext()
        {
            InstantiateDictionaries();
        }

        private bool DictionariesInstantiated = false;

        public void InstantiateDictionaries()
        {
            if (DictionariesInstantiated) return;
            DictionariesInstantiated = true;

            foreach (var p in this.GetType().GetProperties())
                if (p.PropertyType.Name.Contains("AzureTableDictionary"))
                {
                    Type t1 = typeof(AzureTableDictionary<>).MakeGenericType(p.PropertyType.GetGenericArguments().First());
                    p.SetValue(this, Activator.CreateInstance(t1, this));
                }
        }

        public AzureTablesContext(AzureTablesContext _securityContext)
        {
            if (!string.IsNullOrEmpty(_securityContext.PrimaryStorageAccountName))
                PrimaryStorageAccountName = _securityContext.PrimaryStorageAccountName;
            if (!string.IsNullOrEmpty(_securityContext.PrimaryStorageAccountKey))
                PrimaryStorageAccountKey = _securityContext.PrimaryStorageAccountKey;

            if (!string.IsNullOrEmpty(_securityContext.IndexStorageAccountName))
                IndexStorageAccountName = _securityContext.IndexStorageAccountName;
            if (!string.IsNullOrEmpty(_securityContext.IndexStorageAccountKey))
                IndexStorageAccountKey = _securityContext.IndexStorageAccountKey;

            if (!string.IsNullOrEmpty(_securityContext.EncryptionKey16Chars))
                EncryptionKey16Chars = _securityContext.EncryptionKey16Chars;

            if (!string.IsNullOrEmpty(_securityContext.SearchServiceName))
                SearchServiceName = _securityContext.SearchServiceName;
            if (!string.IsNullOrEmpty(_securityContext.SearchServiceManagementKey))
                SearchServiceManagementKey = _securityContext.SearchServiceManagementKey;

            InstantiateDictionaries();
        }

        public AzureTablesContext(IConfigurationRoot config)
        {
            var Name = this.GetType().Name.Replace("Context", "");

            if (!string.IsNullOrEmpty(config[Name + ":PrimaryStorageAccountName"]))
                PrimaryStorageAccountName = config[Name + ":PrimaryStorageAccountName"];
            if (!string.IsNullOrEmpty(config[Name + ":PrimaryStorageAccountKey"]))
                PrimaryStorageAccountKey = config[Name + ":PrimaryStorageAccountKey"];

            if (!string.IsNullOrEmpty(config[Name + ":IndexStorageAccountName"]))
                IndexStorageAccountName = config[Name + ":IndexStorageAccountName"];
            if (!string.IsNullOrEmpty(config[Name + ":IndexStorageAccountKey"]))
                IndexStorageAccountKey = config[Name + ":IndexStorageAccountKey"];

            if (!string.IsNullOrEmpty(config[Name + ":EncryptionKey16Chars"]))
                EncryptionKey16Chars = config[Name + ":EncryptionKey16Chars"];

            if (!string.IsNullOrEmpty(config[Name + ":SearchServiceName"]))
                SearchServiceName = config[Name + ":SearchServiceName"];
            if (!string.IsNullOrEmpty(config[Name + ":SearchServiceManagementKey"]))
                SearchServiceManagementKey = config[Name + ":SearchServiceManagementKey"];

            InstantiateDictionaries();
        }

        public CloudStorageAccount PrimaryStorageAccount()
        {
            return Utils.StorageAccount(PrimaryStorageAccountName, PrimaryStorageAccountKey);
        }

        public CloudStorageAccount IndexStorageAccount()
        {
            return Utils.StorageAccount(IndexStorageAccountName, IndexStorageAccountKey);
        }

        public async Task SaveChangesAsync()
        {
            foreach (var p in this.GetType().GetProperties())
                if (p.PropertyType.Name.Contains("AzureTableDictionary"))
                {
                    //TODO: find out if there is a better way to get a typed property value compatible w aspnet5
                    dynamic Dictionary = Convert.ChangeType(p.GetValue(this), typeof(AzureTableDictionary<>).MakeGenericType(p.PropertyType.GetGenericArguments().First()));
                    await ParseBatchCloudAction(Dictionary);
                }
        }

        //--------------------------------------//

        public async Task ParseBatchCloudAction<T>(AzureTableDictionary<T> dictionary)
        {
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

            //TODO: add test for making sure that all other Rowkeys are deleted
            foreach (var item in ItemsToSave)
            {
                // if (!string.IsNullOrEmpty((item as AzureTableEntity).PartitionKey))
                // if (!string.IsNullOrEmpty((item as AzureTableEntity).ETag)) //? not sure if we need to look them all up
                string ItemPartitionKeyValue = Utils.GetPartitionKeyValue(item, false);

                var foundItems = await dictionary.GetAllByIDAsync(Utils.GetRowKeyValue(item));

                foreach (var found in foundItems.AsNotNull())
                    if (ItemPartitionKeyValue != Utils.GetPartitionKeyValue(found, false))
                        ItemsToHardDelete.Add(found);
            }

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

        private async Task<int> BatchCloudActionAddItems<T>(AzureTableDictionary<T> dictionary, List<T> list)
        {
            // if (!list.Any()) return 0;

            foreach (var item in list)
            {
                CheckRequiredFields(item);
                //(item as AzureTableEntity).Timestamp = _timestamp;
            }

            var results = await BatchCloudTableOperation(typeof(T).Name, PrimaryStorageAccount(),
                Utils.SetRowAndPartitionKeyPropertiesFromTypedObjectList(list), false);

            foreach (var item in results)
                dictionary.Items[Utils.GetRowKeyValue(item)] = item;

            var Indexes = Utils.GetIndexes(results);

            var indexCount = 0;

            foreach (var key in Indexes.Keys)
            {
                await BatchCloudTableOperation(key, IndexStorageAccount(), Indexes[key], false);
                indexCount = indexCount + Indexes[key].Count;
            }

            return indexCount;
        }

        private async Task<int> BatchCloudActionSoftDeleteItems<T>(AzureTableDictionary<T> dictionary, List<T> list)
        {
            // if (!list.Any()) return 0;

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
                dictionary.Items.Remove(Utils.GetRowKeyValue(item));

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
                    var typedBatchResults = Utils.TableResultsToTypedList<T>(batchResultsList);

                    results.AddRange(typedBatchResults);
                    operations++;

                    //+00:00:00.1438848 #1  1 item(s) with PK 654564
                    Debug.WriteLine("+" + BatchSW.Elapsed + " #" + operations + " \t" + typedBatchResults.Count + " item" + (typedBatchResults.Count == 1 ? "" : "s")
                                                + " with PK " + (EditableList.First() as TableEntity).PartitionKey);

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
                if (x.GetValue(obj) == null) throw new Exception(string.Format("Required field {0} on {1} with ID of {2} is Null or Empty", x.Name, typeof(T).Name, Utils.GetRowKeyValue(obj)));
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

    public static partial class Utils
    {
        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        public static Dictionary<string, List<T>> GetIndexes<T>(IEnumerable<T> list)
        {
            var type = list.First().GetType();
            var RowKeyPropertyName = GetRowKeyPropertyName(type);

            var IndexObjectsDictionary = new Dictionary<string, List<T>>();

            var NonPartitionIndexProperties = GetNonPartitionIndexProperties(typeof(T));
            foreach (var property in NonPartitionIndexProperties)
            {
                var IndexName = GetIndexTableName(type.Name, property.Name);
                IndexObjectsDictionary.Add(IndexName, new List<T>());

                foreach (var obj in list)
                {
                    if (property.GetValue(obj) == null) throw new Exception("Indexed field " + property.Name + " on " + typeof(T).Name +
                        " with ID of " + Utils.GetRowKeyValue(obj) + " is Null or Empty." +
                        "Indexed fields can not be null");

                    var objClone = Clone(obj);

                    SetRowKeyValueFromObjectIDProperty(RowKeyPropertyName, objClone);

                    SetVal(objClone, "PartitionKey", GetVal(objClone, property.Name).ToString().MakePartitionAndRowKeysAzureSafe());

                    //(objClone as ElasticTableEntity).Properties.Remove("IsSoftDeleted");
                    //(objClone as ElasticTableEntity).Properties.Remove("HardDeleteWithBatch");

                    SetVal(objClone, "ETag", "*");

                    IndexObjectsDictionary[IndexName].Add(objClone);
                }
            }

            return IndexObjectsDictionary;
        }

        public static List<PropertyInfo> GetNonPartitionIndexProperties(Type type)
        {
            var IndexedProperties = type.GetProperties().Where(x => x.GetCustomAttributes(typeof(IndexAttribute), true).Any()).ToList();
            var PartitionKeyProperty = type.GetProperties().Where(x => x.GetCustomAttributes(typeof(PartitionKeyAttribute), true).Any()).First();

            var indexes = new List<PropertyInfo>();

            foreach (var property in IndexedProperties)
            {
                //no reason to make an index on the partitionkey
                if (PartitionKeyProperty == property) continue;

                indexes.Add(property);
            }

            return indexes;
        }

        public static List<T> SetRowAndPartitionKeyPropertiesFromTypedObjectList<T>(IEnumerable<T> list)
        {
            var clonedList = list.Clone();

            foreach (var obj in clonedList)
            {
                //Debug.WriteLine("PartitionKey Before = " + GetVal(obj, "PartitionKey"));
                SetPartitionKeyFromObjectPartitionKeyProperty(GetPartitionKeyPropertyName(list.First().GetType()), obj);
                //Debug.WriteLine("PartitionKey After = " + GetVal(obj, "PartitionKey"));
                SetRowKeyValueFromObjectIDProperty(GetRowKeyPropertyName(list.First().GetType()), obj);
            }

            return clonedList.ToList();
        }

        public static void SetPartitionKeyFromObjectPartitionKeyProperty<T>(string PartitionKeyPropertyName, T obj)
        {
            var PK = GetPartitionKeyValue(PartitionKeyPropertyName, obj, true);
            var SafePK = PK.MakePartitionAndRowKeysAzureSafe();

            SetVal(obj, "PartitionKey", SafePK);

            if (PK.Equals(SafePK)) SetVal(obj, PartitionKeyPropertyName, null);
        }

        public static void SetRowKeyValueFromObjectIDProperty<T>(string RowKeyPropertyName, T obj)
        {
            var RK = GetRowKeyValue(obj);
            var SafeRK = RK.MakePartitionAndRowKeysAzureSafe();

            SetVal(obj, "RowKey", SafeRK);

            if (RK.Equals(SafeRK)) SetVal(obj, RowKeyPropertyName, null);
        }

        public static string MakePartitionAndRowKeysAzureSafe(this string key)
        {
            return new List<char> { '\\', '#', '/', '%', '?' }.Aggregate(key, (current, c) => current.Replace(c, '_'));
        }
    }
}