using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using DataBuddy;

namespace Graffiti.Core
{

    public partial class Comment
    {

        private bool _dontSendEmail;

        public bool DontSendEmail
        {
            get { return _dontSendEmail; }
            set { _dontSendEmail = value; }
        }

        private bool _dontChangeUser;

        public bool DontChangeUser
        {
            get { return _dontChangeUser; }
            set { _dontChangeUser = value; }
        }

        #region PreUpdate
        protected override void BeforeValidate()
        {
            base.BeforeValidate();

            //By default we allow no markup
            if(IsNew)
            {
                UniqueId = Guid.NewGuid();
                Body = Util.ConvertTextToHTML(Body);
               
                IGraffitiUser gu = GraffitiUsers.Current;
                
                if (gu != null)
                {
                    if (!DontChangeUser)
                    {
                        Name = gu.ProperName;
                        WebSite = gu.WebSite;
                        Email = gu.Email;
                        IsPublished = true;
                        UserName = gu.Name;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(WebSite))
                        WebSite = HttpUtility.HtmlEncode(WebSite);

                    if (!string.IsNullOrEmpty(Email))
                        Email = HttpUtility.HtmlEncode(Email);
                    
                    Name = HttpUtility.HtmlEncode(Name);
                    SpamScore = CommentSettings.ScoreComment(this, new Post(PostId));
                    IsPublished = SpamScore < CommentSettings.Get().SpamScore;
                }
            }

        }
        #endregion 

        #region PostUpdate

        protected override void AfterRemove(bool isDestroy)
        {
            Core.Post.UpdateCommentCount(this.PostId);

            ZCache.RemoveCache("Comments-" + PostId);
            ZCache.RemoveByPattern("Comments-Recent");
        }

        protected override void AfterCommit()
        {
            base.AfterCommit();

            Post.UpdateCommentCount(PostId);

            if (!DontSendEmail)
            {
                try
                {
                    EmailTemplateToolboxContext etc = new EmailTemplateToolboxContext();
                    etc.Put("comment", this);
                    EmailTemplate ef = new EmailTemplate();
                    ef.Context = etc;
                    ef.Subject = "New Comment: " + Post.Title;
                    ef.To = Post.User.Email;
                    ef.TemplateName = "comment.view";
                    ef.ReplyTo = this.Email;
                    Emailer.Send(ef);
                    Log.Info("Comment Sent", "Email sent to {0} ({1}) from the post \"{2}\" ({3}).", Post.User.ProperName, Post.User.Email, Post.Title, Post.Id);
                }
                catch (Exception ex)
                {
                    Log.Error("Email Failure", ex.Message);
                }
            }

            ZCache.RemoveCache("Comments-" + PostId);
            ZCache.RemoveByPattern("Comments-Recent");

        }

        #endregion

        public static void DeleteUnpublishedComments()
        {
            DeleteByColumn(Columns.IsPublished, false);
        }

        public static void DeleteDeletedComments()
        {
            DeleteByColumn(Columns.IsDeleted,true);
        }

        private static void DeleteByColumn(Column column,  bool state)
        {
            List<string> idsToDelete = new List<string>();
            List<int> postIdsChanged = new List<int>();

            Query q = CreateQuery();
            q.AndWhere(column,state);
            q.AndWhere(Columns.Published, DateTime.Now.AddDays(-1 * Int32.Parse(ConfigurationManager.AppSettings["Graffiti::Comments::DaysToDelete"] ?? "7")),Comparison.LessOrEquals);
            q.Top = "25";
            q.OrderByAsc(Columns.Published);
            CommentCollection cc = CommentCollection.FetchByQuery(q);
            foreach(Comment c in cc)
            {
                idsToDelete.Add(c.Id.ToString());
                if (!postIdsChanged.Contains(c.PostId))
                    postIdsChanged.Add(c.PostId);
            }

            if(idsToDelete.Count > 0)
            {
                QueryCommand deleteCommand =
                    new QueryCommand("DELETE FROM graffiti_Comments where Id in (" + string.Join(",", idsToDelete.ToArray()) + ")");

                DataService.ExecuteNonQuery(deleteCommand);

                foreach(int pid in postIdsChanged)
                    Core.Post.UpdateCommentCount(pid);

                Log.Info("Deleted Comments", idsToDelete.Count + " comment(s) were removed from the database since they were older than " + (ConfigurationManager.AppSettings["Graffiti::Comments::DaysToDelete"] ?? "7") + " days and marked as " + ((column.Name == "IsDeleted") ? " deleted" : " unpublished") );
            }
        }

        public static int GetPublishedCommentCount(string user)
        {
            int count = 0;

            QueryCommand cmd;

            cmd = new QueryCommand(
            @"select " + DataService.Provider.SqlCountFunction("a.PostId") + @" from graffiti_Comments a 
                        inner join graffiti_Posts b on a.PostId = b.Id
                        where b.IsPublished <> 0 and b.IsDeleted = 0
                        and a.IsPublished <> 0 and a.IsDeleted = 0 and b.Status = 1");

            if (!String.IsNullOrEmpty(user))
            {
                cmd.Sql += " and b.CreatedBy = " + DataService.Provider.SqlVariable("CreatedBy");
				cmd.Parameters.Add(Post.FindParameter("CreatedBy")).Value = user;
            }

            try
            {
                count = Convert.ToInt32(DataService.ExecuteScalar(cmd));
            }
            catch (Exception) { }

            return count;
        }


        #region Properties

        public IUser User
        {
            get
            {
                 if(UserName != null)
                    return GraffitiUsers.GetUser(UserName);
                else
                    return new AnonymousUser(Name,WebSite);
            }
        }

        public Post Post
        {
            get
            {
                return Core.Post.GetCachedPost(PostId);
            }
        }

        /// <summary>
        /// Enables quick access to any comment.
        /// </summary>
        public string Url
        {
            get
            {
                return Post.Url + "#comment-" + Id;
            }
        }

        public string Title
        {
            get
            {
                return "RE: " + Post.Title;
            }
        }

        #endregion

        #region Comment Body Helpers

        //public static string ConvertTextToParagraph(string text)
        //{

        //    text = text.Replace("\r\n", "\n").Replace("\r", "\n");
        //    text += "\n\n";

        //    text = text.Replace("\n\n", "\n");

        //    string[] lines = text.Split('\n');

        //    StringBuilder paragraphs = new StringBuilder();

        //    foreach (string line in lines)
        //    {
        //        if (line != null && line.Trim().Length > 0)
        //            paragraphs.AppendFormat("<p>{0}</p>\n", line);
        //    }

        //    return paragraphs.ToString();
        //}


        //private static string FormatLinks(string text)
        //{
        //    if (string.IsNullOrEmpty(text))
        //        return text;



        //    //Find any links
        //    string pattern = @"(\s|^)(http|ftp|https):\/\/[\w]+(.[\w]+)([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])(\s|$)";
        //    MatchCollection matchs;
        //    StringCollection uniqueMatches = new StringCollection();

        //    matchs = Regex.Matches(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //    foreach (Match m in matchs)
        //    {
        //        if (!uniqueMatches.Contains(m.ToString()))
        //        {
        //            string link = m.ToString().Trim();
        //            if (link.Length > 30)
        //            {
        //                try
        //                {
        //                    Uri u = new Uri(link);
        //                    string absolutePath = u.AbsolutePath.EndsWith("/")
        //                                    ? u.AbsolutePath.Substring(0, u.AbsolutePath.Length - 1)
        //                                    : u.AbsolutePath;

        //                    int slashIndex = absolutePath.LastIndexOf("/");
        //                    if (slashIndex > -1)
        //                        absolutePath = "/..." + absolutePath.Substring(slashIndex);

        //                    if (absolutePath.Length > 20)
        //                        absolutePath = absolutePath.Substring(0, 20);

        //                    link = u.Host + absolutePath;
        //                }
        //                catch
        //                {
        //                }
        //            }
        //            text = text.Replace(m.ToString(), " <a target=\"_blank\" href=\"" + m.ToString().Trim() + "\">" + link + "</a> ");
        //            uniqueMatches.Add(m.ToString());
        //        }
        //    }

        //    return text;
        //}


        #endregion 
    }
}