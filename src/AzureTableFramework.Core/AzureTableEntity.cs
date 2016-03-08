using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace AzureTableFramework.Core
{
    public class AzureTableEntity : TableEntity
    {
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