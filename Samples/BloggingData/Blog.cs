using AzureTableFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BloggingData
{
    [Backup]
    public class Blog : AzureTableEntity
    {
        [PartitionKey]
        public string AuthorID { get; set; }

        //RowKey
        public string BlogID { get; set; }

        [Required]
        public string Url { get; set; }

        [Index(true)]
        public string TestIndex { get; set; }
    }
}