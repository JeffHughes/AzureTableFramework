using AutoMapper;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    public partial class AzureTableDictionary<T> : IDisposable
    {
        public async Task<T> GetByIDAsync(string ID)
        {
            var q = Utils.FilterString("RowKey", QueryComparisons.Equal, ID);
            var tq = new TableQuery { FilterString = q }.Take(1);
            var data = await QueryAsync(tq, null);

            if (hasResults(data))
                return Add(Utils.DynamicResultsToTypedList<T, T>(data.Results).FirstOrDefault());

            return default(T);
        }

        public async Task<List<T>> GetAllByIDAsync(string ID)
        {
            var q = Utils.FilterString("RowKey", QueryComparisons.Equal, ID);
            var data = await QueryAllAsync(q);

            if (hasResults(data))
                return Utils.DynamicResultsToTypedList<T, T>(data.Results);

            return null;
        }

        public async Task<List<T>> GetAllByPartitionKeyAsync(string PartitionKeyValue)
        {
            var q = Utils.FilterString("PartitionKey", QueryComparisons.Equal, PartitionKeyValue);
            var data = await QueryAllAsync(q);

            if (hasResults(data))
                return Utils.DynamicResultsToTypedList<T, T>(data.Results);

            return null;
        }

        public bool hasResults(AzureTableQueryResults<T> data)
        {
            return data != null && data.Results != null && (bool)data?.Results?.Any();
        }

        public async Task<List<T>> GetLastUpdated(DateTime updatedAfterUTC)
        {
            var q = Utils.FilterString("LastUpdated", QueryComparisons.LessThanOrEqual, updatedAfterUTC);
            var tq = new TableQuery { FilterString = q };
            var data = await QueryAsync(tq, null);

            if (data.Results.Any())
                return Add(data.Results);

            return null;
        }

        //public async Task<AzureTableQueryResults<T>> ComplexIndexedQuery()
        //{
        //    //((LastUpdated ge datetime'2016-01-09T20:00:00.0000000Z') and (LastUpdated ge datetime'2016-01-09T20:00:00.0000000Z')) and (LastUpdated ge datetime'2016-01-09T20:00:00.0000000Z')

        //    //var expressions = Regex.Split(tableQuery.FilterString, " and ");

        //    //foreach (var filter in expressions)
        //    //{
        //    return null;
        //}

        public async Task<AzureTableQueryResults<T>> QueryAsync(TableQuery tableQuery, TableContinuationToken token)
        {
            var table = await Utils.GetCloudTableAsync(typeof(T).Name, _AzureTablesContext.PrimaryStorageAccount(), false);

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

            var tqs = await table.ExecuteQuerySegmentedAsync(tableQuery, token);

            return new AzureTableQueryResults<T>()
            {
                Results = Utils.DynamicResultsToTypedList<T, DynamicTableEntity>(tqs.Results.ToList()),
                token = tqs.ContinuationToken
            };
        }

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

        public async Task<AzureTableQueryResults<T>> QueryAsync(string filterString, TableContinuationToken token)
        {
            return await QueryAsync(new TableQuery { FilterString = filterString }, token);
        }

        public async Task<AzureTableQueryResults<T>> QueryAllAsync(string filterString)
        {
            return await QueryAllAsync(filterString, null, int.MaxValue, TimeSpan.MaxValue);
        }

        public async Task<AzureTableQueryResults<T>> QueryAllAsync(string filterString, TableContinuationToken token, int MaxResults)
        {
            return await QueryAllAsync(filterString, token, MaxResults, TimeSpan.MaxValue);
        }

        public async Task<AzureTableQueryResults<T>> QueryAllAsync(string filterString, TableContinuationToken token, TimeSpan Timeout)
        {
            return await QueryAllAsync(filterString, token, int.MaxValue, Timeout);
        }

        public async Task<AzureTableQueryResults<T>> QueryAllAsync(string filterString, TableContinuationToken token, int MaxResults, TimeSpan Timeout)
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
                " returned " + results.Count + "record" + (results.Count != 1 ? "s" : "") +
                " execution time = " + SW.Elapsed + " with " + segmentCounter + " segment" + (segmentCounter != 1 ? "s" : ""));

            return new AzureTableQueryResults<T>()
            {
                Results = results,
                token = segment.token
            };
        }

        public void Dispose()
        {
        }
    }
}