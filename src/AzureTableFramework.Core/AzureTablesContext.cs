using AzureTableFramework.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    public partial class AzureTablesContext : IDisposable
    {
        public string PrimaryStorageAccountName { get; set; }
        public string PrimaryStorageAccountKey { get; set; }

        private string _IndexStorageAccountName { get; set; }

        public string IndexStorageAccountName
        {
            get
            {
                return (!string.IsNullOrEmpty(_IndexStorageAccountName)) ?
                    _IndexStorageAccountName : PrimaryStorageAccountName;
            }
            set
            {
                _IndexStorageAccountName = value;
            }
        }

        private string _IndexStorageAccountKey { get; set; }

        public string IndexStorageAccountKey
        {
            get
            {
                return (!string.IsNullOrEmpty(_IndexStorageAccountKey)) ?
                    _IndexStorageAccountKey : PrimaryStorageAccountKey;
            }
            set
            {
                _IndexStorageAccountKey = value;
            }
        }

        private string _EncryptionKey16Chars { get; set; }

        public string EncryptionKey16Chars
        {
            get
            {
                return (!string.IsNullOrEmpty(_EncryptionKey16Chars)) ?
                    _EncryptionKey16Chars : PrimaryStorageAccountKey;
            }
            set
            {
                _EncryptionKey16Chars = value;
            }
        }

        public string SearchServiceName { get; set; }
        public string SearchServiceManagementKey { get; set; }

        public AzureTablesContext()
        {
            InstantiateDictionaries();
        }

        private bool DictionariesInstantiated = false;

        public void InstantiateDictionaries()
        {
            if (DictionariesInstantiated) return;
            DictionariesInstantiated = true;

            foreach (var p in this.GetType().GetProperties())
                if (p.PropertyType.Name.Contains("AzureTableDictionary"))
                {
                    Type t1 = typeof(AzureTableDictionary<>).MakeGenericType(p.PropertyType.GetGenericArguments().First());
                    p.SetValue(this, Activator.CreateInstance(t1, this));
                }
        }

        public AzureTablesContext(AzureTablesContext _securityContext)
        {
            if (!string.IsNullOrEmpty(_securityContext.PrimaryStorageAccountName))
                PrimaryStorageAccountName = _securityContext.PrimaryStorageAccountName;
            if (!string.IsNullOrEmpty(_securityContext.PrimaryStorageAccountKey))
                PrimaryStorageAccountKey = _securityContext.PrimaryStorageAccountKey;

            if (!string.IsNullOrEmpty(_securityContext.IndexStorageAccountName))
                IndexStorageAccountName = _securityContext.IndexStorageAccountName;
            if (!string.IsNullOrEmpty(_securityContext.IndexStorageAccountKey))
                IndexStorageAccountKey = _securityContext.IndexStorageAccountKey;

            if (!string.IsNullOrEmpty(_securityContext.EncryptionKey16Chars))
                EncryptionKey16Chars = _securityContext.EncryptionKey16Chars;

            if (!string.IsNullOrEmpty(_securityContext.SearchServiceName))
                SearchServiceName = _securityContext.SearchServiceName;
            if (!string.IsNullOrEmpty(_securityContext.SearchServiceManagementKey))
                SearchServiceManagementKey = _securityContext.SearchServiceManagementKey;

            InstantiateDictionaries();
        }

        public AzureTablesContext(IConfigurationRoot config)
        {
            var Name = this.GetType().Name.Replace("Context", "");

            if (!string.IsNullOrEmpty(config[Name + ":PrimaryStorageAccountName"]))
                PrimaryStorageAccountName = config[Name + ":PrimaryStorageAccountName"];
            if (!string.IsNullOrEmpty(config[Name + ":PrimaryStorageAccountKey"]))
                PrimaryStorageAccountKey = config[Name + ":PrimaryStorageAccountKey"];

            if (!string.IsNullOrEmpty(config[Name + ":IndexStorageAccountName"]))
                IndexStorageAccountName = config[Name + ":IndexStorageAccountName"];
            if (!string.IsNullOrEmpty(config[Name + ":IndexStorageAccountKey"]))
                IndexStorageAccountKey = config[Name + ":IndexStorageAccountKey"];

            if (!string.IsNullOrEmpty(config[Name + ":EncryptionKey16Chars"]))
                EncryptionKey16Chars = config[Name + ":EncryptionKey16Chars"];

            if (!string.IsNullOrEmpty(config[Name + ":SearchServiceName"]))
                SearchServiceName = config[Name + ":SearchServiceName"];
            if (!string.IsNullOrEmpty(config[Name + ":SearchServiceManagementKey"]))
                SearchServiceManagementKey = config[Name + ":SearchServiceManagementKey"];

            InstantiateDictionaries();
        }

        public CloudStorageAccount PrimaryStorageAccount()
        {
            return Utils.StorageAccount(PrimaryStorageAccountName, PrimaryStorageAccountKey);
        }

        public CloudStorageAccount IndexStorageAccount()
        {
            return Utils.StorageAccount(IndexStorageAccountName, IndexStorageAccountKey);
        }

        public async Task SaveChangesAsync()
        {
            foreach (var p in this.GetType().GetProperties())
                if (p.PropertyType.Name.Contains("AzureTableDictionary"))
                {
                    //TODO: find out if there is a better way to get a typed property value compatible w aspnet5
                    dynamic Dictionary = Convert.ChangeType(p.GetValue(this), typeof(AzureTableDictionary<>).MakeGenericType(p.PropertyType.GetGenericArguments().First()));
                    await ParseBatchCloudAction(Dictionary);
                }
        }
    }
}