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
        // await I.Comment.Save(DB.PrimaryStorageAccount(), "This is a test for comment");

        public async Task SaveSimpleBlob()
        {
            var url = "http://2016tempdata2.azurewebsites.net/images/header_round_avatar50.png";
            byte[] result = await new HttpClient().GetByteArrayAsync(new Uri(url));

            if (result == null) throw new Exception("couldn't download image!");

            using (var DB = new BloggingContext())
            {
                var I = new CommentImage() { CommentID = "Testing" };

                DB.CommentImages.Add(I);

                await I.Image.Save(DB.PrimaryStorageAccount(), result);

                await DB.SaveChangesAsync();

                Debug.WriteLine((await I.Image.GetCBB(DB.PrimaryStorageAccount())).Uri);
            }
        }

        [Fact]
        public async Task GetBlob()
        {
            using (var DB = new BloggingContext())
            {
                var I = await DB.CommentImages.GetByIDAsync("223abb96-bd39-4f07-9155-2107c967183d");

                var AvatarCBB = await I.Avatar.GetCBB(DB.PrimaryStorageAccount());

                Debug.WriteLine("AvatarCBB.Uri: " + AvatarCBB.Uri);
            }
        }
    }
}