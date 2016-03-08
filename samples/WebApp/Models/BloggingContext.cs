using AzureTableFramework.Core;
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
}