using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Linq;
using System.Reflection;

using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    public static partial class Utils
    {
        private static SortedList<string, CloudTable> _CloudTables = new SortedList<string, CloudTable>();

        public static SortedList<string, CloudTable> CloudTables { get { return _CloudTables; } set { _CloudTables = value; } }

        public static CloudStorageAccount StorageAccount(string StorageAccountName, string StorageAccountKey)
        {
            var SC = new StorageCredentials(StorageAccountName, StorageAccountKey);
            return new CloudStorageAccount(SC, true);
        }

        public static async Task<CloudTable> GetCloudTableAsync(string TableName, CloudStorageAccount AzureStorageAccount, bool CreateIfNotExist)
        {
            if (CloudTables.ContainsKey(TableName)) return CloudTables[TableName];

            var TableClient = AzureStorageAccount.CreateCloudTableClient();
            TableClient.DefaultRequestOptions.PayloadFormat = TablePayloadFormat.JsonNoMetadata;
            var Table = TableClient.GetTableReference(TableName);

            try
            {
                if (CreateIfNotExist) await Table.CreateIfNotExistsAsync();
            }
            catch (Exception EX)
            {
                throw new Exception("Error creating the table, " +
                    "the most likely problem is the name or key for the storage account is wrong. " +
                    "Original Message: " + EX.Message);
            }

            CloudTables.Add(TableName, Table);

            return Table;
        }

        public static async Task<bool> DeleteTableIfEmptyAsync(CloudTable table)
        {
            var filterString = FilterString((() => new TableEntity().PartitionKey), QueryComparisons.NotEqual, null);
            var TQ = new TableQuery<TableEntity> { FilterString = filterString }.Take(1);

            var segment = await table.ExecuteQuerySegmentedAsync(TQ, null);
            if (segment == null || !segment.Results.Any())
            {
                await table.DeleteAsync();
                if (CloudTables.ContainsKey(table.Name))
                    CloudTables.Remove(table.Name);
                return true;
            }

            return false;
        }

        public static async Task BatchCloudAction<T>(AzureTablesContext securityContext, List<T> list, bool Delete, DateTime _timestamp)
        {
            if (!list.Any()) return;

            foreach (var obj in list) (obj as TableEntity).Timestamp = _timestamp;

            var ticks = Utils.TicksFromMax(_timestamp);

            Debug.WriteLine("Batch Cloud Action part: " + _timestamp + " = " + ticks + " = " + Utils.UTCDateTimeFromTicksFromMax(ticks));

            if (!Delete) foreach (var obj in list) CheckRequiredFields(obj);

            var Indexes = GetIndexes(list);

            foreach (var key in Indexes.Keys)
                await BatchCloudTableOperation(key, securityContext.IndexStorageAccount(), Indexes[key], Delete);

            await BatchCloudTableOperation(list.First().GetType().Name, securityContext.PrimaryStorageAccount(), FixRowAndPartitionKeys(list), Delete);
        }

        private static async Task BatchCloudTableOperation<T>(string TableName, CloudStorageAccount StorageAccount, List<T> list, bool Delete)
        {
            Debug.WriteLine("\n Batch " + ((Delete) ? "Delete" : "Upsert") + " Operation on " + TableName + " =========");
            var OperationSW = new Stopwatch(); OperationSW.Start();

            var table = await GetCloudTableAsync(TableName, StorageAccount, true);

            var PartitionSortedList = new SortedList<string, List<T>>();

            foreach (var obj in list)
            {
                if (Delete)
                    if (GetVal(obj, "ETag") == null)
                        (obj as AzureTableEntity).ETag = "*";

                var PK = GetVal(obj, "PartitionKey").ToString();
                if (!PartitionSortedList.ContainsKey(PK)) PartitionSortedList.Add(PK, new List<T>());
                PartitionSortedList[PK].Add(obj);
            }

            int operations = 0;
            foreach (var PKList in PartitionSortedList.Values)
            {
                var EditableList = PKList;
                while (EditableList.Any())
                {
                    var BatchSW = new Stopwatch(); BatchSW.Start();
                    var batchOperation = new TableBatchOperation();

                    Action<T> takeAction = o => batchOperation.InsertOrReplace(o as TableEntity);
                    if (Delete) takeAction = o => batchOperation.Delete(o as TableEntity);

                    EditableList.Take(100).ToList().ForEach(takeAction);

                    await table.ExecuteBatchAsync(batchOperation);
                    operations++;

                    //+00:00:00.1438848 #1  1 item(s) with PK 654564
                    Debug.WriteLine("+" + BatchSW.Elapsed + " #" + operations + " \t" + EditableList.Count + " item" + (EditableList.Count == 1 ? "" : "s")
                                                + " with PK " + (EditableList.First() as TableEntity).PartitionKey);

                    EditableList = EditableList.Skip(100).ToList();
                }
                EditableList.Clear();
            }

            PartitionSortedList.Clear();

            //=00:00:00.4292620   2 Total on Blog
            Debug.WriteLine("=" + OperationSW.Elapsed + "  \t\t" + list.Count + " Total on " + TableName + " === \n");
        }

        public static List<PropertyInfo> GetNonPartitionIndexProperties(Type type)
        {
            var IndexedProperties = type.GetProperties().Where(x => x.GetCustomAttributes(typeof(IndexAttribute), true).Any()).ToList();
            var PartitionKeyProperty = type.GetProperties().Where(x => x.GetCustomAttributes(typeof(PartionKeyAttribute), true).Any()).First();

            var indexes = new List<PropertyInfo>();

            foreach (var property in IndexedProperties)
            {
                //no reason to make an index on the partitionkey
                if (PartitionKeyProperty == property) continue;

                indexes.Add(property);
            }

            return indexes;
        }

        private static Dictionary<string, List<T>> GetIndexes<T>(List<T> list)
        {
            var type = list.First().GetType();
            var RowKeyPropertyName = GetRowKeyPropertyName(type);

            var IndexedProperties = GetNonPartitionIndexProperties(typeof(T));
            var Indexes = new Dictionary<string, List<T>>();

            foreach (var property in IndexedProperties)
            {
                var IndexName = GetIndexTableName(type.Name, property.Name);
                Indexes.Add(IndexName, new List<T>());

                foreach (var obj in list)
                {
                    if (property.GetValue(obj) == null) throw new Exception("Indexed field " + property.Name + " on " + typeof(T).Name +
                        " with ID of " + GetRowKeyValue(obj) + " is Null or Empty." +
                        "Indexed fields can not be null");

                    var objClone = Clone(obj);

                    FixRowKey(RowKeyPropertyName, objClone);

                    SetVal(objClone, "PartitionKey", GetVal(objClone, property.Name).ToString().MakePartitionAndRowKeysAzureSafe());

                    Indexes[IndexName].Add(objClone);
                }
            }

            return Indexes;
        }

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

        private static void CheckRequiredFields<T>(T obj)
        {
            foreach (var x in typeof(T).GetProperties().Where(x => x.GetCustomAttributes(typeof(RequiredAttribute), false).Any()))
                if (x.GetValue(obj) == null) throw new Exception("Required field " + x.Name + " on " + typeof(T).Name + " with ID of " + GetRowKeyValue(obj) + " is Null or Empty");
        }

        public static string MakePartitionAndRowKeysAzureSafe(this string key)
        {
            return new List<char> { '\\', '#', '/', '%', '?' }.Aggregate(key, (current, c) => current.Replace(c, '_'));
        }

        private static List<T> FixRowAndPartitionKeys<T>(List<T> list)
        {
            var clonedList = list.Clone();

            foreach (var obj in clonedList)
            {
                FixPartitionKey(GetPartitionKeyPropertyName(list.First().GetType()), obj);
                FixRowKey(GetRowKeyPropertyName(list.First().GetType()), obj);
            }

            return clonedList;
        }

        private static void FixPartitionKey<T>(string PartitionKeyPropertyName, T obj)
        {
            var PK = GetPartitionKeyValue(PartitionKeyPropertyName, obj);
            var SafePK = PK.MakePartitionAndRowKeysAzureSafe();

            SetVal(obj, "PartitionKey", SafePK);

            if (PK.Equals(SafePK))
                SetVal(obj, PartitionKeyPropertyName, null);
        }

        private static void FixRowKey<T>(string RowKeyPropertyName, T obj)
        {
            var RK = GetRowKeyValue(obj);
            var SafeRK = RK.MakePartitionAndRowKeysAzureSafe();

            SetVal(obj, "RowKey", SafeRK);

            if (RK.Equals(SafeRK))
                SetVal(obj, RowKeyPropertyName, null);
        }
    }
}