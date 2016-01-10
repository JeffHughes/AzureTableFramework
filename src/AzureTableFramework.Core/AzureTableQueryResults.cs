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

        public List<DynamicTableEntity> DynamicTableEntities
        {
            set
            {
                Mapper.CreateMap<DynamicTableEntity, T>();

                var PartitionKeyPropertyName = Utils.GetPartitionKeyPropertyName(typeof(T));
                var RowKeyPropertyName = Utils.GetRowKeyPropertyName(typeof(T));

                Results = new List<T>();
                foreach (var item in value)
                {
                    var obj = Mapper.Map<DynamicTableEntity, T>(item);

                    if (!(Utils.GetVal(obj, RowKeyPropertyName) == null))
                        Utils.SetVal(obj, RowKeyPropertyName, Utils.GetVal(obj, "RowKey"));

                    if (!(Utils.GetVal(obj, PartitionKeyPropertyName) == null))
                        Utils.SetVal(obj, PartitionKeyPropertyName, Utils.GetVal(obj, "PartitionKey"));

                    //TODO: Decryption

                    Results.Add(obj);
                }
            }
        }
    }
}