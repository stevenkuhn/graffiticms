using System;
using System.Xml;
using DataBuddy;

namespace Graffiti.Core.API
{
    /// <summary>
    /// Handles RESTFul Queries and Updates to Graffiti Comments.
    /// </summary>
    public class CommentResource : BaseServiceResource
    {
        protected override void HandleRequest(IGraffitiUser user, XmlTextWriter writer)
        {
            switch (Context.Request.HttpMethod.ToUpper())
            {
                case "GET":

                    GetComments(writer);

                    break;

                case "POST":

                    UpdateOrDeleteComment(writer, user);
                    break;

                default:


                    break;
            }
        }

        private void UpdateOrDeleteComment(XmlTextWriter writer, IGraffitiUser user)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Request.InputStream);

            if (Request.Headers["Graffiti-Method"] != "DELETE")
            {
                writer.WriteRaw(UpdateComment(doc, user));
            }
            else
            {
                writer.WriteRaw(DeleteComment(doc, user));
            }
        }

        private static string DeleteComment(XmlDocument doc, IGraffitiUser user)
        {
            int id = Int32.Parse(doc.SelectSingleNode("/comment").Attributes["id"].Value);
            Comment comment = new Comment(id);
            if (comment.IsNew)
                throw new Exception("Comment with id " + id + " does not exist");

            if (!RolePermissionManager.GetPermissions(comment.Post.CategoryId, user).Publish)
                throw new Exception("You do not have sufficient privileges to delete this comment.");
    
            Comment.Delete(id);

            return "<result id=\"" + id + "\">deleted</result>";
        }
    

        private static string UpdateComment(XmlDocument doc, IGraffitiUser user)
        {
            int id = Int32.Parse(doc.SelectSingleNode("/comment").Attributes["id"].Value);

            Comment comment = new Comment(id);
            if(comment.IsNew)
                throw new Exception("Comment with id " + id + " does not exist. The REST API only supports updating existing comments at this time.");

            XmlNode node = doc.SelectSingleNode("/comment");

            comment.Body = GetNodeValue(node.SelectSingleNode("body"), comment.Body);
            comment.Name = GetNodeValue(node.SelectSingleNode("name"), comment.Name);
            comment.IsPublished = GetNodeValue(node.SelectSingleNode("isPublished"), comment.IsPublished);
            comment.IsDeleted = GetNodeValue(node.SelectSingleNode("isDeleted"), comment.IsDeleted);
            comment.SpamScore = GetNodeValue(node.SelectSingleNode("spamScore"), comment.SpamScore);
            comment.Email = GetNodeValue(node.SelectSingleNode("email"), comment.Email);
            comment.WebSite = GetNodeValue(node.SelectSingleNode("webSite"), comment.WebSite);

            if (!RolePermissionManager.GetPermissions(comment.Post.CategoryId, user).Edit)
                throw new Exception("You do not have sufficient privileges to update this comment.");

            comment.Save(GraffitiUsers.Current.Name);

            return "<result id=\"" + id + "\">true</result>"; 
        
        }

        private void GetComments(XmlTextWriter writer)
        {
            CommentFilter filter = CommentFilter.FromQueryString(Request.QueryString);
            Query q = filter.ToQuery();
            q.AndWhere(Comment.Columns.IsDeleted, false);
            CommentCollection comments = new CommentCollection();
            comments.LoadAndCloseReader(q.ExecuteReader());


            writer.WriteStartElement("comments");
            writer.WriteAttributeString("pageIndex", q.PageIndex.ToString());
            writer.WriteAttributeString("pageSize", q.PageSize.ToString());
            writer.WriteAttributeString("totalComments", q.GetRecordCount().ToString());
            foreach (Comment coment in comments)
            {
                ConvertComentToXML(coment, writer);
            }

            writer.WriteEndElement();
        }

        private static void ConvertComentToXML(Comment comment, XmlTextWriter writer)
        {
            writer.WriteStartElement("comment");
            writer.WriteAttributeString("id", comment.Id.ToString());
            writer.WriteAttributeString("postId", comment.PostId.ToString());
            writer.WriteElementString("body", comment.Body);
            writer.WriteElementString("ipAddress", comment.IPAddress);
            writer.WriteElementString("name", comment.Name);
            writer.WriteElementString("author", comment.UserName);
            writer.WriteElementString("spamScore", comment.SpamScore.ToString());
            writer.WriteElementString("isPublished", comment.IsPublished.ToString());
            writer.WriteElementString("isDeleted", comment.IsDeleted.ToString());
            writer.WriteElementString("isTrackback", comment.IsTrackback.ToString());
            writer.WriteElementString("url", new Macros().FullUrl(comment.Url));
            writer.WriteElementString("webSite", comment.WebSite);
            writer.WriteElementString("published", comment.Published.ToString());
            writer.WriteElementString("email", comment.Email);
            writer.WriteElementString("ipAddress", comment.IPAddress);
            writer.WriteEndElement();
        }
    }
}