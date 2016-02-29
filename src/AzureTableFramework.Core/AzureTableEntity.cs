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
            _IsSoftDeleted = true;
            _HardDeleteWithBatch = false;
        }

        public void HardDelete()
        {
            _IsSoftDeleted = false;
            _HardDeleteWithBatch = true;
        }

        [Index]
        [IgnoreProperty]
        public string LastUpdated
        {
            get { return Utils.TicksFromMax(Timestamp.UtcDateTime); }
            //get { return (long.MaxValue - Timestamp.Ticks).ToString(); }
        }
    }
}