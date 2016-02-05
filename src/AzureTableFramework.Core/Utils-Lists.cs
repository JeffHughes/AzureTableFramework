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

        //returnArray.AsNotNull()
        public static IEnumerable<T> AsNotNull<T>(this IEnumerable<T> original)
        {
            return original ?? new T[0];
        }

        /// <summary>
        /// returnArray.ForEach(Console.WriteLine)
        ///    or
        /// returnArray.ForEach(i => UpdateStatus(string.Format("{0}% complete", i)));
        ///    or
        /// returnArray.ForEach(i =>
        ///  {
        ///  var thisInt = i;
        ///    var next = i++;
        ///    if (next > 10) Console.WriteLine("Match: {0}", i);
        ///  };
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
    
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null) return;
            foreach (var item in items) action(item);
        }

    }
}