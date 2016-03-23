using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTableFramework
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
                return Add(Utils.DynamicResultsToTypedList<T, T>(data.Results).FirstOrDefault());

            return default(T);
        }

        public async Task<List<T>> GetAllByPartitionKeyAsync(string PartitionKeyValue)
        {
            return await QueryAllAsync("PartitionKey", QueryComparisons.Equal, PartitionKeyValue);
        }

        public async Task<List<T>> GetLastUpdated(DateTime updatedAfterUTC)
        {
            return await QueryAllAsync("LastUpdated", QueryComparisons.LessThanOrEqual, updatedAfterUTC);
        }

        // ---- //

        public async Task<AzureTableQueryResults<T>> QueryOnceAsync(string filterString)
        {
            return await QueryAsync(new TableQuery { FilterString = filterString }, null);
        }

        //public async Task<List<T>> QueryAllAsync(string propertyName, string queryComparison, object value)
        //{
        //    return await QueryAllAsync(Utils.FilterString(propertyName, queryComparison, value));
        //}

        public async Task<List<T>> QueryAllAsync(string filterString)
        {
            var data = await QueryUntilAsync(new TableQuery { FilterString = filterString }, int.MaxValue, TimeSpan.MaxValue);

            if (hasResults(data))
                return Add(Utils.DynamicResultsToTypedList<T, T>(data.Results));

            return null;
        }

        //public async Task<AzureTableQueryResults<T>> QueryUntilAsync(string filterString, int MaxResults)
        //{
        //    return await QueryUntilAsync(filterString, MaxResults, TimeSpan.MaxValue);
        //}

        //public async Task<AzureTableQueryResults<T>> QueryUntilAsync(string filterString, TimeSpan Timeout)
        //{
        //    return await QueryUntilAsync(filterString, int.MaxValue, Timeout);
        //}

        //public async Task<AzureTableQueryResults<T>> QueryUntilAsync(string filterString, int MaxResults, TimeSpan Timeout)
        //{
        //    return await QueryUntilAsync(new TableQuery { FilterString = filterString }, MaxResults, Timeout);
        //}

        public async Task<AzureTableQueryResults<T>> QueryUntilAsync(TableQuery tableQuery, int MaxResults, TimeSpan Timeout)
        {
            var SW = Stopwatch.StartNew(); var segmentCounter = 1;

            var segment = await QueryAsync(tableQuery, null);

            if (segment == null || !segment.Results.Any()) return null;

            var results = segment.Results;
            while (segment.token != null && results.Count < MaxResults && SW.Elapsed < Timeout)
            {
                segment = await QueryAsync(tableQuery, segment.token);
                results.AddRange(segment.Results);
                segmentCounter++;
            }

            Debug.WriteLine($"Query {typeof(T).Name}" +
                $"{(MaxResults < int.MaxValue || Timeout < TimeSpan.MaxValue ? " Until " : " ALL ")}" +
                $"{(MaxResults < int.MaxValue ? MaxResults + $" MaxResults = {MaxResults} " : "")}" +
                $"{(Timeout < TimeSpan.MaxValue ? Timeout + $" Timeout = {Timeout.ToString()} " : "")}" +
                $" returned {results.Count} record{(results.Count == 1 ? "" : "s")}," +
                $" execution time = {SW.Elapsed} with {segmentCounter} segment{(segmentCounter != 1 ? "s" : "")}");

            return new AzureTableQueryResults<T>()
            {
                Results = results,
                token = segment.token
            };
        }

        public async Task<AzureTableQueryResults<T>> QueryAsync(TableQuery tableQuery, TableContinuationToken token)
        {
            var SW = Stopwatch.StartNew();

            /*
            look at the class, see if there is an index

            query the index,

            if it's a partitionkeyonly index, go and get the full object
            */

            var table = await Utils.GetCloudTableAsync(typeof(T).Name, Context.PrimaryStorageAccount(), false);
            var tqs = await table.ExecuteQuerySegmentedAsync(tableQuery, token);

            Debug.WriteLine($"Query " +
               $"{typeof(T).Name}: {tableQuery.FilterString} returned {tqs.Results.Count} record{(tqs.Results.Count == 1 ? "" : "s")}," +
               $" execution time = {SW.Elapsed} ");

            return new AzureTableQueryResults<T>()
            {
                Results = Utils.DynamicResultsToTypedList<T, DynamicTableEntity>(tqs.Results.ToList()),
                token = tqs.ContinuationToken
            };
        }

        public bool hasResults(AzureTableQueryResults<T> data)
        {
            return data != null && data.Results != null && (bool)data?.Results?.Any();
        }
    }
}