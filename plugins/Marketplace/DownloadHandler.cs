using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Graffiti.Core;


namespace Graffiti.Marketplace
{
    public class DownloadHandler : IHttpHandler
    {
        string _postID;

        public DownloadHandler(string postID)
        {
            _postID = postID;
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            Post post = null;

            try
            {
                int postID = int.Parse(_postID);
                post = new Data().GetPost(postID);
            }
            catch { }

            if (post != null && post.IsPublished)
            {
                string downloadFile = post.Custom("FileName");
                if (!string.IsNullOrEmpty(downloadFile))
                {
                    string localPath = context.Server.MapPath(downloadFile);
                    if (!string.IsNullOrEmpty(localPath) && File.Exists(localPath))
                    {
                        // Increment download count for post
                        DataHelper.UpdateMarketplaceStats(post.Id);

                        downloadFile = HttpUtility.UrlEncode(downloadFile).Replace("+", "%20");
                        context.Response.Clear();
                        context.Response.ContentType = "application/octet-stream";
                        context.Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", downloadFile));
                        context.Response.TransmitFile(localPath);
                        context.Response.End();
                        return;
                    }
                }
            }

            throw new HttpException(404, "Page not found");
        }

    }
}
