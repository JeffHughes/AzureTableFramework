using Samples.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AzureTableFramework.Samples.WebApp.MVVM
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(() => Page_Load_Async()));
        }

        private async Task Page_Load_Async()
        {
            using (var DB = new BloggingContext())
            {
                var BLOG = new Blog();

                BLOG.AuthorID = "123456789";

                DB.Blogs.Add(BLOG);

                await DB.SaveChangesAsync();
            }
        }
    }
}