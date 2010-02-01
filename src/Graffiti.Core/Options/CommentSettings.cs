using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using Joel.Net;

namespace Graffiti.Core
{
    /// <summary>
    /// Stores all comment (and spam) settings in an ObjectStore instance
    /// </summary>
    [Serializable]
    public class CommentSettings
    {
        private bool? _useAkismet;
        private int? _spamScore;
        private int? _akismetScore;

        public bool UseAkismet
        {
            get
            {
                if (_useAkismet.HasValue) return _useAkismet.Value;

                return !String.IsNullOrEmpty(AkismetId);
            }
            set
            {
                _useAkismet = value;
            }
        }

        public int SpamScore
        {
            get
            {
                if (_spamScore.HasValue) return _spamScore.Value;

                int spamScore;

                Int32.TryParse(ConfigurationManager.AppSettings["Graffiti:Spam:Score"], out spamScore);

                return spamScore == 0 ? 10 : spamScore;
            }
            set
            {
                _spamScore = value;
            }
        }

        public int AkismetScore
        {
            get
            {
                if (_akismetScore.HasValue) return _akismetScore.Value;

                int akistmetScore;

                Int32.TryParse(ConfigurationManager.AppSettings["Graffiti:Spam:Akismet"], out akistmetScore);

                return akistmetScore == 0 ? 10 : akistmetScore;
            }
            set
            {
                _akismetScore = value;
            }
        }

        public static int ScoreComment(Comment comment, Post p)
        {
            int score = 0;

            CommentSettings cs = Get();

            if (string.IsNullOrEmpty(comment.Body))
                throw new Exception("No comment body found");

            if (!cs.EnableCommentOnPost(p))
                throw new Exception("No new comments are allowed on this post");

            if(comment.Body.Trim().Length < 20)
            {
                score += (-1*(comment.Body.Trim().Length - 20));
            }
             
            score += Regex.Matches(comment.Body, @"(http|ftp|https):\/\/[\w]+(.[\w]+)([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])", RegexOptions.IgnoreCase).Count;

            score += CountWords(comment);

            if (!String.IsNullOrEmpty(cs.AkismetId))
            {
                try
                {
                    AkismetComment akComment = GetComment(comment);
                    Akismet akismet = new Akismet(cs.AkismetId, akComment.Blog, SiteSettings.Version);

                    if (akismet.CommentCheck(akComment))
                        score += cs.AkismetScore;
                }
                catch(Exception ex)
                {
                    Log.Error("Spam - Akismet", "Akismet scoring failed.\n\nReason: {0}", ex);
                }
            }


            return score;
        }

        private static int CountWords(Comment comment)
        {
            try
            {
                string words =
                    Util.GetFileText(HttpContext.Current.Server.MapPath("~/__utility/spam/badwords.txt"));

                int count = 0;
                foreach (string word in words.Split(new char[] {';', '\n'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    count += CountWord(word, comment);
                }

                return count;
            }
            catch(Exception ex)
            {
                Log.Error("Spam - Comment Count", "Counting bad words failed. \n\nReason: {0}", ex);
            }

            return 0;
        }

        private static int CountWord(string word, Comment comment)
        {
            Regex r = new Regex(word.Trim(), RegexOptions.IgnoreCase);

            int count = r.Matches(comment.Body).Count;
            count += r.Matches(comment.Name).Count;
            if (comment.WebSite != null)
                count += r.Matches(comment.WebSite).Count;

            return count;
        }

        private static AkismetComment GetComment(Comment zComment)
        {
            Joel.Net.AkismetComment comment = new Joel.Net.AkismetComment();
            comment.Blog = new Macros().FullUrl(new Urls().Home);
            comment.CommentAuthor = zComment.Name;
            comment.CommentAuthorUrl = zComment.WebSite;
            comment.CommentContent = zComment.Body;
            comment.CommentType = "comment";
            comment.UserAgent = HttpContext.Current.Request.UserAgent;
            comment.UserIp = zComment.IPAddress;

            return comment;
        }

        public bool EnableCommentOnPost(Post p)
        {
            if (!p.EnableComments)
                return false;

            if(CommentDays == -1)
                return true;

            if(CommentDays == 0)
                return false;

            return p.Published.AddDays(CommentDays) > DateTime.Now;
        }

        public string AkismetId = null;
        public int CommentDays = 14;
        public string Email;
        public bool EnableCommentsDefault = true;

        public void Save()
        {
            ObjectManager.Save(this, "CommentSettings");
        }

        public static CommentSettings Get()
        {
            return ObjectManager.Get<CommentSettings>("CommentSettings");
        }
    }
}