using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AzureTableFramework
{
    public static partial class Utils
    {
        public static Dictionary<string, AzureTableEntity> Indexes(object obj)
        {
            var IndexDictionary = new Dictionary<string, AzureTableEntity>();

            foreach (var prop in obj.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(IndexAttribute), true).Any()))
            {
                var IA = (IndexAttribute)prop.GetCustomAttribute(typeof(IndexAttribute), false);

                var indexVersionOfObj = IA.PartitionKeyOnly ? new AzureTableEntity() : obj as AzureTableEntity;
                indexVersionOfObj._IsIndexVersion = true;

                var propKeyValue = GetVal(obj, prop.Name).ToString();
                var rowKeyValue = GetRowKeyValue(obj);

                indexVersionOfObj.PartitionKey = propKeyValue.MakeAzureSafe();
                indexVersionOfObj.RowKey = rowKeyValue.MakeAzureSafe();

                if (!IA.PartitionKeyOnly)
                {
                    if (indexVersionOfObj.PartitionKey.Equals(propKeyValue))
                        SetVal(indexVersionOfObj, prop.Name, null);

                    if (indexVersionOfObj.RowKey.Equals(rowKeyValue))
                        SetVal(indexVersionOfObj, GetRowKeyPropertyName(obj.GetType()), null);
                }

                var indexedProperties = IA.Properties ?? new List<string>() { prop.Name };
                var indexTableName = IndexTableName(obj, indexedProperties);

                IndexDictionary.Add(indexTableName, indexVersionOfObj);
            }

            return IndexDictionary;
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

            var type = source.GetType();

            var clonedObj = (T)Activator.CreateInstance(type);
            var props = type.GetProperties();

            foreach (var prop in props)
            {
                string propName = prop.Name;
                if (!prop.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
                    try
                    {
                        SetVal(clonedObj, propName, GetVal(source, prop.Name));
                    }
                    catch (Exception EX)
                    {
                        Debug.WriteLine("????? Clone Error on " + type.Name + ": " + EX.Message);
                    }
            }

            return clonedObj;

            //var json = JsonConvert.SerializeObject(source);
            //return JsonConvert.DeserializeObject<T>(json);

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result

            var deserializeSettings = new JsonSerializerSettings
            {
                //ObjectCreationHandling = ObjectCreationHandling.Replace,
                Error = (sender, args) =>
                {
                    Debug.WriteLine(args.ErrorContext.Error.Message);
                    args.ErrorContext.Handled = false;
                }
            };

            var jsonConvertSerializeObject = JsonConvert.SerializeObject(source);

            return JsonConvert.DeserializeObject<T>(jsonConvertSerializeObject, deserializeSettings);
        }

        public static List<T> SetRowAndPartitionKeys<T>(IEnumerable<T> list)
        {
            foreach (var obj in list)
            {
                //Debug.WriteLine("PartitionKey Before = " + GetVal(obj, "PartitionKey"));
                SetPartitionKey(GetPartitionKeyPropertyName(list.First().GetType()), obj);
                //Debug.WriteLine("PartitionKey After = " + GetVal(obj, "PartitionKey"));
                SetRowKey(GetRowKeyPropertyName(list.First().GetType()), obj);
            }

            return list.ToList();
        }

        public static void SetPartitionKey<T>(string PartitionKeyPropertyName, T obj)
        {
            var PK = GetPartitionKeyValue(PartitionKeyPropertyName, obj, true);
            var SafePK = PK.MakeAzureSafe();

            SetVal(obj, "PartitionKey", SafePK);

            if (PK.Equals(SafePK)) SetVal(obj, PartitionKeyPropertyName, null);
        }

        public static void SetRowKey<T>(string RowKeyPropertyName, T obj)
        {
            var RK = GetRowKeyValue(obj);
            var SafeRK = RK.MakeAzureSafe();

            SetVal(obj, "RowKey", SafeRK);

            if (RK.Equals(SafeRK)) SetVal(obj, RowKeyPropertyName, null);
        }

        public static string MakeAzureSafe(this string key)
        {
            return new List<char> { '\\', '#', '/', '%', '?' }.Aggregate(key, (current, c) => current.Replace(c, '_'));
        }

        public static string IndexTableName(object obj, string propertyName)
        {
            return IndexTableName(obj, new List<string>() { propertyName });
        }

        public static string IndexTableName(object obj, List<string> propertyNames)
        {
            return $"{obj.GetType().Name}Idx{(propertyNames.Count == 1 ? propertyNames.First() : Hash(propertyNames.Aggregate("", (current, prop) => current + prop)))}" +
               (propertyNames.Count > 1 ? $"-{ Hash(propertyNames.Aggregate("", (current, prop) => current + GetVal(obj, prop)))}" : "");
        }

        public static string Hash(string input)
        {
            return Hash(Encoding.ASCII.GetBytes(input)).ToString();
        }

        public static UInt32 Hash(Byte[] data)
        {
            return Hash(data, 0xc58f1a7b);
        }

        private const UInt32 m = 0x5bd1e995;
        private const Int32 r = 24;

        [StructLayout(LayoutKind.Explicit)]
        private struct BytetoUInt32Converter
        {
            [FieldOffset(0)]
            public Byte[] Bytes;

            [FieldOffset(0)]
            public UInt32[] UInts;
        }

        public static UInt32 Hash(Byte[] data, UInt32 seed)
        {
            Int32 length = data.Length;
            if (length == 0)
                return 0;
            UInt32 h = seed ^ (UInt32)length;
            Int32 currentIndex = 0;
            // array will be length of Bytes but contains Uints
            // therefore the currentIndex will jump with +1 while length will jump with +4
            UInt32[] hackArray = new BytetoUInt32Converter { Bytes = data }.UInts;
            while (length >= 4)
            {
                UInt32 k = hackArray[currentIndex++];
                k *= m;
                k ^= k >> r;
                k *= m;

                h *= m;
                h ^= k;
                length -= 4;
            }
            currentIndex *= 4; // fix the length
            switch (length)
            {
                case 3:
                    h ^= (UInt16)(data[currentIndex++] | data[currentIndex++] << 8);
                    h ^= (UInt32)data[currentIndex] << 16;
                    h *= m;
                    break;

                case 2:
                    h ^= (UInt16)(data[currentIndex++] | data[currentIndex] << 8);
                    h *= m;
                    break;

                case 1:
                    h ^= data[currentIndex];
                    h *= m;
                    break;

                default:
                    break;
            }

            // Do a few final mixes of the hash to ensure the last few
            // bytes are well-incorporated.

            h ^= h >> 13;
            h *= m;
            h ^= h >> 15;

            return h;
        }

        //public static string LettersAndNumbersOnly(this string s)
        //{
        //    return s.ToCharArray().Where(Char.IsLetterOrDigit).Aggregate("", (current, c) => current + c);
        //}

        //public static string CharactersOnly(this string val)
        //{
        //    return val.ToCharArray().Where(Char.IsLetter).Aggregate("", (current, c) => current + c);
        //}
    }
}