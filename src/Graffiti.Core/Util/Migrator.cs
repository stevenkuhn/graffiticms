using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;

namespace Graffiti.Core
{
    // This interface must be implemented in order to migrate into graffiti
    #region IMigrateFrom

    public interface IMigrateFrom
    {
        List<MigratorPost> GetPosts();
        List<MigratorComment> GetComments(int postID);
    }

    #endregion

    // These are the temporary holding objects from an IMigrateFrom to Graffiti
    #region Temporary Entities

    public class MigratorPost
    {
        private int _postID;
        private string _legacyID;
        private string _body;
        private string _subject;
        private string _name;
        private DateTime _createdOn;
        private string _author;
        private List<String> _tagsAndCategories;
        private bool _isPublished;
        private List<MigratorComment> _comments;

        public int PostID
        {
            get { return _postID; }
            set { _postID = value; }
        }

        public string LegacyID
        {
            get { return _legacyID; }
            set { _legacyID = value; }
        }

        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public DateTime CreatedOn
        {
            get { return _createdOn; }
            set { _createdOn = value; }
        }

        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        public List<String> TagsAndCategories
        {
            get { return _tagsAndCategories; }
            set { _tagsAndCategories = value; }
        }

        public bool IsPublished
        {
            get { return _isPublished; }
            set { _isPublished = value; }
        }

        public List<MigratorComment> Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }
    }

    public class MigratorComment
    {
        private int _postID;
        private string _body;
        private DateTime _publishedOn;
        private bool _isPublished;
        private string _ipAddress;
        private string _website;
        private string _userName;
        private int _spamScore;
        private string _postAuthor;
        private string _email;
        private bool _isTrackback = false;

        public int PostID
        {
            get { return _postID; }
            set { _postID = value; }
        }

        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        public DateTime PublishedOn
        {
            get { return _publishedOn; }
            set { _publishedOn = value; }
        }

        public bool IsPublished
        {
            get { return _isPublished; }
            set { _isPublished = value; }
        }

        public string IPAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        public string WebSite
        {
            get { return _website; }
            set { _website = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public int SpamScore
        {
            get { return _spamScore; }
            set { _spamScore = value; }
        }

        public string PostAuthor
        {
            get { return _postAuthor; }
            set { _postAuthor = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public bool IsTrackback
        {
            get { return _isTrackback; }
            set { _isTrackback = value; }
        }

    }

    #endregion

    // utilities
    #region Utilities

    public static class MigratorUtilities
    {
        public static string GetEncodedFileFromFileUpload(FileUpload control)
        {
            control.PostedFile.InputStream.Position = 0;

            int fileLength = control.PostedFile.ContentLength;

            Byte[] input = new Byte[fileLength];
            Stream stream = control.FileContent;
            stream.Read(input, 0, fileLength);

            UTF8Encoding utf = new UTF8Encoding();
            string encodedFile = utf.GetString(input);

            return encodedFile;
        }

        public static byte[] GetFileBytes(FileUpload control)
        {
            control.PostedFile.InputStream.Position = 0;
            int fileLength = control.PostedFile.ContentLength;
            
            Byte[] input = new Byte[fileLength];
            Stream stream = control.FileContent;
            stream.Read(input, 0, fileLength);

            return input;
        }
    }

    #endregion

    // CS2.1 database migration
    #region CS2.1Database

    public class CS21Database : IMigrateFrom
    {
        private string _databaseConnString;
        private string _applicationKey;
        private string _userName;

        public string DatabaseConnectionString
        {
            get { return _databaseConnString; }
            set { _databaseConnString = value; }
        }

        public string ApplicationKey
        {
            get { return _applicationKey; }
            set { _applicationKey = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        #region IMigrateFrom Members

        public List<MigratorPost> GetPosts()
        {
            List<MigratorPost> posts = new List<MigratorPost>();

            int userid = 0;
            int sectionid = 0;

            using (SqlConnection conn = new SqlConnection(_databaseConnString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("select U.UserID from cs_Users U inner join aspnet_Users AU on AU.UserId = U.MembershipID where AU.Username = @username", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@username", _userName));
                    userid = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (SqlCommand cmd = new SqlCommand("select SectionID from cs_Sections where ApplicationKey = @appkey and ApplicationType=1", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@appkey", _applicationKey));
                    sectionid = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (SqlCommand cmd = new SqlCommand("select PostID, PostAuthor, FormattedBody, UserTime, PostName, Subject, IsApproved from cs_Posts where PostLevel = 1 and SectionID = @sectionid and UserID=@userid", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@sectionid", sectionid));
                    cmd.Parameters.Add(new SqlParameter("@userid", userid));

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            MigratorPost post = new MigratorPost();

                            post.PostID = Convert.ToInt32(rdr["PostID"]);
                            post.Author = rdr["PostAuthor"] as string;
                            post.Body = rdr["FormattedBody"] as string;
                            post.CreatedOn = Convert.ToDateTime(rdr["UserTime"]);
                            post.Name = rdr["PostName"] as string;
                            post.Subject = rdr["Subject"] as string;
                            post.TagsAndCategories = GetTags(Convert.ToInt32(rdr["PostID"]));
                            post.IsPublished = Convert.ToBoolean(rdr["IsApproved"].ToString());

                            posts.Add(post);
                        }

                        rdr.Close();
                    }
                }

                conn.Close();
            }

            // add the connection string to cache
            ZCache.MaxCache("MigratorConnectionString", _databaseConnString);

            return posts;
        }

        public List<MigratorComment> GetComments(int postID)
        {
            string dbConnString = ZCache.Get<string>("MigratorConnectionString");

            if (String.IsNullOrEmpty(dbConnString))
                throw new Exception("The database connection string has expired. Please restart your import.");

            List<MigratorComment> comments = new List<MigratorComment>();

            using (SqlConnection conn = new SqlConnection(dbConnString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("select p.PostID, p.FormattedBody, p.PostDate, p.IPAddress, p.ApplicationPostType, p.PropertyNames, p.PropertyValues, p.IsApproved, p.SpamScore, IsNull(m.Email, '') as Email from cs_Posts p LEFT OUTER JOIN cs_Users u ON u.UserID = p.UserID LEFT OUTER JOIN aspnet_Membership m ON u.MembershipID = m.UserID where ParentID = @parentid and PostLevel > 1 and ApplicationPostType in (4,8) order by PostID asc", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@parentid", postID));

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            MigratorComment comment = new MigratorComment();

                            comment.PostID = postID;
                            comment.PublishedOn = Convert.ToDateTime(rdr["PostDate"]);
                            comment.IPAddress = rdr["IPAddress"] as string;

                            // Remove <p> tags from comment body, as Graffiti will add these back in when saving the comment
                            comment.Body = rdr["FormattedBody"] as string;
                            if (!string.IsNullOrEmpty(comment.Body))
                                comment.Body = comment.Body.Replace("<p>", "").Replace("</p>", "");

                            NameValueCollection nvc =
                                ConvertToNameValueCollection(rdr["PropertyNames"] as string,
                                                             rdr["PropertyValues"] as string);

                            comment.IsPublished = Convert.ToBoolean(rdr["IsApproved"]);
                            comment.SpamScore = Convert.ToInt32(rdr["SpamScore"]);
                            comment.WebSite = nvc["TitleUrl"];

                            // ApplicationPostType:  Comment = 4; Trackback = 8;
                            Int32 applicationPostType = Convert.ToInt32(rdr["ApplicationPostType"]);

                            if (applicationPostType == 8)
                            {
                                comment.UserName = !string.IsNullOrEmpty(nvc["trackbackName"]) ? nvc["trackbackName"] : "TrackBack";
                                comment.IsTrackback = true;
                            }
                            else
                            {
                                comment.Email = rdr["Email"] as string;
                                comment.UserName = nvc["SubmittedUserName"];
                                comment.IsTrackback = false;
                            }



                            comments.Add(comment);
                        }

                        rdr.Close();
                    }
                }

                conn.Close();
            }

            return comments;
        }

        #endregion

        private List<String> GetTags(int postid)
        {
            List<String> list = new List<String>();

            using (SqlConnection conn = new SqlConnection(_databaseConnString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("select a.Name from cs_Post_Categories a join cs_Posts_InCategories b on a.CategoryID = b.CategoryID where b.PostID = @postid", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@postid", postid));

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            list.Add(rdr["Name"] as string);
                        }

                        rdr.Close();
                    }
                }

                conn.Close();
            }

            return list;
        }

        private static NameValueCollection ConvertToNameValueCollection(string keys, string values)
        {
            NameValueCollection nvc = new NameValueCollection();

            if (keys != null && values != null && keys.Length > 0 && values.Length > 0)
            {
                char[] splitter = new char[1] { ':' };
                string[] keyNames = keys.Split(splitter);

                for (int i = 0; i < (keyNames.Length / 4); i++)
                {
                    int start = int.Parse(keyNames[(i * 4) + 2], CultureInfo.InvariantCulture);
                    int len = int.Parse(keyNames[(i * 4) + 3], CultureInfo.InvariantCulture);
                    string key = keyNames[i * 4];

                    //Future version will support more complex types	
                    if (((keyNames[(i * 4) + 1] == "S") && (start >= 0)) && (values.Length >= (start + len)))
                    {
                        nvc[key] = values.Substring(start, len);
                    }
                }
            }

            return nvc;
        }
    }

    #endregion

    // CS2007 database migration
    #region CS2007Database

    public class CS2007Database : IMigrateFrom
    {
        private string _databaseConnString;
        private string _applicationKey;
        private string _userName;

        public string DatabaseConnectionString
        {
            get { return _databaseConnString; }
            set { _databaseConnString = value; }
        }

        public string ApplicationKey
        {
            get { return _applicationKey; }
            set { _applicationKey = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        #region IMigrateFrom Members

        public List<MigratorPost> GetPosts()
        {
            List<MigratorPost> posts = new List<MigratorPost>();

            int userid = 0;
            int sectionid = 0;

            using (SqlConnection conn = new SqlConnection(_databaseConnString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("select UserID from cs_Users where UserName = @username", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@username", _userName));
                    userid = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (SqlCommand cmd = new SqlCommand("select SectionID from cs_Sections where ApplicationKey = @appkey and ApplicationType=1", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@appkey", _applicationKey));
                    sectionid = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (SqlCommand cmd = new SqlCommand("select PostID, PostAuthor, FormattedBody, UserTime, PostName, Subject, IsApproved from cs_Posts where PostLevel = 1 and SectionID = @sectionid and UserID=@userid", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@sectionid", sectionid));
                    cmd.Parameters.Add(new SqlParameter("@userid", userid));

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            MigratorPost post = new MigratorPost();

                            post.PostID = Convert.ToInt32(rdr["PostID"]);
                            post.Author = rdr["PostAuthor"] as string;
                            post.Body = rdr["FormattedBody"] as string;
                            post.CreatedOn = Convert.ToDateTime(rdr["UserTime"]);
                            post.Name = rdr["PostName"] as string;
                            post.Subject = rdr["Subject"] as string;
                            post.TagsAndCategories = GetTags(Convert.ToInt32(rdr["PostID"]));
                            post.IsPublished = Convert.ToBoolean(rdr["IsApproved"].ToString());

                            posts.Add(post);
                        }

                        rdr.Close();
                    }
                }

                conn.Close();
            }
            
            // add the connection string to cache
            ZCache.MaxCache("MigratorConnectionString", _databaseConnString);
            
            return posts;
        }

        public List<MigratorComment> GetComments(int postID)
        {
            string dbConnString = ZCache.Get<string>("MigratorConnectionString");

            if (String.IsNullOrEmpty(dbConnString))
                throw new Exception("The database connection string has expired. Please restart your import.");
            
            List<MigratorComment> comments = new List<MigratorComment>();

            using (SqlConnection conn = new SqlConnection(dbConnString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("select p.PostID, p.FormattedBody, p.PostDate, p.IPAddress, p.ApplicationPostType, p.PropertyNames, p.PropertyValues, p.IsApproved, p.SpamScore, IsNull(u.Email, '') as Email from cs_Posts p LEFT OUTER JOIN cs_Users u ON p.UserID = u.UserID where ParentID = @parentid and PostLevel > 1 and ApplicationPostType in (4,8) order by PostID asc", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@parentid", postID));

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            MigratorComment comment = new MigratorComment();

                            comment.PostID = postID;
                            comment.PublishedOn = Convert.ToDateTime(rdr["PostDate"]);
                            comment.IPAddress = rdr["IPAddress"] as string;

                            // Remove <p> tags from comment body, as Graffiti will add these back in when saving the comment
                            comment.Body = rdr["FormattedBody"] as string;
                            if (!string.IsNullOrEmpty(comment.Body))
                                comment.Body = comment.Body.Replace("<p>", "").Replace("</p>", "");

                            NameValueCollection nvc =
                                ConvertToNameValueCollection(rdr["PropertyNames"] as string,
                                                             rdr["PropertyValues"] as string);

                            comment.IsPublished = Convert.ToBoolean(rdr["IsApproved"]);
                            comment.SpamScore = Convert.ToInt32(rdr["SpamScore"]);
                            comment.WebSite = nvc["TitleUrl"];

                            // ApplicationPostType:  Comment = 4; Trackback = 8;
                            Int32 applicationPostType = Convert.ToInt32(rdr["ApplicationPostType"]);

                            if (applicationPostType == 8)
                            {
                                comment.UserName = !string.IsNullOrEmpty(nvc["trackbackName"]) ? nvc["trackbackName"] : "TrackBack";
                                comment.IsTrackback = true;
                            }
                            else
                            {
                                comment.Email = rdr["Email"] as string;
                                comment.UserName = nvc["SubmittedUserName"];
                                comment.IsTrackback = false;
                            }



                            comments.Add(comment);
                        }

                        rdr.Close();
                    }
                }

                conn.Close();
            }

            return comments;
        }

        #endregion

        private List<String> GetTags(int postid)
        {
            List<String> list = new List<String>();

            using (SqlConnection conn = new SqlConnection(_databaseConnString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("select a.Name from cs_Post_Categories a join cs_Posts_InCategories b on a.CategoryID = b.CategoryID where b.PostID = @postid", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@postid", postid));

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            list.Add(rdr["Name"] as string);
                        }

                        rdr.Close();
                    }
                }

                conn.Close();
            }

            return list;
        }

        private static NameValueCollection ConvertToNameValueCollection(string keys, string values)
        {
            NameValueCollection nvc = new NameValueCollection();

            if (keys != null && values != null && keys.Length > 0 && values.Length > 0)
            {
                char[] splitter = new char[1] { ':' };
                string[] keyNames = keys.Split(splitter);

                for (int i = 0; i < (keyNames.Length / 4); i++)
                {
                    int start = int.Parse(keyNames[(i * 4) + 2], CultureInfo.InvariantCulture);
                    int len = int.Parse(keyNames[(i * 4) + 3], CultureInfo.InvariantCulture);
                    string key = keyNames[i * 4];

                    //Future version will support more complex types	
                    if (((keyNames[(i * 4) + 1] == "S") && (start >= 0)) && (values.Length >= (start + len)))
                    {
                        nvc[key] = values.Substring(start, len);
                    }
                }
            }

            return nvc;
        }
    }

    #endregion

    // Wordpress migration
    #region Wordpress

    public class Wordpress : IMigrateFrom
    {
        private string _encodedFile;

        public string EncodedFile
        {
            get { return _encodedFile; }
            set { _encodedFile = value; }
        }

        #region IMigrateFrom Members

        public List<MigratorPost> GetPosts()
        {
            // do some manual cleanup of the wordpress export
            _encodedFile = _encodedFile.Replace("<wp:comment_content>", "<wp:comment_content><![CDATA[");
            _encodedFile = _encodedFile.Replace("</wp:comment_content>", "]]></wp:comment_content>");

            _encodedFile = _encodedFile.Replace("<wp:comment_content><![CDATA[<![CDATA", "<wp:comment_content><![CDATA[");
            _encodedFile = _encodedFile.Replace("]]>]]></wp:comment_content>", "]]></wp:comment_content>");

            _encodedFile = _encodedFile.Replace("<wp:comment_author>", "<wp:comment_author><![CDATA[");
            _encodedFile = _encodedFile.Replace("</wp:comment_author>", "]]></wp:comment_author>");

            _encodedFile = _encodedFile.Replace("<wp:comment_author><![CDATA[<![CDATA[", "<wp:comment_author><![CDATA[");
            _encodedFile = _encodedFile.Replace("]]>]]></wp:comment_author>", "]]></wp:comment_author>");

            _encodedFile = _encodedFile.Replace("<wp:comment_author_url>", "<wp:comment_author_url><![CDATA[");
            _encodedFile = _encodedFile.Replace("</wp:comment_author_url>", "]]></wp:comment_author_url>");

            _encodedFile = _encodedFile.Replace("<wp:comment_author_url><![CDATA[<![CDATA", "<wp:comment_author_url><![CDATA[");
            _encodedFile = _encodedFile.Replace("]]>]]></wp:comment_author_url>", "]]></wp:comment_author_url>");

            _encodedFile = _encodedFile.Replace("<wp:comment_author_email>", "<wp:comment_author_email><![CDATA[");
            _encodedFile = _encodedFile.Replace("</wp:comment_author_email>", "]]></wp:comment_author_email>");

            _encodedFile = _encodedFile.Replace("<wp:comment_author_email><![CDATA[<![CDATA", "<wp:comment_author_email><![CDATA[");
            _encodedFile = _encodedFile.Replace("]]>]]></wp:comment_author_email>", "]]></wp:comment_author_email>");

            List<MigratorPost> posts = new List<MigratorPost>();
            List<MigratorComment> comments = new List<MigratorComment>();

            XmlDocument doc = new XmlDocument(); 
            doc.LoadXml(_encodedFile);

            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("content", "http://purl.org/rss/1.0/modules/content/");
            mgr.AddNamespace("wp", "http://wordpress.org/export/1.0/");
            mgr.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");

            XmlNode node = doc.SelectSingleNode("/rss/channel");

            foreach(XmlNode itemNode in node.SelectNodes("item"))
            {
                if (itemNode.SelectSingleNode("wp:post_type", mgr).InnerText == "post"
                        && itemNode.SelectSingleNode("wp:status", mgr).InnerText == "publish")
                {
                    MigratorPost post = new MigratorPost();

                    post.PostID = Convert.ToInt32(itemNode.SelectSingleNode("wp:post_id", mgr).InnerText);
                    post.Author = itemNode.SelectSingleNode("dc:creator", mgr).InnerText;
                    post.Body = itemNode.SelectSingleNode("content:encoded", mgr).InnerText;
                    post.CreatedOn = Convert.ToDateTime(itemNode.SelectSingleNode("pubDate").InnerText);
                    post.Name = itemNode.SelectSingleNode("wp:post_name", mgr).InnerText;
                    post.Subject = itemNode.SelectSingleNode("title").InnerText;
                    
                    // tags
                    foreach (XmlNode catNode in itemNode.SelectNodes("category"))
                    {
                        if (catNode.Attributes["domain"] != null
                            && catNode.Attributes["domain"].Value == "tag")
                        {
                            post.TagsAndCategories = new List<String>();

                            string[] tags = catNode.InnerText.Split(' ');
                            foreach (string tag in tags)
                                post.TagsAndCategories.Add(tag);
                        }
                        else
                        {
                            post.TagsAndCategories = new List<string>();
                            post.TagsAndCategories.Add(catNode.InnerText);
                        }
                    }

                    post.IsPublished = true;

                    // comments : these are going to be stored in cache for wordpress types
                    foreach (XmlNode commentNode in itemNode.SelectNodes("wp:comment", mgr))
                    {
                        if (commentNode.SelectSingleNode("wp:comment_approved", mgr).InnerText == "1")
                        {
                            MigratorComment comment = new MigratorComment();

                            comment.PostID = post.PostID;
                            comment.Body = commentNode.SelectSingleNode("wp:comment_content", mgr).InnerText;
                            comment.PublishedOn = Convert.ToDateTime(commentNode.SelectSingleNode("wp:comment_date", mgr).InnerText);
                            comment.IPAddress = commentNode.SelectSingleNode("wp:comment_author_IP", mgr).InnerText;
                            
                            comment.WebSite = commentNode.SelectSingleNode("wp:comment_author_url", mgr).InnerText;
                            comment.UserName = commentNode.SelectSingleNode("wp:comment_author", mgr).InnerText;
                            comment.Email = commentNode.SelectSingleNode("wp:comment_author_email", mgr).InnerText;
                            comment.IsPublished = true;

                            comments.Add(comment);
                        }
                    }

                    posts.Add(post);
                }
            }

            // add the comments to cache
            ZCache.MaxCache("MigratorComments", comments);

            return posts;
        }

        public List<MigratorComment> GetComments(int postID)
        {
            List<MigratorComment> comments = ZCache.Get<List<MigratorComment>>("MigratorComments");

            if (comments == null)
                throw new Exception("The comment cache has expired. Please restart your import.");

            List<MigratorComment> postComments = comments.FindAll(
                                        delegate(MigratorComment c)
                                        {
                                            return c.PostID == postID;
                                        });

            return postComments;
        }

        #endregion
    }

    #endregion

    // BlogML migration
    #region BlogML

    public class BlogML : IMigrateFrom
    {
        private string _encodedFile;
        private byte[] _fileBytes;

        public string EncodedFile
        {
            get { return _encodedFile; }
            set { _encodedFile = value; }
        }

        public byte[] FileBytes
        {
            get { return _fileBytes; }
            set { _fileBytes = value; }
        }

        private void CleanupEncodedFile()
        {
            string ns = "xmlns=\"http://www.blogml.com/2006/09/BlogML\"";
            if (_encodedFile.IndexOf(ns) > 0)
                _encodedFile = _encodedFile.Replace(ns, "");

            ns = "xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"";
            if (_encodedFile.IndexOf(ns) > 0)
                _encodedFile = _encodedFile.Replace(ns, "");

            try
            {
                _encodedFile = _encodedFile.Replace(_encodedFile.Substring(0, _encodedFile.IndexOf("<")), "");
            }
            catch (Exception) { } // temp hack
        }

        #region IMigrateFrom Member

        public List<MigratorPost> GetPosts()
        {
            int postId = 100; // this is only used if the id format is not an int (dasblog)

            CleanupEncodedFile();

            List<MigratorPost> posts = new List<MigratorPost>();
            List<MigratorComment> comments = new List<MigratorComment>();
            List<BlogMLTag> tags = new List<BlogMLTag>();

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(_encodedFile);
            }
            catch (Exception exc)
            {
                // might be unicode, and since DOM detection doesn't work correctly, we will try to load it unicode
                try
                {
                    _encodedFile = Encoding.Unicode.GetString(_fileBytes);
                    CleanupEncodedFile();

                    doc.LoadXml(_encodedFile);
                }
                catch(Exception)
                {
                    // didn't work here either, bubble it up
                    throw exc;
                }
            }

            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);

            XmlNode node = doc.SelectSingleNode("blog", mgr);

            foreach(XmlNode catNode in node.SelectNodes("categories/category"))
            {
                BlogMLTag t = new BlogMLTag();
                t.Id = catNode.Attributes["id"].Value;

                if(catNode.SelectSingleNode("title") == null)
                    t.Description = catNode.Attributes["id"].Value;
                else
                    t.Description = catNode.SelectSingleNode("title").InnerText;
                
                tags.Add(t);
            }

            foreach (XmlNode itemNode in node.SelectNodes("posts/post"))
            {
                if (itemNode.Attributes["approved"].Value == "true")
                {
                    MigratorPost post = new MigratorPost();

                    int temp = 0;
                    if(itemNode.Attributes["id"] != null && int.TryParse(itemNode.Attributes["id"].Value, out temp))
                    {
                        postId = Convert.ToInt32(itemNode.Attributes["id"].Value);
                        post.PostID = postId;
                    }
                    else
                    {
                        post.PostID = postId;
                        postId ++;
                    }

                    //post.Author = itemNode.SelectSingleNode("dc:creator").InnerText;
                    post.Body = itemNode.SelectSingleNode("content").InnerText;
                    post.CreatedOn = Convert.ToDateTime(itemNode.Attributes["date-created"].Value);
                    post.Name = itemNode.SelectSingleNode("title").InnerText;
                    post.Subject = itemNode.SelectSingleNode("title").InnerText;

                    post.IsPublished = true;

                    // tags
                    post.TagsAndCategories = new List<String>();

                    foreach (XmlNode catNode in itemNode.SelectNodes("categories/category"))
                    {
                        string catId = catNode.Attributes["ref"].Value;

                        BlogMLTag cat = tags.Find(delegate(BlogMLTag t)
                                                    {
                                                        return t.Id == catId;
                                                    });

                        if(cat != null)
                            post.TagsAndCategories.Add(cat.Description);
                    }

                    // comments : these are going to be stored in cache for wordpress types
                    foreach (XmlNode commentNode in itemNode.SelectNodes("comments/comment"))
                    {
                        if (commentNode.Attributes["approved"].Value == "true")
                        {
                            MigratorComment comment = new MigratorComment();

                            comment.PostID = post.PostID;
                            comment.Body = commentNode.SelectSingleNode("content").InnerText;
                            comment.PublishedOn = Convert.ToDateTime(commentNode.Attributes["date-created"].Value);

                            comment.WebSite = commentNode.Attributes["user-url"] == null ? "" : commentNode.Attributes["user-url"].Value;
                            comment.UserName = commentNode.Attributes["user-name"].Value;
                            comment.IsPublished = true;

                            comments.Add(comment);
                        }
                    }

                    posts.Add(post);
                }
            }

            // add the comments to cache
            ZCache.MaxCache("MigratorComments", comments);

            return posts;
        }

        public List<MigratorComment> GetComments(int postID)
        {
            List<MigratorComment> comments = ZCache.Get<List<MigratorComment>>("MigratorComments");

            if (comments == null)
                throw new Exception("The comment cache has expired. Please restart your import.");

            List<MigratorComment> postComments = comments.FindAll(
                                        delegate(MigratorComment c)
                                        {
                                            return c.PostID == postID;
                                        });

            return postComments;
        }

        #endregion
    }

    public class BlogMLTag
    {
        private string _id;
        private string _description;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }

    #endregion

    // dasBlog migration
    #region dasBlog

    public class dasBlog : IMigrateFrom
    {
        private string CleanupDayEntryFileForMerge(string toClean)
        {
            try
            {
                string cleaned = "";

                cleaned = toClean.Replace(toClean.Substring(0, toClean.IndexOf("</Date>") + 7), "");
                cleaned = cleaned.Replace(cleaned.Substring(0, cleaned.IndexOf("<")), "");
                cleaned = cleaned.Replace("</DayEntry>", "");
                cleaned = cleaned.Replace(" xsi:nil=\"true\"", "");

                return cleaned;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string CleanupDayFeedbackFileForMerge(string toClean)
        {
            try
            {
                string cleaned = "";

                cleaned = toClean.Replace(toClean.Substring(0, toClean.IndexOf("</Date>") + 7), "");
                cleaned = cleaned.Replace(cleaned.Substring(0, cleaned.IndexOf("<")), "");
                cleaned = cleaned.Replace("</DayExtra>", "");

                return cleaned;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string MergePostFiles()
        {
            StringBuilder allFilesMerged = new StringBuilder();

            allFilesMerged.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            allFilesMerged.Append("<Posts>");

            string contentDir = HttpContext.Current.Server.MapPath("~/files/temp/content");
            
            if(!Directory.Exists(contentDir))
                throw new Exception("The content folder could not be found");

            foreach (string f in Directory.GetFiles(contentDir))
            {
                if (f.Contains("dayentry"))
                {
                    StringBuilder contents = new StringBuilder();

                    StreamReader sr = File.OpenText(Path.Combine(contentDir, f));

                    do
                    {
                        contents.Append(sr.ReadLine());
                    }
                    while (sr.Peek() != -1);

                    string cleaned = CleanupDayEntryFileForMerge(contents.ToString());

                    allFilesMerged.Append(cleaned);
                }
            }

            allFilesMerged.Append("</Posts>");

            string filename = Path.Combine(contentDir, System.Guid.NewGuid().ToString().Substring(0, 7) + "-entry.xml");

            StreamWriter sw = File.CreateText(filename);
            sw.Write(allFilesMerged.ToString());
            sw.Close();

            return filename;
        }

        private string MergeFeedbackFiles()
        {
            StringBuilder allFilesMerged = new StringBuilder();

            allFilesMerged.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            allFilesMerged.Append("<Feedback>");

            string contentDir = HttpContext.Current.Server.MapPath("~/files/temp/content");

            if (!Directory.Exists(contentDir))
                throw new Exception("The content folder could not be found");

            foreach (string f in Directory.GetFiles(contentDir))
            {
                if (f.Contains("dayfeedback"))
                {
                    StringBuilder contents = new StringBuilder();

                    StreamReader sr = File.OpenText(Path.Combine(contentDir, f));

                    do
                    {
                        contents.Append(sr.ReadLine());
                    }
                    while (sr.Peek() != -1);

                    string cleaned = CleanupDayFeedbackFileForMerge(contents.ToString());

                    allFilesMerged.Append(cleaned);
                }
            }

            allFilesMerged.Append("</Feedback>");

            string filename = Path.Combine(contentDir, System.Guid.NewGuid().ToString().Substring(0, 7) + "-feedback.xml");

            StreamWriter sw = File.CreateText(filename);
            sw.Write(allFilesMerged.ToString());
            sw.Close();

            return filename;
        }

        #region IMigrateFrom Member

        public List<MigratorPost> GetPosts()
        {
            string allXmlPosts = MergePostFiles();
            string allXmlFeedback = MergeFeedbackFiles();

            List<MigratorPost> posts = new List<MigratorPost>();
            List<MigratorComment> comments = new List<MigratorComment>();

            XmlDocument doc = new XmlDocument();
            doc.Load(new StreamReader(allXmlPosts));

            XmlDocument doc2 = new XmlDocument();
            doc2.Load(new StreamReader(allXmlFeedback));

            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);

            XmlNode node = doc.SelectSingleNode("Posts", mgr);

            XmlNamespaceManager mgr2 = new XmlNamespaceManager(doc2.NameTable);

            XmlNode node2 = doc2.SelectSingleNode("Feedback", mgr2);

            //foreach (XmlNode catNode in node.SelectNodes("categories/category"))
            //{
            //    BlogMLTag t = new BlogMLTag();
            //    t.Id = catNode.Attributes["id"].Value;

            //    if (catNode.SelectSingleNode("title") == null)
            //        t.Description = catNode.Attributes["id"].Value;
            //    else
            //        t.Description = catNode.SelectSingleNode("title").InnerText;

            //    tags.Add(t);
            //}

            int postId = 100;

            // grab the dasBlog mapping table from the cache if it already exists
            // so we can add to it, otherwise create a new object
            List<PermalinkMap> permalinkMapping = ObjectManager.Get<List<PermalinkMap>>("dasBlogPermalinks");
            if (permalinkMapping == null)
                permalinkMapping = new List<PermalinkMap>();

            foreach (XmlNode itemNode in node.SelectNodes("Entries/Entry"))
            {
                if (itemNode.SelectSingleNode("IsPublic") == null || itemNode.SelectSingleNode("IsPublic").InnerText == "true")
                {
                    MigratorPost post = new MigratorPost();

                    post.PostID = postId;
                    postId++;

                    post.LegacyID = itemNode.SelectSingleNode("EntryId").InnerText;
                    post.Body = itemNode.SelectSingleNode("Content").InnerText;
                    post.CreatedOn = Convert.ToDateTime(itemNode.SelectSingleNode("Created").InnerText);
                    if (itemNode.SelectSingleNode("Categories") != null)
                    {
                        string categoryText = itemNode.SelectSingleNode("Categories").InnerText;
                        categoryText = categoryText.Replace('|', ';');
                        string[] categories = categoryText.Split(';');

                        post.TagsAndCategories = new List<string>();
                        foreach (string c in categories)
                        {
                            if (post.TagsAndCategories.Contains(c) == false)
                                post.TagsAndCategories.Add(c);
                        }
                    }

                    if (itemNode.SelectSingleNode("Title") == null)
                        post.Name = "";
                    else
                        post.Name = itemNode.SelectSingleNode("Title").InnerText;

                    if (itemNode.SelectSingleNode("Subject") == null)
                        post.Subject = post.Name;
                    else
                        post.Subject = itemNode.SelectSingleNode("Subject").InnerText;

                    post.IsPublished = true;

            //        // tags
            //        post.TagsAndCategories = new List<String>();

            //        foreach (XmlNode catNode in itemNode.SelectNodes("categories/category"))
            //        {
            //            string catId = catNode.Attributes["ref"].Value;

            //            BlogMLTag cat = tags.Find(delegate(BlogMLTag t)
            //                                        {
            //                                            return t.Id == catId;
            //                                        });

            //            if (cat != null)
            //                post.TagsAndCategories.Add(cat.Description);
            //        }

                    // store the post id and name in a url map so we can look this up
                    // in the dasBlog301 plugin
                    PermalinkMap permalinkMap = new PermalinkMap(post.LegacyID, post.Name);
                    if (permalinkMapping.Contains(permalinkMap) == false)
                    {
                        permalinkMapping.Add(permalinkMap);
                    }

                    posts.Add(post);
                }

            }

            ObjectManager.Save(permalinkMapping, "dasBlogPermalinks");

            foreach (XmlNode commentNode in node2.SelectNodes("*", mgr2))
            {
                if (commentNode.Name == "Trackings")
                {
                    foreach (XmlNode trackingNode in commentNode.SelectNodes("*"))
                    {
                        MigratorComment comment = new MigratorComment();

                        MigratorPost p = posts.Find(
                                    delegate(MigratorPost c)
                                    {
                                        return c.LegacyID.ToUpperInvariant() == trackingNode.SelectSingleNode("TargetEntryId").InnerText.ToUpperInvariant();
                                    });

                        if (p != null)
                            comment.PostID = p.PostID;

                        comment.Body = trackingNode.SelectSingleNode("TargetTitle") != null ? trackingNode.SelectSingleNode("TargetTitle").InnerText : "";
                        comment.PublishedOn = Convert.ToDateTime("01/01/1900");

                        comment.WebSite = trackingNode.SelectSingleNode("PermaLink").InnerText;
                        comment.UserName = trackingNode.SelectSingleNode("TargetTitle") != null ? trackingNode.SelectSingleNode("TargetTitle").InnerText : "";
                        comment.IsPublished = true;
                        comment.IsTrackback = true;

                        comments.Add(comment);
                    }
                }
                if (commentNode.Name == "Comments")
                {
                    foreach (XmlNode cmtNode in commentNode.SelectNodes("*", mgr2))
                    {
                            MigratorComment comment = new MigratorComment();

                            MigratorPost p = posts.Find(
                                    delegate(MigratorPost c)
                                    {
                                        return c.LegacyID.ToUpperInvariant() == cmtNode.SelectSingleNode("TargetEntryId").InnerText.ToUpperInvariant();
                                    });

                            if (p != null)
                                comment.PostID = p.PostID;

                            if (cmtNode.SelectSingleNode("Content", mgr2) != null)
                                comment.Body = cmtNode.SelectSingleNode("Content", mgr2).InnerText;
                            
                            comment.PublishedOn = Convert.ToDateTime(cmtNode.SelectSingleNode("Created", mgr2).InnerText);

                            comment.WebSite = cmtNode.SelectSingleNode("AuthorHomepage", mgr2) != null ? cmtNode.SelectSingleNode("AuthorHomepage", mgr2).InnerText : "";
                            comment.UserName = cmtNode.SelectSingleNode("Author", mgr2).InnerText;
                            comment.Email = cmtNode.SelectSingleNode("AuthorEmail", mgr2).InnerText;
                            comment.IsPublished = true;

                            comments.Add(comment);
                    }
                }
            }

            // add the comments to cache
            ZCache.MaxCache("MigratorComments", comments);

            return posts;
        }

        [Serializable]
        public class PermalinkMap : IEquatable<PermalinkMap>
        {
            private string _entryId;
            private string _title;
            public PermalinkMap()
            {
            }

            public PermalinkMap(string from, string to)
            {
                this._entryId = from;
                this._title = to;
            }

            public string Title
            {
                get { return _title; }
                set { _title = value; }
            }

            public string EntryId
            {
                get { return _entryId; }
                set { _entryId = value; }
            }

            #region IEquatable<PermalinkMap> Members

            public bool Equals(PermalinkMap other)
            {
                if (other._entryId == this._entryId)
                    return true;

                return false;
            }

            #endregion
        }


        public List<MigratorComment> GetComments(int postID)
        {
            List<MigratorComment> comments = ZCache.Get<List<MigratorComment>>("MigratorComments");

            if (comments == null)
                throw new Exception("The comment cache has expired. Please restart your import.");

            List<MigratorComment> postComments = comments.FindAll(
                                        delegate(MigratorComment c)
                                        {
                                            return c.PostID == postID;
                                        });

            return postComments;
        }

        #endregion
    }

    #endregion
}