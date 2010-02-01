<%@ WebHandler Language="C#" Class="Graffiti.Web.Approve" %>

using System;
using System.Text;
using System.Web;
using System.Xml;
using Graffiti.Core;
using DataBuddy;

namespace Graffiti.Web
{
    public class Approve : IHttpHandler 
	{
	    
		public void ProcessRequest (HttpContext context) 
		{
            string u = context.Request.QueryString["u"];
            string key = context.Request.QueryString["key"];
            string p = context.Request.QueryString["id"];
            string v = context.Request.QueryString["v"];

            if (u != null || key != null || p != null || v != null)
            {
                IGraffitiUser user = GraffitiUsers.GetUser(u);
                Post the_Post = new Post(p);

                Permission perm = RolePermissionManager.GetPermissions(the_Post.CategoryId, user);
                if (user != null && (GraffitiUsers.IsAdmin(user) || perm.Publish))
                {
                    if (user.UniqueId == new Guid(key))
                    {
                        if (!the_Post.IsNew)
                        {
                            int version = Int32.Parse(v);
                            if (the_Post.Version != version)
                            {
                                the_Post = null;

                                Query q = VersionStore.CreateQuery();
                                q.AndWhere(VersionStore.Columns.Type, "post/xml");
                                q.AndWhere(VersionStore.Columns.ItemId, p);
                                q.AndWhere(VersionStore.Columns.Version, version);

                                VersionStore vs = VersionStore.FetchByQuery(q);
                                if (vs != null && !vs.IsNew)
                                {
                                    the_Post = Post.FromXML(vs.Data);
                                }
                            }

                        }
                        else
                        {
                            context.Response.Redirect("~/");
                        }

                        if (the_Post != null)
                        {
                            the_Post.PostStatus = PostStatus.Publish;
                            PostRevisionManager.CommitPost(the_Post, user, SiteSettings.Get().FeaturedId == the_Post.Id, the_Post.Category.FeaturedId == the_Post.Id);

                            context.Response.Redirect(the_Post.VirtualUrl);
                        }
                    }
                }
            }

            context.Response.Redirect("~/");
	        
		}
	 
		public bool IsReusable {
			get {
				return false;
			}
		}

	}
}