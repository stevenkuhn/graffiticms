using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using DataBuddy;
using System.Data;
using System.IO;

namespace Graffiti.Core
{

    /// <summary>
    /// Helper methods and properties relevant to the current Post object. (commonly $post)
    /// </summary>
    public partial class Post : IMenuItem
    {
        #region Custom Field Support

        //container for custom field values
        private NameValueCollection _nvc = new NameValueCollection();
        private bool _isCustomReady = false;

        /// <summary>
        /// Returns a custom fiels keys and values
        /// </summary>
        /// <returns></returns>
        public NameValueCollection CustomFields()
        {
            if(!_isCustomReady)
                DeserializeCustomFields();
            return _nvc;
        }

        /// <summary>
        /// Called Pre-Update to set the PropertyKeys and PropertyValues properties
        /// </summary>
        internal void SerializeCustomFields()
        {
            StringBuilder sbKey = new StringBuilder();
            StringBuilder sbValue = new StringBuilder();

            int index = 0;
            foreach (string key in _nvc.AllKeys)
            {
                if (key.IndexOf(':') != -1)
                    throw new ArgumentException("Extened Fields Key can not contain the character \":\"");

                string v = _nvc[key];
                if (!string.IsNullOrEmpty(v))
                {
                    sbKey.AppendFormat("{0}:S:{1}:{2}:", key, index, v.Length);
                    sbValue.Append(v);
                    index += v.Length;
                }
            }


            PropertyKeys = sbKey.ToString();
            PropertyValues = sbValue.ToString();
        }

        /// <summary>
        /// Called during Loaded() method to copy values from PropertyKeys and PropertyValues 
        /// to the CustomFields NameValueCollection
        /// </summary>
        internal void DeserializeCustomFields()
        {
            _nvc.Clear();

            if (PropertyKeys != null && PropertyValues != null)
            {
                char[] splitter = new char[1] { ':' };
                string[] keyNames = PropertyKeys.Split(splitter);

                for (int i = 0; i < (keyNames.Length / 4); i++)
                {
                    int start = int.Parse(keyNames[(i * 4) + 2], CultureInfo.InvariantCulture);
                    int len = int.Parse(keyNames[(i * 4) + 3], CultureInfo.InvariantCulture);
                    string key = keyNames[i * 4];

                    if (((keyNames[(i * 4) + 1] == "S") && (start >= 0)) && (len > 0) && (PropertyValues.Length >= (start + len)))
                    {
                        _nvc[key] = PropertyValues.Substring(start, len);
                    }
                }
            }

            _isCustomReady = true;
        }

        /// <summary>
        /// Provides access to the custom fields collection
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return _nvc[key]; }
            set { _nvc[key] = value; }
        }

        /// <summary>
        /// Returns the custom field value for the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <example>
        /// #foreach($post in $posts)
        /// [TAB]The value of my custom field is $post.Custom("MyCustomFieldName")
        /// #end
        /// </example>
        public string Custom(string key)
        {
            return this[key];
        }

    #endregion

        #region DataBuddy, Validation, and Pages


        protected override void Loaded()
        {
            base.Loaded();
            DeserializeCustomFields();
        }

        protected override void BeforeValidate()
        {
            base.BeforeValidate();

            SerializeCustomFields();

            //Make sure no updates are made to a post created via search. We do not store the full
            //post content in the search index, so it is not safe to save.
            if (IsCreatedBySearch)
                throw new Exception("Cannot save a post from the search index");

            //We always need a post name
            Name = Util.CleanForUrl(!string.IsNullOrEmpty(Name) ? Name : Title);

            if (IsNew)
            {
                UniqueId = Guid.NewGuid();
                if (UserName == null)
                    UserName = CreatedBy;
            }

            if (string.IsNullOrEmpty(UserName))
                throw new Exception("Cannot save a post without a username");

            //Check for reserved words
            if (!Util.IsValidFileName(Name))
            {
                throw new Exception("Sorry, you cannot use the reserved word *" + Name + "* for a file name.");
            }

            if (CategoryId == categories.GetUnCategorizedCategory().Id)
            {
                //Check to make sure there is no collision between the post name and a category
                //since uncategrozied posts are accessible via .com/post-name

                Regex regex = new Regex("^(" + Name + ")$", RegexOptions.IgnoreCase);
                foreach (Category category in categories.GetCachedCategories())
                {
                    if (regex.IsMatch(category.LinkName))
                    {
                        throw new Exception("Uncategorized posts cannot use the same name as a category. *" + Name +
                                            "* already exists as a category");
                    }
                }
            }

            if(!string.IsNullOrEmpty(TagList))
            {
                List<string> the_Tags = Util.ConvertStringToList(TagList);
                if(the_Tags.Count > 0)
                {
                    for(int i = 0;i<the_Tags.Count; i++)
                    {
                        the_Tags[i] = Util.CleanForUrl(the_Tags[i]);
                    }

                    TagList = string.Join(",", the_Tags.ToArray());
                }
            }
            
        }


        protected override void AfterCommit()
        {
            base.AfterCommit();

            //Update the number of posts per category
            CategoryController.UpdatePostCounts();
            //PostController.UpdateVersionCount(Id);

            //Save tags per post
            Tag.Destroy(Tag.Columns.PostId, Id);
            if(TagList != null)
            {
                foreach(string t in Util.ConvertStringToList(TagList))
                {
                    Tag tag = new Tag();
                    tag.Name = t.Trim();
                    tag.PostId = Id;
                    tag.Save();
                }
            }

            WritePages();

            ZCache.RemoveByPattern("Posts-");
            ZCache.RemoveCache("Post-" + Id);
        }

        /// <summary>
        /// Writes out the empty pages for the post. Also handles the redirect pages
        /// </summary>
        public void WritePages()
        {
            PageTemplateToolboxContext templateContext = new PageTemplateToolboxContext();
            templateContext.Put("PostId", Id);
            templateContext.Put("CategoryId", CategoryId);
            templateContext.Put("PostName", Name);
            templateContext.Put("Name", Name);
            templateContext.Put("CategoryName", Category.LinkName);
            templateContext.Put("MetaDescription",
                                !string.IsNullOrEmpty(MetaDescription)
                                    ? MetaDescription
                                    : HttpUtility.HtmlEncode(Util.RemoveHtml(PostBody,255) ?? string.Empty));

            templateContext.Put("MetaKeywords",
                                !string.IsNullOrEmpty(MetaKeywords)
                                    ? MetaKeywords
                                    : TagList);
                                   

            string pageName = null;
            
            if(CategoryController.UnCategorizedId != CategoryId)
                     pageName =  categories.GetCachedCategory(CategoryId, false).LinkName + "/";

            pageName = "~/" + pageName + Name + "/" + Util.DEFAULT_PAGE;
            
            PageWriter.Write("post.view", pageName, templateContext);
  
        }

#endregion

        static readonly CategoryController categories = new CategoryController();

        #region Properties

        /// <summary>
        /// User of the current post. This will cause an exception if the UserName value is null
        /// </summary>
        public IGraffitiUser User
        {
            get
            {
                return GraffitiUsers.GetUser(UserName);
            }
        }

        /// <summary>
        /// The proper name of the user of the current post. If the user does not exist, will return "deleted user"
        /// </summary>
        public string UserProperName
        {
            get
            {
                if (!string.IsNullOrEmpty(UserName))
                {
                    IGraffitiUser u = GraffitiUsers.GetUser(UserName);
                    if (u != null)
                        return u.ProperName;
                }
                return "deleted user";
            }
        }

        public string ResolvedImageUrl
        {
            get
            {
                if (_ImageUrl != null)
                {
                    if (_ImageUrl.StartsWith("~/"))
                        return VirtualPathUtility.ToAbsolute(_ImageUrl);

                    return _ImageUrl.Trim();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Flag used to make sure posts returned via search results are not committed to the database
        /// </summary>
        public bool IsCreatedBySearch = false;

        /// <summary>
        /// The current category of the post. 
        /// </summary>
        public Category Category
        {
            get { return categories.GetCachedCategory(CategoryId, false); }
        }

        /// <summary>
        /// Combines both the PostBody and ExtendedBody properties. This should be used when you
        /// wish to show the full post
        /// </summary>
        public string Body
        {
			get 
            { 
                StringBuilder sb = new StringBuilder();
                sb.Append(PostBody);
                sb.Append(ExtendedBody);

                // Allow plugins to append aditional content to the post body
                Events.Instance().ExecuteRenderPostBody(sb, this, PostRenderLocation.Web);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Combines both the PostBody and ExtendedBody properties. This should be used by RSS feeds and
        /// other non-html locations. It will show the full post.
        /// </summary>
        public string RenderBody(PostRenderLocation renderLocation)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(PostBody);
            sb.Append(ExtendedBody);

            // Allow plugins to append aditional content to the post body
            Events.Instance().ExecuteRenderPostBody(sb, this, renderLocation);

            return sb.ToString();
        }

			/// <summary>
			/// Returns the Post excerpt. If a PostBody exists, it will be used.
			/// If not, it returns the first 300 non-HTML characters
			/// </summary>
			public string Excerpt(string startText, string endText, string linkText, int len)
			{
			string link = "";

			if (!String.IsNullOrEmpty(linkText))
				link = string.Format("{0}<a href=\"{1}\">{2}</a>{3}", startText, Url, linkText, endText);

            if (PostBody.Length <= len && String.IsNullOrEmpty(ExtendedBody))
                return "<p>" + Util.RemoveHtml(PostBody, len) + "</p>";
            else
                return "<p>" + Util.RemoveHtml(PostBody, len) + "...</p>" + link;
			}


        /// <summary>
        /// Returns the first len non-HTML chacters of the PostBody property
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public string CustomExcerpt(int len)
        {
            return Util.RemoveHtml(PostBody, len);
        }

        /// <summary>
        /// Helper access to the Status int.
        /// </summary>
        public PostStatus PostStatus
        {
            get { return (PostStatus)Status; }
            set { Status = (int)value; }
        }

        /// <summary>
        /// Returns absolute URL to the post.
        /// </summary>
        public string Url
        {
            get
            {
                return VirtualPathUtility.ToAbsolute(VirtualUrl);
            }
        }

        /// <summary>
        /// Returns absolute URL to the post.
        /// </summary>
        public string RevisionUrl
        {
            get
            {
                return VirtualPathUtility.ToAbsolute(VirtualUrl) + "?revision=" + Version;
            }
        }

        /// <summary>
        /// Returns the virtual URL to the post.
        /// </summary>
        public string VirtualUrl
        {
            get
            {
                if (CategoryController.UnCategorizedId != CategoryId)
                    return "~/" + new CategoryController().GetCachedCategory(CategoryId, false).LinkName +"/" + Name + "/";
                else
                    return "~/" + Name + "/";
            }
        }

        /// <summary>
        /// Returns True if CommentCount > 0. There maybe unapproved comments
        /// </summary>
        public bool HasComments { get { return CommentCount > 0; } }

        /// <summary>
        /// Returns True if this post will allow new comments
        /// </summary>
        public bool EnableNewComments { get { return CommentSettings.Get().EnableCommentOnPost(this); } }

        #endregion

        #region IMenuItem Members

        public List<MenuItem> GetMenuItems()
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            menuItems.Add(new MenuItem("User", "$post.User", "Graffiti User", "Post"));
            menuItems.Add(new MenuItem("Category", "$post.Category", "Graffiti Category", "Post"));
            menuItems.Add(new MenuItem("Body", "$post.Body", "Renders PostBody + ExtendedBody", "Post"));
            menuItems.Add(new MenuItem("Excerpt", "$post.Excerpt", "If ExtendedBody is not null, it renders PostBody. Otherwise, it will render the first 200 non-HTML characters from PostBody", "Post"));
            menuItems.Add(new MenuItem("Url", "$post.Url", "Absolute url to the post", "Post"));
            menuItems.Add(new MenuItem("HasComments", "$post.HasComments", "Returns true if a post has one or more comment", "Post"));
            menuItems.Add(new MenuItem("EnableNewComments", "$post.EnableNewComments", "Returns true if the post allows new comments", "Post"));
            menuItems.Add(new MenuItem("Id", "$post.Id", "The post id", "Post"));
            menuItems.Add(new MenuItem("Title", "$post.Title", "The title of the post", "Post"));
            menuItems.Add(new MenuItem("PostBody", "$post.PostBody", "The main post content - this can be \'lead-in\' or excerpt if ExtendedBody is specified", "Post"));
            menuItems.Add(new MenuItem("Name", "$post.Name", "The value used in the post\'s url", "Post"));
            menuItems.Add(new MenuItem("CommentCount", "$post.CommentCount", "Number of published comments", "Post"));
            menuItems.Add(new MenuItem("TagList", "$post.TagList", "Comma seperated list of tags", "Post"));
            menuItems.Add(new MenuItem("Version", "$post.Version", "Version number of the post", "Post"));
            menuItems.Add(new MenuItem("ExtendedBody", "$post.ExtendedBody", "Optional post content - this is usually displayed when the full post view is requested", "Post"));
            menuItems.Add(new MenuItem("PendingCommentCount", "$post.PendingCommentCount ", "Number of comments which require approval", "Post"));
            menuItems.Add(new MenuItem("Views", "$post.Views", "Number of times the post has been viewed (note: we do try to prevent against refreshes)", "Post"));
            menuItems.Add(new MenuItem("UniqueId", "$post.UniqueId", "Globally unique id for the post (guid)", "Post"));
            menuItems.Add(new MenuItem("EnableComments", "$post.EnableComments", "Are comments enabled for this post", "Post"));
            menuItems.Add(new MenuItem("UserName", "$post.UserName", "Username of the post\'s author", "Post"));

            return menuItems;
        }

        #endregion

        #region Post Helpers

        public static void DeletePostDirectory(Post p)
        {
            string dir = HttpContext.Current.Server.MapPath(p.Url);
            
            if(Directory.Exists(dir))
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch(Exception ex)
                {
                    Log.Error("Post Directory Delete", "The directory for {0} could not be deleted. {1}", p.Title, ex.Message);
                }
            } 
        }

        public static void DestroyDeletedPosts()
        {
            DateTime dt = DateTime.Now.AddDays(-1*SiteSettings.DestroyDeletedPostsOlderThanDays);
            Query q = CreateQuery();
            q.AndWhere(Columns.IsDeleted, true);
            q.AndWhere(Columns.ModifiedOn,dt,Comparison.LessOrEquals);

            PostCollection pc = PostCollection.FetchByQuery(q);
            if(pc.Count > 0)
            {
                Log.Info("Deleting Posts", "Deleting {0} post(s) since they were deleted before {1}", pc.Count,dt);

                foreach(Post p in pc)
                {
                    try
                    {
                        DestroyDeletedPost(p.Id);
                        Log.Info("Post Deleted", "The post \"{0}\" ({3}) and related content was deleted. It had been marked to be deleted on {1} by {2}",p.Title,p.ModifiedOn,p.ModifiedBy, p.Id);
                    }
                    catch(Exception ex)
                    {
                        Log.Error("Post Delete", "The post \"{0}\" was not successfully deleted. Reason: {1}", p.Title,ex.Message);
                    }
                }
            }
        }

        public static void DestroyDeletedPost(int postid)
        {
            Post p = new Post(postid);

            // Check if post is featured in it's category before deletion
            Core.Category c = p.Category;
            if (p.Id == c.FeaturedId)
            {
                c.FeaturedId = 0;
                c.Save();
            }

            // Check site featured post
            SiteSettings settings = SiteSettings.Get();
            if (p.Id == settings.FeaturedId)
            {
                settings.FeaturedId = 0;
                settings.Save();
            }


            PostStatistic.Destroy(PostStatistic.Columns.PostId, postid);
            Tag.Destroy(Tag.Columns.PostId, postid);
            Comment.Destroy(Comment.Columns.PostId, postid);
            DataService.ExecuteNonQuery(new QueryCommand("delete from graffiti_VersionStore where Type = 'post/xml' and ItemId = " + postid));

            Post.Destroy(postid);

            DeletePostDirectory(p);
        }

        public static void DestroyDeletedPostCascadingForCategory(int categoryId)
        {
            Query q = CreateQuery();
            q.AndWhere(Columns.IsDeleted, true);
            q.AndWhere(Columns.CategoryId, categoryId);

            PostCollection pc = PostCollection.FetchByQuery(q);
            if (pc.Count > 0)
            {
                foreach (Post p in pc)
                {
                    DestroyDeletedPost(p.Id);
                }
            }
        }

        public static Post GetCachedPost(int id)
        {
            Post post = ZCache.Get<Post>("Post-" + id);
            if (post == null)
            {
                post = new Post(id);
                ZCache.InsertCache("Post-" + id, post, 10);
            }

            return post;
        }

		public static int GetPostIdByName(string name)
		{
            object id = ZCache.Get<object>("PostIdByName-" + name);
			if (id == null)
			{
				string postName;
				string categoryName = null;

				if (name.Contains("/"))
				{
					string[] parts = name.Split('/');

					for (int i = 0; i < parts.Length; i++)
						parts[i] = Util.CleanForUrl(parts[i]);

					switch(parts.Length)
					{
						case 2:
							categoryName = parts[0];
							postName = parts[1];
							break;
						case 3:
							categoryName = parts[0] + "/" + parts[1];
							postName = parts[2];
							break;
						default:
							return -1;
					}
				}
				else
					postName = Util.CleanForUrl(name);

				int categoryId = -1;
				if (categoryName != null)
				{
                    CategoryCollection the_categories = categories.GetCachedCategories();

                    foreach (Category category in the_categories)
						if (category.LinkName == categoryName)
							categoryId = category.Id;

					if (categoryId == -1)
						return -1;
				}

				List<Parameter> parameters = Post.GenerateParameters();

				/* this is supposed to be TOP 1, but the ExecuteScalar will pull only the first one */
				QueryCommand cmd = new QueryCommand("Select Id FROM graffiti_Posts Where Name = " + DataService.Provider.SqlVariable("Name") + " and IsDeleted = 0");
				cmd.Parameters.Add(Post.FindParameter(parameters, "Name")).Value = postName;

				if (categoryId > -1)
				{
					cmd.Sql += " and CategoryId = " + DataService.Provider.SqlVariable("CategoryId");
					cmd.Parameters.Add(Post.FindParameter(parameters, "CategoryId")).Value = categoryId;
				}

				cmd.Sql += " order by CategoryId asc";

				object postobj = DataService.ExecuteScalar(cmd);
				if (postobj != null)
				{
					id = postobj;
					ZCache.InsertCache("PostIdByName-" + name, (int)id, 60);
				}
				else
					id = -1;
			}
			return (int) id;
		}

        public static List<PostCount> GetPostCounts(int catID, string user)
        {
            List<PostCount> postCounts = new List<PostCount>();
            List<PostCount> final = new List<PostCount>();

			List<Parameter> parameters = Post.GenerateParameters();
            QueryCommand cmd = new QueryCommand("Select Status, CategoryId, " + DataService.Provider.SqlCountFunction("Id") + " as StatusCount FROM graffiti_Posts Where IsDeleted = 0");
            
            if(catID > 0)
            {
                cmd.Sql += " and CategoryId = " + DataService.Provider.SqlVariable("CategoryId");
                cmd.Parameters.Add(Post.FindParameter(parameters, "CategoryId")).Value = catID;
            }
                
            if(!String.IsNullOrEmpty(user))
            {
                cmd.Sql += " and CreatedBy = " + DataService.Provider.SqlVariable("CreatedBy");
				cmd.Parameters.Add(Post.FindParameter(parameters, "CreatedBy")).Value = user;
            }

            cmd.Sql += " group by Status, CategoryId";

            using (IDataReader reader = DataService.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    PostCount postCount = new PostCount();
                    postCount.PostStatus = (PostStatus)Int32.Parse(reader["Status"].ToString());
					postCount.Count = Int32.Parse(reader["StatusCount"].ToString());
                    postCount.CategoryId = Int32.Parse(reader["CategoryId"].ToString());

                    postCounts.Add(postCount);
                }

                reader.Close();
            }

            List<PostCount> filteredPermissions = new List<PostCount>();
            filteredPermissions.AddRange(postCounts);

            foreach (PostCount ac in postCounts)
            {
                if (!RolePermissionManager.GetPermissions(ac.CategoryId, GraffitiUsers.Current).Read)
                    filteredPermissions.Remove(ac);
            }

            foreach (PostCount ac in filteredPermissions)
            {
                PostCount existing = final.Find(
                                                delegate(PostCount postcount)
                                                {
                                                    return postcount.PostStatus == ac.PostStatus;
                                                });

                if (existing == null)
                {
                    final.Add(ac);
                }
                else
                {
                    existing.Count += ac.Count;
                }
            }

            return final;
        }

        public static List<CategoryCount> GetCategoryCountForStatus(PostStatus status, string authorID)
        {
            List<CategoryCount> catCounts = new List<CategoryCount>();
            List<CategoryCount> final = new List<CategoryCount>();

			DataProvider dp = DataService.Provider;
            QueryCommand cmd = new QueryCommand(String.Empty);
            
            if (String.IsNullOrEmpty(authorID))
            {
                cmd.Sql = @"select c.Id, " + dp.SqlCountFunction("c.Name") + @" as IdCount, p.CategoryId from graffiti_Posts AS p
                inner join graffiti_Categories AS c on p.CategoryId = c.Id
                where p.Status = " + dp.SqlVariable("Status") + @" and p.IsDeleted = 0
                group by c.Id, p.CategoryId";
            }
            else
            {
                cmd.Sql = @"select c.Id, " + dp.SqlCountFunction("c.Name") + @" as IdCount, p.CategoryId from ((graffiti_Posts AS p
                inner join graffiti_Categories AS c on p.CategoryId = c.Id)
                inner join graffiti_Users AS u on p.CreatedBy = u.Name)
                where p.Status = " + dp.SqlVariable("Status") + @" and p.IsDeleted = 0 and u.Id = " + dp.SqlVariable("AuthorId") +
                @" group by c.Id, p.CategoryId";
            }

            cmd.Parameters.Add(Post.FindParameter("Status")).Value = (int)status;

            if (!String.IsNullOrEmpty(authorID))
            {
                cmd.Parameters.Add("AuthorId", Convert.ToInt32(authorID), Graffiti.Core.User.FindParameter("Id").DbType);
            }

            using (IDataReader reader = DataService.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    CategoryCount catCount = new CategoryCount();
                    catCount.ID = Int32.Parse(reader["Id"].ToString());
                    catCount.Count = Int32.Parse(reader["IdCount"].ToString());
                    catCount.CategoryId = Int32.Parse(reader["CategoryId"].ToString());

                    catCounts.Add(catCount);
                }

                reader.Close();
            }

            // populate the category name
            CategoryCollection cats = new CategoryController().GetAllCachedCategories();

            List<CategoryCount> tempParentList = new List<CategoryCount>();

            foreach (CategoryCount cc in catCounts)
            {
                Category temp = cats.Find(
                                 delegate(Category c)
                                 {
                                     return c.Id == cc.ID;
                                 });

                if (temp != null)
                {
                    cc.Name = temp.Name;
                    cc.ParentId = temp.ParentId;
                }

                if (cc.Count > 0 && cc.ParentId >= 1)
                {
                    // if it's not already in the list, add it
                    CategoryCount parent = catCounts.Find(
                                                delegate(CategoryCount cac)
                                                {
                                                    return cac.ID == cc.ParentId;
                                                });

                    if (parent == null)
                    {
                        parent = tempParentList.Find(
                                                    delegate(CategoryCount cac)
                                                    {
                                                        return cac.ID == cc.ParentId;
                                                    });

                        if (parent == null)
                        {
                            Category tempParent = cats.Find(
                                                    delegate(Category cttemp)
                                                    {
                                                        return cttemp.Id == cc.ParentId;
                                                    });

                            parent = new CategoryCount();
                            parent.ID = tempParent.Id;
                            parent.ParentId = tempParent.ParentId;
                            parent.Name = tempParent.Name;
                            parent.Count = 0;
                            
                            tempParentList.Add(parent);
                        }
                    }
                }
            }

            catCounts.AddRange(tempParentList);

            List<CategoryCount> filteredPermissions = new List<CategoryCount>();
            filteredPermissions.AddRange(catCounts);

            foreach (CategoryCount ac in catCounts)
            {
                if (!RolePermissionManager.GetPermissions(ac.CategoryId, GraffitiUsers.Current).Read)
                    filteredPermissions.Remove(ac);
            }

            foreach (CategoryCount ac in filteredPermissions)
            {
                CategoryCount existing = final.Find(
                                                delegate(CategoryCount catcount)
                                                {
                                                    return catcount.ID == ac.ID;
                                                });

                if (existing == null)
                {
                    final.Add(ac);
                }
                else
                {
                    existing.Count += ac.Count;
                }
            }

            return final;
        }

        public static List<AuthorCount> GetAuthorCountForStatus(PostStatus status, string categoryID)
        {
            List<AuthorCount> autCounts = new List<AuthorCount>();
            List<AuthorCount> final = new List<AuthorCount>();

            QueryCommand cmd = new QueryCommand(
                    @"select u.Id, " + DataService.Provider.SqlCountFunction("u.Id") + @" as IdCount, u.ProperName, p.CategoryId from graffiti_Posts AS p
                    inner join graffiti_Users as u on p.CreatedBy = u.Name
                    where p.Status = " + DataService.Provider.SqlVariable("Status") + @" and p.IsDeleted = 0");
                    
            if(!String.IsNullOrEmpty(categoryID))
            {
                cmd.Sql += " and p.CategoryId = " + DataService.Provider.SqlVariable("CategoryId");
            }

            cmd.Sql += " group by u.Id, u.ProperName, p.CategoryId";  

			List<Parameter> parameters = Post.GenerateParameters();
			cmd.Parameters.Add(Post.FindParameter(parameters, "Status")).Value = (int)status;
            
            if (!String.IsNullOrEmpty(categoryID))
            {
				cmd.Parameters.Add(Post.FindParameter(parameters, "CategoryId")).Value = Convert.ToInt32(categoryID);
            }

            using (IDataReader reader = DataService.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    AuthorCount autCount = new AuthorCount();
                    autCount.ID = Int32.Parse(reader["Id"].ToString());
                    autCount.Count = Int32.Parse(reader["IdCount"].ToString());
					autCount.Name = reader["ProperName"].ToString();
                    autCount.CategoryId = Int32.Parse(reader["CategoryId"].ToString());

                    autCounts.Add(autCount);
                }

                List<AuthorCount> filteredPermissions = new List<AuthorCount>();
                filteredPermissions.AddRange(autCounts);

                foreach (AuthorCount ac in autCounts)
                {
                    if (!RolePermissionManager.GetPermissions(ac.CategoryId, GraffitiUsers.Current).Read)
                        filteredPermissions.Remove(ac);
                }

                foreach (AuthorCount ac in filteredPermissions)
                {
                    AuthorCount existing = final.Find(
                                                    delegate(AuthorCount authcount)
                                                    {
                                                        return authcount.Name == ac.Name;
                                                    });

                    if (existing == null)
                    {
                        final.Add(ac);
                    }
                    else
                    {
                        existing.Count += ac.Count;
                    }
                }

                reader.Close();
            }

            return final;
        }

        public static Post FromXML(string xml)
        {
            Post the_Post = ObjectManager.ConvertToObject<Post>(xml);
            the_Post.Loaded();
            the_Post.ResetStatus();

            return the_Post;
        }

        public string ToXML()
        {
            SerializeCustomFields();
            return ObjectManager.ConvertToString(this);
        }

        public static void UpdatePostStatus(int id, PostStatus status)
        {
            //UpdateVersionCount(id);

            QueryCommand command = new QueryCommand("Update graffiti_Posts Set Status = " + DataService.Provider.SqlVariable("Status") + " Where Id = " + DataService.Provider.SqlVariable("Id"));
            List<Parameter> parameters = Post.GenerateParameters();
			command.Parameters.Add(Post.FindParameter(parameters, "Status")).Value = (int)status;
            command.Parameters.Add(Post.FindParameter(parameters, "Id")).Value = id;

            DataService.ExecuteNonQuery(command);

            ZCache.RemoveByPattern("Posts-");
            ZCache.RemoveCache("Post-" + id);
        }

        /// <summary>
        /// Returns a collection of posts for a given tag. This is a special call
        /// since tagName is not available on the post query
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static PostCollection FetchPostsByTag(string tagName)
        {
            QueryCommand command = new QueryCommand("SELECT p.* FROM graffiti_Posts AS p INNER JOIN graffiti_Tags AS t ON p.Id = t.PostId WHERE p.IsPublished <> 0 and p.IsDeleted = 0 and p.Published <= " + DataService.Provider.SqlVariable("Published") + " and t.Name = " + DataService.Provider.SqlVariable("Name") + " ORDER BY p.Published DESC");
            command.Parameters.Add(Post.FindParameter("Published")).Value = SiteSettings.CurrentUserTime;
            command.Parameters.Add(Tag.FindParameter("Name")).Value = tagName;
            PostCollection pc = new PostCollection();
            pc.LoadAndCloseReader(DataService.ExecuteReader(command));
            return pc;
        }

        public static PostCollection FetchPostsByTagAndCategory(string tagName, int categoryId)
        {
            QueryCommand command = new QueryCommand("SELECT p.* FROM graffiti_Posts AS p INNER JOIN graffiti_Tags AS t ON p.Id = t.PostId WHERE p.CategoryId = " + categoryId.ToString() + " and p.IsPublished <> 0 and p.IsDeleted = 0 and p.Published <= " + DataService.Provider.SqlVariable("Published") + " and t.Name = " + DataService.Provider.SqlVariable("Name") + " ORDER BY p.Published DESC");
            command.Parameters.Add(Post.FindParameter("Published")).Value = SiteSettings.CurrentUserTime;
            command.Parameters.Add(Tag.FindParameter("Name")).Value = tagName;
            PostCollection pc = new PostCollection();
            pc.LoadAndCloseReader(DataService.ExecuteReader(command));
            return pc;
        }

        public static void UpdateViewCount(int postid)
        {
            QueryCommand command = new QueryCommand("UPDATE graffiti_Posts Set Views = Views + 1 WHERE Id = " + DataService.Provider.SqlVariable("Id"));
			command.Parameters.Add(Post.FindParameter("Id")).Value = postid;
            DataService.ExecuteNonQuery(command);

            PostStatistic ps = new PostStatistic();
            ps.PostId = postid;
            ps.DateViewed = DateTime.Now;

            ps.Save();
        }

        public static void UpdateCommentCount(int postid)
        {
            QueryCommand command = null;
			DataProvider dp = DataService.Provider;
			
            if (Util.IsAccess)
            {
                Query q1 = Comment.CreateQuery();
                q1.AndWhere(Comment.Columns.PostId, postid);
                q1.AndWhere(Comment.Columns.IsPublished, true);
                q1.AndWhere(Comment.Columns.IsDeleted,false);

                int Comment_Count = q1.GetRecordCount();

                Query q2 = Comment.CreateQuery();
                q2.AndWhere(Comment.Columns.PostId, postid);
                q2.AndWhere(Comment.Columns.IsPublished, false);
                q2.AndWhere(Comment.Columns.IsDeleted, false);

                int Pending_Comment_Count = q2.GetRecordCount();

                command = new QueryCommand("UPDATE graffiti_Posts Set Comment_Count = "
                    + dp.SqlVariable("Comment_Count")
                    + ", Pending_Comment_Count = " + dp.SqlVariable("Pending_Comment_Count")
                    + " WHERE Id = " + dp.SqlVariable("Id"));
                List<Parameter> parameters = Post.GenerateParameters();
				command.Parameters.Add(Post.FindParameter(parameters, "Comment_Count")).Value = Comment_Count;
				command.Parameters.Add(Post.FindParameter(parameters, "Pending_Comment_Count")).Value = Pending_Comment_Count;
				command.Parameters.Add(Post.FindParameter(parameters, "Id")).Value = postid;
            }
            else
            {
                string sql =
                    @"Update graffiti_Posts
                    Set
	                    Comment_Count = (Select " + dp.SqlCountFunction() + @" FROM graffiti_Comments AS c where c.PostId = " + dp.SqlVariable("Id") + @" and c.IsPublished = 1 and c.IsDeleted = 0),
	                    Pending_Comment_Count = (Select " + dp.SqlCountFunction() + @" FROM graffiti_Comments AS c where c.PostId = " + dp.SqlVariable("Id") + @" and c.IsPublished = 0 and c.IsDeleted = 0)
                   Where Id = " + dp.SqlVariable("Id");

                command = new QueryCommand(sql);
                command.Parameters.Add(Post.FindParameter("Id")).Value = postid;
            }

            DataService.ExecuteNonQuery(command);
        }

        #endregion
    }

    public partial class PostCollection
    {
        public static Query DefaultQuery()
        {
            return DefaultQuery(SortOrderType.Descending);
        }

        /// <summary>
        /// Returns a query which applies all of the common filters
        /// </summary>
        /// <returns></returns>
        public static Query DefaultQuery(SortOrderType sot)
        {
            Query q = Post.CreateQuery();
            q.AndWhere(Post.Columns.IsPublished, true);
            q.AndWhere(Post.Columns.IsDeleted, false);

            if(SiteSettings.Get().FilterUncategorizedPostsFromLists)
                q.AndWhere(Post.Columns.CategoryId, CategoryController.UnCategorizedId, Comparison.NotEquals);
            
            q.AndWhere(Post.Columns.Published, SiteSettings.CurrentUserTime, Comparison.LessOrEquals);
            
            switch(sot)
            {
                case SortOrderType.Ascending:
                    q.OrderByAsc(Post.Columns.Published);
                    break;

                case SortOrderType.Views:
                    q.OrderByDesc(Post.Columns.Views);
                    break;

                case SortOrderType.Custom:
                    q.OrderByAsc(Post.Columns.SortOrder);
                    break;

                case SortOrderType.Alphabetical:
                    q.OrderByAsc(Post.Columns.Title);
                    break;

                default:
                    q.OrderByDesc(Post.Columns.Published);
                    break;
            }

            
            return q;
        }

        public static Query HomeQueryOverride(int pageIndex, int pageSize)
        {
            Query q = Post.CreateQuery();
            q.AndWhere(Post.Columns.IsPublished, true);
            q.AndWhere(Post.Columns.IsDeleted, false);
            q.AndWhere(Post.Columns.IsHome, true);
            q.AndWhere(Post.Columns.Published, SiteSettings.CurrentUserTime, Comparison.LessOrEquals);

            q.PageSize = pageSize;
            q.PageIndex = pageIndex;

            q.OrderByAsc(Post.Columns.HomeSortOrder);

            return q;
        }

        /// <summary>
        /// Returns a query which applies all of the common filters and enables paging.
        /// </summary>
        /// <returns></returns>
        public static Query DefaultQuery(int pageIndex, int pageSize, SortOrderType sot)
        {
            Query q = DefaultQuery(sot);
            q.PageIndex = pageIndex;
            q.PageSize = pageSize;
            return q;
            
        }
    }

    

    /// <summary>
    /// Defines the state of a post.
    /// </summary>
    public enum PostStatus
    {
        NotSet = 0,
        Publish = 1,
        Draft = 2,
        PendingApproval = 3,
        RequiresChanges = 4
    }
}