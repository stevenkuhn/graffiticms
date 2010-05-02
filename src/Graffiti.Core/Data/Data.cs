using System;
using System.Collections;
using System.Collections.Generic;
using DataBuddy;

namespace Graffiti.Core
{
    /// <summary>
    /// Data is a built-in Chalk extension, similar to Macros, which enables quick and easy access to additional Graffiti data. ($data)
    /// </summary>
    [Chalk("data")]
    public class Data
    {
        /// <summary>
        /// Gets the Post for the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Post GetPost(int id)
        {
            return Post.GetCachedPost(id);
        }

        /// <summary>
        /// Gets the Post for the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public Post GetPost(string name)
		{
			if (string.IsNullOrEmpty(name))
				return null;
			if (name.StartsWith("/"))
				name = name.Substring(1);
			if (name.EndsWith("/"))
				name = name.Substring(0, name.Length - 1);
			if (string.IsNullOrEmpty(name))
				return null;

			int postId = Post.GetPostIdByName(name);

			if (postId == -1)
				return null;
			return Post.GetCachedPost(postId);
		}

        /// <summary>
        /// Gets the sites featured post
        /// </summary>
        /// <returns></returns>
    	public Post Featured()
        {
            SiteSettings settings = SiteSettings.Get();
            if(settings.FeaturedId > 0)
            {
                Post p = Post.GetCachedPost(settings.FeaturedId);

                if (p.IsPublished)
                    return p;
            }

            return null;
        }

        /// <summary>
        /// Gets the sites featured post for the specified category Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public Post Featured(int categoryId)
        {
            return Data.Featured(new CategoryController().GetCachedCategory(categoryId, true));
        }

        /// <summary>
        /// Gets the sites featured post for the specified category name
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public Post Featured(string categoryName)
        {
            return Data.Featured(GetCategory(categoryName));
        }

        /// <summary>
        /// Gets the sites featured post for the specifed Category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private static Post Featured(Category category)
        {
            if (category != null)
            {
                if (category.FeaturedId > 0)
                {
                    Post p = Post.GetCachedPost(category.FeaturedId);

                    if (p.IsPublished)
                        return p;
                }
            }

            return null;
        }

		 /// <summary>
		 /// Gets all posts by the specified user in the specified category name
		 /// </summary>
		 /// <param name="username"></param>
		 /// <param name="categoryId"></param>
		 /// <param name="numberOfPosts"></param>
		  public PostCollection PostsByUserAndCategory(string username, int categoryId, int numberOfPosts)
		  {
			  Category category = GetCategory(categoryId);
			  IGraffitiUser user = GraffitiUsers.GetUser(username);
			  return PostsByUserAndCategory(user, category, numberOfPosts);
		  }

		 /// <summary>
		 /// Gets all posts by the specified user in the specified category name
		 /// </summary>
		 /// <param name="username"></param>
		 /// <param name="categoryName"></param>
		 /// <param name="numberOfPosts"></param>
		  public PostCollection PostsByUserAndCategory(string username, string categoryName, int numberOfPosts)
		  {
			  Category category = GetCategory(categoryName);
			  IGraffitiUser user = GraffitiUsers.GetUser(username);
			  return PostsByUserAndCategory(user, category, numberOfPosts);
		  }

			/// <summary>
		   /// Gets all posts by the specified user in the specified category name
			/// </summary>
			/// <param name="user"></param>
			/// <param name="category"></param>
			/// <param name="numberOfPosts"></param>
		  public PostCollection PostsByUserAndCategory(IGraffitiUser user, Category category, int numberOfPosts)
		  {
			  if (category == null || user == null)
				  return null;

			  const string CacheKey = "Posts-Users-Categories-P:{0}-U:{1}-C:{2}-T:{3}-PS:{4}";

			  PostCollection pc = ZCache.Get<PostCollection>(string.Format(CacheKey, 1, user.UniqueId, category.Id, category.SortOrder, numberOfPosts));
			  if (pc == null)
			  {
				  pc = new PostCollection();
				  Query q = PostCollection.DefaultQuery(1, numberOfPosts, category.SortOrder);
				  q.AndWhere(Post.Columns.UserName, user.Name);
				  if (Category.IncludeChildPosts)
				  {
					  if (category.ParentId > 0)
						  q.AndWhere(Post.Columns.CategoryId, category.Id);
					  else
					  {
						  List<int> ids = new List<int>(category.Children.Count + 1);
						  foreach (Category child in category.Children)
							  ids.Add(child.Id);
						  ids.Add(category.Id);
						  q.AndInWhere(Post.Columns.CategoryId, ids.ToArray());
					  }
				  }
				  else
				  {
					  q.AndWhere(Post.Columns.CategoryId, category.Id);
				  }
				  pc.LoadAndCloseReader(q.ExecuteReader());
				  ZCache.InsertCache(string.Format(CacheKey, 1, user.UniqueId, category.Id, category.SortOrder, numberOfPosts), pc, 60);
			  }

			  return pc;
		  }

        /// <summary>
        /// Gets all Posts by the specified tag
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public PostCollection PostsByTag(string tagName)
        {
            string TagName = Util.CleanForUrl(tagName);
            PostCollection pc = ZCache.Get<PostCollection>("Tags-" + TagName);
            if (pc == null)
            {
                pc = Post.FetchPostsByTag(TagName);
                ZCache.InsertCache("Tags-" + TagName, pc, 60);
            }
            
            return pc;
        }

        /// <summary>
        /// Gets all posts by the specified tag in the specified category name
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public PostCollection PostsByTagAndCategory(string tagName, string category)
        {
            Category cat = GetCategory(category);
            return PostsByTagAndCategory(tagName, cat, Int32.MaxValue);
        }

        /// <summary>
        /// Gets all posts by the specified tag with the specified category name with a max number of results
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="category"></param>
        /// <param name="numberOfPosts"></param>
        /// <returns></returns>
        public PostCollection PostsByTagAndCategory(string tagName, string category, int numberOfPosts)
        {
            Category cat = GetCategory(category);
            return PostsByTagAndCategory(tagName, cat, numberOfPosts);
        }

        /// <summary>
        /// Gets all posts by the specified tag in the specified Category
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public PostCollection PostsByTagAndCategory(string tagName, Category category)
        {
            return PostsByTagAndCategory(tagName, category, Int32.MaxValue);
        }

        /// <summary>
        /// Gets all posts by the specified tag in the specified category with a max number of results
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="category"></param>
        /// <param name="numberOfPosts"></param>
        /// <returns></returns>
        public PostCollection PostsByTagAndCategory(string tagName, Category category, int numberOfPosts)
        {
			if (category == null)
				return null;

            if (String.IsNullOrEmpty(tagName))
                return PostsByCategory(category, numberOfPosts);
            else
            {
                PostCollection temp = Post.FetchPostsByTagAndCategory(tagName, category.Id);

                PostCollection pc = new PostCollection();

                int tempCount = 0;
                foreach (Post p in temp)
                {
                    tempCount++;
                    if (tempCount > numberOfPosts)
                        break;

                    pc.Add(p);
                }

                return pc;
            }
        }

        /// <summary>
        /// Gets the last x amount of posts from the specified category name
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="numberOfPosts"></param>
        /// <returns></returns>
        public PostCollection PostsByCategory(string categoryName, int numberOfPosts)
        {
            Category category = GetCategory(categoryName);
            return category == null ? null : PostsByCategory(category, numberOfPosts);
        }

			/// <summary>
			/// Gets the last x amount of posts from the specified category
			/// </summary>
			/// <param name="categoryId"></param>
			/// <param name="numberOfPosts"></param>
			public PostCollection PostsByCategory(int categoryId, int numberOfPosts)
			{
				Category category = GetCategory(categoryId);
				return category == null ? null : PostsByCategory(category, numberOfPosts);
			}

        /// <summary>
        /// Gets the last x amount of posts from the specified Category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="numberOfPosts"></param>
        public PostCollection PostsByCategory(Category category, int numberOfPosts)
        {
            return PostsByCategory(category, numberOfPosts, false);
        }

        public PostCollection PostsByCategory(Category category, int numberOfPosts, bool filterHome)
        {
			if (category == null)
				return null;

            const string CacheKey = "Posts-Categories-P:{0}-C:{1}-T:{2}-PS:{3}";

            PostCollection pc = ZCache.Get<PostCollection>(string.Format(CacheKey, 1, category.Id, category.SortOrder, numberOfPosts));
            if (pc == null)
            {
                pc = new PostCollection();
                Query q = PostCollection.DefaultQuery(1, numberOfPosts, category.SortOrder);
                
                if (Category.IncludeChildPosts)
                {
                    if (category.ParentId > 0)
                        q.AndWhere(Post.Columns.CategoryId, category.Id);
                    else
                    {
                        List<int> ids = new List<int>(category.Children.Count + 1);
                        foreach (Category child in category.Children)
                            ids.Add(child.Id);

                        ids.Add(category.Id);

                        q.AndInWhere(Post.Columns.CategoryId, ids.ToArray());
                    }
                }
                else
                {
                    q.AndWhere(Post.Columns.CategoryId, category.Id);
                }

                if (filterHome)
                {
                    string where = GraffitiContext.Current["where"] as string;
                    if (!string.IsNullOrEmpty(where) && where == "home" && Site.UseCustomHomeList)
                        q.AndWhere(Post.Columns.IsHome, true);
                }

                pc.LoadAndCloseReader(q.ExecuteReader());
                ZCache.InsertCache(string.Format(CacheKey, 1, category.Id, category.SortOrder, numberOfPosts), pc, 60);
            }

            return pc;
        }

        public IEnumerable Query(IDictionary paramaters)
        {
            string type = paramaters["type"] as string;
            if (type != null)
                paramaters.Remove("type");
            else
                type = "post";

            switch(type)
            {
                case "post":

                    Query postQuery = Post.CreateQuery();
                    SetLimits(postQuery,paramaters);

                    string categoryName = paramaters["category"] as string;
                    if (categoryName != null)
                        paramaters.Remove("category");
                        
                        if(categoryName == "none")
                            categoryName = CategoryController.UncategorizedName;

                    if (categoryName != null)
                        paramaters["categoryid"] = new CategoryController().GetCachedCategory(categoryName, false).Id;

                    if(paramaters["isDeleted"] == null)
                        paramaters["isDeleted"] = false;

                    if (paramaters["isPublished"] == null)
                    {
                        paramaters["isPublished"] = true;
                    }

                    string orderBy = paramaters["orderby"] as string;
                    if (orderBy != null)
                        paramaters.Remove("orderBy");
                    else
                        orderBy = "Published DESC";
                    
                    postQuery.Orders.Add(orderBy);

                    string cacheKey = "Posts-";
                    foreach (string key in paramaters.Keys)
                    {
                        Column col = GetPostColumn(key);
                        postQuery.AndWhere(col, paramaters[key]);
                        cacheKey += "|" + col.Name + "|" + paramaters[key];
                    }

                    PostCollection pc = ZCache.Get<PostCollection>(cacheKey);
                    if (pc == null)
                    {
                        pc = new PostCollection();
                        pc.LoadAndCloseReader(postQuery.ExecuteReader());
                        ZCache.InsertCache(cacheKey, pc, 90);
                    }
                    return pc;
     
                case "comment":

                    break;

                case "category":

                    break;
            }

            return null;
        }

        private static Column GetPostColumn(string key)
        {
            foreach(Column c in Post.Table.Columns)
            {
                if (Util.AreEqualIgnoreCase(c.Name, key))
                    return c;
            }

            throw new Exception("Post Column " + key + " was not found");
        }

        private static void SetLimits(Query q, IDictionary paramaters)
        {
            if(paramaters["top"] != null)
            {
                q.Top = paramaters["top"] as string;
                paramaters.Remove("top");
            }
            else if(paramaters["pageIndex"] != null)
            {
                q.PageIndex = Convert.ToInt32(paramaters["pageIndex"]);
                paramaters.Remove("pageIndex");

                if (paramaters["pageSize"] != null)
                {
                    q.PageSize = Convert.ToInt32(paramaters["pageSize"]);
                    paramaters.Remove("pageSize");

                }
                else
                    q.PageSize = 10;
            }
        }

        /// <summary>
        /// Gets all of the sites Categories
        /// </summary>
        /// <returns></returns>
        public CategoryCollection Categories()
        {
            return new CategoryController().GetTopLevelCachedCategories();
        }

        /// <summary>
        /// Gets a Category by the specified Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public Category GetCategory(int categoryId)
        {
            return new CategoryController().GetCachedCategory(categoryId,true);
        }

        /// <summary>
        /// Gets a Category by the specified name
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public Category GetCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                return null;

            if (categoryName.StartsWith("/"))
                categoryName = categoryName.Substring(1);


            if (categoryName.EndsWith("/"))
                categoryName = categoryName.Substring(0, categoryName.Length - 1);

            if (string.IsNullOrEmpty(categoryName))
                return null;

            string[] parts = categoryName.Split(new char[] { '/' });
            Category category = null;
            Category parent_Category = new CategoryController().GetCachedCategory(parts[0], true);

            if (parent_Category == null)
                return null;


            if (parts.Length > 1)
            {
                if (parent_Category.HasChildren)
                {
                    foreach (Category child_Category in parent_Category.Children)
                    {
                        if (Util.AreEqualIgnoreCase(child_Category.Name, parts[1]))
                        {
                            category = child_Category;
                            break;
                        }
                    }
                }
            }
            else
                category = parent_Category;

            return category;
        }

        /// <summary>
        /// Gets the last x amount specified of recent comments
        /// </summary>
        /// <param name="numberOfComments"></param>
        /// <returns></returns>
        public CommentCollection RecentComments(int numberOfComments)
        {
            return RecentComments(numberOfComments, -1);
        }

        /// <summary>
        /// Gets the last x amount of comments from the specified category name
        /// </summary>
        /// <param name="numberOfComments"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public CommentCollection RecentComments(int numberOfComments, string categoryName)
        {
            Category category = GetCategory(categoryName);
            return category == null ? null : RecentComments(numberOfComments, category.Id); 
        }

        /// <summary>
        /// Gets the last x amount of comments from the specified category Id
        /// </summary>
        /// <param name="numberOfComments"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public CommentCollection RecentComments(int numberOfComments, int categoryId)
        {
            CommentCollection cc = ZCache.Get<CommentCollection>("Comments-Recent-" + numberOfComments + "c:" + categoryId);
            if (cc == null)
            {
                cc = new CommentCollection();
                Query q = Comment.CreateQuery();
                q.AndWhere(Comment.Columns.IsDeleted, false);
                q.AndWhere(Comment.Columns.IsPublished, true);
                if (categoryId > 0)
                {
                    Category category = new CategoryController().GetCachedCategory(categoryId, true);
                    if (category != null)
                    {
                        if (category.ParentId > 0)
                            q.AndWhere(Post.Columns.CategoryId, categoryId);
                        else
                        {
                            List<int> ids = new List<int>(category.Children.Count + 1);
                            foreach (Category child in category.Children)
                                ids.Add(child.Id);

                            ids.Add(category.Id);

                            q.AndInWhere(Post.Columns.CategoryId, ids.ToArray());
                        }
                    }
                    else
                    {
                        //this should result in no data, but it will signal to 
                        //the end user to edit/remove this widget
                        q.AndWhere(Post.Columns.CategoryId, categoryId);
                    }
                }
                q.Top = numberOfComments.ToString();
                q.OrderByDesc(Comment.Columns.Id);

                cc.LoadAndCloseReader(q.ExecuteReader());

                ZCache.InsertCache("Comments-Recent-" + numberOfComments + "c:" + categoryId, cc, 60);
            }

            return cc;
        }

        /// <summary>
        /// Gets the post feedback from the specified postId
        /// </summary>
        /// <param name="PostId"></param>
        /// <returns></returns>
        public CommentCollection PostFeedback(int PostId)
        {
            CommentCollection cc = ZCache.Get<CommentCollection>("Feedback-" + PostId);
            if (cc == null)
            {
                cc = new CommentCollection();
                Query q = Comment.CreateQuery();
                q.AndWhere(Comment.Columns.PostId, PostId);
                q.AndWhere(Comment.Columns.IsPublished, true);
                q.AndWhere(Comment.Columns.IsDeleted, false);
                q.OrderByAsc(Comment.Columns.Published);
                cc.LoadAndCloseReader(q.ExecuteReader());
                ZCache.InsertCache("Feedback-" + PostId, cc, 10);
            }

            return cc;
        }

        /// <summary>
        /// Gets the post comments from the specified postId
        /// </summary>
        /// <param name="PostId"></param>
        /// <returns></returns>
        public CommentCollection PostComments(int PostId)
        {
            CommentCollection cc = ZCache.Get<CommentCollection>("Comments-" + PostId);
            if (cc == null)
            {
                cc = new CommentCollection();
                Query q = Comment.CreateQuery();
                q.AndWhere(Comment.Columns.PostId, PostId);
                q.AndWhere(Comment.Columns.IsPublished, true);
                q.AndWhere(Comment.Columns.IsDeleted, false);
                q.AndWhere(Comment.Columns.IsTrackback, false);
                q.OrderByAsc(Comment.Columns.Published);
                cc.LoadAndCloseReader(q.ExecuteReader());
                ZCache.InsertCache("Comments-" + PostId, cc, 10);
            }

            return cc;
        }

        /// <summary>
        /// Gets the post trackbacks from the specified postId
        /// </summary>
        /// <param name="PostId"></param>
        /// <returns></returns>
        public CommentCollection PostTrackbacks(int PostId)
        {
            CommentCollection cc = ZCache.Get<CommentCollection>("Trackbacks-" + PostId);
            if (cc == null)
            {
                cc = new CommentCollection();
                Query q = Comment.CreateQuery();
                q.AndWhere(Comment.Columns.PostId, PostId);
                q.AndWhere(Comment.Columns.IsPublished, true);
                q.AndWhere(Comment.Columns.IsDeleted, false);
                q.AndWhere(Comment.Columns.IsTrackback, true);
                q.OrderByAsc(Comment.Columns.Published);
                cc.LoadAndCloseReader(q.ExecuteReader());
                ZCache.InsertCache("Trackbacks-" + PostId, cc, 60);
            }

            return cc;
        }

        /// <summary>
        /// Gets x amount of recent posts
        /// </summary>
        /// <param name="numberOfPosts"></param>
        /// <returns></returns>
        public PostCollection RecentPosts(int numberOfPosts)
        {
            return RecentPosts(numberOfPosts, -1);
        }

        /// <summary>
        /// Gets x amount of recent posts from the specified category Id
        /// </summary>
        /// <param name="numberOfPosts"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public PostCollection RecentPosts(int numberOfPosts, int categoryId)
        {
            PostCollection pc = ZCache.Get<PostCollection>("Posts-Recent-" + numberOfPosts + "c:" + categoryId);
            if (pc == null)
            {
                Query q = PostCollection.DefaultQuery();

                if (categoryId > 0)
                {
                    Category category = new CategoryController().GetCachedCategory(categoryId, true);
                    if (category != null)
                    {
                        if (category.ParentId > 0)
                            q.AndWhere(Post.Columns.CategoryId, categoryId);
                        else
                        {
                            List<int> ids = new List<int>(category.Children.Count + 1);
                            foreach (Category child in category.Children)
                                ids.Add(child.Id);

                            ids.Add(category.Id);

                            q.AndInWhere(Post.Columns.CategoryId, ids.ToArray());
                        }
                    }
                    else
                    {
                        //this should result in no data, but it will signal to 
                        //the end user to edit/remove this widget
                        q.AndWhere(Post.Columns.CategoryId, categoryId);
                    }
                }

                q.Top = numberOfPosts.ToString();
                pc = PostCollection.FetchByQuery(q);
                ZCache.InsertCache("Posts-Recent-" + numberOfPosts + "c:" + categoryId, pc, 60);
            }
            return pc;
        }

        /// <summary>
        /// Gets x amount of popular post
        /// </summary>
        /// <param name="numberOfPosts"></param>
        /// <returns></returns>
        public PostCollection PopularPosts(int numberOfPosts)
        {
            return PopularPosts(numberOfPosts, -1);
        }

        /// <summary>
        /// Gets x amount of popular post by the specified category Name
        /// </summary>
        /// <param name="numberOfPosts"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public PostCollection PopularPosts(int numberOfPosts, string categoryName)
        {
            Category category = GetCategory(categoryName);
            return category == null ? null : PopularPosts(numberOfPosts, category.Id);
        }

        /// <summary>
        /// Gets x amount of popular posts from the specified category Id
        /// </summary>
        /// <param name="numberOfPosts"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public PostCollection PopularPosts(int numberOfPosts, int categoryId)
        {
            PostCollection pc = ZCache.Get<PostCollection>("Posts-Popular-" + numberOfPosts + "c:" + categoryId);
            if (pc == null)
            {
                Query q = PostCollection.DefaultQuery();

                if (categoryId > 0)
                {
                    Category category = new CategoryController().GetCachedCategory(categoryId, true);
                    if (category != null)
                    {
                        if (category.ParentId > 0)
                            q.AndWhere(Post.Columns.CategoryId, categoryId);
                        else
                        {
                            List<int> ids = new List<int>(category.Children.Count + 1);
                            foreach (Category child in category.Children)
                                ids.Add(child.Id);

                            ids.Add(category.Id);

                            q.AndInWhere(Post.Columns.CategoryId, ids.ToArray());
                        }
                    }
                    else
                    {
                        //this should result in no data, but it will signal to 
                        //the end user to edit/remove this widget
                        q.AndWhere(Post.Columns.CategoryId, categoryId);
                    }
                }

                q.Orders.Clear();
                q.OrderByDesc(Post.Columns.Views);
                q.Top = numberOfPosts.ToString();
                pc = PostCollection.FetchByQuery(q);
                ZCache.InsertCache("Posts-Popular-" + numberOfPosts + "c:" + categoryId, pc, 60);
            }
            return pc;
        }

        /// <summary>
        /// Gets the specified min/max amount of Tags
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public TagWeightCollection Tags(int min, int max)
        {
            return TagWeightCloud.FetchTags(min, max);
        }

        /// <summary>
        /// Gets all tags
        /// </summary>
        /// <returns></returns>
        public TagCollection GetAllTags()
        {
            TagCollection tc = ZCache.Get<TagCollection>("AllTags");

            if (tc == null)
            {
                tc = new TagCollection();

                TagCollection temp = TagCollection.FetchAll();

                foreach (Tag t in temp)
                {
                    Tag tag = tc.Find(
                                        delegate(Tag tempTag)
                                        {
                                            return tempTag.Name == t.Name;
                                        });

                    if (tag == null)
                        tc.Add(t);
                }

                ZCache.InsertCache("AllTags", tc, 90);
            }

            return tc;
        }

        /// <summary>
        /// Gets a reference to the SiteSettings object
        /// </summary>
        public SiteSettings Site
        {
            get { return SiteSettings.Get(); }
        }

        /// <summary>
        /// Gets a searchresult set by the specified query, pageIndex (usually 0) and max pageSize
        /// </summary>
        /// <param name="q"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public SearchResultSet<Post> Search(string q, int pageIndex, int pageSize)
        {
            SearchQuery sq = new SearchQuery();
            sq.QueryText = q;
            sq.PageIndex = pageIndex;
            sq.PageSize = pageSize;

            return new SearchIndex().Search(sq);
        }

        /// <summary>
        /// Gets x amount of similar posts by the postid
        /// </summary>
        /// <param name="postid"></param>
        /// <param name="numberOfRelatedPosts"></param>
        /// <returns></returns>
        public List<Post> SimilarSearch(int postid, int numberOfRelatedPosts)
        {
            return new SearchIndex().Similar(postid, numberOfRelatedPosts);
        }

        /// <summary>
        /// Gets all widgets in the specified WidgetLocation
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private static List<Widget> GetWidgets(WidgetLocation location)
        {
            List<Widget> _widgets = Widgets.FetchByLocation(location);
            List<Widget> widgets = new List<Widget>();
            foreach(Widget w in _widgets)
                if(w.IsUserValid())
                    widgets.Add(w);

            return widgets;
        }

        /// <summary>
        /// Returns all widgets on the Right sidebar
        /// </summary>
        /// <returns></returns>
        public List<Widget> RightWidgets()
        {
            return GetWidgets(WidgetLocation.Right);
        }

        /// <summary>
        /// Returns all widgets on the Left sidebar
        /// </summary>
        /// <returns></returns>
        public List<Widget> LeftWidgets()
        {
            return GetWidgets(WidgetLocation.Left);
        }

        /// <summary>
        /// Returns all of the queue widgets
        /// </summary>
        /// <returns></returns>
        public List<Widget> QueueWidgets()
        {
            return GetWidgets(WidgetLocation.Queue);
        }
    }
}