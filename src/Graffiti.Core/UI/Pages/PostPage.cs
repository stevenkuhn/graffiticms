using System;
using System.Web;
using DataBuddy;

namespace Graffiti.Core
{
    public class PostPage : TemplatedThemePage
    {
        private string viewName;
        protected override string ViewName
        {
            get
            {
                    return ViewLookUp(".view", "post.view");
            }
        }

        protected override string ViewLookUp(string baseName, string defaultViewName)
        {
            Category category = new CategoryController().GetCachedCategory(CategoryID, false);

            if (ViewExists(CategoryName + "." + PostName + baseName))
                return CategoryName + "." + PostName + baseName;
            else if (ViewExists(CategoryName.Replace("/", ".") + ".post" + baseName))
                return CategoryName.Replace("/", ".") + ".post" + baseName;
            else if (ViewExists(CategoryName + ".post" + baseName))
                return CategoryName + ".post" + baseName;
            else if (defaultViewName == "post.view" && ViewExists(PostName + ".view"))
                return PostName + ".view";
            else if (ViewExists(PostName + baseName))
                return PostName + baseName;

            // Subcategories
            if (category.ParentId > 0)
            {
                // parent-name.child-name.view
                if (ViewExists(category.LinkName.Replace("/", ".") + ".post" + baseName))
                    return category.LinkName.Replace("/", ".") + ".post" + baseName;

                // childcategory.parent-name.post.view
                if (ViewExists("childcategory." + category.Parent.LinkName + baseName))
                    return "childcategory." + category.Parent.LinkName + baseName;

                if (ViewExists(category.LinkName.Replace("/", ".") + baseName))
                    return category.LinkName.Replace("/", ".") + baseName;

                if (ViewExists(category.Parent.LinkName + ".post" + baseName))
                    return category.Parent.LinkName + ".post" + baseName;

                if (ViewExists(category.Parent.LinkName + baseName))
                    return category.Parent.LinkName + baseName;
            }
            /*
            else if (ViewExists(CategoryName + baseName))
                return CategoryName + baseName;
            */
            else if (CategoryID == CategoryController.UnCategorizedId && ViewExists("page" + baseName))
                return "page" + baseName;

            else if (ViewExists("post" + baseName))
                return "post" + baseName;

            else if (ViewExists(defaultViewName))
                return defaultViewName;

            return base.ViewLookUp(baseName, defaultViewName);
        }

        protected override void LoadContent(GraffitiContext graffitiContext)
        {

            graffitiContext["where"] = "post";

            Post post = Post.GetCachedPost(PostId);

            if (post.IsDeleted || (!post.IsPublished && GraffitiUsers.Current != null))
            {
                RedirectTo(new Urls().Home);
            }
            else if (PostName != null && CategoryName != null && (!Util.AreEqualIgnoreCase(PostName, post.Name) || !Util.AreEqualIgnoreCase(CategoryName, post.Category.LinkName)))
            {
                RedirectTo(post.Url);
            }
            else if (Context.Request.Cookies["Graffiti-Post-" + PostId] == null)
            {
                Post.UpdateViewCount(PostId);
                //SPs.UpdatePostView(PostId).Execute();
                HttpCookie cookie = new HttpCookie("Graffiti-Post-" + PostId, PostId.ToString());
                Context.Response.Cookies.Add(cookie);
            }


            graffitiContext["title"] = post.Title + " : " + SiteSettings.Get().Title;

            graffitiContext["category"] = post.Category;

            graffitiContext["post"] = post;

            graffitiContext.RegisterOnRequestDelegate("feedback", GetPostFeedback);
            graffitiContext.RegisterOnRequestDelegate("comments", GetPostComments);
            graffitiContext.RegisterOnRequestDelegate("trackbacks", GetPostTrackbacks);
        }

        protected virtual object GetPostFeedback(string key, GraffitiContext graffitiContext)
        {
            return new Data().PostFeedback(PostId);
        }

        protected virtual object GetPostComments(string key, GraffitiContext graffitiContext)
        {
            return new Data().PostComments(PostId);
        }

        protected virtual object GetPostTrackbacks(string key, GraffitiContext graffitiContext)
        {
            return new Data().PostTrackbacks(PostId);
        }
    }
}