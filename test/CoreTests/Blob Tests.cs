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
        public async Task SimpleBinaryBlob()
        {
            var url = "http://2016tempdata2.azurewebsites.net/images/header_round_avatar50.png";
            byte[] result = await new HttpClient().GetByteArrayAsync(new Uri(url));

            if (result == null) throw new Exception("couldn't download image!");

            var CommentImageID = Guid.NewGuid().ToString();

            using (var DB = new BloggingContext())
            {
                var I = DB.CommentImages.New(
                    new vals() {
                         { "CommentID", "Testing" },
                         { "CommentImageID", CommentImageID }
                    });

                // await I.Image.Save(DB.PrimaryStorageAccount(), result);
                //I.Image.FileNameWithoutExtension = "Avatar";
                //I.Image.FileExtension = "png";
                await I.Image.SaveData(result);

                await DB.SaveChangesAsync();

                var ImageCBB = await I.Image.GetCBB();

                if (ImageCBB != null) Debug.WriteLine(ImageCBB.Uri);
            }

            using (var DB = new BloggingContext())
            {
                var I = await DB.CommentImages.GetByIDAsync(CommentImageID);

                var CBB = await I.Image.GetCBB();

                if (CBB != null) Debug.WriteLine("CBB.Uri: " + CBB.Uri);

                I.HardDelete();

                await DB.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task SimpleTextBlob()
        {
            var CommentID = Guid.NewGuid().ToString();

            using (var DB = new BloggingContext())
            {
                var I = DB.Comments.New(
                    new vals() {
                         { "PostID", "Testing" },
                         { "UserID", "Testing" },
                         { "CommentID", CommentID }
                    });

                await I.CommentHtml.SaveData("This is a test of the <html> content");

                await DB.SaveChangesAsync();
            }

            using (var DB = new BloggingContext())
            {
                var I = await DB.Comments.GetByIDAsync(CommentID);

                var html = await I.CommentHtml.GetData(DB.PrimaryStorageAccount());

                Debug.WriteLine("CommentHtml: " + html);

                I.HardDelete();

                await DB.SaveChangesAsync();
            }
        }
    }
}