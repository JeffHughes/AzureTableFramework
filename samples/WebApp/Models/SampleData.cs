﻿using AzureTableFramework.Core;
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
            var BlogID = Guid.NewGuid().ToString();

            using (var DB = new BloggingContext())
            {
                var B = DB.Blogs.Add(new Blog { BlogID = BlogID });

                B.AuthorID = Guid.NewGuid().ToString();
                B.Url = "SomeURL";

                await DB.SaveChangesAsync();
            }

            using (var DB = new BloggingContext())
            {
                var B = await DB.Blogs.GetByIDAsync(BlogID);
                // DB.Blogs.Items[BlogID].HardDelete();

                B.HardDelete();
                await DB.SaveChangesAsync();
            }
        }
    }
}