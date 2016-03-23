using BloggingData;
using coreWebsite;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class CRUD
    {
        [Fact]
        public async Task AddBlog()
        {
            var OperationSW = Stopwatch.StartNew();

            var BlogID = Guid.NewGuid().ToString();

            Debug.WriteLine("===== Start => Operation #1 Add Blog ======");
            using (var DB = new BloggingContext(new ConfigurationBuilder().AddUserSecrets().Build()))
            {
                var B = DB.Blogs.Add(new Blog { BlogID = BlogID });

                B.AuthorID = Guid.NewGuid().ToString();
                B.Url = "SomeURL";
                B.TestIndex = "IndexTest1";

                await DB.SaveChangesAsync(true);
            }
            Debug.WriteLine($"===== End  => Operation #1 Add Blog ====== {OperationSW.Elapsed} \n\n");

            Debug.WriteLine("===== Start => Operation #2 Search for Blog ======\n");
            using (var DB = new BloggingContext(new ConfigurationBuilder().AddUserSecrets().Build()))
            {
                var B = await DB.Blogs.GetByIDAsync(BlogID);
                Debug.WriteLine("Retreived: " + B.Timestamp.ToUniversalTime().ToString());
                Debug.WriteLine("===== End  => Operation #2 Search for Blog ======\n\n");

                Debug.WriteLine("===== Start => Operation #3 Delete Blog ======");
                Assert.True(B != null);
                B.HardDelete();
                await DB.SaveChangesAsync();
            }
            Debug.WriteLine("===== End  => Operation #3 Delete Blog ======\n\n");

            Debug.WriteLine("===== Start => Operation #4 Verify Blog is gone ======\n");
            using (var DB = new BloggingContext(new ConfigurationBuilder().AddUserSecrets().Build()))
            {
                var B2 = await DB.Blogs.GetByIDAsync(BlogID);
                Assert.True(B2 == null);
            }
            Debug.WriteLine("\n===== End  => Operation #4 Verify Blog is gone ======");
        }
    }
}