﻿using AutoMapper;
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
            Debug.WriteLine("instantiated w " + context.PrimaryStorageAccountKey);
        }

        public Dictionary<string, T> Items { get; set; } = new Dictionary<string, T>();

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

            Items.Add(key, item);
            return item;
        }

        public List<T> Add(List<T> items)
        {
            foreach (var item in items) Add(item);
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
        /// Items that will be inserted or updated on "SaveChanges"
        /// </summary>
        /// <returns></returns>
        public List<T> Upserts()
        {
            var list = new List<T>();

            foreach (var item in Items.Values)
            {
                var deleted = (bool)Utils.GetVal(item, "_DeleteWithBatch");
                if (!deleted) list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// Items that will be deleted on "SaveChanges"
        /// </summary>
        /// <returns></returns>
        public List<T> Deletes()
        {
            var list = new List<T>();
            foreach (var item in Items.Values)
                if ((bool)Utils.GetVal(item, "_DeleteWithBatch"))
                    list.Add(item);
            return list;
        }

        /// <summary>
        /// Returns the values of Items as a list
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            return (List<T>)Convert.ChangeType(Items.Values, typeof(List<>).MakeGenericType(new[] { typeof(T) }));
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