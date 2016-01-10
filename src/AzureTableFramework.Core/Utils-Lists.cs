using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    public static partial class Utils
    {
        public static IQueryable<IGrouping<TColumn, T>> GroupBy<T, TColumn>(IQueryable<T> source, string column)
        {
            PropertyInfo columnProperty = typeof(T).GetProperty(column);
            var sourceParm = Expression.Parameter(typeof(T), "x");
            var propertyReference = Expression.Property(sourceParm, columnProperty);
            var groupBySelector = Expression.Lambda<Func<T, TColumn>>(propertyReference, sourceParm);

            return source.GroupBy(groupBySelector);
        }

        public static List<T1> ExcludeOn<T1, T2>(this List<T1> includeList, List<T2> excludeList, string propertyName)
        {
            if (includeList == null || excludeList == null) return includeList;
            var excludeIDs = excludeList.Select(item => GetVal(item, propertyName)).Where(v => v != null).ToList();
            if (!excludeIDs.Any()) return includeList;
            return includeList.Where(item => !excludeIDs.Contains(GetVal(item, propertyName))).ToList();
        }
    }
}