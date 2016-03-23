using AzureTableFramework;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloggingData
{
    public class BloggingContext : AzureTableContext
    {
        public AzureTableDictionary<Blog> Blogs { get; set; } = new AzureTableDictionary<Blog>();

        public BloggingContext(IConfigurationRoot config) : base(config)
        {
        }
    }
}