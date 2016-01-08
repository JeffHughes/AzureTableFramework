using AutoMapper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public string Name
        {
            get { return typeof(T).Name; }
        }

        public Type Type
        {
            get { return typeof(T); }
        }

        public T Add(T item)
        {
            var key = Utils.GetRowKeyValue(item);

            if (string.IsNullOrEmpty(key))
                throw new Exception("There is a problem w the RowKey for " + item.GetType().Name);

            Items.Add(key, item);
            return item;
        }

        public T New(params object[] args)
        {
            var o = (T)Activator.CreateInstance(typeof(T), args);
            Add(o);
            return o;
        }

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

        public List<T> Deletes()
        {
            var list = new List<T>();
            foreach (var item in Items.Values)
                if ((bool)Utils.GetVal(item, "_DeleteWithBatch"))
                    list.Add(item);
            return list;
        }
    }
}