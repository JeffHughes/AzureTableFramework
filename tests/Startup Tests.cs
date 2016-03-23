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
    public class Startup
    {
        [Fact]
        public void UserSecretsFromConfigBuilder()
        {
            var c = new BloggingData.BloggingContext(new ConfigurationBuilder().AddUserSecrets().Build());
            Debug.WriteLine("PSAN = " + c.PrimaryStorageAccountName);
            Assert.True(!string.IsNullOrEmpty(c.PrimaryStorageAccountName));
        }

        [Fact]
        public void DictionaryContext()
        {
            var c = new BloggingData.BloggingContext(new ConfigurationBuilder().AddUserSecrets().Build());
            Debug.WriteLine("PSAN = " + c.Blogs.Context.PrimaryStorageAccountName);
            Assert.True(!string.IsNullOrEmpty(c.Blogs.Context.PrimaryStorageAccountName));
        }
    }
}