using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Graffiti.Core;


namespace Graffiti.Marketplace
{
    public class DownloadHandler : IHttpHandler
    {
        string _postID;

        public DownloadHandler(string postIDvalue)
        {
            _postID = postIDvalue;
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
                    // ToDo: stream download file to client
                }
            }

            throw new HttpException(404, "Page not found");
        }

    }
}
