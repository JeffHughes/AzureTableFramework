using AzureTableFramework.Core;
using Samples.Common;
using System;
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
            var BlogID = "11111111-1111-1111-1111-111111111111"; // Guid.NewGuid().ToString();
            var AuthorID = "11111111-1111-1111-1111-111111111112"; // Guid.NewGuid().ToString();

            using (var DB = new BloggingContext())
            {
                var B = DB.Blogs.Add(new Blog { BlogID = BlogID });

                B.AuthorID = AuthorID;
                B.Url = "SomeURL";
                B.TestIndex = "testing16";

                await DB.SaveChangesAsync();
            }

            //using (var DB = new BloggingContext())
            //{
            //    var B = await DB.Blogs.GetByIDAsync(BlogID);
            //    // DB.Blogs.Items[BlogID].HardDelete();

            //    if (B is object)
            //    {
            //        B.HardDelete();
            //        await DB.SaveChangesAsync();
            //    }
            //}
        }
    }
}