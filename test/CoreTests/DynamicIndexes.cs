using Samples.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace CoreTests
{
    public class DynamicIndexes
    {
        [Fact]
        public async Task CreateObjectWLongNamedDynamicIndex()
        {
            var CommentID = "11111111-1111-1111-1111-111111111111-11111111-1111-1111-1111-111111111111"; // Guid.NewGuid().ToString(); //
            var UserID = "11111111-1111-1111-1111-111111111112-11111111-1111-1111-1111-111111111112"; //Guid.NewGuid().ToString(); //

            using (var DB = new BloggingContext())
            {
                var B = DB.Comments.Add(new Comment { CommentID = CommentID });

                B.UserID = UserID;
                B.PostID = "TestPostID";

                await DB.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task CreateObjectWDynamicIndex()
        {
            var CommentID = "11111111-1111-1111-1111-111111111111"; // Guid.NewGuid().ToString(); //
            var UserID = "11111111-1111-1111-1111-111111111112"; //Guid.NewGuid().ToString(); //

            using (var DB = new BloggingContext())
            {
                var B = DB.Comments.Add(new Comment { CommentID = CommentID });

                B.UserID = UserID;
                B.PostID = "TestPostID";

                await DB.SaveChangesAsync();
            }

            //using (var DB = new BloggingContext())
            //{
            //    var B = await DB.Comments.GetByIDAsync(CommentID);
            //    // DB.Comments.Items[CommentID].HardDelete();
            //    Assert.True(B != null);
            //    B.HardDelete();
            //    await DB.SaveChangesAsync();
            //}

            //using (var DB = new BloggingContext())
            //{
            //    var B2 = await DB.Comments.GetByIDAsync(CommentID);
            //    Assert.True(B2 == null);
            //}
        }

        /*
        [Fact]
        private async Task RetreiveOnLastUpdated()
        {
            var BlogID = Guid.NewGuid().ToString();

            using (var DB = new BloggingContext())
            {
                var B = DB.Blogs.Add(new Blog { BlogID = BlogID });

                B.AuthorID = Guid.NewGuid().ToString();
                B.Url = "SomeURL";
                B.TestIndex = "IndexTest1";

                await DB.SaveChangesAsync();

                Debug.WriteLine("B.Timestamp =" + DB.Blogs.Items[BlogID].Timestamp);
            }

            using (var DB = new BloggingContext())
            {
                var sinceDate = DateTime.UtcNow.AddMinutes(-2);

                Debug.WriteLine("sinceDate =" + sinceDate);

                var list = await DB.Blogs.GetLastUpdated(sinceDate);

                var found = false;
                if (list.Count > 0)
                    foreach (var item in list)
                    {
                        if (item.BlogID == BlogID) found = true;
                    }

                Assert.True(found);
            }

            using (var DB = new BloggingContext())
            {
                var B = await DB.Blogs.GetByIDAsync(BlogID);
                // DB.Blogs.Items[BlogID].HardDelete();
                Assert.True(B != null);
                B.HardDelete();
                await DB.SaveChangesAsync();
            }

            using (var DB = new BloggingContext())
            {
                var B2 = await DB.Blogs.GetByIDAsync(BlogID);
                Assert.True(B2 == null);
            }
        }

        [Fact]
        private static async Task AddAndDeleteLots()
        {
            var AuthorID = Guid.NewGuid().ToString();

            using (var DB = new BloggingContext())
            {
                var SW = new Stopwatch(); SW.Start();

                for (int x = 0; x < 5; x++)
                {
                    var B = DB.Blogs.New();

                    B.AuthorID = AuthorID;
                    B.Url = "SomeURL";
                    B.TestIndex = "IndexTest1";
                }
                await DB.SaveChangesAsync();

                Debug.WriteLine("===== Total Insert Time: " + SW.Elapsed);

                foreach (var B in DB.Blogs.ToList()) B.HardDelete();

                var SW2 = new Stopwatch(); SW2.Start();

                await DB.SaveChangesAsync();

                Debug.WriteLine("===== Total Delete Time: " + SW2.Elapsed);
            }
        }

        [Fact]
        private static async Task AddAndDeleteDifferentTypes()
        {
            var AuthorID = Guid.NewGuid().ToString();

            using (var DB = new BloggingContext())
            {
                var SW = new Stopwatch(); SW.Start();

                for (int x = 0; x < 5; x++)
                {
                    var B = DB.Blogs.New();

                    B.AuthorID = AuthorID;
                    B.Url = "SomeURL";
                    B.TestIndex = "IndexTest1";
                }

                for (int x = 0; x < 5; x++)
                {
                    var B = DB.Posts.New();
                }

                await DB.SaveChangesAsync();

                Debug.WriteLine("===== Total Insert Time: " + SW.Elapsed);

                foreach (var B in DB.Blogs.ToList()) B.HardDelete();
                foreach (var B in DB.Posts.ToList()) B.HardDelete();

                var SW2 = new Stopwatch(); SW2.Start();

                await DB.SaveChangesAsync();

                Debug.WriteLine("===== Total Delete Time: " + SW2.Elapsed);
            }
        }

        [Fact]
        public async Task RemoveObsoleteData()
        {
            var AuthorID1 = "11111111-1111-1111-1111-111111111111"; // Guid.NewGuid().ToString();
            var AuthorID2 = "11111111-1111-1111-1111-111111111112"; // Guid.NewGuid().ToString();

            var BlogID = Guid.NewGuid().ToString();

            using (var DB = new BloggingContext())
            {
                var B = DB.Blogs.Add(new Blog { BlogID = BlogID });

                B.AuthorID = AuthorID1;
                B.Url = "SomeURL";
                B.TestIndex = "IndexTest1";

                await DB.SaveChangesAsync();
            }

            using (var DB = new BloggingContext())
            {
                var B = DB.Blogs.Add(new Blog { BlogID = BlogID });

                B.AuthorID = AuthorID2;
                B.Url = "SomeURL";
                B.TestIndex = "IndexTest1";

                await DB.SaveChangesAsync();
            }

            using (var DB = new BloggingContext())
            {
                var B = await DB.Blogs.GetAllByPartitionKeyAsync(AuthorID1);
                Assert.True(B == null);
                await DB.SaveChangesAsync();
            }

            using (var DB = new BloggingContext())
            {
                var B2 = await DB.Blogs.GetByIDAsync(BlogID);
                B2.HardDelete();
                await DB.SaveChangesAsync();
            }

    */
    }
}