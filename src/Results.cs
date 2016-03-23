using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTableFramework
{
    public class AzureTableQueryResults<T>
    {
        public List<T> Results { get; set; }

        public TableContinuationToken token { get; set; }
    }
}