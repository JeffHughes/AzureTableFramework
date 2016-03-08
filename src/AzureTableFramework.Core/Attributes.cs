using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PartitionKeyAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EncryptAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IndexAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DynamicIndexAttribute : Attribute
    {
        public List<string> Properties { get; set; }

        //public DynamicIndexAttribute()
        //{
        //}

        public DynamicIndexAttribute(params string[] properties)
        {
            Properties = properties.ToList();
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BlobAttribute : Attribute
    {
        public string FileExtension { get; set; }

        public BlobAttribute()
        {
        }

        public BlobAttribute(string ext)
        {
            FileExtension = ext;
        }
    }

    /*
    [AttributeUsage(AttributeTargets.Class)]
    public class AzureSearchAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SearchAttribute : Attribute
    {
        public bool IsSearchable { get; set; }

        public bool IsFilterable { get; set; }

        public bool IsFacetable { get; set; }

        public bool IsRetreivable { get; set; }

        public SearchAttribute(bool isSearchable)
        {
            this.IsSearchable = isSearchable;
        }

        public SearchAttribute(bool isSearchable, bool filterable)
        {
            this.IsSearchable = isSearchable;
            this.IsFilterable = filterable;
        }

        public SearchAttribute(bool isSearchable, bool isFilterable, bool isFacetable)
        {
            this.IsSearchable = isSearchable;
            this.IsFilterable = isFilterable;
            this.IsFacetable = isFacetable;
        }

        public SearchAttribute(bool isSearchable, bool isFilterable, bool isFacetable, bool isRetreivable)
        {
            this.IsSearchable = isSearchable;
            this.IsFilterable = isFilterable;
            this.IsFacetable = isFacetable;
            this.IsRetreivable = isRetreivable;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SuggesterAttribute : Attribute
    {
        public string Name { get; set; }

        public SuggesterAttribute(string name)
        {
            this.Name = name;
        }
    }

    public class IndexDetails
    {
        public string TableName { get; set; }

        public string PartitionKey { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RunBeforeSaveAttribute : Attribute
    {
        //public int Order { get; set; }

        //public RunBeforeSaveAttribute(int _Order)
        //{
        //    Order = _Order;
        //}
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RunBeforeDeleteAttribute : Attribute
    {
        //public int Order { get; set; }

        //public RunBeforeDeleteAttribute(int _Order)
        //{
        //    Order = _Order;
        //}
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RunAfterSaveAttribute : Attribute
    {
        //public int Order { get; set; }

        //public RunAfterSaveAttribute(int _Order)
        //{
        //    Order = _Order;
        //}
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RunAfterDeleteAttribute : Attribute
    {
        //public int Order { get; set; }

        //public RunAfterDeleteAttribute(int _Order)
        //{
        //    Order = _Order;
        //}
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class LoadAllBlobsAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class LoadAllBlobPrerequisitesAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class LoadAllBlobsFlagAttribute : Attribute
    {
    }

    */
}