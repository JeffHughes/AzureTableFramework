using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    public partial class AzureTableDictionary<T> : IDisposable
    {
        public async Task<T> GetByIDAsync(string ID)
        {
            return await GetByRowKeyAsync(ID);
        }

        public async Task<T> GetByRowKeyAsync(string ID)
        {
            var q = Utils.FilterString("RowKey", QueryComparisons.Equal, ID);
            var tq = new TableQuery { FilterString = q }.Take(1);
            var data = await QueryAsync(tq, null);

            if (hasResults(data))
                return Utils.DynamicResultsToTypedList<T, T>(data.Results).FirstOrDefault();

            return default(T);
        }

        /// <summary>
        /// There should never be more than 1 item with the same RowKey
        /// This method should only be used to make sure that there is only one such object
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public async Task<List<T>> GetAllByRowKeyAsync(string ID)
        {
            return await DataQueryAsync("RowKey", QueryComparisons.Equal, ID);
        }

        public async Task<List<T>> GetAllByPartitionKeyAsync(string PartitionKeyValue)
        {
            return await DataQueryAsync("PartitionKey", QueryComparisons.Equal, PartitionKeyValue);
        }

        public async Task<List<T>> GetLastUpdated(DateTime updatedAfterUTC)
        {
            return await DataQueryAsync("LastUpdated", QueryComparisons.LessThanOrEqual, updatedAfterUTC);
        }

        public async Task<List<T>> DataQueryAsync(string propertyName, string queryComparison, object value)
        {
            return await DataQueryAsync(Utils.FilterString(propertyName, queryComparison, value));
        }

        public async Task<List<T>> DataQueryAsync(string FilterString)
        {
            var data = await QueryAsync(FilterString, null);

            if (hasResults(data))
                return Utils.DynamicResultsToTypedList<T, T>(data.Results);

            return null;
        }

        public bool hasResults(AzureTableQueryResults<T> data)
        {
            return data != null && data.Results != null && (bool)data?.Results?.Any();
        }

        //Query

        public async Task<AzureTableQueryResults<T>> QueryAsync(string filterString, TableContinuationToken token)
        {
            return await QueryAsync(new TableQuery { FilterString = filterString }, token);
        }

        public async Task<AzureTableQueryResults<T>> QueryAsync(TableQuery tableQuery, TableContinuationToken token)
        {
            var table = await Utils.GetCloudTableAsync(typeof(T).Name, _AzureTablesContext.PrimaryStorageAccount(), false);

            /*      Work in progress
            if (!tableQuery.FilterString.Contains(") and (") && !tableQuery.FilterString.Contains(") or ("))
            {
                var startingFilterString = tableQuery.FilterString;
                Debug.WriteLine("Starting FilterString: " + tableQuery.FilterString);

                foreach (var indexedProp in Utils.GetNonPartitionIndexProperties(typeof(T)))
                    if (tableQuery.FilterString.Contains(indexedProp.Name))
                    {
                        table = await Utils.GetCloudTableAsync(Utils.IndexTableName(typeof(T).Name, indexedProp.Name), _AzureTablesContext.IndexStorageAccount(), false);
                        tableQuery.FilterString = tableQuery.FilterString.Replace(indexedProp.Name, "PartitionKey");

                        if (indexedProp.Name.Equals("LastUpdated")) UpdateFilterForLastUpdatedIndexCall(tableQuery);
                    }

                if (!startingFilterString.Equals(tableQuery.FilterString))
                    Debug.WriteLine("Executing FilterString: " + tableQuery.FilterString);
            }
            //else
            //{
            //return ComplexIndexedQuery();
            //}
            */

            var tqs = await table.ExecuteQuerySegmentedAsync(tableQuery, token);

            return new AzureTableQueryResults<T>()
            {
                Results = Utils.DynamicResultsToTypedList<T, DynamicTableEntity>(tqs.Results.ToList()),
                token = tqs.ContinuationToken
            };
        }

        //Query All

        public async Task<AzureTableQueryResults<T>> QueryAllAsync(string filterString)
        {
            return await QueryAllAsync(filterString, int.MaxValue, TimeSpan.MaxValue);
        }

        public async Task<AzureTableQueryResults<T>> QueryAllAsync(string filterString, int MaxResults)
        {
            return await QueryAllAsync(filterString, MaxResults, TimeSpan.MaxValue);
        }

        public async Task<AzureTableQueryResults<T>> QueryAllAsync(string filterString, TimeSpan Timeout)
        {
            return await QueryAllAsync(filterString, int.MaxValue, Timeout);
        }

        public async Task<AzureTableQueryResults<T>> QueryAllAsync(string filterString, int MaxResults, TimeSpan Timeout)
        {
            var SW = new Stopwatch(); SW.Start(); var segmentCounter = 0;

            var segment = await QueryAsync(filterString, null);

            if (segment == null || !segment.Results.Any()) return null;

            var results = segment.Results;
            while (segment.token != null && results.Count < MaxResults && SW.Elapsed < Timeout)
            {
                segment = await QueryAsync(filterString, segment.token);
                results.AddRange(segment.Results);
                segmentCounter++;
            }

            Debug.WriteLine(Name + ": " + filterString +
                " returned " + results.Count + " record" + (results.Count == 1 ? "" : "s") +
                " execution time = " + SW.Elapsed + " with " + segmentCounter + " segment" + (segmentCounter != 1 ? "s" : ""));

            return new AzureTableQueryResults<T>()
            {
                Results = results,
                token = segment.token
            };
        }

        //Dynamic Indexes

        public async Task<AzureTableQueryResults<T>> DynamicIndexQueryAsync(T obj, string indexedProperty, DateTime LessThanTime, TableContinuationToken token)
        {
            var TFM = Utils.TicksFromMax(LessThanTime);
            var FS = Utils.FilterString("PartitionKey", QueryComparisons.LessThanOrEqual, TFM);

            return await DynamicIndexQueryAsync(obj, FS, indexedProperty, token);
        }

        public async Task<AzureTableQueryResults<T>> DynamicIndexQueryAsync(T obj, string indexedProperty, TimeSpan LessThanTimeSpan, TableContinuationToken token)
        {
            var TFM = Utils.TicksFromMax(DateTime.UtcNow + LessThanTimeSpan);
            var FS = Utils.FilterString("PartitionKey", QueryComparisons.LessThanOrEqual, TFM);

            return await DynamicIndexQueryAsync(obj, FS, indexedProperty, token);
        }

        public async Task<AzureTableQueryResults<T>> DynamicIndexQueryAsync(T obj, string indexedProperty, string QueryComparison, TimeSpan TimeSpan, TableContinuationToken token)
        {
            var TFM = Utils.TicksFromMax(DateTime.UtcNow + TimeSpan);
            var FS = Utils.FilterString("PartitionKey", QueryComparison, TFM);

            return await DynamicIndexQueryAsync(obj, FS, indexedProperty, token);
        }

        public async Task<AzureTableQueryResults<T>> DynamicIndexQueryAsync(T obj, string indexedProperty, string QueryComparison, string QueryValue, TableContinuationToken token)
        {
            var FS = Utils.FilterString("PartitionKey", QueryComparison, QueryValue);

            return await DynamicIndexQueryAsync(obj, FS, indexedProperty, token);
        }

        public async Task<AzureTableQueryResults<T>> DynamicIndexQueryAsync(T obj, string indexedProperty, string filterString, TableContinuationToken token)
        {
            foreach (var dynamicIdxProp in obj.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(DynamicIndexAttribute), true).Any()))
            {
                if (dynamicIdxProp.Name == indexedProperty)
                {
                    var props = ((DynamicIndexAttribute)dynamicIdxProp.GetCustomAttribute(typeof(DynamicIndexAttribute), false)).Properties;
                    var DynamicIndexTableName = Utils.IndexTableName(obj, props);

                    var table = await Utils.GetCloudTableAsync(DynamicIndexTableName, _AzureTablesContext.IndexStorageAccount(), false);

                    if (table == null) return null;

                    var tqs = await table.ExecuteQuerySegmentedAsync(new TableQuery { FilterString = filterString }, token);

                    return new AzureTableQueryResults<T>()
                    {
                        Results = Utils.DynamicResultsToTypedList<T, DynamicTableEntity>(tqs.Results.ToList()),
                        token = tqs.ContinuationToken
                    };
                }
            }
            return null;
        }

        //Complex Queries

        /*         Work in progress

            private static void UpdateFilterForLastUpdatedIndexCall(TableQuery tableQuery)
            {
            if (tableQuery.FilterString.Contains("datetime'"))
            {
                var filterparts = tableQuery.FilterString.Replace("datetime'", "").Replace("'", "").Split(' ');

                try
                {
                    var part = filterparts.Last();
                    var dateTime = new DateTime(Convert.ToDateTime(part).Ticks, DateTimeKind.Utc);
                    var val = Utils.TicksFromMax(dateTime);

                    tableQuery.FilterString = tableQuery.FilterString.Replace(part, val).Replace("datetime", "");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            }
            */

        //public async Task<AzureTableQueryResults<T>> ComplexIndexedQuery()
        //{
        //    //((LastUpdated ge datetime'2016-01-09T20:00:00.0000000Z') and (LastUpdated ge datetime'2016-01-09T20:00:00.0000000Z')) and (LastUpdated ge datetime'2016-01-09T20:00:00.0000000Z')

        //    //var expressions = Regex.Split(tableQuery.FilterString, " and ");

        //    //foreach (var filter in expressions)
        //    //{
        //    return null;
        //}

        public void Dispose()
        {
        }
    }
}