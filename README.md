# AzureTableFramework 

AzureTableFramework is an object mapper that enables .NET developers to work with Azure Table data using domain-specific objects. It eliminates the need for most of the data-access code that developers usually need to write 

(...or what I wanted EF7 to do for Azure Tables - In all fairness, Azure Tables are different enough from SQL Tables that it deserves its own library from a SQL-based EF).

Written in .Net Core (compatible with the full framework), it utilizes the official WindowsAzure.Storage API > 6.2.2


## Getting Started

Define
```
  public class BloggingContext : AzureTablesContext
    {
        public AzureTableDictionary<Blog> Blogs { get; set; }
        public AzureTableDictionary<Post> Posts { get; set; }
    }

    public class Blog : AzureTableEntity
    {
        [PartionKey]
        public string AuthorID { get; set; }
        //RowKey
        public string BlogID { get; set; }
        public string url { get; set; }
        public Dictionary<string, Post> Posts { get; set; }
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
```

Get
```
	using (var DB = new BloggingContext())
        {
            var B2 = await DB.Blogs.GetAsync("10b6c97d-115a-4fa7-bdfc-737d2444a2ec");
            Debug.WriteLine("B.AuthorID  =  " + B2?.AuthorID);
        }
```

Save
```
    using (var DB = new BloggingContext())
        {
            var BLOG = DB.Blogs.New();
	            BLOG.AuthorID = "654564";
	        await DB.SaveChangesAsync();
        }
```
## Important Notes




### Key Similarities to EF



### Key Differences

Azure Tables require PartitionKeys to be defined!
(Think of it like a free index)

RowKeys are ClassNames + "ID"
RowKeys are always GUIDS


## Why Azure Tables

I LOVE AZURE TABLES!

AzureTables can be dramatically less expensive and more reliable than almost ANY other database.

	 

|   | [AzureTableFramework](http://AzureTableFramework.com/ "Visit AzureTableFramework.com")	 |
| --------- | ----------- |
| Website | [http://AzureTableFramework.com](http://AzureTableFramework.com/ "Visit AzureTableFramework.com")	 |
| Documentation		|  [http://azuretableframework.rtfd.org](http://azuretableframework.rtfd.org/ "Visit Read the Docs azuretableframework.rtfd.org")   [![Documentation Status](https://readthedocs.org/projects/azuretableframework/badge/?version=latest)](http://azuretableframework.readthedocs.org/en/latest/?badge=latest) |
| GitHub Repository	| https://github.com/JeffHughes/AzureTableFramework.git |
| Owner		| Jeff Hughes (me@jeffhughes.com) |
| First Version	|  created 5 Jan 2016  |


