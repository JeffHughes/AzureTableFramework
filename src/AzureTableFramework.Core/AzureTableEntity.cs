using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace AzureTableFramework.Core
{
    public class AzureTableEntity : TableEntity
    {
        public AzureTableEntity()
        {
            init();
        }

        public void init()
        {
            initBlobs();
        }

        public void initBlobs()
        {
            var Blobs = GetType().GetProperties().Where(prop => prop.PropertyType == typeof(Blob));

            foreach (var item in Blobs)
            {
                Blob bp = (Blob)item.GetValue(this, null);

                if (bp == null || bp == new Blob())
                    bp = new Blob();

                if (bp.CallingObject == null)
                    bp.CallingObject = this;

                if (string.IsNullOrEmpty(bp.PropertyName))
                    bp.PropertyName = item.Name;

                Debug.WriteLine("blob.PropertyName: " + bp.PropertyName);

                var blobAttr = (BlobAttribute)item.GetCustomAttribute(typeof(BlobAttribute), false);
                if (blobAttr != null)
                {
                    if (string.IsNullOrEmpty(bp.FileExtension))
                        bp.FileExtension = blobAttr.FileExtension;

                    if (string.IsNullOrEmpty(bp.MimeType))
                        bp.MimeType = blobAttr.MimeType;
                }
            }
        }

        [IgnoreProperty]
        public bool _IsSoftDeleted { get; set; }

        [IgnoreProperty]
        public bool _HardDeleteWithBatch { get; set; }

        public void SoftDelete()
        {
            //if (!(this is object) || this == null) return;

            _IsSoftDeleted = true;
            _HardDeleteWithBatch = false;
        }

        public void HardDelete()
        {
            //if (!(this is object) || this == null) return;

            _IsSoftDeleted = false;
            _HardDeleteWithBatch = true;
        }

        [Index]
        [IgnoreProperty]
        public string LastUpdated
        {
            get { return Utils.TicksFromMax(Timestamp.UtcDateTime); }
        }
    }
}