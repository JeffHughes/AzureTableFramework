using AzureTableFramework.Core;
using Samples.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CoreTests
{
    public class Blobs
    {
        [Fact]
        public async Task SaveSimpleBlob()
        {
            var url = "http://2016tempdata2.azurewebsites.net/images/header_round_avatar50.png";
            byte[] result = await new HttpClient().GetByteArrayAsync(new Uri(url));

            using (var DB = new BloggingContext())
            {
                var I = new CommentImage()
                {
                    CommentID = "1",
                    Picture = result
                };

                DB.CommentImages.Add(I);

                await DB.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task BlobWithNamedBlobData()
        {
            var url = "http://2016tempdata2.azurewebsites.net/images/header_round_avatar50.png";
            byte[] result = await new HttpClient().GetByteArrayAsync(new Uri(url));

            using (var DB = new BloggingContext())
            {
                var I = new CommentImage()
                {
                    CommentID = "1",
                    Picture1 = result,
                    Picture1BlobData = new BlobData { FileExtension = "png" }
                };

                DB.CommentImages.Add(I);

                await DB.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task BlobWithTargetedBlobData()
        {
            var url = "http://2016tempdata2.azurewebsites.net/images/header_round_avatar50.png";
            byte[] result = await new HttpClient().GetByteArrayAsync(new Uri(url));

            using (var DB = new BloggingContext())
            {
                var I = new CommentImage()
                {
                    CommentID = "1",
                    Picture2 = result,
                    PictureUnspecifiedByNameBlobData = new BlobData { FileExtension = ".png" }
                };

                DB.CommentImages.Add(I);

                await DB.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task GetBlob()
        {
            using (var DB = new BloggingContext())
            {
                var I = await DB.CommentImages.GetByIDAsync("2d2979a2-1853-4c52-823b-ac130dba3358");

                Debug.WriteLine(I.Picture.ToString());
            }
        }
    }
}