using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections;
using CookComputing.XmlRpc;
using DataBuddy;
using System.Collections.Generic;

namespace Graffiti.Core
{

	/// <summary>
	/// Summary description for MetaWeblog.
	/// </summary>
	public class MetaWeblog : XmlRpcService, IMetaWeblog
	{

		#region Structs

		public struct BlogInfo
		{
			public string blogid;
			public string url;
			public string blogName;
		}

		public struct Category
		{
			public string categoryId;
			public string categoryName;
		}

		[Serializable]
		public struct CategoryInfo
		{
			public string description;
			public string htmlUrl;
			public string rssUrl;
			public string title;
			public string categoryid;
		}

        [Serializable]
        public struct Tag
        {
            public int tag_id;
            public string name;
            public string slug;
            public string html_url;
            public string rss_url;
            public int count;
        }

		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public struct Enclosure
		{
			public int length;
			public string type;
			public string url;
		}

		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public struct Post
		{
			[XmlRpcMissingMapping(MappingAction.Error)]
			[XmlRpcMember(Description="Required when posting.")]
			public DateTime dateCreated;
			[XmlRpcMissingMapping(MappingAction.Error)]
			[XmlRpcMember(Description="Required when posting.")]
			public string description;
			[XmlRpcMissingMapping(MappingAction.Error)]
			[XmlRpcMember(Description="Required when posting.")]
			public string title;

			public string[] categories;
			public Enclosure enclosure;
			public string link;
			public string permalink;
			[XmlRpcMember(
				 Description="Not required when posting. Depending on server may "
				 + "be either string or integer. "
				 + "Use Convert.ToInt32(postid) to treat as integer or "
				 + "Convert.ToString(postid) to treat as string")]
			public object postid;       
			public Source source;
			public string userid;

            [XmlRpcMember(Description = "Optional when posting.")]
            public string mt_excerpt;

            [XmlRpcMember(Description = "Optional when posting.")]
            public string wp_slug;

            [XmlRpcMember(Description = "Optional when posting.")]
		    public string mt_basename;

            [XmlRpcMember(Description = "Optional when posting.")]
            public string mt_text_more;

            [XmlRpcMember(Description = "Optional when posting.")]
		    public string mt_keywords;

            [XmlRpcMember(Description = "Optional when posting.")]
            public string mt_tags;

            public string GetSlug() {
                return !string.IsNullOrEmpty(wp_slug) ? wp_slug : mt_basename;
            }

            public string GetTagList()
            {
                return mt_keywords ?? mt_tags; 
            }
		}


		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public struct Source
		{
			public string name;
			public string url;
		}

		public struct UserInfo
		{
			public string userid;
			public string firstname;
			public string lastname;
			public string nickname;
			public string email;
			public string url;
		}

		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public struct MediaObject
		{
			public string name;
			public string type;
			public byte[] bits;
		}

		[Serializable]
		public struct MediaObjectInfo
		{
			public string url;
		}

		#endregion

		#region MetaWeblog API Members

        private static Graffiti.Core.Category AddOrFetchCategory(string name, IGraffitiUser user)
        {
            int index = name.IndexOf(">");
            if (index > -1)
            {
                string parentName = name.Substring(0, index).Trim();
                string childName = name.Substring(index+1).Trim();

                Graffiti.Core.Category parent = new CategoryController().GetCachedCategory(parentName, true);

                if (parent != null)
                {
                    foreach (Graffiti.Core.Category childCategory in parent.Children)
                    {
                        if (Util.AreEqualIgnoreCase(childCategory.Name, childName))
                            return childCategory;
                    }

                    if (GraffitiUsers.IsAdmin(user))
                    {
                        Core.Category child = new Core.Category();
                        child.Name = HttpUtility.HtmlEncode(childName);
                        child.ParentId = parent.Id;
                        child.Save();

                        return child;
                    }
                }
                else
                {
                    if (GraffitiUsers.IsAdmin(user))
                    {
                        parent = new Core.Category();
                        parent.Name = HttpUtility.HtmlEncode(parentName);
                        parent.Save();

                        Core.Category child = new Core.Category();
                        child.Name = HttpUtility.HtmlEncode(childName);
                        child.ParentId = parent.Id;
                        child.Save();

                        return child;
                    }
                }
            }
            else
            {

                Core.Category category = new CategoryController().GetCachedCategory(name, true);
                if (category == null)
                {
                    if (GraffitiUsers.IsAdmin(user))
                    {
                        category = new Core.Category();
                        category.Name = name;
                        category.Save();
                    }
                }

                return category;
            }

            Log.Warn("Categories", "The user {0} does not have permission to create the category {1}", user.ProperName,HttpUtility.HtmlEncode(name));
            throw new Exception("You do not have permission to create a new category or sub-category");
        }

		public string newPost(string blogid, string username, string password, MetaWeblog.Post post, bool publish)
		{
			if(ValidateUser(username,password))
			{
			    IGraffitiUser user = GraffitiUsers.Current;
			    Graffiti.Core.Post postToAdd = new Graffiti.Core.Post();
			    postToAdd.ContentType = "text/html";

                postToAdd.PostStatus = (publish ? PostStatus.Publish : PostStatus.Draft);
			    postToAdd.IsPublished = publish;
				postToAdd.PostBody = post.description;
			    postToAdd.Title = post.title;
			    postToAdd.TagList = post.GetTagList();
			    postToAdd.UserName = username;
			    postToAdd.EnableComments = CommentSettings.Get().EnableCommentsDefault;

                if (post.categories != null && post.categories.Length > 0)
                {
                    postToAdd.CategoryId = AddOrFetchCategory(post.categories[0],user).Id;
                }
                else
                {
                    postToAdd.CategoryId = CategoryController.UnCategorizedId;
                }

			    postToAdd.Name = post.GetSlug();

                if (!string.IsNullOrEmpty(post.mt_text_more))
                {
                    postToAdd.ExtendedBody = post.mt_text_more;
                }

                
				
				// Get UserTime safely (some clients pass in a DateTime that is not valid)
				try
				{
                    if (post.dateCreated != DateTime.MinValue)
                    {
                        DateTime dtUTC = post.dateCreated;
                        DateTime dtLocal = dtUTC.ToLocalTime();
                        postToAdd.Published = dtLocal.AddHours(SiteSettings.Get().TimeZoneOffSet);
                    }
				}
				catch { postToAdd.Published = DateTime.Now.AddHours(SiteSettings.Get().TimeZoneOffSet); }
				
                if(postToAdd.Published <= new DateTime(2000,1,1))
                    postToAdd.Published = DateTime.Now.AddHours(SiteSettings.Get().TimeZoneOffSet); 
				
                try
                {
                    return PostRevisionManager.CommitPost(postToAdd, user, false, false).ToString();
                }
                catch(Exception ex)
                {
                    if (ex.Message.IndexOf("UNIQUE") > -1)
                        throw new XmlRpcFaultException(2,"Duplicate Post Name");

                    else
                    {
                        Log.Error("MetaBlog Error", "An error occored editing the post {0}. Exception: {1} Stack: {2}", post.postid, ex.Message, ex.StackTrace);
                        throw;
                    }
                }
				

			}


			throw new XmlRpcFaultException(0,"User does not exist");
		}


		public bool editPost(string postid,	string username,string password,Post post,bool publish)
		{
			if(ValidateUser(username,password))
			{
                Graffiti.Core.Post wp = new Graffiti.Core.Post(postid);
			    IGraffitiUser user = GraffitiUsers.Current;

                if(post.categories != null && post.categories.Length > 0)
                {
                    wp.CategoryId = AddOrFetchCategory(post.categories[0],user).Id;
                }

                wp.Name = post.wp_slug ?? wp.Name;

                if (!string.IsNullOrEmpty(post.mt_text_more))
                {
					wp.ExtendedBody = post.mt_text_more;
                }
                else
                {
					wp.ExtendedBody = null;
                }
                wp.PostBody = post.description;

			    wp.Title = post.title;
                wp.PostStatus = (publish ? PostStatus.Publish : PostStatus.Draft);
			    wp.IsPublished = publish;
			    wp.TagList = post.GetTagList() ?? wp.TagList;

				try
				{
					if (post.dateCreated != DateTime.MinValue)
					{
                        DateTime dtUTC = post.dateCreated;
                        DateTime dtLocal = dtUTC.ToLocalTime();
                        wp.Published = dtLocal.AddHours(SiteSettings.Get().TimeZoneOffSet);
                        //wp.Published = post.dateCreated;
					}
						
				}
				catch {  }

                try
                {
                    PostRevisionManager.CommitPost(wp, user, SiteSettings.Get().FeaturedId == wp.Id,wp.Category.FeaturedId == wp.Id);
                    return true;
                }
                catch (Exception ex)
                {
                    if(ex.Message.IndexOf("UNIQUE") > -1)
                    throw new XmlRpcFaultException(2,
                                                   "Sorry, but the name of this post is not unqiue and the post was not saved");

                    else
                    {
                        Log.Error("MetaBlog Error", "An error occored editing the post {0}. Exception: {1} Stack: {2}", post.postid,ex.Message,ex.StackTrace);
                        throw;
                    }
                }
			    
			}

			throw new XmlRpcFaultException(0,"User does not exist");
		}


		public MetaWeblog.Post getPost(string postid,string username,string password)
		{
			if(ValidateUser(username,password))
			{
                VersionStoreCollection vsc = VersionStore.GetVersionHistory(Convert.ToInt32(postid));

                Graffiti.Core.Post p = new Graffiti.Core.Post();

                if (vsc != null && vsc.Count > 0)
                {
                    List<Graffiti.Core.Post> the_Posts = new List<Graffiti.Core.Post>();
                    foreach (VersionStore vs in vsc)
                    {
                        the_Posts.Add(ObjectManager.ConvertToObject<Graffiti.Core.Post>(vs.Data));
                    }

                    the_Posts.Sort(delegate(Graffiti.Core.Post p1, Graffiti.Core.Post p2) { return Comparer<int>.Default.Compare(p2.Version, p1.Version); });
                    p = the_Posts[0];
                }
                else
                {
                    p = new Graffiti.Core.Post(postid);
                }

                return ConvertToPost(p);
			}

			throw new XmlRpcFaultException(0,"User does not exist");
		}

        public MetaWeblog.CategoryInfo[] getCategories2(string blogid,string username,string password)
        {
            return getCategories(blogid, username, password);
        }
		
		public MetaWeblog.CategoryInfo[] getCategories(string blogid,string username,string password)
		{
			if(ValidateUser(username,password))
			{
			    CategoryCollection cc = new CategoryController().GetAllTopLevelCachedCategories();
                ArrayList al = new ArrayList();
				Macros m = new Macros();
				foreach(Graffiti.Core.Category c in cc)
				{
					CategoryInfo ci = new CategoryInfo();
				    ci.categoryid = c.Id.ToString();
					ci.description = c.Name; 
					ci.title = c.Name;
				    ci.htmlUrl = m.FullUrl(c.Url);
				    ci.rssUrl = m.FullUrl(c.Url + "feed/");
					al.Add(ci);

                    if (c.HasChildren)
                    {
                        foreach (Core.Category child in c.Children)
                        {
                            CategoryInfo ci2 = new CategoryInfo();
                            ci2.categoryid = child.Id.ToString();
                            ci2.description = c.Name + " > " + child.Name;
                            ci2.title = c.Name + " > " + child.Name;
                            ci2.htmlUrl = m.FullUrl(child.Url);
                            ci2.rssUrl = m.FullUrl(child.Url + "feed/");
                            al.Add(ci2);
                        }
                    }
				}

				return al.ToArray(typeof(CategoryInfo)) as CategoryInfo[];
			}
			throw new XmlRpcFaultException(0,"User does not exist");
		}

        public Tag[] getTags(string blogid, string username, string password)
        {
            if (ValidateUser(username, password))
            {
                var wpGetTagsList = new List<Tag>();
                var tags = TagWeightCollection.FetchAll();
                foreach (var tag in tags)
                {
                    var metablogTag = new Tag();
                    metablogTag.tag_id = tag.Name.GetHashCode();
                    metablogTag.count = tag.Count;
                    metablogTag.name = tag.Name;
                    metablogTag.slug = tag.Name;
                    metablogTag.html_url = tag.Url;
                    metablogTag.rss_url = tag.Url + "feed/";
                    wpGetTagsList.Add(metablogTag);

                }

                return wpGetTagsList.ToArray();
            }

            throw new XmlRpcFaultException(0, "User does not exist");
        }

		public MetaWeblog.Post[] getRecentPosts(string blogid,string username,string password,int numberOfPosts)
		{
			if(ValidateUser(username,password))
			{
			    Query q = Graffiti.Core.Post.CreateQuery();
                q.AndWhere(Core.Post.Columns.IsDeleted, false);
			    q.Top = numberOfPosts.ToString();
                q.OrderByDesc(Graffiti.Core.Post.Columns.Published);
                PostCollection pc = new PostCollection();
                pc.LoadAndCloseReader(q.ExecuteReader());
				
				ArrayList al = new ArrayList(pc.Count);
				foreach(Graffiti.Core.Post p in pc)
				{
					al.Add(ConvertToPost(p));
				}

				return (Post[])al.ToArray(typeof(Post));

			}
			throw new XmlRpcFaultException(0,"User does not exist");
		}

		public MetaWeblog.MediaObjectInfo newMediaObject(string blogid, string username, string password, MetaWeblog.MediaObject mediaObject)
		{

            if (ValidateUser(username, password))
            {
                if (mediaObject.bits != null && !string.IsNullOrEmpty(mediaObject.name))
                {
                    string fileName = mediaObject.name;
                    
                    //Mars Edit appends '/' and this causes .NET to save the file at the root of Windows.
                    if (fileName.StartsWith("/"))
                        fileName = fileName.Substring(1);

                    
                    int i = fileName.LastIndexOf(Path.DirectorySeparatorChar);
                    if (i != -1)
                        fileName = fileName.Substring(i + 1);
                    
                    bool isImage = Regex.IsMatch(Path.GetExtension(mediaObject.name), ".jpg|.jpeg|.png|.gif",RegexOptions.IgnoreCase);

                    string name = isImage ?  Path.Combine(Context.Server.MapPath("~/files/media/image"), fileName) :  Path.Combine(Context.Server.MapPath("~/files/media/file"), fileName);
                                                            
                    FileInfo fi = new FileInfo(name);
                    if (!fi.Directory.Exists)
                        fi.Directory.Create();

                    using (FileStream fs = new FileStream(name, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = new byte[256*1024];
                        int bytes;

                        MemoryStream content = new MemoryStream(mediaObject.bits);

                        while ((bytes = content.Read(buffer, 0, 256*1024)) > 0)
                            fs.Write(buffer, 0, bytes);
                        fs.Close();
                    }
                    
                    MediaObjectInfo info = new MediaObjectInfo();
                    string absolteUrl = VirtualPathUtility.ToAbsolute((isImage ? "~/files/media/image/" : "~/files/media/file/") + fileName);

                    info.url = new Macros().FullUrl(absolteUrl);
                    return info;
                }
            }
		    throw new XmlRpcFaultException(0, "User does not exist");

		}
		
		#endregion

		#region Blogger API Members

		public MetaWeblog.BlogInfo[] getUsersBlogs(string appKey, string username,string password)
		{
			if(ValidateUser(username,password))
			{
                BlogInfo[] al = new BlogInfo[1];

			    BlogInfo bi = new BlogInfo();
			    bi.blogid = "root_blog";
			    bi.blogName = SiteSettings.Get().Title;
			    bi.url = new Macros().FullUrl("~/");
			    al[0] = bi;

			    return al;
			}


		    
			
			throw new XmlRpcFaultException(0,"User does not exist");
		}


		public bool deletePost(string appKey,string postid, string username, string password, [XmlRpcParameter(Description="Where applicable, this specifies whether the weblog should be republished after the post has been deleted.")] bool publish)
		{
			if(ValidateUser(username,password))
			{
			    Graffiti.Core.Post.Delete(postid);
				return true;
			}

			throw new XmlRpcFaultException(0,"User does not exist");
		}


        public MetaWeblog.UserInfo getUserInfo(string appKey, string username, string password)
        {
            if (ValidateUser(username, password))
            {
                IGraffitiUser gu = GraffitiUsers.GetUser(username);
                UserInfo ui = new UserInfo();
                ui.userid = gu.Name;
                ui.firstname = gu.ProperName;
                ui.lastname = "";

                ui.email = gu.Email;

                ui.nickname = gu.ProperName;


                ui.url = gu.WebSite ?? new Macros().FullUrl(new Urls().Home);


                return ui;
            }
            throw new XmlRpcFaultException(0, "User does not exist");
        }

	    #endregion

		#region Private Helper Methods

        protected static bool ValidateUser(string username, string password)
        {
            IGraffitiUser userToLogin = GraffitiUsers.Login(username, password,true);
            if (userToLogin != null)
            {
                //HttpContext.Current.User = userToLogin;
                return true;
            }

            Log.Warn("Security", "Invalid login attempt by {0}", username);

            return false;
        }


	    protected static MetaWeblog.Post ConvertToPost(Graffiti.Core.Post wp)
		{
			MetaWeblog.Post p = new Post();

            if(wp.Category.ParentId > 0)
                p.categories = new string[] {new CategoryController().GetCachedCategory(wp.Category.ParentId,false).Name + " > " +  wp.Category.Name };
            else
		        p.categories = new string[] {wp.Category.Name};
		    p.dateCreated = wp.Published;//.ToUniversalTime();
		    p.description = wp.PostBody;
			p.mt_text_more = wp.ExtendedBody;

		    p.link = new Macros().FullUrl(wp.Url);
			p.permalink = p.link;
		    p.postid = wp.Id;
		    p.title = wp.Title;
		    p.wp_slug = p.mt_basename = Util.UnCleanForUrl(wp.Name);
		    p.mt_keywords = wp.TagList;
	        p.mt_tags = wp.TagList;
			return p;
		}


		#endregion
	
	}
}

