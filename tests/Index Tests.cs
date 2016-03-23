using AzureTableFramework;
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
    public class ObjectForNameTest
    {
        public string Prop1string { get; set; }
        public string Prop2string { get; set; }
        public string Prop3string { get; set; }
        public string Prop4string { get; set; }
        public int Prop5int { get; set; }
        public double Prop6Double { get; set; }
        public Guid Prop7Guid { get; set; }
        public bool Prop8bool { get; set; }
        public long Prop9long { get; set; }
        public DateTime Prop10DateTime { get; set; }
    }

    public class Indexes
    {
        [Fact]
        public void SimpleName()
        {
            var tableName = Utils.IndexTableName(new ObjectForNameTest(), "Prop1string");
            Debug.WriteLine(tableName);
            Assert.Equal(tableName, "ObjectForNameTestIdxProp1string");
        }

        [Fact]
        public void ComplexName()
        {
            var OFNT = new ObjectForNameTest()
            {
                Prop1string = "HelloFromTheOtherSideThankYouVeryMuch!Property1",
                Prop2string = "134d50df-d9ad-4571-8b95-4bfe6ddb7771",
                Prop3string = "2a0e2feb-3a3e-4589-bbfb-9df9ef4f62ee",
                Prop4string = "b498d8d9-b194-470c-bd2f-3d9e6f9feed5",
                Prop5int = int.MaxValue,
                Prop6Double = double.MaxValue,
                Prop7Guid = new Guid("b498d8d9-b194-470c-bd2f-3d9e6f9feed5"),
                Prop8bool = true,
                Prop9long = long.MaxValue,
                Prop10DateTime = DateTime.MaxValue,
            };

            var tableName = Utils.IndexTableName(OFNT, new List<string> { "Prop1string", "Prop2string", "Prop3string", "Prop4string" });
            Debug.WriteLine(tableName);
            Assert.Equal(tableName, "ObjectForNameTestIdx3611779572-7763995");

            tableName = Utils.IndexTableName(OFNT, new List<string> {
                "Prop1string", "Prop2string", "Prop3string", "Prop4string",
                "Prop5int", "Prop6Double", "Prop7Guid", "Prop8bool", "Prop9long"
            });

            Debug.WriteLine(tableName);
            Assert.Equal(tableName, "ObjectForNameTestIdx4114560242-3314409468");
        }
    }
}