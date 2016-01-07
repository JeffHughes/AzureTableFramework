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

**Tables are created auto-magically, there are no database create/update migration tasks**
All you need are azure storage credentials.
There are no Server-side relationships and
If you want to keep

**Azure Tables require PartitionKeys to be defined!**
(Think of it like a free index)
If you don't decorate ONE (String) property with a [PartionKey] attribute; the table won't be created and the object can't be saved.
I suggest using a foriegn key.
Results are sorted by PartitionKey, then RowKey alphanumerically.  

**Tables are Named after ClassNames**
If you change a class name, a new table will be created the next time you save the changes and previous data will remain in the old table with the old class name.

**RowKeys are ClassNames + "ID"**  
There must be a (string) property that matches the case-sensitive pattern  ClassNames + "ID" (e.g. class named Blog, must have a (string) property of "BlogID").
RowKeys are GUIDs by default, if you don't otherwise populate the property, a new guid will be created.

**Classes must inherit : AzureTableEntity**
RowKey, PartitionKey, and ETag come from the Official API : EntityTable.
LastUpdated and a few other goodies come from this library : AzureTableEntity

### Additional Differences



**Code-First Only!**

**AzureTableDictionary.Items stores a dictionary of items by ID**
New() adds a GUID automatically

**You won't be able to find the columns for Class + "ID" and whatevery property you decorated with [PartitionKey] in the storage table**
	There's no reason to save the data twice.  So, in the table the ID property data is saved as the RowKey and  
	
```
    public class Blog : AzureTableEntity
    {
        [PartionKey]
        public string AuthorID { get; set; }
        //RowKey
        public string BlogID { get; set; }
        public string url { get; set; }
        public Dictionary<string, Post> Posts { get; set; }
    }   
```

So, Tables will look like this:
**Blog Table**


|PartitionKey|RowKey|Timestamp|Url|
|-----|------|------|------|
|HughesJeff|Blog1|1 Jan 2016 2:30PM|Url1|
|HughesJeff|Blog2|1 Jan 2016 4:30PM|Url2|

Instead of this:


|PartitionKey|AuthorID|RowKey|BlogID|Timestamp|Url|
|-----|------|------|------|------|------|
|HughesJeff|HughesJeff|Blog1|Blog1|1 Jan 2016 2:30PM|Url1|
|HughesJeff|HughesJeff|Blog2|Blog2|1 Jan 2016 4:30PM|Url2|


The library handles the deduping and reconstruction, behind the scenes.


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


