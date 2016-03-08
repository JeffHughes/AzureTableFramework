using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AzureTableFramework.Core
{
    public static partial class Utils
    {
        /// <param name="ex">Utils.IsPartitionKey(() => new Object().Property))</param>
        /// <returns>boolean</returns>
        public static bool IsPartitionKey<T>(Expression<Func<T>> ex)
        {
            return ((MemberExpression)ex.Body).Member.GetCustomAttributes(typeof(PartitionKeyAttribute), false).Any();
        }

        ///// <param name="ex">Utils.IsPartitionKey(() => new SObjectS().SPropertyS))</param>
        ///// <returns>boolean</returns>
        //public static bool IsRowKey<T>(Expression<Func<T>> ex)
        //{
        //    return ((MemberExpression)ex.Body).Member.GetCustomAttributes(typeof(RowKeyAttribute), false).Any();
        //}

        /// <param name="ex">Utils.IsEncrypted(() => new SObjectS().SPropertyS))</param>
        /// <returns>boolean</returns>
        public static bool IsEncrypted<T>(Expression<Func<T>> ex)
        {
            return ((MemberExpression)ex.Body).Member.GetCustomAttributes(typeof(EncryptAttribute), false).Any();
        }

        public static string GetPartitionKeyPropertyName(Type t)
        {
            foreach (var prop in t.GetProperties())
                foreach (var atts in prop.GetCustomAttributes())
                    if (atts.GetType() == typeof(PartitionKeyAttribute))
                        if (prop.PropertyType != typeof(string))
                            throw new Exception(t.Name + " partitionkey attribute is on " + prop.Name +
                                " a " + prop.PropertyType + ".  But, partition keys must be strings");
                        else
                            return prop.Name;

            throw new Exception("PartitionKey property attribute not found for " + t.Name);
        }

        public static string GetPartitionKeyValue<T>(T obj, bool SetDefaultValueIfNull)
        {
            var type = obj.GetType();
            string PartitionKeyPropertyName = GetPartitionKeyPropertyName(type);
            return GetPartitionKeyValue(PartitionKeyPropertyName, obj, SetDefaultValueIfNull);
        }

        public static string GetPartitionKeyValue<T>(string PartitionKeyPropertyName, T obj, bool SetDefaultValueIfNull)
        {
            var PK = $"{DateTime.UtcNow.Year}-{DateTime.UtcNow.Month}";

            var PossiblyNullObject = Utils.GetVal(obj, PartitionKeyPropertyName);

            if (PossiblyNullObject == null)
                if (SetDefaultValueIfNull)
                    SetVal(obj, PartitionKeyPropertyName, PK);
                else
                    return string.Empty;
            else
                PK = PossiblyNullObject.ToString();

            return PK;
        }

        public static string GetRowKeyValue(Object obj)
        {
            var type = obj.GetType();
            var RowKeyPropertyName = GetRowKeyPropertyName(type);
            var RK = "";

            var PossiblyNullObject = Utils.GetVal(obj, RowKeyPropertyName);

            if (PossiblyNullObject == null)
            {
                RK = Guid.NewGuid().ToString();
                Utils.SetVal(obj, RowKeyPropertyName, RK);
            }
            else
            {
                RK = PossiblyNullObject.ToString();
            }

            return RK;
        }

        public static string GetRowKeyPropertyName(Type t)
        {
            return t.Name + "ID";
        }

        public static object GetVal<T>(T obj, string propertyName)
        {
            if (obj == null) return null;

            if (!PropertyExists(obj, propertyName)) return null;

            return obj.GetType().GetProperty(propertyName).GetValue(obj);
        }

        public static object SetVal(object obj, string propertyName, object value)
        {
            if (obj == null) throw new Exception(string.Format(" object not instantiated on prop:{0} val:{1}", propertyName, value));

            var typedValue = Convert.ChangeType(value, obj.GetType().GetProperty(propertyName).PropertyType);
            obj.GetType().GetProperty(propertyName).SetValue(obj, typedValue);
            return obj;
        }

        private static bool PropertyExists(object obj, string propertyName)
        {
            foreach (var prop in obj.GetType().GetProperties())
                if (prop.Name.Equals(propertyName)) { return true; }

            return false;
        }

        public static PropertyInfo GetPropInfo<T1, TProperty>(Expression<Func<T1, TProperty>> propertyLambda)
        {
            var member = propertyLambda.Body as MemberExpression;

            if (member == null)
                throw new ArgumentException($"Expression '{propertyLambda }' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{propertyLambda }' refers to a field, not a property.");

            Type type = typeof(T1);

            //if (type != propInfo.ReflectedType &&
            //              !type.IsSubclassOf(propInfo.ReflectedType))
            //    throw new ArgumentException($"Expresion '{propertyLambda }' refers to a property that is not from type {type}.");

            return propInfo;
        }

        public static string Name<T>(Expression<Func<T>> ex)
        {
            return (((MemberExpression)ex.Body).Member).Name;
        }

        public static string Name<T, TProp>(T Object, Expression<Func<T, TProp>> ex)
        {
            return GetPropInfo(ex).Name;
        }

        public static string GetObjectName<T>(Expression<Func<T>> Property)
        {
            return ((MemberExpression)Property.Body).Expression.Type.Name;
        }

        public static bool IsNumericType(object obj)
        {
            if (obj is byte ||
                obj is SByte ||
                obj is UInt16 ||
                obj is UInt32 ||
                obj is UInt64 ||
                obj is Int16 ||
                obj is Int32 ||
                obj is Int64 ||
                obj is Decimal ||
                obj is Double ||
                obj is Single) return true;

            return false;
        }

        public static string TicksFromMax(DateTime DTUTC)
        {
            return (long.MaxValue - DTUTC.Ticks).ToString();
        }

        public static DateTime UTCDateTimeFromTicksFromMax(string TicksFromMax)
        {
            var TicksFromMaxLong = long.MaxValue - Convert.ToInt64(TicksFromMax);
            return new DateTime(TicksFromMaxLong, DateTimeKind.Utc);
        }

        public static List<T> TableResultsToTypedList<T>(IList<TableResult> TR)
        {
            var ConvertedResults = new List<T>();

            foreach (var item in TR)
            {
                T obj = AzureTableRecordToTypedObject<T>((T)item.Result);
                ConvertedResults.Add(obj);
            }

            return ConvertedResults;
        }

        public static List<T> DynamicResultsToTypedList<T, T2>(List<T2> items)
        {
            var ConvertedResults = new List<T>();

            Mapper.CreateMap<T2, T>();

            foreach (var obj1 in items)
            {
                T obj = (T)Mapper.Map<T>(obj1);

                obj = AzureTableRecordToTypedObject<T>(obj);

                if (typeof(T2) == typeof(DynamicTableEntity))
                    foreach (var p in ((dynamic)obj1).Properties)
                        Utils.SetVal(obj, p.Key, p.Value.PropertyAsObject);

                ConvertedResults.Add(obj);
            }

            return ConvertedResults;
        }

        public static T AzureTableRecordToTypedObject<T>(T obj)
        {
            var PartitionKeyPropertyName = GetPartitionKeyPropertyName(typeof(T));
            var RowKeyPropertyName = GetRowKeyPropertyName(typeof(T));

            if ((GetVal(obj, RowKeyPropertyName) == null))
                SetVal(obj, RowKeyPropertyName, (string)GetVal(obj, "RowKey"));

            if ((GetVal(obj, PartitionKeyPropertyName) == null))
                SetVal(obj, PartitionKeyPropertyName, (string)GetVal(obj, "PartitionKey"));

            //SetVal(obj, "PartitionKey", null);
            //SetVal(obj, "RowKey", null);

            if (GetVal(obj, "ETag").ToString().Contains("datetime"))
            {    //"W/\"datetime'2016-01-11T17%3A29%3A41.953478Z'\""
                var timestring = Regex.Split((obj as TableEntity).ETag, "datetime").Last().Replace("'", "").Replace("%3A", ":").Replace("\"", "");
                (obj as TableEntity).Timestamp = Convert.ToDateTime(timestring);
            }

            //TODO: Decryption

            return obj;
        }
    }
}