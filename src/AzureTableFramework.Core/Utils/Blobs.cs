using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Linq;
using System.Reflection;

using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    public static partial class Utils
    {
        //[Blob(".txt")]
        //public string PropName { get; set; }

        //[Blob(".pdf")]
        //public Byte[] PropName { get; set; }

        private static SortedList<string, CloudBlobContainer> _CloudBlobs = new SortedList<string, CloudBlobContainer>();

        public static SortedList<string, CloudBlobContainer> CloudBlobs { get { return _CloudBlobs; } set { _CloudBlobs = value; } }

        public static async Task<CloudBlobContainer> BlobContainerAsync(string TableName, CloudStorageAccount AzureStorageAccount)
        {
            if (CloudBlobs.ContainsKey(TableName)) return CloudBlobs[TableName];

            var c = AzureStorageAccount
                    .CreateCloudBlobClient()
                    .GetContainerReference(TableName.ToLower());

            await c.CreateIfNotExistsAsync();
            await c.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });
            if (!CloudBlobs.ContainsKey(TableName)) CloudBlobs.Add(TableName, c);
            return c;
        }

        public static async Task SaveBlobContentAsync(CloudBlockBlob CBB, object Content, string filenameWithoutExtension, string extension)
        {
            string context = "application";
            switch (Content.GetType().ToString().Replace("System.", ""))
            {
                case "Byte[]":
                    if (((byte[])Content).Any())
                        await CBB.UploadFromByteArrayAsync((byte[])Content, 0, ((byte[])Content).Length, null, null, null);

                    break;

                default:
                    if (!string.IsNullOrEmpty((string)Content))
                    {
                        await CBB.UploadTextAsync((string)Content);
                        context = "text";
                    }
                    break;
            }

            CBB.Properties.ContentType = $"{context}/{extension}";
            CBB.Properties.ContentDisposition = $"inline; filename={filenameWithoutExtension}.{extension}";

            await CBB.SetPropertiesAsync();
        }

        public static async Task DeleteBlobContentAsync(CloudBlockBlob CBB)
        {
            await CBB.DeleteIfExistsAsync();
        }
    }
}