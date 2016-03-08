using Samples.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace CoreTests
{
    public class RequirementTests
    {
        [Fact]
        public async Task MissingRequiredProperty()
        {
            using (var DB = new BloggingContext())
            {
                var B = DB.Blogs.New();

                var exceptionThrown = false;
                try
                {
                    await DB.SaveChangesAsync();
                }
                catch (Exception Ex)
                {
                    Debug.WriteLine(Ex.Message + "");
                    exceptionThrown = true;
                }

                Console.WriteLine("write line");

                Assert.True(exceptionThrown);
            }
        }
    }
}