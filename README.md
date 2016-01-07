# AzureTableFramework 

AzureTableFramework is an object mapper that enables .NET developers to work with Azure Table data using domain-specific objects. It eliminates the need for most of the data-access code that developers usually need to write.
_(...or what I wanted EF7 to do for Azure Tables - In all fairness, Azure Tables are different enough from SQL Tables that it deserves its own library from a SQL-based EF)._

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

There are a few key differences in using this library than you are probably used to; 
**If you don't read any other documentation, read this next section ...** 

### Key Differences

**Azure Tables require PartitionKeys to be defined!**
(Think of it like a free index)
If you don't decorate ONE (String) property with a [PartionKey] attribute; the table won't be created and the object can't be saved.
I suggest using a foriegn key.
Results are sorted by PartitionKey, then RowKey alphanumerically.  

**Tables are Named after ClassNames**
	If you change a class name, a new table will be created the next time you save the changes.

**RowKeys are ClassNames + "ID"**
**RowKeys are always GUIDS**

**Tables are created auto-magically, there are no database create/update migration tasks**
All you need are azure storage credentials.

### Additional Differences

**Unique**

**Code-First Only!**


	
### Key Similarities to EF

POCO defined data access


## Why Azure Tables?

I LOVE AZURE TABLES!

For TCO, AzureTables can be better and more reliable than almost ANY other database.

	 

|   | [AzureTableFramework](http://AzureTableFramework.com/ "Visit AzureTableFramework.com")	 |
| --------- | ----------- |
| Website | [http://AzureTableFramework.com](http://AzureTableFramework.com/ "Visit AzureTableFramework.com")	 |
| Documentation		|  [http://azuretableframework.rtfd.org](http://azuretableframework.rtfd.org/ "Visit Read the Docs azuretableframework.rtfd.org")   [![Documentation Status](https://readthedocs.org/projects/azuretableframework/badge/?version=latest)](http://azuretableframework.readthedocs.org/en/latest/?badge=latest) |
| GitHub Repository	| https://github.com/JeffHughes/AzureTableFramework.git |
| Owner		| Jeff Hughes (me@jeffhughes.com) |
| First Version	|  created 5 Jan 2016  |


