using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace AzureTableFramework
{
    public static partial class Utils
    {
        public static IQueryable<IGrouping<TColumn, T>> GroupBy<T, TColumn>(IQueryable<T> source, string column)
        {
            var columnProperty = typeof(T).GetProperty(column);
            var sourceParm = Expression.Parameter(typeof(T), "x");
            var propertyReference = Expression.Property(sourceParm, columnProperty);
            var groupBySelector = Expression.Lambda<Func<T, TColumn>>(propertyReference, sourceParm);

            return source.GroupBy(groupBySelector);
        }
    }
}