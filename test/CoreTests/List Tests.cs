using Samples.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace CoreTests
{
    public class ListTests
    {
        [Fact]
        public async Task DifferentProperties()
        {
            var BlogID = Guid.NewGuid().ToString();
            var AuthorID = "ABCDEFG \\ # / % ? '";

            using (var DB = new BloggingContext())
            {
                var B = DB.Blogs.Add(new Blog { BlogID = BlogID });

                B.AuthorID = AuthorID;
                B.Url = "SomeURL";

                await DB.SaveChangesAsync();
            }

            using (var DB = new BloggingContext())
            {
                var B = await DB.Blogs.GetByIDAsync(BlogID);
                DB.Blogs.Items[BlogID].HardDelete();

                Debug.WriteLine(string.Format(" B.AuthorID : {0} AuthorID: {1}", B.AuthorID, AuthorID));

                Assert.True(B != null);
                Assert.True(B.AuthorID.Equals(AuthorID));

                B.HardDelete();
                await DB.SaveChangesAsync();
            }

            using (var DB = new BloggingContext())
            {
                var B2 = await DB.Blogs.GetByIDAsync(BlogID);
                Assert.True(B2 == null);
            }
        }
    }
}