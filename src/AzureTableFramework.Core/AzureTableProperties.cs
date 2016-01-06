using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AzureTableFramework.Core
{
    public static partial class Utils
    {
        public static string MakeAzureSafe(this string key)
        {
            return new List<char> { '\\', '#', '/', '%', '?' }.Aggregate(key, (current, c) => current.Replace(c, '_'));
        }

        /// <param name="ex">AzureUtils.IsPartitionKey(() => new Object().Property))</param>
        /// <returns>boolean</returns>
        public static bool IsPartitionKey<T>(Expression<Func<T>> ex)
        {
            return ((MemberExpression)ex.Body).Member.GetCustomAttributes(typeof(PartionKeyAttribute), false).Any();
        }

        ///// <param name="ex">AzureUtils.IsPartitionKey(() => new SObjectS().SPropertyS))</param>
        ///// <returns>boolean</returns>
        //public static bool IsRowKey<T>(Expression<Func<T>> ex)
        //{
        //    return ((MemberExpression)ex.Body).Member.GetCustomAttributes(typeof(RowKeyAttribute), false).Any();
        //}

        /// <param name="ex">AzureUtils.IsEncrypted(() => new SObjectS().SPropertyS))</param>
        /// <returns>boolean</returns>
        public static bool IsEncrypted<T>(Expression<Func<T>> ex)
        {
            return ((MemberExpression)ex.Body).Member.GetCustomAttributes(typeof(EncryptAttribute), false).Any();
        }

        public static string GetRowKeyValue(Object obj)
        {
            var type = obj.GetType();
            return type.GetProperty(GetRowKeyPropertyName(type)).GetValue(obj).ToString();
        }

        public static string GetRowKeyPropertyName(Type t)
        {
            return t.Name + "ID";
        }

        public static object GetVal(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj);
        }

        public static object SetVal(object obj, string propertyName, object value)
        {
            var typedValue = Convert.ChangeType(value, obj.GetType().GetProperty(propertyName).PropertyType);
            obj.GetType().GetProperty(propertyName).SetValue(obj, typedValue);
            return obj;
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
    }
}