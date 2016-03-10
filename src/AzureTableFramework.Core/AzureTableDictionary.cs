using AutoMapper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime;
using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    public partial class AzureTableDictionary<T>
    {
        public AzureTablesContext _AzureTablesContext { get; set; }

        public AzureTableDictionary(AzureTablesContext context)
        {
            _AzureTablesContext = context;
            //Debug.WriteLine("AzureTableDictionary instantiated for " + typeof(T).Name);
        }

        public Dictionary<string, T> Items { get; set; } = new Dictionary<string, T>();

        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Returns the name of the type of the AzureTableDictionary
        /// </summary>
        public string Name
        {
            get { return Type.Name; }
        }

        /// <summary>
        /// returns the type of the AzureTableDictionary
        /// </summary>
        public Type Type
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Adds an item to the Items list, returns that item only
        /// </summary>
        /// <param name="item"></param>
        /// <returns>the item that was added</returns>
        public T Add(T item)
        {
            var key = Utils.GetRowKeyValue(item);

            if (string.IsNullOrEmpty(key))
                throw new Exception("There is a problem w the RowKey for " + item.GetType().Name);

            if (Items.ContainsKey(key)) Items.Remove(key);

            Items.Add(key, item);
            return item;
        }

        public List<T> Add(List<T> items)
        {
            foreach (var item in items) Add(item);
            return items;
        }

        public async Task<T> Prep(T item)
        {
            Add(item);
            await Utils.LoadEagerBlobs(item, _AzureTablesContext.PrimaryStorageAccount());
            return item;
        }

        public async Task<List<T>> Prep(List<T> items)
        {
            foreach (var item in items) await Prep(item);
            return items;
        }

        /// <summary>
        /// Creates a new item, generates a guid for the ID and adds it to the list
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public T New(params object[] args)
        {
            var o = (T)Activator.CreateInstance(typeof(T), args);
            Add(o);
            return o;
        }

        /// <summary>
        /// Returns the values of Items as a list
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            return (List<T>)Convert.ChangeType(Items.Values.ToList(), typeof(List<>).MakeGenericType(new[] { typeof(T) }));
        }

        /// <summary>
        /// Group the items in the list based on a property in the class
        /// </summary>
        /// <param name="propertyName">Utils.Name(() => new T().PropertyName)</param>
        /// <returns></returns>
        public List<IGrouping<string, T>> GroupBy(string propertyName)
        {
            return Utils.GroupBy<T, string>(ToList().AsQueryable(), propertyName).ToList();
        }

        /// <summary>
        /// Create a distinct list based on a property in the class
        /// </summary>
        /// <param name="propertyName">Utils.Name(() => new T().PropertyName)</param>
        /// <returns></returns>
        public List<T> Distinct(string propertyName)
        {
            return GroupBy(propertyName).Select(group => group.First()).ToList();
        }

        /// <summary>
        /// Returns a list that excludes any items that share a property value with any of the items on the excluded item list
        /// </summary>
        /// <param name="excludeList"></param>
        /// <param name="propertyName">tils.Name(() => new T().PropertyName)</param>
        /// <returns></returns>
        public List<T> Exclude<T2>(List<T2> excludeList, string propertyName)
        {
            return Utils.ExcludeOn<T, T2>(ToList(), excludeList, propertyName);
        }
    }
}