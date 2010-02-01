using System;
using System.Collections.Generic;
using System.Xml;
using DataBuddy;

namespace Graffiti.Core.API
{
    /// <summary>
    /// Handles RESTFul Queries and Updates to Graffiti Posts.
    /// </summary>
    public class PostResource : BaseServiceResource
    {
        protected override void HandleRequest(IGraffitiUser user, XmlTextWriter writer)
        {
            switch (Context.Request.HttpMethod.ToUpper())
            {
                case "GET":

                    if(!String.IsNullOrEmpty(Context.Request.QueryString["revision"]))
                        GetPostsForRevision(writer);
                    else
                        GetPosts(writer);

                    break;

                case "POST":

                    CreateUpdateDeletePost(writer, user);
                    break;

                default:
                   

                    break;
            }
        }

        private void CreateUpdateDeletePost(XmlTextWriter writer, IGraffitiUser user)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Request.InputStream);

            if (Request.Headers["Graffiti-Method"] != "DELETE")
            {
                writer.WriteRaw(CreateUpdatePost(doc,user));
            }
            else
            {
                XmlAttribute postidAttribute = doc.SelectSingleNode("/post").Attributes["id"];

                int pid = Int32.Parse(postidAttribute.Value);
                Post p = new Post(pid);

                Permission perm = RolePermissionManager.GetPermissions(p.CategoryId, user);

                if (GraffitiUsers.IsAdmin(user) || perm.Publish)
                    writer.WriteRaw(DeletePost(doc));
                else
                    UnuathorizedRequest();
            }
        }

        private void GetPostsForRevision(XmlTextWriter writer)
        {
            string v = Request.QueryString["revision"];
            string p = Request.QueryString["id"];

            if (string.IsNullOrEmpty(p))
                throw new RESTConflict("The Post Id (id) querystring value is missing");

            int postid = Int32.Parse(p);

            int version = (Int32.Parse(v ?? "-1"));

            Query q = VersionStore.CreateQuery();
            q.AndWhere(VersionStore.Columns.ItemId, postid);
            q.AndWhere(VersionStore.Columns.Type, "post/xml");
            if (version > 0)
                q.AndWhere(VersionStore.Columns.Version, version);

            VersionStoreCollection vsc = VersionStoreCollection.FetchByQuery(q);
            PostCollection posts = new PostCollection();
            posts.Add(new Post(postid));

            foreach (VersionStore vs in vsc)
            {
                posts.Add(ObjectManager.ConvertToObject<Post>(vs.Data));
            }

            posts.Sort(delegate(Post p1, Post p2) { return Comparer<int>.Default.Compare(p2.Version, p1.Version); });

            writer.WriteStartElement("posts");
            writer.WriteAttributeString("pageIndex", "1");
            writer.WriteAttributeString("pageSize", posts.Count.ToString());
            writer.WriteAttributeString("totalPosts", posts.Count.ToString());
            foreach (Post post in posts)
            {
                if (version <= 0 || post.Version == version)
                    ConvertPostToXML(post, writer);
            }

            writer.WriteEndElement();
        }

        private void GetPosts(XmlTextWriter writer)
        {
            PostFilter filter = PostFilter.FromQueryString(Request.QueryString);
            Query q = filter.ToQuery();
            q.AndWhere(Post.Columns.IsDeleted, false);

            PostCollection posts = PostCollection.FetchByQuery(q);
                 
            writer.WriteStartElement("posts");
            writer.WriteAttributeString("pageIndex", q.PageIndex.ToString() );
            writer.WriteAttributeString("pageSize", q.PageSize.ToString());
            writer.WriteAttributeString("totalPosts", q.GetRecordCount().ToString());
            foreach (Post post in posts)
            {
                ConvertPostToXML(post, writer);
            }
                       
            writer.WriteEndElement();
        }

        protected  static void ConvertPostToXML(Post post, XmlTextWriter writer)
        {
            writer.WriteStartElement("post");
            writer.WriteAttributeString("id", post.Id.ToString());
            writer.WriteElementString("title", post.Title);
            writer.WriteElementString("body", post.Body);
            writer.WriteElementString("postBody", post.PostBody);
			writer.WriteElementString("extendedBody", post.ExtendedBody);
            writer.WriteElementString("categoryId", post.CategoryId.ToString());
            writer.WriteElementString("commentCount", post.CommentCount.ToString());
            writer.WriteElementString("pendingCommentCount", post.PendingCommentCount.ToString());
            writer.WriteElementString("author", post.UserName);
            writer.WriteElementString("publishedDate", post.Published.ToString());
            writer.WriteElementString("status", post.Status.ToString());
            writer.WriteElementString("viewCount", post.Views.ToString());
            writer.WriteElementString("url", new Macros().FullUrl(post.Url));
            writer.WriteElementString("name", post.Name);
            writer.WriteElementString("isDeleted", post.IsDeleted.ToString());
            writer.WriteElementString("tags", post.TagList);
            writer.WriteElementString("views", post.Views.ToString());
            writer.WriteElementString("image", post.ImageUrl);
            writer.WriteElementString("notes", post.Notes);
            writer.WriteElementString("contenttype", post.ContentType);
            writer.WriteElementString("revision", post.Version.ToString());
            writer.WriteElementString("sortOrder", post.SortOrder.ToString());

            writer.WriteElementString("modifiedBy", post.ModifiedBy);
            writer.WriteElementString("createdBy", post.CreatedBy);
            writer.WriteElementString("modifiedOn", post.ModifiedOn.ToString());
            writer.WriteElementString("createdOn", post.CreatedOn.ToString());


            writer.WriteElementString("enableComments", post.EnableComments.ToString());
            writer.WriteElementString("isPublished", post.IsPublished.ToString());
            writer.WriteElementString("isHome", post.IsHome.ToString());
            writer.WriteElementString("homeSortOrder", post.HomeSortOrder.ToString());
            writer.WriteElementString("parentId", post.ParentId.ToString());

            writer.WriteElementString("metaDescription", post.MetaDescription);
            writer.WriteElementString("metaKeywords", post.MetaKeywords);

            writer.WriteElementString("isFeatured", (SiteSettings.Get().FeaturedId == post.Id).ToString());
            writer.WriteElementString("isFeaturedCategory", (post.Category.FeaturedId == post.Id).ToString());

            writer.WriteStartElement("customFields");
            foreach(string key in post.CustomFields().AllKeys)
            {
                writer.WriteStartElement("customField");
                writer.WriteAttributeString("key", key);
                writer.WriteValue(post.Custom(key));

                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private static string DeletePost(XmlDocument doc)
        {
            XmlAttribute postidAttribute = doc.SelectSingleNode("/post").Attributes["id"];
            if (postidAttribute == null)
                throw new RESTConflict("No post id was specified to delete");

            int pid = Int32.Parse(postidAttribute.Value);
            Post p = new Post(pid);
            if (p.IsNew)
                throw new RESTConflict("No post exists with an id of " + pid);

            Post.Delete(pid);

            return "<result id=\"" + pid + "\">deleted</result>";
        }

        private static string CreateUpdatePost(XmlDocument doc, IGraffitiUser user)
        {
            Post post = null;
            XmlAttribute postidAttribute = doc.SelectSingleNode("/post").Attributes["id"];
            if (postidAttribute == null)
                post = new Post();
            else
            {
                int pid = Int32.Parse(postidAttribute.Value);
                if (pid > 0)
                    post = new Post(pid);
                else
                    post = new Post();
            }
            XmlNode node = doc.SelectSingleNode("/post");

            
            
            if (GraffitiUsers.IsUserInRole(user.Name, GraffitiUsers.AdminRole))
            {
                XmlNode usernameNode = node.SelectSingleNode("author");
                if (usernameNode != null && !string.IsNullOrEmpty(usernameNode.Value))
                {
                    post.UserName = GraffitiUsers.GetUser(usernameNode.Value).Name;
                }
            }

            if (string.IsNullOrEmpty(post.UserName) && post.IsNew)
                post.UserName = user.Name;


            post.PostBody = GetNodeValue(node.SelectSingleNode("postBody"), null);
            if (string.IsNullOrEmpty(post.PostBody))
                throw new RESTConflict("The Post body element is missing and is required");


            post.CategoryId = GetNodeValue(node.SelectSingleNode("categoryId"), -1);
                if(post.CategoryId <= 0)
                    throw new RESTConflict("The category element is missing (or has an invalid value) and is required");

            post.Title = GetNodeValue(node.SelectSingleNode("title"), null);
            if (string.IsNullOrEmpty(post.Title))
                throw new RESTConflict("The title element is missing and is required");

			post.ExtendedBody = GetNodeValue(node.SelectSingleNode("extendedBody"), null);

            XmlNode publishedDateNode = node.SelectSingleNode("publishedDate");
            if (publishedDateNode != null && !string.IsNullOrEmpty(publishedDateNode.InnerText) &&
                DateTime.Parse(publishedDateNode.InnerText) > new DateTime(2000, 1, 1))
                post.Published = DateTime.Parse(publishedDateNode.InnerText);
            else if (post.IsNew)
                post.Published = SiteSettings.CurrentUserTime;

            post.Name = GetNodeValue(node.SelectSingleNode("name"), post.Name);


            post.Status = GetNodeValue(node.SelectSingleNode("status"), post.IsNew ? (int)PostStatus.Draft : post.Status);

            post.TagList = GetNodeValue(node.SelectSingleNode("tags"), null);

            post.ContentType = GetNodeValue(node.SelectSingleNode("contenttype"), null);

            post.SortOrder = GetNodeValue(node.SelectSingleNode("sortOrder"), post.SortOrder);

            post.HomeSortOrder = GetNodeValue(node.SelectSingleNode("homeSortOrder"), post.HomeSortOrder);

            post.MetaDescription = GetNodeValue(node.SelectSingleNode("metaDescription"), post.MetaDescription);
            post.MetaKeywords = GetNodeValue(node.SelectSingleNode("metaKeywords"), post.MetaKeywords);
            post.IsHome = GetNodeValue(node.SelectSingleNode("isHome"), post.IsHome);
            post.EnableComments = GetNodeValue(node.SelectSingleNode("enableComments"), post.EnableComments);

            XmlNodeList customFields = node.SelectNodes("customFields/customField");
            foreach (XmlNode cNode in customFields)
            {
                post[cNode.Attributes["key"].Value] = cNode.InnerText;
            }

            Permission perm = RolePermissionManager.GetPermissions(post.CategoryId, user);

            if (GraffitiUsers.IsAdmin(user) || perm.Publish)
                post.IsDeleted = GetNodeValue(node.SelectSingleNode("isDeleted"), post.IsDeleted);

            int id =
                PostRevisionManager.CommitPost(post, user, SiteSettings.Get().FeaturedId == post.Id,
                                               post.Category.FeaturedId == post.Id);

            return string.Format("<result id=\"{0}\">true</result>", id);
        }
    }
}
