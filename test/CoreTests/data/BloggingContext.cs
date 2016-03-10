using AzureTableFramework.Core;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Samples.Common
{
    public class BloggingContext : AzureTablesContext
    {
        public AzureTableDictionary<Blog> Blogs { get; set; }
        public AzureTableDictionary<Post> Posts { get; set; }
        public AzureTableDictionary<Comment> Comments { get; set; }
        public AzureTableDictionary<CommentImage> CommentImages { get; set; }

        public BloggingContext()
        {
            //PrimaryStorageAccountName = "AzureBlogTest1";
            //PrimaryStorageAccountKey = "u6eGo/IfJ0CaO4gwpDNWeTNwhu6GyThisIsAFakeCodeSBtPCs0okJ9zy3bguKV1oR2Ukqcr7ala6i872A==";

            //IndexStorageAccountName = "AzureBlogTest1idx";
            //IndexStorageAccountKey = "q0jIB778aJWZMwBgkVIiZ3zypNm/YQjkThisIsAFakeCodekNsyrm4jFGdLhV+EipOPtE+QZJXfSdnXRvMZ8EKw==";

            //EncryptionKey16Chars = "IfJ0CaO4gwpDNWeTNwhu6GyInRGlR+aA";

            /* Swap before committing  */
            PrimaryStorageAccountName = "jhtest2";
            PrimaryStorageAccountKey = "9EzOsuJ4wJf17YeUnDjVIaRqjX3BEmJ3nNgSjSPwSAppqG9YmXYeIsGzXdozmUDA6Mr8/DhylqcgUN44YNd0aw==";

            IndexStorageAccountName = "jhtest2idx";
            IndexStorageAccountKey = "3uk/55TOQ+tInk55YyOc4yuClSkq3fJwgB7MRp/dTdoW78D9aiv4vwq8nVxz0wZ5O5c35V6zMXMp2OnHAjW0Dw==";

            EncryptionKey16Chars = "IfJ0CaO4gwpDNWeTNwhu6GyInRGlR+aA";

            SearchServiceName = "";
            SearchServiceManagementKey = "";
        }
    }

    public class Blog : AzureTableEntity
    {
        [PartitionKey]
        public string AuthorID { get; set; }

        //RowKey
        public string BlogID { get; set; }

        [Required]
        public string Url { get; set; }

        [Index]
        public string TestIndex { get; set; }

        public Dictionary<string, Post> Posts { get; set; }

        public Dictionary<string, Post> GetPosts()
        {
            return null;
        }
    }

    public class Post : AzureTableEntity
    {
        [PartitionKey]
        public string BlogID { get; set; }

        //RowKey
        public string PostID { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class Comment : AzureTableEntity
    {
        [PartitionKey]
        public string PostID { get; set; }

        public string CommentID { get; set; }

        public string UserID { get; set; }

        [DynamicIndex("UserID")]
        [IgnoreProperty]
        public new string LastUpdated
        {
            get { return Utils.TicksFromMax(Timestamp.UtcDateTime); }
        }
    }

    public class CommentImage : AzureTableEntity
    {
        [PartitionKey]
        public string CommentID { get; set; }

        public string CommentImageID { get; set; }

        [Blob("jpg"), IgnoreProperty, Eager]
        public Byte[] Picture { get; set; }

        [Blob, IgnoreProperty]
        public Byte[] Picture1 { get; set; }

        public BlobData Picture1BlobData { get; set; }

        [Blob, IgnoreProperty]
        public Byte[] Picture2 { get; set; }

        [BlobData("Picture2")]
        public BlobData PictureUnspecifiedByNameBlobData { get; set; }

        [Blob("image/jpg", "jpg"), IgnoreProperty]
        public Byte[] Picture3 { get; set; }
    }

    public class CommentFile : AzureTableEntity
    {
        [PartitionKey]
        public string CommentID { get; set; }

        public string CommentFileID { get; set; }

        [Blob("html"), Eager]
        public Byte[] Html { get; set; }
    }
}