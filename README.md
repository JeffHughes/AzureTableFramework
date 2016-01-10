This is very much a work in progress - don't judge me, yet!

# AzureTableFramework 

AzureTableFramework is an object mapper that enables .NET developers to work with Azure Table data using domain-specific objects. It eliminates the need for most of the data-access code that developers usually need to write.
_(...or what I wanted EF7 to do for Azure Tables - In all fairness, Azure Tables are different enough from SQL Tables that it deserves its own library seperate from a SQL-based EF)._

Written in .Net Core (compatible with the full framework), it utilizes the official WindowsAzure.Storage API > 6.2.2, Dependency Injection.


## Features
--Indexing
--Azure Search
--Encryption
--Required Fields
--Parent/Child Relationships

## Getting Started

Download the nugetpackage (coming soon), define your classes and call them in context (in a familiar EF-like way).

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
    public string Url { get; set; }
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
    var B = await DB.Blogs.GetAsync("Blog1");
    Debug.WriteLine("B.Url  =  " + B?.Url);
}
```

Save
```
using (var DB = new BloggingContext())
{
    var BLOG = DB.Blogs.New();
	    BLOG.AuthorID = "HughesJeff";
	await DB.SaveChangesAsync();
}
```
## Important Notes

There are a few key differences in using this library than you are probably used to; 

**If you don't read any other documentation, read this next section ...** 

### Key Differences

**Tables are created auto-magically, there are no database create/update migration tasks**
All you need are azure storage credentials.  When "SaveChanges" is called, tables that don't exist will be created.

**Tables are named after Classes**  (public class Blog {} => "Blog" Table  )
If you change a class name (_and please don't forget it's corresponding ID property name, too_), a new table will be created the next time "SaveChanges" is called and previous data will remain in the old table with the old class name.  There is no migration.

**RowKeys are ClassNames + "ID"**  
There must be a (string) property that matches the case-sensitive pattern  Class Name + "ID" (e.g. class named Blog, must have a (string) property of "BlogID").
RowKeys are GUIDs by default, if you don't otherwise populate the property, a new guid will be created before saving.

```
public class Blog : AzureTableEntity
{
    ...
    //RowKey
    public string BlogID { get; set; }
    ...
}
```

**Storage Classes must inherit : AzureTableEntity**
- RowKey, PartitionKey, and ETag come from the Official API : EntityTable.
- LastUpdated and a few other goodies come from this library : AzureTableEntity (which inherits EntityTable).

**Azure Tables require PartitionKeys to be defined!**
(_Think of it as less of a burden and more like a free index_)
**If you don't decorate one (string) property in each class with a [PartionKey] attribute; an error will be thrown.** I suggest using a foriegn key propery and situating it above the ID property.
Results will eventually be returned sorted by PartitionKey, then RowKey alphanumerically.  

### Additional Differences

**Code-First Only!**



**You won't be able to find the columns for Class + "ID" and whatevery property you decorated with [PartitionKey] in the storage table**
	There's no reason to save the data twice.  So, in the table, the data from the Class Name + "ID" property is removed and saved as the RowKey column and the data in the property decorated by [PartitionKey] is removed from that property and saved as the PartitionKey column.  The data is restored to the appropriate field from storage.
	
```
    public class Blog : AzureTableEntity
    {
        [PartionKey]
        public string AuthorID { get; set; }
        //RowKey
        public string BlogID { get; set; }
        public string Url { get; set; }
        public Dictionary<string, Post> Posts { get; set; }
    }   
```

So, **Blog Table** will look like this:


|PartitionKey|RowKey|Timestamp		  |Url |
|------------|------|-----------------|----|
|HughesJeff  |Blog1 |1 Jan 2016 2:30PM|Url1|
|HughesJeff  |Blog2 |1 Jan 2016 4:30PM|Url2|

instead of this:


|PartitionKey	|	AuthorID	|	RowKey		|	BlogID		|	Timestamp		|	Url			|
|---------------|---------------|---------------|---------------|-------------------|---------------|
|HughesJeff		|**HughesJeff**	|Blog1			|**Blog1**		|1 Jan 2016 2:30PM	|Url1			|
|HughesJeff		|**HughesJeff**	|Blog2			|**Blog2**		|1 Jan 2016 4:30PM	|Url2			|
||:small_red_triangle: **Duplicate** :small_red_triangle: || :small_red_triangle: **Duplicate** :small_red_triangle:|||


The library handles the deduping and reconstruction, behind the scenes.


**AzureTableDictionary.Items stores a dictionary retreivable by ID**
-- Using Items.New() **adds a GUID automatically**, and adds the item to the items list by that GUID.  If you change the ID, it won't be automagically reflected in the Items list.

```
DB.Blogs.Items["Blog1"]
```

<!--There are no Server-side relationships and Azure couldn't care less about data definition changes.
If you want to keep track of--> 


### Key Similarities to EF

POCO defined data access


## Why Azure Tables?

I LOVE AZURE TABLES!

For TCO, AzureTables can be better and more reliable than almost ANY other database.

	 

|   | [AzureTableFramework](http://AzureTableFramework.com/ "Visit AzureTableFramework.com")	 |
| ---------: | ----------- |
| Website | [http://AzureTableFramework.com](http://AzureTableFramework.com/ "Visit AzureTableFramework.com")	 |
| Documentation		|  [http://azuretableframework.rtfd.org](http://azuretableframework.rtfd.org/ "Visit Read the Docs azuretableframework.rtfd.org")   [![Documentation Status](https://readthedocs.org/projects/azuretableframework/badge/?version=latest)](http://azuretableframework.readthedocs.org/en/latest/?badge=latest) |
| GitHub Repository	| https://github.com/JeffHughes/AzureTableFramework.git |
| Owner		| Jeff Hughes (me@jeffhughes.com) |
| First Version	|  created 5 Jan 2016  |


