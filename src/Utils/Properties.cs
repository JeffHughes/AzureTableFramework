using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AzureTableFramework
{
    public static partial class Utils
    {
        private static bool PropertyExists(object obj, string propertyName)
        {
            foreach (var prop in obj.GetType().GetProperties())
                if (prop.Name.Equals(propertyName)) { return true; }

            return false;
        }

        public static string Name<T>(Expression<Func<T>> ex)
        {
            return (((MemberExpression)ex.Body).Member).Name;
        }

        public static string Name<T, TProp>(T Object, Expression<Func<T, TProp>> ex)
        {
            return GetPropInfo(ex).Name;
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
            var property = obj.GetType().GetProperty(propertyName);
            if (property.CanWrite)
                property.SetValue(obj, typedValue);
            return obj;
        }

        //public static object ResetVal(object obj, string propertyName)
        //{
        //    if (obj == null) throw new Exception(string.Format(" object not instantiated on prop:{0} val:{1}", propertyName, value));

        //    var typedValue = Convert.ChangeType(value, obj.GetType().GetProperty(propertyName).PropertyType);
        //    var property = obj.GetType().GetProperty(propertyName);
        //    if (property.CanWrite)
        //        property.SetValue(obj, typedValue);
        //    return obj;
        //}

        public static string GetRowKeyPropertyName(Type t)
        {
            return t.Name + "ID";
        }

        public static string GetRowKeyValue(Object obj)
        {
            var RowKeyPropertyName = GetRowKeyPropertyName(obj.GetType());
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

        public static PropertyInfo GetPropInfo<T1, TProperty>(Expression<Func<T1, TProperty>> propertyLambda)
        {
            var member = propertyLambda.Body as MemberExpression;

            if (member == null)
                throw new ArgumentException($"Expression '{propertyLambda }' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{propertyLambda }' refers to a field, not a property.");

            //Type type = typeof(T1);
            //if (type != propInfo.ReflectedType &&
            //              !type.IsSubclassOf(propInfo.ReflectedType))
            //    throw new ArgumentException($"Expresion '{propertyLambda }' refers to a property that is not from type {type}.");

            return propInfo;
        }

        //---//

        //---//

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

        public static List<object> DynamicResultsToAzureTableEntityList<ReturnType>(ReturnType type, object obj, IList<DynamicTableEntity> items)
        {
            var l = new List<object>();

            var blankObj = obj.Clone();
            foreach (var p in type.GetType().GetProperties())
                if (p.CanWrite)
                    p.SetValue(blankObj, null, null);

            foreach (var item in items)
            {
                var newItem = blankObj.Clone();

                CopyObjectData(item, newItem, "");

                foreach (var p in ((dynamic)item).Properties)
                    Utils.SetVal(newItem, p.Key, p.Value.PropertyAsObject);

                l.Add(newItem);
            }

            return l;
        }

        /// <summary>
        /// Copies the data of one object to another. The target object 'pulls' properties of the first.
        /// This any matching properties are written to the target.
        ///
        /// The object copy is a shallow copy only. Any nested types will be copied as
        /// whole values rather than individual property assignments (ie. via assignment)
        /// </summary>
        /// <param name="source">The source object to copy from</param>
        /// <param name="target">The object to copy to</param>
        /// <param name="excludedProperties">A comma delimited list of properties that should not be copied</param>

        public static void CopyObjectData(object source, object target, string excludedProperties)
        {
            string[] excluded = null;
            if (!string.IsNullOrEmpty(excludedProperties))
                excluded = excludedProperties.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            MemberInfo[] miT = target.GetType().GetMembers();
            foreach (MemberInfo Field in miT)
            {
                string name = Field.Name;

                // Skip over any property exceptions
                if (!string.IsNullOrEmpty(excludedProperties) &&
                    excluded.Contains(name))
                    continue;

                //if (Field.MemberType == MemberTypes.Field)
                //{
                //    FieldInfo SourceField = source.GetType().GetField(name);
                //    if (SourceField == null)
                //        continue;

                //    object SourceValue = SourceField.GetValue(source);
                //    ((FieldInfo)Field).SetValue(target, SourceValue);
                //}
                //else if (Field.MemberType == MemberTypes.Property)
                //{
                PropertyInfo piTarget = Field as PropertyInfo;
                PropertyInfo SourceField = source.GetType().GetProperty(name);
                if (SourceField == null)
                    continue;

                if (piTarget.CanWrite && SourceField.CanRead)
                {
                    object SourceValue = SourceField.GetValue(source, null);
                    piTarget.SetValue(target, SourceValue, null);
                }
                // }
            }
        }

        public static List<ReturnType> DynamicResultsToTypedList<ReturnType, OriginalType>(ReturnType type, List<OriginalType> items)
        {
            return DynamicResultsToTypedList<ReturnType, OriginalType>(items);
        }

        public static List<ReturnType> DynamicResultsToTypedList<ReturnType, OriginalType>(List<OriginalType> items)
        {
            Mapper.CreateMap<OriginalType, ReturnType>();

            var ConvertedResults = new List<ReturnType>();

            foreach (var origItem in items)
            {
                ReturnType newItem = Mapper.Map<ReturnType>(origItem);

                newItem = AzureTableRecordToTypedObject<ReturnType>(newItem);

                if (typeof(OriginalType) == typeof(DynamicTableEntity))
                    foreach (var p in ((dynamic)origItem).Properties)
                        Utils.SetVal(newItem, p.Key, p.Value.PropertyAsObject);

                ConvertedResults.Add(newItem);
            }

            return ConvertedResults;
        }

        public static T AzureTableRecordToTypedObject<T>(T obj)
        {
            if ((obj as AzureTableEntity)._IsIndexVersion) return obj;

            var PartitionKeyPropertyName = GetPartitionKeyPropertyName(typeof(T));
            var RowKeyPropertyName = GetRowKeyPropertyName(typeof(T));

            if ((GetVal(obj, RowKeyPropertyName) == null))
                SetVal(obj, RowKeyPropertyName, (string)GetVal(obj, "RowKey"));

            if ((GetVal(obj, PartitionKeyPropertyName) == null))
                SetVal(obj, PartitionKeyPropertyName, (string)GetVal(obj, "PartitionKey"));

            if (GetVal(obj, "ETag").ToString().Contains("datetime"))
            {    //"W/\"datetime'2016-01-11T17%3A29%3A41.953478Z'\""
                var timestring = Regex.Split((obj as TableEntity).ETag, "datetime").Last().Replace("'", "").Replace("%3A", ":").Replace("\"", "");
                (obj as TableEntity).Timestamp = Convert.ToDateTime(timestring);
            }

            return obj;
        }
    }
}