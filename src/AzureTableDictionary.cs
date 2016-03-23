using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTableFramework
{
    public partial class AzureTableDictionary<T> : IDisposable
    {
        [JsonIgnore]
        public AzureTableContext Context { get; set; }

        [JsonIgnore]
        public List<T> Items { get; set; } = new List<T>();

        public T New()
        {
            var o = (T)Activator.CreateInstance(typeof(T), null);
            Add(o);
            return o;
        }

        public T Add(T item)
        {
            var key = Utils.GetRowKeyValue(item);

            if (string.IsNullOrEmpty(key))
                throw new Exception("There is a problem w the RowKey for " + item.GetType().Name);

            if (Context == null)
                throw new Exception("Context is null");
            else
                (item as AzureTableEntity).Context = Context;

            Items.Add(item);
            return item;
        }

        public List<T> Add(List<T> items)
        {
            foreach (var item in items) Add(item);
            return items;
        }

        /// <summary>
        /// Group the items in the list based on a property in the class
        /// </summary>
        /// <param name="propertyName">Utils.Name(() => new T().PropertyName)</param>
        /// <returns></returns>
        public List<IGrouping<string, T>> GroupBy(string propertyName)
        {
            return Utils.GroupBy<T, string>(Items.AsQueryable(), propertyName).ToList();
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