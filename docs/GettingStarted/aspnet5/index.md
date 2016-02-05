ASP.Net Core Instructions
===================================

Make sure Secret Manager is installed
https://docs.asp.net/en/latest/security/app-secrets.html
For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709

Manage User Secrets
```
{
  "Blogging:PrimaryStorageAccountName": "",
  "Blogging:PrimaryStorageAccountKey": "",
  "Blogging:IndexStorageAccountName": "",
  "Blogging:IndexStorageAccountKey": "",
  "Blogging:EncryptionKey16Chars": "",
  "Blogging:SearchServiceName": "",
  "Blogging:SearchServiceManagementKey": ""
}
```

Don't forget to Save the settings on Application Settings in Azure

in Startup.cs
```
public Startup(IHostingEnvironment env)
{
    ...

    if (env.IsDevelopment())
    {

        builder.AddUserSecrets();
    ...
    }

    builder.AddEnvironmentVariables();
    Configuration = builder.Build();
}

...

public IConfigurationRoot Configuration { get; set; }

public void ConfigureServices(IServiceCollection services)
{
   ...
   services.AddInstance(Configuration);
}
```

```
public class BloggingContext : AzureTablesContext
{
    public AzureTableDictionary<Blog> Blogs { get; set; }

    public BloggingContext(IConfigurationRoot config) : base(config)
    {
    }
}
```

```
public class BloggingController : Controller
{
    public IConfigurationRoot Config { get; set; }

    public BloggingController(IConfigurationRoot root)
    {
        Config = root;
    }

	[HttpGet]
    public async Task<IActionResult> Blog(string id)
    {
        Blog p = new Blog();

        if (!string.IsNullOrEmpty(id))
            using (var DB = new BloggingContext(Config))
                p = await DB.Blogs.GetByIDAsync(id);

        return View(p);
    }

    [HttpPost]
    public async Task<IActionResult> Blog(Blog p)
    {
        using (var DB = new BloggingContext(Config))
        {
            DB.Blogs.Add(p);
            await DB.SaveChangesAsync();
        }

        return View(p);
    }

}
```