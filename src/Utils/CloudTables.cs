using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureTableFramework
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
                if (CreateIfNotExist) { await Table.CreateIfNotExistsAsync(); }
                else if (!(await Table.ExistsAsync())) return null;
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

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">() => new Object().Property</param>
        /// <param name="queryComparison">Microsoft.WindowsAzure.Storage.Table.QueryComparisons.Equals</param>
        /// <param name="value">value of the item</param>
        /// <returns>string of the query</returns>
        public static string FilterString<T>(Expression<Func<T>> property, string queryComparison, object value)
        {
            return FilterString(Utils.Name(property), queryComparison, value);
        }

        public static string FilterString(string propertyName, string queryComparison, object value)
        {
            var filterString = "";

            var val = "";
            if (value != null)
                val = value.GetType().ToString().Replace("System.", "");

            switch (val)
            {
                //case "":
                //    filterString = TableQuery.GenerateFilterCondition(property, queryComparison, "");
                //    break;

                case "Byte[]":
                    filterString = TableQuery.GenerateFilterConditionForBinary(propertyName, queryComparison, (byte[])value);
                    break;

                case "Boolean":
                    filterString = TableQuery.GenerateFilterConditionForBool(propertyName, queryComparison, (bool)value);
                    break;

                case "DateTime":
                    filterString = TableQuery.GenerateFilterConditionForDate(propertyName, queryComparison, (DateTime)value);
                    break;

                case "Double":
                    filterString = TableQuery.GenerateFilterConditionForDouble(propertyName, queryComparison, (double)value);
                    break;

                case "Guid":
                    filterString = TableQuery.GenerateFilterConditionForGuid(propertyName, queryComparison, (Guid)value);
                    break;

                case "Int32":
                    filterString = TableQuery.GenerateFilterConditionForInt(propertyName, queryComparison, (int)value);
                    break;

                case "Int64":
                    filterString = TableQuery.GenerateFilterConditionForLong(propertyName, queryComparison, (long)value);
                    break;

                default:
                    //if (Utils.IsRowKey(property))
                    //    filterString = TableQuery.GenerateFilterCondition("RowKey", queryComparison, (String)value);
                    //else

                    //if (Utils.IsPartitionKey(property))
                    //    filterString = TableQuery.GenerateFilterCondition("PartitionKey", queryComparison, (String)value);
                    //else
                    filterString = TableQuery.GenerateFilterCondition(propertyName, queryComparison, (String)value);
                    break;
            }

            //Event("Type Not Handled error on QueryOn");

            return filterString;
        }

        public static string CombineFilterStrings(params string[] queries)
        {
            return CombineFilterStrings(queries.ToList());
        }

        public static string CombineFilterStrings(List<string> queries)
        {
            var currentQuery = queries.First();
            while (queries.Count > 1)
            {
                queries = queries.Skip(1).ToList();
                currentQuery = TableQuery.CombineFilters(currentQuery, TableOperators.And, queries.First());
            }
            return currentQuery;
        }
    }
}