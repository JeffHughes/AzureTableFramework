using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTableFramework
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PartitionKeyAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BackupAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IndexAttribute : Attribute
    {
        public List<string> Properties { get; set; }

        public bool PartitionKeyOnly { get; set; }

        public IndexAttribute()
        {
        }

        public IndexAttribute(bool PKOnly)
        {
            PartitionKeyOnly = PKOnly;
        }

        public IndexAttribute(bool PKOnly, params string[] properties)
        {
            PartitionKeyOnly = PKOnly;
            Properties = properties.ToList();
        }

        public IndexAttribute(params string[] properties)
        {
            Properties = properties.ToList();
        }
    }
}