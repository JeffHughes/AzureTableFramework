using AzureTableFramework.Core;
using Microsoft.WindowsAzure.Storage.Table;
using Samples.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                var B2 = await DB.Blogs.GetByIDAsync("10b6c97d-115a-4fa7-bdfc-737d2444a2ec");

                Debug.WriteLine("B.AuthorID for 10b6c97d-115a-4fa7-bdfc-737d2444a2ec =  " + B2?.AuthorID);
            }

            using (var DB = new BloggingContext())
            {
                var BLOG = DB.Blogs.New();

                BLOG.AuthorID = "654564";

                var tq = Utils.FilterString("BlogID", QueryComparisons.NotEqual.ToString(), "");
                await DB.Blogs.QueryAsync(tq, null);

                await DB.SaveChangesAsync();
            }
        }
    }
}