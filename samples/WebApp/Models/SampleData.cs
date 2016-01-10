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
            //using (var DB = new BloggingContext())
            //{
            //    var B2 = await DB.Blogs.GetByIDAsync("10b6c97d-115a-4fa7-bdfc-737d2444a2ec");

            //    Debug.WriteLine("B.AuthorID for 10b6c97d-115a-4fa7-bdfc-737d2444a2ec =  " + B2?.AuthorID);
            //}

            using (var DB = new BloggingContext())
            {
                // 8587492251349240893 - written
                // 8587492503338016601 - query

                var BLOG = DB.Blogs.New();

                BLOG.AuthorID = "654564";
                BLOG.Url = "SomeURL";

                var BLOG2 = DB.Blogs.New();

                BLOG2.AuthorID = "654565";
                BLOG2.Url = "SomeURL";

                await DB.SaveChangesAsync();

                var list = await DB.Blogs.GetLastUpdated(DateTime.UtcNow.AddMinutes(-2)); //Convert.ToDateTime("1/1/2015 1:00PM GMT")

                foreach (var item in list)
                {
                    Debug.WriteLine(item.Timestamp + " " + item.BlogID);
                }

                //BLOG.HardDelete();

                //await DB.SaveChangesAsync();
            }
        }
    }
}