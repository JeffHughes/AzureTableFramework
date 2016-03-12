﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    public static partial class Utils
    {
        private static SortedList<string, CloudBlobContainer> _CloudBlobs = new SortedList<string, CloudBlobContainer>();

        public static SortedList<string, CloudBlobContainer> CloudBlobs { get { return _CloudBlobs; } set { _CloudBlobs = value; } }

        public static async Task<CloudBlobContainer> BlobContainer(string TableName, CloudStorageAccount AzureStorageAccount)
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

        public static async Task<CloudBlobContainer> BlobContainer(object obj, CloudStorageAccount AzureStorageAccount)
        {
            return await BlobContainer(obj.GetType().Name, AzureStorageAccount);
        }

        public static async Task<CloudBlobDirectory> BlobDirectory(object obj, CloudStorageAccount AzureStorageAccount)
        {
            return (await BlobContainer(obj, AzureStorageAccount)).GetDirectoryReference(GetRowKeyValue(obj));
        }

        public static async Task<Dictionary<string, IListBlobItem>> Blobs(object obj, CloudStorageAccount AzureStorageAccount)
        {
            var directory = await BlobDirectory(obj, AzureStorageAccount);
            var list = await directory.ListBlobsSegmentedAsync(true, BlobListingDetails.All, int.MaxValue, null, new BlobRequestOptions { }, new OperationContext { });

            var RowKey = GetRowKeyValue(obj);

            if (list.Results.Any())
            {
                var dictionary = new Dictionary<string, IListBlobItem>();
                foreach (var item in list.Results)
                {
                    var key = item.Uri.LocalPath.Replace($"/{obj.GetType().Name.ToLower()}/{RowKey}/", "").Split('/');
                    dictionary.Add(key[0], item);
                }
                return dictionary;
            }

            return null;
        }

        public static async Task SaveBlob(CloudBlockBlob CBB, object Content, string mimeType, string filenameWithoutExtension, string extension)
        {
            // if (string.IsNullOrEmpty(extension)) { throw new Exception("extension required"); }

            switch (Content.GetType().ToString().Replace("System.", ""))
            {
                case "Byte[]":
                    if (((byte[])Content).Any())
                        await CBB.UploadFromByteArrayAsync((byte[])Content, 0, ((byte[])Content).Length, null, null, null);
                    else return;
                    break;

                default:
                    if (!string.IsNullOrEmpty((string)Content))
                        await CBB.UploadTextAsync((string)Content);
                    else return;
                    break;
            }

            if (!string.IsNullOrEmpty(mimeType))
                CBB.Properties.ContentType = $"{mimeType}";

            if (!string.IsNullOrEmpty(extension))
                CBB.Properties.ContentDisposition = $"inline; filename={filenameWithoutExtension}.{extension}";

            if (!string.IsNullOrEmpty(mimeType) || !string.IsNullOrEmpty(extension))
                await CBB.SetPropertiesAsync();
        }

        public static async Task DeleteBlob(CloudBlockBlob CBB)
        {
            await CBB.DeleteIfExistsAsync();
        }
    }

    public class Blob
    {
        private object _CallingObject;
        private string _FileNameWithoutExtension = "";
        private string _MimeType = "";
        private string _FileExtension = "";
        private Type _type;

        public Blob()
        {
        }

        public Blob(string extension)
        {
            FileExtension = extension;
        }

        public Blob(Type type, string extension)
        {
            _type = type;
            FileExtension = extension;
        }

        public async Task Save(CloudStorageAccount AzureStorageAccount, object Content)
        {
            if (this == null)
            {
                throw new Exception("you must instantiate!!!");
            }

            if (string.IsNullOrEmpty(FileExtension))
            {
                switch (Content.GetType().ToString().Replace("System.", ""))
                {
                    case "Byte[]":
                        //Throw error???
                        break;

                    default:
                        FileExtension = "txt";
                        break;
                }
            }

            var CBB = await CreateCBB(AzureStorageAccount);

            if (String.IsNullOrEmpty(MimeType)) _MimeType = "";
            if (String.IsNullOrEmpty(FileNameWithoutExtension)) _FileNameWithoutExtension = "";
            if (String.IsNullOrEmpty(FileExtension)) _FileExtension = "";

            await Utils.SaveBlob(CBB, Content, MimeType, FileNameWithoutExtension, FileExtension);
        }

        public async Task<object> Get(CloudStorageAccount AzureStorageAccount)
        {
            var CBB = await GetCBB(AzureStorageAccount);
            if (!await CBB.ExistsAsync()) return null;

            if (CBB.Properties.ContentType.Contains("text"))
                return await CBB.DownloadTextAsync();

            var ba = new byte[] { };
            await CBB.DownloadToByteArrayAsync(ba, 0);
            return ba;
        }

        public async Task<Uri> GetUri(CloudStorageAccount AzureStorageAccount)
        {
            var CBB = await GetCBB(AzureStorageAccount);
            return CBB.Uri;
        }

        [JsonIgnore]
        public object CallingObject
        {
            get
            {
                try
                {
                    if (_CallingObject is object)
                        RowKey = Utils.GetRowKeyValue(_CallingObject);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(" error on set RK value from Blob CallingObject, message: " + ex.Message);
                }
                return _CallingObject;
            }
            set
            {
                _CallingObject = value;
            }
        }

        public string RowKey { get; set; }

        public string PropertyName { get; set; }

        public string BlobPath
        {
            get
            {
                return $"{RowKey}/{PropertyName}";
            }
        }

        public async Task<CloudBlockBlob> CreateCBB(CloudStorageAccount AzureStorageAccount)
        {
            return await CreateCloudBlockBlobReference(AzureStorageAccount);
        }

        public async Task<CloudBlockBlob> CreateCloudBlockBlobReference(CloudStorageAccount AzureStorageAccount)
        {
            var container = await Utils.BlobContainer(CallingObject, AzureStorageAccount);
            return container.GetBlockBlobReference(BlobPath);
        }

        public async Task<CloudBlockBlob> GetCBB(CloudStorageAccount AzureStorageAccount)
        {
            return await GetCloudBlockBlobreference(AzureStorageAccount);
        }

        public async Task<CloudBlockBlob> GetCloudBlockBlobreference(CloudStorageAccount AzureStorageAccount)
        {
            var container = await Utils.BlobContainer(CallingObject, AzureStorageAccount);
            var b = container.GetBlockBlobReference(BlobPath);
            await b.FetchAttributesAsync();
            return b;
        }

        public string MimeType
        {
            get
            {
                if (string.IsNullOrEmpty(_MimeType))
                    if (MIMETypes.ContainsKey(FileExtension))
                        _MimeType = MIMETypes[FileExtension];

                return _MimeType;
            }
            set
            {
                _MimeType = value;
            }
        }

        public string FileNameWithoutExtension
        {
            get
            {
                if (string.IsNullOrEmpty(_FileNameWithoutExtension))
                    _FileNameWithoutExtension = PropertyName;

                return _FileNameWithoutExtension;
            }
            set
            {
                _FileNameWithoutExtension = value;
            }
        }

        public string FileExtension
        {
            get
            {
                return _FileExtension;
            }
            set
            {
                _FileExtension = value.Trim().TrimStart('.');
            }
        }

        #region Types

        public static readonly Dictionary<string, string> MIMETypes = new Dictionary<string, string>
          {
            {"ai", "application/postscript"},
            {"aif", "audio/x-aiff"},
            {"aifc", "audio/x-aiff"},
            {"aiff", "audio/x-aiff"},
            {"asc", "text/plain"},
            {"atom", "application/atom+xml"},
            {"au", "audio/basic"},
            {"avi", "video/x-msvideo"},
            {"bcpio", "application/x-bcpio"},
            {"bin", "application/octet-stream"},
            {"bmp", "image/bmp"},
            {"cdf", "application/x-netcdf"},
            {"cgm", "image/cgm"},
            {"class", "application/octet-stream"},
            {"cpio", "application/x-cpio"},
            {"cpt", "application/mac-compactpro"},
            {"csh", "application/x-csh"},
            {"css", "text/css"},
            {"dcr", "application/x-director"},
            {"dif", "video/x-dv"},
            {"dir", "application/x-director"},
            {"djv", "image/vnd.djvu"},
            {"djvu", "image/vnd.djvu"},
            {"dll", "application/octet-stream"},
            {"dmg", "application/octet-stream"},
            {"dms", "application/octet-stream"},
            {"doc", "application/msword"},
            {"docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {"dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
            {"docm","application/vnd.ms-word.document.macroEnabled.12"},
            {"dotm","application/vnd.ms-word.template.macroEnabled.12"},
            {"dtd", "application/xml-dtd"},
            {"dv", "video/x-dv"},
            {"dvi", "application/x-dvi"},
            {"dxr", "application/x-director"},
            {"eps", "application/postscript"},
            {"etx", "text/x-setext"},
            {"exe", "application/octet-stream"},
            {"ez", "application/andrew-inset"},
            {"gif", "image/gif"},
            {"gram", "application/srgs"},
            {"grxml", "application/srgs+xml"},
            {"gtar", "application/x-gtar"},
            {"hdf", "application/x-hdf"},
            {"hqx", "application/mac-binhex40"},
            {"htm", "text/html"},
            {"html", "text/html"},
            {"ice", "x-conference/x-cooltalk"},
            {"ico", "image/x-icon"},
            {"ics", "text/calendar"},
            {"ief", "image/ief"},
            {"ifb", "text/calendar"},
            {"iges", "model/iges"},
            {"igs", "model/iges"},
            {"jnlp", "application/x-java-jnlp-file"},
            {"jp2", "image/jp2"},
            {"jpe", "image/jpeg"},
            {"jpeg", "image/jpeg"},
            {"jpg", "image/jpeg"},
            {"js", "application/x-javascript"},
            {"kar", "audio/midi"},
            {"latex", "application/x-latex"},
            {"lha", "application/octet-stream"},
            {"lzh", "application/octet-stream"},
            {"m3u", "audio/x-mpegurl"},
            {"m4a", "audio/mp4a-latm"},
            {"m4b", "audio/mp4a-latm"},
            {"m4p", "audio/mp4a-latm"},
            {"m4u", "video/vnd.mpegurl"},
            {"m4v", "video/x-m4v"},
            {"mac", "image/x-macpaint"},
            {"man", "application/x-troff-man"},
            {"mathml", "application/mathml+xml"},
            {"me", "application/x-troff-me"},
            {"mesh", "model/mesh"},
            {"mid", "audio/midi"},
            {"midi", "audio/midi"},
            {"mif", "application/vnd.mif"},
            {"mov", "video/quicktime"},
            {"movie", "video/x-sgi-movie"},
            {"mp2", "audio/mpeg"},
            {"mp3", "audio/mpeg"},
            {"mp4", "video/mp4"},
            {"mpe", "video/mpeg"},
            {"mpeg", "video/mpeg"},
            {"mpg", "video/mpeg"},
            {"mpga", "audio/mpeg"},
            {"ms", "application/x-troff-ms"},
            {"msh", "model/mesh"},
            {"mxu", "video/vnd.mpegurl"},
            {"nc", "application/x-netcdf"},
            {"oda", "application/oda"},
            {"ogg", "application/ogg"},
            {"pbm", "image/x-portable-bitmap"},
            {"pct", "image/pict"},
            {"pdb", "chemical/x-pdb"},
            {"pdf", "application/pdf"},
            {"pgm", "image/x-portable-graymap"},
            {"pgn", "application/x-chess-pgn"},
            {"pic", "image/pict"},
            {"pict", "image/pict"},
            {"png", "image/png"},
            {"pnm", "image/x-portable-anymap"},
            {"pnt", "image/x-macpaint"},
            {"pntg", "image/x-macpaint"},
            {"ppm", "image/x-portable-pixmap"},
            {"ppt", "application/vnd.ms-powerpoint"},
            {"pptx","application/vnd.openxmlformats-officedocument.presentationml.presentation"},
            {"potx","application/vnd.openxmlformats-officedocument.presentationml.template"},
            {"ppsx","application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
            {"ppam","application/vnd.ms-powerpoint.addin.macroEnabled.12"},
            {"pptm","application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
            {"potm","application/vnd.ms-powerpoint.template.macroEnabled.12"},
            {"ppsm","application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
            {"ps", "application/postscript"},
            {"qt", "video/quicktime"},
            {"qti", "image/x-quicktime"},
            {"qtif", "image/x-quicktime"},
            {"ra", "audio/x-pn-realaudio"},
            {"ram", "audio/x-pn-realaudio"},
            {"ras", "image/x-cmu-raster"},
            {"rdf", "application/rdf+xml"},
            {"rgb", "image/x-rgb"},
            {"rm", "application/vnd.rn-realmedia"},
            {"roff", "application/x-troff"},
            {"rtf", "text/rtf"},
            {"rtx", "text/richtext"},
            {"sgm", "text/sgml"},
            {"sgml", "text/sgml"},
            {"sh", "application/x-sh"},
            {"shar", "application/x-shar"},
            {"silo", "model/mesh"},
            {"sit", "application/x-stuffit"},
            {"skd", "application/x-koan"},
            {"skm", "application/x-koan"},
            {"skp", "application/x-koan"},
            {"skt", "application/x-koan"},
            {"smi", "application/smil"},
            {"smil", "application/smil"},
            {"snd", "audio/basic"},
            {"so", "application/octet-stream"},
            {"spl", "application/x-futuresplash"},
            {"src", "application/x-wais-source"},
            {"sv4cpio", "application/x-sv4cpio"},
            {"sv4crc", "application/x-sv4crc"},
            {"svg", "image/svg+xml"},
            {"swf", "application/x-shockwave-flash"},
            {"t", "application/x-troff"},
            {"tar", "application/x-tar"},
            {"tcl", "application/x-tcl"},
            {"tex", "application/x-tex"},
            {"texi", "application/x-texinfo"},
            {"texinfo", "application/x-texinfo"},
            {"tif", "image/tiff"},
            {"tiff", "image/tiff"},
            {"tr", "application/x-troff"},
            {"tsv", "text/tab-separated-values"},
            {"txt", "text/plain"},
            {"ustar", "application/x-ustar"},
            {"vcd", "application/x-cdlink"},
            {"vrml", "model/vrml"},
            {"vxml", "application/voicexml+xml"},
            {"wav", "audio/x-wav"},
            {"wbmp", "image/vnd.wap.wbmp"},
            {"wbmxl", "application/vnd.wap.wbxml"},
            {"wml", "text/vnd.wap.wml"},
            {"wmlc", "application/vnd.wap.wmlc"},
            {"wmls", "text/vnd.wap.wmlscript"},
            {"wmlsc", "application/vnd.wap.wmlscriptc"},
            {"wrl", "model/vrml"},
            {"xbm", "image/x-xbitmap"},
            {"xht", "application/xhtml+xml"},
            {"xhtml", "application/xhtml+xml"},
            {"xls", "application/vnd.ms-excel"},
            {"xml", "application/xml"},
            {"xpm", "image/x-xpixmap"},
            {"xsl", "application/xml"},
            {"xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {"xltx","application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
            {"xlsm","application/vnd.ms-excel.sheet.macroEnabled.12"},
            {"xltm","application/vnd.ms-excel.template.macroEnabled.12"},
            {"xlam","application/vnd.ms-excel.addin.macroEnabled.12"},
            {"xlsb","application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
            {"xslt", "application/xslt+xml"},
            {"xul", "application/vnd.mozilla.xul+xml"},
            {"xwd", "image/x-xwindowdump"},
            {"xyz", "chemical/x-xyz"},
            {"zip", "application/zip"}
        };

        #endregion Types
    }
}