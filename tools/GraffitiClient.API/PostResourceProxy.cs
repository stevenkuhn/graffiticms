using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;

namespace GraffitiClient.API
{
    public class PostResourceProxy : ResourceProxy<Post>
    {
        internal PostResourceProxy(string username, string password, string baseUrl)
            : base(username, password, baseUrl)
        {
        }

        public override Post Get(int id)
        {
            return Get(id, 0); 
        }

        public Post Get(int id, int revision)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc["id"] = id.ToString();

            if (revision > 0)
                nvc["revision"] = revision.ToString();

            PagedList<Post> list = Get(nvc);

            if (list.Count > 0)
                return list[0];

            return null;
        }

        public override PagedList<Post> Get(NameValueCollection nvc)
        {
            XmlDocument doc = GetXML(nvc);
            return GetPosts(doc);
        }

        public override int Create(Post t)
        {

            string xml = GetXML(t);

            XmlDocument doc = SendXML(xml,false);

            XmlNode node = doc.SelectSingleNode("/result");

            if (node != null && node.InnerText == "true")
            {
                return Int32.Parse(node.Attributes["id"].Value);
            }

            throw new Exception("No result returned");
        }

        public override bool Update(Post t)
        {
            string xml = GetXML(t);

            XmlDocument doc = SendXML(xml,false);

            XmlNode node = doc.SelectSingleNode("/result");

            if (node == null || node.InnerText != "true")
                return false;

            return true;
        }

        public override bool Delete(int id)
        {
            XmlDocument doc = SendXML("<post id = \"" + id + "\" />",true);

            return CheckResult(doc, "deleted");
        }


        private static string GetXML(Post post)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter writer = new XmlTextWriter(sw);

            writer.WriteStartElement("post");
            writer.WriteAttributeString("id", post.Id.ToString());
            writer.WriteElementString("title", post.Title);
            writer.WriteElementString("postBody", post.PostBody);
            writer.WriteElementString("body", post.Body);
            writer.WriteElementString("extendedBody", post.ExtendedBody);
            writer.WriteElementString("categoryId", post.CategoryId.ToString());
            writer.WriteElementString("author", post.Author);
            writer.WriteElementString("publishedDate", post.PublishDate.ToString());
            writer.WriteElementString("status", post.Status.ToString());
            writer.WriteElementString("name", post.Name);
            writer.WriteElementString("isDeleted", post.IsDeleted.ToString());
            writer.WriteElementString("tags", post.Tags);
            writer.WriteElementString("contenttype", post.ContentType);
            writer.WriteElementString("sortOrder", post.SortOrder.ToString());
            writer.WriteElementString("homeSortOrder", post.HomeSortOrder.ToString());
            writer.WriteElementString("metaDescription", post.MetaDescription);
            writer.WriteElementString("metaKeywords", post.MetaKeywords);
            writer.WriteElementString("isHome", post.IsHome.ToString());
            writer.WriteElementString("enableComments", post.EnableComments.ToString());

            //Write out the customFields
            writer.WriteStartElement("customFields");
            foreach (string key in post.CustomFields.AllKeys)
            {
                
                writer.WriteStartElement("customField");
                writer.WriteAttributeString("key", key);
                writer.WriteString(post.CustomFields[key]);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            //End customFields

            writer.WriteEndElement();
            writer.Close();
            sw.Close();

            return sb.ToString();
        }

        private static PagedList<Post> GetPosts(XmlDocument doc)
        {
            PagedList<Post> posts = new PagedList<Post>();

            XmlNode rootNode = doc.SelectSingleNode("/posts");

            posts.PageIndex = Int32.Parse(rootNode.Attributes["pageIndex"].Value);
            posts.PageSize = Int32.Parse(rootNode.Attributes["pageSize"].Value);
            posts.TotalRecords = Int32.Parse(rootNode.Attributes["totalPosts"].Value);

            XmlNodeList nodes = doc.SelectNodes("/posts/post");
            foreach(XmlNode node in nodes)
            {
                Post p = new Post();
                p.Id = Int32.Parse(node.Attributes["id"].Value);
                
                p.Author = node.SelectSingleNode("author").InnerText;
                p.Body = node.SelectSingleNode("body").InnerText;
                p.PostBody = node.SelectSingleNode("postBody").InnerText;
                p.CategoryId = Int32.Parse(node.SelectSingleNode("categoryId").InnerText);
                p.CommentCount = Int32.Parse(node.SelectSingleNode("commentCount").InnerText);
                p.PendingCommentCount = Int32.Parse(node.SelectSingleNode("pendingCommentCount").InnerText);
                p.ExtendedBody = node.SelectSingleNode("extendedBody").InnerText;
                p.PublishDate = DateTime.Parse(node.SelectSingleNode("publishedDate").InnerText);
                p.ModifiedBy = node.SelectSingleNode("modifiedBy").InnerText;
                p.ModifiedOn = DateTime.Parse(node.SelectSingleNode("modifiedOn").InnerText);
                p.CreatedOn = DateTime.Parse(node.SelectSingleNode("createdOn").InnerText);
                p.Name = node.SelectSingleNode("name").InnerText;
                p.Status = Int32.Parse(node.SelectSingleNode("status").InnerText);
                p.Tags = node.SelectSingleNode("tags").InnerText;
                p.Title = node.SelectSingleNode("title").InnerText;
                p.Url = node.SelectSingleNode("url").InnerText;
                p.Views = Int32.Parse(node.SelectSingleNode("views").InnerText);
                p.IsDeleted = bool.Parse(node.SelectSingleNode("isDeleted").InnerText);
                p.ContentType = node.SelectSingleNode("contenttype").InnerText;
                p.SortOrder = Int32.Parse(node.SelectSingleNode("sortOrder").InnerText);
                p.HomeSortOrder = Int32.Parse(node.SelectSingleNode("homeSortOrder").InnerText);
                p.ParentId = Int32.Parse(node.SelectSingleNode("parentId").InnerText);
                p.MetaDescription = node.SelectSingleNode("metaDescription").InnerText;
                p.MetaKeywords = node.SelectSingleNode("metaKeywords").InnerText;
                p.IsHome = bool.Parse(node.SelectSingleNode("isHome").InnerText);
                p.EnableComments = bool.Parse(node.SelectSingleNode("enableComments").InnerText);

                XmlNodeList customFields = node.SelectNodes("customFields/customField");
                foreach(XmlNode cNode in customFields)
                {
                    p.CustomFields[cNode.Attributes["key"].Value] = cNode.InnerText;
                }
                
                posts.Add(p);
            }


            return posts;
        }
    }
}