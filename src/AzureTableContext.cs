using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AzureTableFramework
{
    public partial class AzureTableContext : IDisposable
    {
        public string PrimaryStorageAccountName { get; set; }
        public string PrimaryStorageAccountKey { get; set; }
        private string _IndexStorageAccountName { get; set; }
        private string _IndexStorageAccountKey { get; set; }
        public string SearchServiceName { get; set; }
        public string SearchServiceManagementKey { get; set; }

        public bool NewInsertsOnlyOperation { get; set; }

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

        public AzureTableContext()
        {
            Init();
        }

        public AzureTableContext(IConfigurationRoot config)
        {
            var Name = GetType().Name.Replace("Context", "");

            if (!string.IsNullOrEmpty(config[Name + ":PrimaryStorageAccountName"]))
                PrimaryStorageAccountName = config[Name + ":PrimaryStorageAccountName"];
            if (!string.IsNullOrEmpty(config[Name + ":PrimaryStorageAccountKey"]))
                PrimaryStorageAccountKey = config[Name + ":PrimaryStorageAccountKey"];

            if (!string.IsNullOrEmpty(config[Name + ":IndexStorageAccountName"]))
                IndexStorageAccountName = config[Name + ":IndexStorageAccountName"];
            if (!string.IsNullOrEmpty(config[Name + ":IndexStorageAccountKey"]))
                IndexStorageAccountKey = config[Name + ":IndexStorageAccountKey"];

            if (!string.IsNullOrEmpty(config[Name + ":SearchServiceName"]))
                SearchServiceName = config[Name + ":SearchServiceName"];
            if (!string.IsNullOrEmpty(config[Name + ":SearchServiceManagementKey"]))
                SearchServiceManagementKey = config[Name + ":SearchServiceManagementKey"];

            Init();
        }

        public void Init()
        {
            foreach (var p in this.GetType().GetProperties().Where(t => t.PropertyType.Name.Contains("AzureTableDictionary")))
            {
                var ATD = p.GetValue(this); //Add the context of this object to it's children
                foreach (var c in ATD.GetType().GetProperties().Where(c => c.PropertyType == typeof(AzureTableContext)))
                    c.SetValue(p.GetValue(this), this);
            }
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
            await SaveChangesAsync(false);
        }

        public async Task SaveChangesAsync(bool newInsertsOnly)
        {
            NewInsertsOnlyOperation = newInsertsOnly;

            foreach (var p in this.GetType().GetProperties())
                if (p.PropertyType.Name.Contains("AzureTableDictionary"))
                {
                    dynamic Dictionary = Convert.ChangeType(p.GetValue(this), typeof(AzureTableDictionary<>).MakeGenericType(p.PropertyType.GetGenericArguments().First()));
                    await ParseBatchCloudAction(Dictionary);
                }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AzureTableContext() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}