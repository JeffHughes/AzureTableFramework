﻿using AzureTableFramework.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Samples.Common
{
    public class BloggingContext : AzureTablesContext
    {
        public AzureTableDictionary<Blog> Blogs { get; set; }
        public AzureTableDictionary<Post> Posts { get; set; }

        public BloggingContext()
        {
            PrimaryStorageAccountName = "jhtest2";
            PrimaryStorageAccountKey = "u6eGo/IfJ0CaO4gwpDNWeTNwhu6GyInRGlR+aAYlO9uiAIfYSBtPCs0okJ9zy3bguKV1oR2Ukqcr7ala6i872A==";

            IndexStorageAccountName = "jhtest2idx";
            IndexStorageAccountKey = "q0jIB778aJWZMwBgkVIiZ3zypNm/YQjko7HApJqyCmhp3rkNsyrm4jFGdLhV+EipOPtE+QZJXfSdnXRvMZ8EKw==";

            EncryptionKey16Chars = "IfJ0CaO4gwpDNWeTNwhu6GyInRGlR+aA";

            SearchServiceName = "";
            SearchServiceManagementKey = "";
        }
    }

    public class Blog : AzureTableEntity
    {
        [PartionKey]
        public string AuthorID { get; set; }

        //RowKey
        public string BlogID { get; set; }

        [Required]
        public string Url { get; set; }

        public Dictionary<string, Post> Posts { get; set; }

        public Dictionary<string, Post> GetPosts()
        {
            return null;
        }
    }

    public class Post : AzureTableEntity
    {
        [PartionKey]
        public string BlogID { get; set; }

        //RowKey
        public string PostID { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
    }
}