using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace AzureTableFramework.Core
{
    public class AzureTableQueryResults<T>
    {
        public List<T> Results { get; set; }

        public TableContinuationToken token { get; set; }
    }
}