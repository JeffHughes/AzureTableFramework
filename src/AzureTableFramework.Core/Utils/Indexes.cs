using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    public static partial class Utils
    {
        public static string LettersAndNumbersOnly(this string s)
        {
            return s.ToCharArray().Where(Char.IsLetterOrDigit).Aggregate("", (current, c) => current + c);
        }

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        private static string indexSeparator = "Idx";

        public static string IndexTableName(string objectName, string indexPropertyName)
        {
            var S = $"{objectName}{indexSeparator}{indexPropertyName}".LettersAndNumbersOnly();
            if (S.Length <= 63) return S;
            if (S.Length > 63) return S.Substring(0, 63);
            return S;
        }

        public static string IndexTableName<T>(T obj, List<string> tableNameProperties)
        {
            var itemName = obj.GetType().Name.CharactersOnly();
            var propStr = "";

            var objProps = obj.GetType().GetProperties();

            foreach (var prop in tableNameProperties)
            {
                propStr = propStr + prop;
                foreach (var propinfo in objProps)
                    if (prop == propinfo.Name) propStr = propStr + GetVal(obj, prop);
            }

            var FullName = ($"{itemName}{indexSeparator}{propStr}").CharactersOnly();

            if (FullName.Length <= 63)
                return FullName;

            propStr = "";
            var valStr = "";
            foreach (var prop in tableNameProperties)
            {
                propStr = propStr + prop;
                foreach (var propinfo in objProps)
                    if (prop == propinfo.Name) valStr = valStr + GetVal(obj, prop);
            }

            var hash = Hash(Encoding.ASCII.GetBytes(valStr), (uint)itemName.Length).ToString();

            var hashedName = $"{itemName}{indexSeparator}{propStr}{hash}";

            if (hashedName.Length > 63) return hashedName.Substring(0, 63);

            return hashedName;
        }

        public static T DynamicIndexValue<T>(T obj, string partitionKeyProperty)
        {
            var type = obj.GetType();

            if (GetVal(obj, partitionKeyProperty) == null) throw new Exception("Indexed field " + partitionKeyProperty + " on " + typeof(T).Name +
               " with ID of " + Utils.GetRowKeyValue(obj) + " is Null or Empty." + "Indexed fields can not be null");

            var objClone = Clone(obj);

            SetRowKeyValueFromObjectIDProperty(GetRowKeyPropertyName(type), objClone);
            SetVal(objClone, "PartitionKey", GetVal(objClone, partitionKeyProperty).ToString().MakePartitionAndRowKeysAzureSafe());
            SetVal(objClone, "ETag", "*");

            return objClone;
        }

        public static Dictionary<string, List<T>> GetIndexes<T>(IEnumerable<T> list)
        {
            var type = list.First().GetType();
            var RowKeyPropertyName = GetRowKeyPropertyName(type);

            var IndexObjectsDictionary = new Dictionary<string, List<T>>();

            var NonPartitionIndexProperties = GetNonPartitionIndexProperties(typeof(T));
            foreach (var property in NonPartitionIndexProperties)
            {
                var IndexName = IndexTableName(type.Name, property.Name);
                IndexObjectsDictionary.Add(IndexName, new List<T>());

                foreach (var obj in list)
                {
                    if (property.GetValue(obj) == null) throw new Exception("Indexed field " + property.Name + " on " + typeof(T).Name +
                        " with ID of " + Utils.GetRowKeyValue(obj) + " is Null or Empty." +
                        "Indexed fields can not be null");

                    var objClone = Clone(obj);

                    SetRowKeyValueFromObjectIDProperty(RowKeyPropertyName, objClone);

                    SetVal(objClone, "PartitionKey", GetVal(objClone, property.Name).ToString().MakePartitionAndRowKeysAzureSafe());

                    SetVal(objClone, "ETag", "*");

                    IndexObjectsDictionary[IndexName].Add(objClone);
                }
            }

            return IndexObjectsDictionary;
        }

        public static List<PropertyInfo> GetNonPartitionIndexProperties(Type type)
        {
            var IndexedProperties = type.GetProperties().Where(x => x.GetCustomAttributes(typeof(IndexAttribute), true).Any()).ToList();
            var PartitionKeyProperty = type.GetProperties().Where(x => x.GetCustomAttributes(typeof(PartitionKeyAttribute), true).Any()).First();

            var indexes = new List<PropertyInfo>();

            foreach (var property in IndexedProperties)
                if (PartitionKeyProperty != property)
                    indexes.Add(property);

            return indexes;
        }

        public static List<T> SetRowAndPartitionKeyPropertiesFromTypedObjectList<T>(IEnumerable<T> list)
        {
            var clonedList = list.Clone();

            foreach (var obj in clonedList)
            {
                //Debug.WriteLine("PartitionKey Before = " + GetVal(obj, "PartitionKey"));
                SetPartitionKeyFromObjectPartitionKeyProperty(GetPartitionKeyPropertyName(list.First().GetType()), obj);
                //Debug.WriteLine("PartitionKey After = " + GetVal(obj, "PartitionKey"));
                SetRowKeyValueFromObjectIDProperty(GetRowKeyPropertyName(list.First().GetType()), obj);
            }

            return clonedList.ToList();
        }

        public static void SetPartitionKeyFromObjectPartitionKeyProperty<T>(string PartitionKeyPropertyName, T obj)
        {
            var PK = GetPartitionKeyValue(PartitionKeyPropertyName, obj, true);
            var SafePK = PK.MakePartitionAndRowKeysAzureSafe();

            SetVal(obj, "PartitionKey", SafePK);

            if (PK.Equals(SafePK)) SetVal(obj, PartitionKeyPropertyName, null);
        }

        public static void SetRowKeyValueFromObjectIDProperty<T>(string RowKeyPropertyName, T obj)
        {
            var RK = GetRowKeyValue(obj);
            var SafeRK = RK.MakePartitionAndRowKeysAzureSafe();

            SetVal(obj, "RowKey", SafeRK);

            if (RK.Equals(SafeRK)) SetVal(obj, RowKeyPropertyName, null);
        }

        public static string MakePartitionAndRowKeysAzureSafe(this string key)
        {
            return new List<char> { '\\', '#', '/', '%', '?' }.Aggregate(key, (current, c) => current.Replace(c, '_'));
        }
    }
}