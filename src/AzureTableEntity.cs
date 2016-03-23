using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTableFramework
{
    public class AzureTableEntity : TableEntity
    {
        [JsonIgnore]
        public AzureTableContext Context { get; set; }

        [IgnoreProperty]
        public bool _IsIndexVersion { get; set; }

        //[IgnoreProperty]
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
        }
    }
}