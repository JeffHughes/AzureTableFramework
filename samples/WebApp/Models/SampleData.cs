using AzureTableFramework.Core;
using Samples.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class SampleData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            AsyncHelpers.RunSync(() => RunSampleData());
        }

        public static async Task RunSampleData()
        {
            using (var DB = new BloggingContext())
            {
                var BLOG = new Blog();

                BLOG.AuthorID = "123456789";
                BLOG.BlogID = Guid.NewGuid().ToString();

                DB.Blogs.Add(BLOG);

                await DB.SaveChangesAsync();
            }
        }
    }
}