using AzureTableFramework.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Samples.Common
{
    public class BTEST
    {
    }

    public class BloggingContext : AzureTablesContext
    {
        public BloggingContext()
        {
            PrimaryStorageAccountName = "jhtest2";
            PrimaryStorageAccountKey = "u6eGo/IfJ0CaO4gwpDNWeTNwhu6GyInRGlR+aAYlO9uiAIfYSBtPCs0okJ9zy3bguKV1oR2Ukqcr7ala6i872A==";

            IndexStorageAccountName = "";
            IndexStorageAccountKey = "";

            EncryptionKey16Chars = "IfJ0CaO4gwpDNWeTNwhu6GyInRGlR+aA";

            SearchServiceName = "";
            SearchServiceManagementKey = "";
        }

        public AzureTableDictionary<Blog> Blogs { get; set; } = new AzureTableDictionary<Blog>();
        public AzureTableDictionary<Post> Posts { get; set; } = new AzureTableDictionary<Post>();
    }

    //[DataObject]
    public class Blog : AzureTableEntity
    {
        [PartionKey]
        public string AuthorID { get; set; }

        public string BlogID { get; set; }

        [Required]
        public string url { get; set; }

        public Dictionary<string, Post> Posts { get; set; }
    }

    //[DataObject]
    public class Post : AzureTableEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}