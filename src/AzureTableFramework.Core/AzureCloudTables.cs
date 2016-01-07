using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    public static partial class Utils
    {
        private static SortedList<string, CloudTable> _CloudTables = new SortedList<string, CloudTable>();

        public static SortedList<string, CloudTable> CloudTables { get { return _CloudTables; } set { _CloudTables = value; } }

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
            var Q = Utils.Query((() => new TableEntity().PartitionKey), QueryComparisons.NotEqual, null);
            var TQ = new TableQuery<TableEntity>() { FilterString = Q, TakeCount = 1 }.Take(1);

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

        public static async Task BatchCloudTableOperation<T>(CloudTable table, List<T> list, bool DeleteAll)
        {
            if (!list.Any()) return;

            var type = list.First().GetType();
            var PartitionKeyPropertyName = Utils.GetPartitionKeyPropertyName(type);
            var RowKeyPropertyName = Utils.GetRowKeyPropertyName(type);

            var PartitionSortedList = new SortedList<string, List<T>>();

            foreach (var obj in list)
            {
                var PK = Utils.GetPartitionKeyValue(PartitionKeyPropertyName, obj);

                Utils.SetVal(obj, "PartitionKey", PK);
                Utils.SetVal(obj, "RowKey", Utils.GetRowKeyValue(obj));

                Utils.SetVal(obj, RowKeyPropertyName, null);
                Utils.SetVal(obj, PartitionKeyPropertyName, null);

                //if (!(bool)Utils.GetVal(obj, "IsSoftDeleted"))
                //    Utils.SetVal(obj, "IsSoftDeleted", null);

                //TODO: uncomment for indexing
                // await DeleteIndexesAsync(O2);
                //if (!DeleteAll) { } //upsert indexes

                if (!PartitionSortedList.ContainsKey(PK))
                    PartitionSortedList.Add(PK, new List<T>());

                PartitionSortedList[PK].Add((T)obj);
            }

            foreach (var PKList in PartitionSortedList.Values)
            {
                var EditableList = PKList;
                while (EditableList.Any())
                {
                    // var SW = new Stopwatch(); SW.Start();
                    var batchOperation = new TableBatchOperation();
                    if (DeleteAll)
                        EditableList.Take(100).ToList().ForEach(o => batchOperation.Delete(o as TableEntity));
                    else
                        EditableList.Take(100).ToList().ForEach(o => batchOperation.InsertOrReplace(o as TableEntity));
                    await table.ExecuteBatchAsync(batchOperation);
                    EditableList = EditableList.Skip(100).ToList();
                    // AzureUtils.Trace(string.Format("Batch Operation {0} executed in {1}", batchOperation.Count, SW.Elapsed));
                }
                EditableList.Clear();
            }

            PartitionSortedList.Clear();

            foreach (T obj in list)
            {
                Utils.SetVal(obj, RowKeyPropertyName, Utils.GetVal(obj, "RowKey"));
                Utils.SetVal(obj, PartitionKeyPropertyName, Utils.GetVal(obj, "PartitionKey"));
            }
        }
    }
}