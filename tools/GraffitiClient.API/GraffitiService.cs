using System;
using System.Collections.Generic;
using System.Text;

namespace GraffitiClient.API
{
    public class GraffitiService
    {
        public GraffitiService(string username, string password, string baseUrl)
        {
            if (!baseUrl.EndsWith("/"))
                baseUrl += "/";

            _posts = new PostResourceProxy(username,password, baseUrl + "posts/default.aspx");
            _categories = new CategoryResourceProxy(username, password, baseUrl + "categories/default.aspx");
            _comments = new CommentResourceProxy(username, password, baseUrl + "comments/default.aspx");
        }

        private PostResourceProxy _posts = null;
        private CategoryResourceProxy _categories = null;
        private CommentResourceProxy _comments = null;

        public PostResourceProxy Posts
        {
            get { return _posts; }
        }

        public CategoryResourceProxy Categories
        {
            get
            {
                return _categories;
            }
        }

        public CommentResourceProxy Comments
        {
            get
            {
                return _comments;
            }
        }
    }
}
