using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    public static partial class Utils
    {
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

        public static string CombineQueries(string q1, string q2)
        {
            return CombineQueries(new List<string> { q1, q2 });
        }

        public static string CombineQueries(List<string> queries)
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