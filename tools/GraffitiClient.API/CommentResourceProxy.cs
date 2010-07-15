using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;

namespace GraffitiClient.API
{
    public class CommentResourceProxy : ResourceProxy<Comment>
    {
        internal CommentResourceProxy(string username, string password, string baseUrl)
            : base(username, password, baseUrl)
        {
        }

        public override Comment Get(int id)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc["id"] = id.ToString();

            PagedList<Comment> comments = Get(nvc);

            if (comments.Count > 0)
                return comments[0];

            return null;
        }

        public override PagedList<Comment> Get(NameValueCollection nvc)
        {
            return GetCategories(GetXML(nvc));
        }

        public override int Create(Comment t)
        {
            throw new NotImplementedException();
        }

        public override bool Update(Comment t)
        {
            XmlDocument doc = SendXML(GetXML(t), false);

            return CheckResult(doc, "true");
        }

        public override bool Delete(int id)
        {
            XmlDocument doc = SendXML("<comment id = \"" + id + "\" />",true);

            return CheckResult(doc, "deleted");
        }

        private static string GetXML(Comment comment)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter writer = new XmlTextWriter(sw);

            writer.WriteStartElement("comment");
            writer.WriteAttributeString("id", comment.Id.ToString());
            writer.WriteElementString("name", comment.Name);
            writer.WriteElementString("body", comment.Body);

            writer.WriteElementString("isPublished", comment.IsPublished.ToString());
            writer.WriteElementString("isDeleted", comment.IsDeleted.ToString());
            writer.WriteElementString("spamScore", comment.SpamScore.ToString());
            writer.WriteElementString("webSite", comment.WebSite);
            writer.WriteElementString("email", comment.Email);
            
            writer.WriteEndElement();

            writer.Close();
            sw.Close();

            return sb.ToString();
        }

        private static PagedList<Comment> GetCategories(XmlDocument doc)
        {
            PagedList<Comment> comments = new PagedList<Comment>();

            XmlNode rootNode = doc.SelectSingleNode("/comments");

            comments.PageIndex = Int32.Parse(rootNode.Attributes["pageIndex"].Value);
            comments.PageSize = Int32.Parse(rootNode.Attributes["pageSize"].Value);
            comments.TotalRecords = Int32.Parse(rootNode.Attributes["totalComments"].Value);

            XmlNodeList nodes = doc.SelectNodes("/comments/comment");
            foreach (XmlNode node in nodes)
            {
                Comment comment = new Comment();
                comment.Id = Int32.Parse(node.Attributes["id"].Value);
                comment.PostId = Int32.Parse(node.Attributes["postId"].Value);
                comment.Body = node.SelectSingleNode("body").InnerText;
                comment.SpamScore = Int32.Parse(node.SelectSingleNode("spamScore").InnerText);
                comment.IsPublished = bool.Parse(node.SelectSingleNode("isPublished").InnerText);
                comment.IsTrackback = bool.Parse(node.SelectSingleNode("isTrackback").InnerText);
                comment.IsDeleted = bool.Parse(node.SelectSingleNode("isDeleted").InnerText);
                comment.Url = node.SelectSingleNode("url").InnerText;

                XmlNode userName = node.SelectSingleNode("userName");
                if(userName != null)
                    comment.UserName = userName.InnerText;


                XmlNode name = node.SelectSingleNode("name");
                if (name != null)
                    comment.Name = name.InnerText;

                XmlNode email = node.SelectSingleNode("email");
                if (email != null)
                    comment.Email = email.InnerText;

                XmlNode ws = node.SelectSingleNode("webSite");
                if (ws != null)
                    comment.WebSite = ws.InnerText;

                XmlNode ip = node.SelectSingleNode("ipAddress");
                if (ip != null)
                    comment.IPAddress = ip.InnerText;


                comment.Date = DateTime.Parse(node.SelectSingleNode("published").InnerText);

                comments.Add(comment);
            }


            return comments;
        }
    }
}