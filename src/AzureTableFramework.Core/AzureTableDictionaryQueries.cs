using AutoMapper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    public partial class AzureTableDictionary<T>
    {
        public async Task<T> GetByIDAsync(string ID)
        {
            var q = Utils.FilterString("RowKey", QueryComparisons.Equal, ID);
            var tq = new TableQuery { FilterString = q }.Take(1);
            var data = await QueryAsync(tq, null);
            return Add(data.Results.FirstOrDefault());
        }

        public async Task<AzureTableQueryResults<T>> QueryAsync(TableQuery tableQuery, TableContinuationToken token)
        {
            var table = await Utils.GetCloudTableAsync(typeof(T).Name, _AzureTablesContext.StorageAccount, false);
            var tqs = await table.ExecuteQuerySegmentedAsync(tableQuery, token);

            return new AzureTableQueryResults<T>()
            {
                DynamicTableEntities = (List<DynamicTableEntity>)tqs.Results,
                token = tqs.ContinuationToken
            };
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
    }
}