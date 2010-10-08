using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using DataBuddy;
using System.Web.UI;
using System.Linq;

namespace Graffiti.Core
{
	/// <summary>
	/// Macros is a helper tool loaded by Graffiti on every page request. ($macros)
	/// </summary>
	[Chalk("macros")]
	public class Macros
	{
		#region const
		private const string lihrefWithClassFormat = "<li {3}><a href=\"{0}\" {2}>{1}</a></li>\n";
		private const string liHrefFormat = "<li><a href=\"{0}\">{1}</a></li>\n";
		private const string themesShortCut = "~|";
		#endregion

		#region Theme

		/// <summary>
		/// Loads a view found in the same directory of the current theme.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public string LoadThemeView(string file)
		{
			GraffitiContext context = GraffitiContext.Current;
			return ViewManager.RenderTemplate(HttpContext.Current, context, context.Theme, file);
		}


		/// <summary>
		/// Returns the absolute path to a file in the current theme
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public string ThemeFile(string file)
		{
			return ViewManager.GetAbsoluateFilePath(GraffitiContext.Current.Theme, file);
		}

		/// <summary>
		/// Renders a style link element - defaults to screen media type.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public string Style(string file)
		{
			return Style(file, "screen");
		}

		/// <summary>
		/// Renders a style link element and allows setting the media type
		/// </summary>
		public string Style(string file, string media)
		{
			return
				 string.Format("<link rel=\"stylesheet\" href=\"{0}\" type=\"text/css\" media=\"{1}\" />",
									ThemeFile(file), media);
		}

		/// <summary>
		/// Renders a javascript script element
		/// </summary>
		public string JavaScript(string file)
		{
			return
				 string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>",
									ThemeFile(file));
		}

		/// <summary>
		/// Renders an IMG element for the supplied imageName. imageName is relative to the current theme 
		/// </summary>
		/// <returns></returns>
		public string Image(string imageName)
		{
			return Image(imageName, "");
		}

		/// <summary>
		/// Renders an IMG element. Extra attibutes are applied to the img.
		/// </summary>
		/// <param name="imageName"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public string Image(string imageName, IDictionary id)
		{
			return Image(imageName, GetAttributes(id));
		}

		/// <summary>
		/// Renders an IMG element. Extra attibutes are applied to the img.
		/// </summary>
		/// <param name="imageName"></param>
		/// <param name="imgAttributes"></param>
		/// <returns></returns>
		public string Image(string imageName, string imgAttributes)
		{
			return string.Format("<img src = \"{0}\" {1} />", ThemeFile(imageName), imgAttributes);
		}

		/// <summary>
		/// Renders the right widget sidebar
		/// </summary>
		/// <param name="dictionary"></param>
		/// <returns></returns>
		public string RightSideBar(IDictionary dictionary)
		{
			return SmartSideBar(WidgetLocation.Right, dictionary);
		}

		/// <summary>
		/// Renders the left widget sidebar
		/// </summary>
		/// <param name="dictionary"></param>
		/// <returns></returns>
		public string LeftSideBar(IDictionary dictionary)
		{
			return SmartSideBar(WidgetLocation.Left, dictionary);
		}

		/// <summary>
		/// Renders a widget sidebar
		/// </summary>
		/// <param name="location"></param>
		/// <param name="dictionary"></param>
		/// <returns></returns>
		private static string SmartSideBar(WidgetLocation location, IDictionary dictionary)
		{
			string beforeWidget = dictionary["beforeWidget"] as string ?? "<li class=\"widget\">";
			string afterWidget = dictionary["afterWidget"] as string ?? "</li>";
			string afterTitle = dictionary["afterTitle"] as string ?? "</h2>";
			string beforeTitle = dictionary["beforeTitle"] as string ?? "<h2>";
			string beforeContent = dictionary["beforeContent"] as string;
			string afterContent = dictionary["afterContent"] as string;

			string beforeFirstWidget = dictionary["beforeFirstWidget"] as string;
			string afterFirstWidget = dictionary["afterFirstWidget"] as string;
			string beforeLastWidget = dictionary["beforeLastWidget"] as string;
			string afterLastWidget = dictionary["afterLastWidget"] as string;

			StringBuilder sb = new StringBuilder();

			List<Widget> allWidgets = Widgets.FetchByLocation(location);

			int curCount = 0;

			foreach (Widget widget in allWidgets)
			{
				if (widget.IsUserValid())
				{
					curCount++;

					if ((!String.IsNullOrEmpty(beforeFirstWidget) || !String.IsNullOrEmpty(afterFirstWidget)) && curCount == 1)
					{
						sb.Append(String.IsNullOrEmpty(beforeFirstWidget) ? beforeWidget : beforeFirstWidget);
						sb.Append(widget.Render(beforeTitle, afterTitle, beforeContent, afterContent));
						sb.Append(String.IsNullOrEmpty(afterFirstWidget) ? afterWidget : afterFirstWidget);
					}
					else if ((!String.IsNullOrEmpty(beforeLastWidget) || !String.IsNullOrEmpty(afterLastWidget)) && curCount == allWidgets.Count)
					{
						sb.Append(String.IsNullOrEmpty(beforeLastWidget) ? beforeWidget : beforeLastWidget);
						sb.Append(widget.Render(beforeTitle, afterTitle, beforeContent, afterContent));
						sb.Append(String.IsNullOrEmpty(afterLastWidget) ? afterWidget : afterLastWidget);
					}
					else
					{
						sb.Append(beforeWidget + "\n");
						sb.Append(widget.Render(beforeTitle, afterTitle, beforeContent, afterContent));
						sb.Append(afterWidget + "\n");
					}
				}
			}
			return sb.ToString();
		}

		public string Widget(string title, string beforeTitle, string afterTitle, string beforeContent, string afterContent)
		{
			StringBuilder sb = new StringBuilder("");
			Widget widget = Widgets.FetchByTitle(title);
			if (widget != null)
			{
				sb.Append(widget.Render(beforeTitle, afterTitle, beforeContent, afterContent));
			}
			return sb.ToString();
		}

		/// <summary>
		/// Renders the value of a variable.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string Variable(string id)
		{
			GraffitiContext context = GraffitiContext.Current;

			object value = context.ThemeConfiguration.GetObjectValue(id, string.Empty);
			if (value == null)
				return string.Empty;
			else if (value is Color)
				return ColorTranslator.ToHtml((Color)value);
			else if (value is Uri)
				return Link(value.ToString());
			else
				return value.ToString();
		}

		#endregion

		#region Links and Navigation

		/// <summary>
		/// Converts a virtual (~/) link to an absolute. If ~| is used instead of ~/ the link is assumed to be to a file in the themes directory
		/// </summary>
		/// <param name="virtualPath"></param>
		/// <returns></returns>
		public string Link(string virtualPath)
		{
			if (!String.IsNullOrEmpty(virtualPath))
			{
				if (virtualPath.StartsWith(themesShortCut))
					return ThemeFile(virtualPath.Replace(themesShortCut, string.Empty));

				return VirtualPathUtility.ToAbsolute(virtualPath);
			}
			return string.Empty;
		}

		/// <summary>
		/// Determines if the logged in user can view the control panel
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public bool CanViewControlPanel(IGraffitiUser user)
		{
			return RolePermissionManager.CanViewControlPanel(user);
		}

		/// <summary>
		/// Returns an edit link for the post if the current user has edit permissions.
		/// </summary>
		public string EditLink(Post post)
		{
			IGraffitiUser user = GraffitiUsers.Current;
			Permission p = RolePermissionManager.GetPermissions(post.CategoryId, user);

			if (user != null && p.Edit)
			{
				return
					 "[<a class=\"editlink\" href=\"" + new Urls().Write + "?id=" + post.Id +
					 "\">Edit - " + post.Title +
					 "</a>]";
			}

			return string.Empty;
		}

		/// <summary>
		/// Fully qualifies an absolute url
		/// </summary>
		/// <param name="aboluteUrl"></param>
		/// <returns></returns>
		public string FullUrl(string absoluteUrl)
		{
			return new Uri(HttpContext.Current.Request.Url, absoluteUrl).ToString();
		}

		/// <summary>
		/// Creates a list of links which can be used to build custom navigation.
		/// </summary>
		/// <returns></returns>
		public List<Link> NavigationLinks()
		{
			int current_PostID = -1;
			int current_Parent_CategoryID = -1;
			int current_CategoryID = -1;

			List<DynamicNavigationItem> items = NavigationSettings.Get().SafeItems();
			List<Link> links = new List<Link>();

			// Will hold a reference to the selected item
			DynamicNavigationItem selectedItem = null;

			// We can only do this on a graffit page that exposes the post/category/etc properties
			TemplatedThemePage ttp = HttpContext.Current.Handler as TemplatedThemePage;
			if (ttp != null)
			{
				// Is this page a post? 
				if (ttp.PostId > 0)
					current_PostID = ttp.PostId;

				// Is this page a category?
				// Note: Post pages expose their categories and do not require a seperate lookup
				if (ttp.CategoryID > 0)
				{
					// This could be a subcategory. Since we do not expose subcategories via NavBar(), 
					// we should mark the parent (if it exists) as selected item
					current_CategoryID = ttp.CategoryID;
					Category the_Category = new CategoryController().GetCachedCategory(current_CategoryID, true);
					if (the_Category != null)
					{
						current_Parent_CategoryID = the_Category.ParentId;
					}
				}

				// If we are a post, see if it has a DynamicNavigationItem
				if (current_PostID > 0)
				{
					foreach (DynamicNavigationItem item in items)
					{
						if (item.NavigationType == DynamicNavigationType.Post)
						{
							if (item.PostId == current_PostID)
							{
								selectedItem = item;
								break;
							}
						}
					}
				}

				// We default to post first, but if that has not been selected, try to find a category
				if (selectedItem == null && (current_CategoryID > 0 || current_Parent_CategoryID > 0))
				{
					foreach (DynamicNavigationItem item in items)
					{
						if (item.NavigationType == DynamicNavigationType.Category)
						{
							if (item.CategoryId == current_CategoryID || item.CategoryId == current_Parent_CategoryID)
							{
								selectedItem = item;
								break;
							}
						}
					}
				}

				// Graffiti has two pages which could end up in the NavBar (home and search).
				// If still no item exists, lets see if this was added. 
				// Note: Tags are not yet part of this
				if (selectedItem == null)
				{
					bool isHomePage = ttp.GetType().ToString().IndexOf("GraffitiHomePage") > -1;
					bool isSearchPage = ttp.GetType().ToString().IndexOf("GraffitiSearchPage") > -1;

					if (isHomePage || isSearchPage)
					{
						foreach (DynamicNavigationItem item in items)
						{
							if (item.NavigationType == DynamicNavigationType.Link)
							{
								if (item.Text.ToLower().IndexOf("search") > -1)
								{
									selectedItem = item;
									break;
								}

								if (item.Text.ToLower().IndexOf("home") > -1 || item.Href == "~/" || item.Href == "/")
								{
									selectedItem = item;
									break;
								}
							}
						}
					}
				}
			}

			foreach (DynamicNavigationItem item in items)
			{
				Link the_Link = new Link();
				the_Link.IsSelected = (selectedItem == item);
				the_Link.Text = item.Name;
				the_Link.CategoryId = item.CategoryId;
				the_Link.PostId = item.PostId;
				the_Link.NavigationType = item.NavigationType;

				switch (item.NavigationType)
				{
					case DynamicNavigationType.Post:

						Post p = Post.GetCachedPost(item.PostId);

						if (!p.IsNew && p.IsLoaded)
						{
							the_Link.Url = p.Url;
						}

						break;

					case DynamicNavigationType.Category:

						Category category = new CategoryController().GetCachedCategory(item.CategoryId, true);
						if (category != null)
						{
							the_Link.Url = category.Url;
						}

						break;

					case DynamicNavigationType.Link:

						if (!string.IsNullOrEmpty(item.Href))
						{
							the_Link.Url = item.Href.StartsWith("~/")
													 ? VirtualPathUtility.ToAbsolute(item.Href)
													 : item.Href;
						}
						break;
				}

				if (!string.IsNullOrEmpty(the_Link.Url) && !string.IsNullOrEmpty(the_Link.Text))
					links.Add(the_Link);
			}

			List<Link> permissionsFiltered = new List<Link>();
			permissionsFiltered.AddRange(links);

			foreach (Link link in links)
			{
				if (!RolePermissionManager.GetPermissions(link.CategoryId, GraffitiUsers.Current).Read && link.CategoryId != 0)
					permissionsFiltered.Remove(link);
			}

			return permissionsFiltered;
		}

		/// <summary>
		/// Renders the dynamic NavBar based on the list generated in Macros.NavigationLinks()
		/// </summary>
		/// <returns></returns>
		public string NavBar()
		{
			StringBuilder sb = new StringBuilder();
			List<Link> links = NavigationLinks();

			for (int i = 0; i < links.Count; i++)
			{
				Link link = links[i];

				sb.AppendFormat(lihrefWithClassFormat, link.Url, link.Text, GetNavigationCSSClass(link.IsSelected, i == 0, i == links.Count - 1, null),
									 GetNavigationCSSClass(link.IsSelected, i == 0, i == links.Count - 1, "li"));
			}
			return sb.ToString();
		}

		/// <summary>
		/// Enables checking a link to see if it is part of NavigationLinks() 
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public bool IsInNavigation(string url)
		{
			List<Link> links = NavigationLinks();

			Link temp = links.Find(delegate(Link l)
												{
													return l.Url == url;
												});

			if (temp == null)
				return false;
			else
				return true;
		}

		/// <summary>
		/// Helper method responsible for setting the CSS class on the NavBar
		/// </summary>
		private static string GetNavigationCSSClass(bool isSelected, bool isFirst, bool isLast, string text)
		{
			if (!isSelected && !isFirst && !isLast)
				return null;

			return
				 string.Format("class=\"{0}{1}{2}\"",
									isFirst ? "first" + text + " " : null,
									isSelected ? "selected" + text : null,
									isLast ? (((isFirst || isSelected) ? " " : null) + "last" + text) : null);
		}


		/// <summary>
		/// Creates a list of links which can be used to build custom sub category navigation.
		/// </summary>
		/// <param name="usePostsIfNoChildCategories">If the current top-level category does not have any child categories, should posts from that category be returned as links instead?</param>
		/// <returns></returns>
		public List<Link> NavigationSubLinks(bool usePostsIfNoChildCategories)
		{
			List<Link> links = new List<Link>();

			Category currentCategory = null;
			TemplatedThemePage ttp = HttpContext.Current.Handler as TemplatedThemePage;
			CategoryCollection categories = new CategoryCollection();

			if (ttp != null)
			{
				// Is this page a category?
				// Note: Post pages expose their categories and do not require a seperate lookup
				if (ttp.CategoryID > 0)
				{
					// This could be a subcategory. Since we do not expose subcategories via NavBar(), 
					// we should mark the parent (if it exists) as selected item
					currentCategory = new CategoryController().GetCachedCategory(ttp.CategoryID, true);
					if (currentCategory != null)
					{
						// If the current category has a parent, we have to assume it's a child category.
						// In which case, we need to list out all the children of the parent.
						if (currentCategory.ParentId > 0)
						{
							Category parentCategory = new CategoryController().GetCachedCategory(currentCategory.ParentId, true);
							categories = parentCategory.Children;
						}
						else
						{
							categories = currentCategory.Children;
						}

						if (categories != null && categories.Count > 0)
						{
							foreach (Category category in categories)
							{
								Link link = new Link();
								link.IsSelected = (currentCategory.Id == category.Id);
								link.Text = category.Name;
								link.CategoryId = category.Id;
								link.NavigationType = DynamicNavigationType.Category;
								link.Url = category.Url;
								links.Add(link);
							}
						}
						else if (usePostsIfNoChildCategories && currentCategory.PostCount > 0)
						{
							PostCollection posts = new Data().PostsByCategory(currentCategory, 10);
							foreach (Post post in posts)
							{
								Link link = new Link();
								link.IsSelected = (ttp.PostId == post.Id);
								link.Text = post.Title;
								link.CategoryId = post.Id;
								link.PostId = post.Id;
								link.NavigationType = DynamicNavigationType.Post;
								link.Url = post.Url;
								links.Add(link);
							}
						}
					}
				}
			}

			return links;
		}

		/// <summary>
		/// Renders sub category navigation based on the current page
		/// </summary>
		/// <returns></returns>
		public string SubNavBar()
		{
			StringBuilder sb = new StringBuilder();
			List<Link> links = NavigationSubLinks(false);

			for (int i = 0; i < links.Count; i++)
			{
				Link link = links[i];

				sb.AppendFormat(lihrefWithClassFormat, link.Url, link.Text, GetNavigationCSSClass(link.IsSelected, i == 0, i == links.Count - 1, null),
									 GetNavigationCSSClass(link.IsSelected, i == 0, i == links.Count - 1, "li"));
			}
			return sb.ToString();
		}


		/// <summary>
		/// Returns an href for a category. Includes subcategory if it exists.
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		public string CategoryLink(Category category)
		{
			if (category.ParentId <= 0)
				return string.Format("<a href=\"{0}\">{1}</a>", category.Url, category.Name);
			else
			{
				Category parent = new CategoryController().GetCachedCategory(category.ParentId, false);
				return string.Format("<a href=\"{0}\">{1}</a>", parent.Url, parent.Name) + " / " + string.Format("<a href=\"{0}\">{1}</a>", category.Url, category.Name);
			}
		}

		#endregion

		#region Header

		/// <summary>
		/// Returns the standard (name, keywords, robots, and generator) metatags for a Graffiti page.
		/// </summary>
		/// <returns></returns>
		public string MetaTags()
		{
			string description = null;
			string keywords = null;
			string robots;
			bool indexable = true;
			TemplatedThemePage ttp = HttpContext.Current.Handler as TemplatedThemePage;
			if (ttp != null)
			{
				description = ttp.MetaDescription;
				keywords = ttp.MetaKeywords;
				indexable = ttp.IsIndexable;
			}

			if (string.IsNullOrEmpty(description))
				description = SiteSettings.Get().MetaDescription;

			if (string.IsNullOrEmpty(keywords))
				keywords = SiteSettings.Get().MetaKeywords;

			if (!string.IsNullOrEmpty(description))
				description = "<meta name=\"description\" content=\"" + description + "\" />\n";

			if (!string.IsNullOrEmpty(keywords))
				keywords = "<meta name=\"keywords\" content=\"" + keywords + "\" />\n";

			if (indexable)
				robots = "<meta name=\"robots\" content=\"index,follow\" />\n";
			else
				robots = "<meta name=\"robots\" content=\"noindex,follow\" />\n";


			string generator = "\n<meta name=\"generator\" content=\"" + SiteSettings.VersionDescription + "\" />\n";

			return generator + description + keywords + robots;
		}

		/// <summary>
		/// Helper which renders all the default HTML Head settings
		/// </summary>
		/// <returns></returns>
		public string Head()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(MetaTags()).Append("\n");
			sb.Append(RssAutodiscovery()).Append("\n");
			sb.Append(RSD()).Append("\n");
			sb.Append(GraffitiJavaScript).Append("\n");
			sb.Append(Favicon()).Append("\n");
			sb.Append(SiteSettings.Get().Header).Append("\n");


			// Allow plugins to add aditional content to the HTML head
			Events.Instance().ExecuteRenderHtmlHeader(sb);

			return sb.ToString();
		}

		/// <summary>
		/// Returns the RSD element
		/// </summary>
		/// <returns></returns>
		public string RSD()
		{
			return
				 string.Format(
					  "<link rel=\"EditURI\" type=\"application/rsd+xml\" title=\"RSD\" href=\"{0}api/rsd.ashx\" />\n<link rel=\"wlwmanifest\" type=\"application/wlwmanifest+xml\" title=\"WLWManifest\" href=\"{0}api/wlwmanifest.ashx\" />\n",
					  FullUrl(new Urls().Home));
		}

		/// <summary>
		/// Returns RSS Autodiscoverable feeds.
		/// </summary>
		/// <returns></returns>
		public string RssAutodiscovery()
		{
			string pattern = "<link rel=\"alternate\" type=\"application/rss+xml\" title=\"{0}\" href=\"{1}\" />\n";
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(pattern, "Rss Feed",
								 SiteSettings.Get().ExternalFeedUrl ?? FullUrl(VirtualPathUtility.ToAbsolute("~/feed/")));

			TemplatedThemePage ttp = HttpContext.Current.Handler as TemplatedThemePage;
			if (ttp != null)
			{
				if (ttp.CategoryID > -1)
				{
					Category category = new CategoryController().GetCachedCategory(ttp.CategoryID, false);
					if (category.Name != CategoryController.UncategorizedName)
					{
						if (category.ParentId > 0)
						{
							Category parent = new CategoryController().GetCachedCategory(category.ParentId, false);
							sb.AppendFormat(pattern, parent.Name + " Rss Feed", FullUrl(parent.Url + "feed/"));
						}

						sb.AppendFormat(pattern, category.Name + " Rss Feed", FullUrl(category.Url + "feed/"));
					}
				}

				if (ttp.TagName != null)
				{
					sb.AppendFormat(pattern, ttp.TagName + " Rss Feed",
										 FullUrl(VirtualPathUtility.ToAbsolute("~/tags/" + Util.CleanForUrl(ttp.TagName)) +
													"/feed/"));
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Returns the generator element.
		/// </summary>
		public string Generator
		{
			get { return "<meta name=\"generator\" content=\"" + SiteSettings.Version + "\" />"; }
		}

		/// <summary>
		/// Renders default Graffiti javascript includes
		/// </summary>
		public string GraffitiJavaScript
		{
			get
			{
				return string.Format("\n<script type=\"text/javascript\" src=\"{0}\"></script>\n<script type=\"text/javascript\" src=\"{1}\" ></script>\n",
					SiteSettings.Get().UseExternalJQuery ? "http://ajax.microsoft.com/ajax/jQuery/jquery-1.4.2.min.js" : VirtualPathUtility.ToAbsolute("~/__utility/js/jquery-1.4.2.min.js"),
					VirtualPathUtility.ToAbsolute("~/__utility/js/graffiti.js"));
			}
		}

		#endregion

		#region Content Lists

		/// <summary>
		/// Renders an unordered list of the of the top level categories
		/// </summary>
		/// <returns></returns>
		public string ULCategories()
		{
			CategoryCollection cc = new CategoryController().GetTopLevelCachedCategories();
			StringBuilder sb = new StringBuilder();
			if (cc.Count > 0)
			{
				foreach (Category c in cc)
				{
					sb.AppendFormat(liHrefFormat, c.Url, c.Name);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Renders the tags for the post as "Tagged as: " + tags
		/// </summary>
		/// <param name="tags"></param>
		/// <returns></returns>
		public string TagList(string tags)
		{
			return TagList(tags, "Tagged as: ");
		}

		/// <summary>
		/// Renders the tags for the post with a custom start element "[desc]" + tags
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="desc"></param>
		/// <returns></returns>
		public string TagList(string tags, string desc)
		{
			if (string.IsNullOrEmpty(tags))
				return string.Empty;

			var tagListHtml = new StringBuilder(desc);
			var tagList = from tag in tags.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries)
							  orderby tag ascending
							  select tag;

			foreach (var tag in tagList)
			{
				tagListHtml.AppendFormat("<a rel=\"tag\" href=\"{0}\">{1}</a>, ",
					VirtualPathUtility.ToAbsolute("~/tags/" + Util.CleanForUrl(tag) + "/"), tag);
			}

			tagListHtml.Remove(tagListHtml.Length - 2, 2);

			return tagListHtml.ToString();
		}


		/// <summary>
		/// Renders an unordered list with the sites most recent posts by categoryID
		/// </summary>
		/// <param name="categoryId"></param>
		/// <returns></returns>
		public string ULPostsInCategory(int categoryId)
		{
			PostCollection pc = new PostCollection();
			Query q = Post.CreateQuery();
			q.PageIndex = 0;
			q.AndWhere(Post.Columns.CategoryId, categoryId);
			q.AndWhere(Post.Columns.IsDeleted, 0);
			q.AndWhere(Post.Columns.IsPublished, 1);

			pc.LoadAndCloseReader(q.ExecuteReader());

			StringBuilder sb = new StringBuilder();
			if (pc.Count > 0)
			{
				foreach (Post p in pc)
				{
					sb.AppendFormat(liHrefFormat, p.Url, p.Title);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Renders an unordered lists with the sites most recent posts
		/// </summary>
		/// <param name="numberOfPosts"></param>
		/// <returns></returns>
		public string ULRecentPosts(int numberOfPosts)
		{
			PostCollection pc = new PostCollection();
			Query q = PostCollection.DefaultQuery();
			q.PageIndex = 1;
			q.PageSize = numberOfPosts;
			pc.LoadAndCloseReader(q.ExecuteReader());

			StringBuilder sb = new StringBuilder();
			if (pc.Count > 0)
			{
				foreach (Post p in pc)
				{
					sb.AppendFormat(liHrefFormat, p.Url, p.Title);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Renders an unordred list of recent (published) comments
		/// </summary>
		/// <param name="numberOfComments"></param>
		/// <returns></returns>
		public string ULRecentComments(int numberOfComments)
		{
			CommentCollection cc = new Data().RecentComments(numberOfComments);

			StringBuilder sb = new StringBuilder();
			if (cc.Count > 0)
			{
				foreach (Comment c in cc)
				{
					sb.AppendFormat("<li><a href=\"{0}\" title=\"{2} on {1} at {3}\">{2} on {1}</a></li>\n", c.Url, c.Post.Title,
										 c.User.ProperName, c.Published);
				}
			}

			return sb.ToString();
		}


		/// <summary>
		/// Renders a pager for the current values in the Graffiti context (with customizable Older Posts and Newer Posts text)
		/// </summary>
		/// <param name="cssClass"></param>
		/// <param name="previousText"></param>
		/// <param name="nextText"></param>
		/// <returns></returns>
		public string Pager(string cssClass, string previousText, string nextText)
		{
			GraffitiContext graffiti = GraffitiContext.Current;
			string sq = (SearchQuery == null) ? null : sq = "?q=" + SearchQuery;
			return Util.Pager(graffiti.PageIndex, graffiti.PageSize, graffiti.TotalRecords, cssClass, sq, previousText, nextText);
		}

		/// <summary>
		/// Renders a pager for the current values in the Graffiti context
		/// </summary>
		/// <param name="cssClass"></param>
		/// <returns></returns>
		public string Pager(string cssClass)
		{
			GraffitiContext graffiti = GraffitiContext.Current;
			string sq = (SearchQuery == null) ? null : sq = "?q=" + SearchQuery;
			return Util.Pager(graffiti.PageIndex, graffiti.PageSize, graffiti.TotalRecords, cssClass, sq);
		}

		/// <summary>
		/// Renders a tag cloud
		/// </summary>
		/// <param name="min">Minimum number of posts with the tag</param>
		/// <param name="max">Maximum number of tags to return</param>
		/// <returns></returns>
		public string TagCloud(int min, int max)
		{
			StringBuilder sb = new StringBuilder();

			foreach (TagWeight tw in TagWeightCloud.FetchTags(min, max))
			{
				sb.AppendFormat("<a title=\"{3} posts\" style=\"font-size:{0}\" href=\"{1}\">{2}</a> ", tw.FontSize, tw.Url, tw.Name, tw.Count);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Renders the tag cloud as an unordered list
		/// </summary>
		/// <param name="min">Minimum number of posts with the tag</param>
		/// <param name="max">Maximum number of tags to return</param>
		/// <returns></returns>
		public string TagCloudUL(int min, int max)
		{
			StringBuilder sb = new StringBuilder();

			foreach (TagWeight tw in TagWeightCloud.FetchTags(min, max))
			{
				sb.AppendFormat("<li><a title=\"{3} posts\" style=\"font-size:{0}\" href=\"{1}\">{2}</a></li>", tw.FontSize, tw.Url, tw.Name, tw.Count);
			}

			return sb.ToString();
		}

		#endregion

		#region Search
		/// <summary>
		/// Returns the current search query (q)
		/// </summary>
		public string SearchQuery
		{
			get { return HttpUtility.HtmlEncode(HttpContext.Current.Request.QueryString["q"]); }
		}

		/// <summary>
		/// Returns the search url from $urls.Search
		/// </summary>
		/// <returns></returns>
		public string SearchUrl()
		{
			return new Urls().Search;
		}

		/// <summary>
		/// Returns the template to the search form (.view)
		/// </summary>
		/// <returns></returns>
		public string SearchForm()
		{
			return
				 ViewManager.RenderTemplate(HttpContext.Current, GraffitiContext.Current, "~/__utility/forms/search.view");
		}
		#endregion

		#region Comments

		/// <summary>
		/// Renders the element which links directly to the new comment form.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public string CommentUrl(Post p)
		{
			return CommentUrl(p, null);
		}

		/// <summary>
		/// Renders the element which links directly to the new comment form and includes additional attributes.
		/// </summary>
		public string CommentUrl(Post p, IDictionary dictionary)
		{
			string anchor = "comments";
			if (dictionary != null)
			{
				if (dictionary.Contains("anchor"))
				{
					anchor = dictionary["anchor"] as string;
					dictionary.Remove("anchor");
				}
			}

			string text;
			if (p.CommentCount <= 0)
				text = "No Comments";
			else if (p.CommentCount == 1)
				text = "1 Comment";
			else
				text = p.CommentCount + " Comments";

			return string.Format("<a href=\"{0}#{1}\" {2}>{3}</a>", p.Url, anchor, GetAttributes(dictionary), text);
		}

		/// <summary>
		/// Renders an href with the user details for the comment
		/// </summary>
		/// <param name="comment"></param>
		/// <returns></returns>
		public string CommentLink(Comment comment)
		{
			string webSite = comment.WebSite;

			if (!string.IsNullOrEmpty(webSite))
			{
				Uri uri;
				// attempt to create a Uri out of this
				if (!Uri.TryCreate(webSite, UriKind.Absolute, out uri))
					// if that didn't work as-is, try appending the HTTP scheme to it
					Uri.TryCreate("http://" + webSite, UriKind.Absolute, out uri);

				// only show this if it is HTTP or HTTPS
				if (uri != null && (
											  uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
					return string.Format("<a href=\"{0}\">{1}</a>", uri, comment.Name);
			}

			// either the website was not set, it couldn't be converted to a Uri, or was not HTTP or HTTPS
			return comment.Name;
		}

		#endregion

		#region Publc Helpers

		/// <summary>
		/// Truncates a block of HTML
		/// </summary>
		/// <param name="html"></param>
		/// <param name="len"></param>
		/// <returns></returns>
		public string TruncateHTML(string html, int len)
		{
			return "<p>" + Util.RemoveHtml(html, len) + "</p>";
		}

		/// <summary>
		/// Returns a random image from the current theme based on a pattern
		/// </summary>
		/// <param name="path"></param>
		/// <param name="pattern"></param>
		/// <returns></returns>
		public string RandomImage(string path, string pattern)
		{
			string newpath = ViewManager.GetFilePath(GraffitiContext.Current.Theme, path);
			DirectoryInfo di = new DirectoryInfo(newpath);
			FileInfo[] fis = di.GetFiles(pattern);

			if (fis != null && fis.Length > 0)
			{
				List<string> files = new List<string>(fis.Length);
				foreach (FileInfo fi in fis)
					files.Add(path + "/" + fi.Name);

				return ThemeFile(Util.Randomize(files.ToArray(), 1)[0]);

			}

			return null;
		}

		/// <summary>
		/// Checks to see if an object is null. If the object is a string, it will return true if it is null or empty. 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool IsNull(object obj)
		{
			bool b = (obj == null);

			if (!b && obj is string)
			{
				b = (obj as string).Length == 0;
			}

			return b;
		}

		/// <summary>
		/// Checks to if an object is not null (!IsNull)
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool IsNotNull(object obj)
		{
			return !IsNull(obj);
		}

		/// <summary>
		/// Enables parsing of text as a template on the fly.
		/// </summary>
		/// <param name="templateText"></param>
		/// <returns></returns>
		public string Parse(string templateText)
		{
			return TemplateEngine.Evaluate(templateText, GraffitiContext.Current);
		}

		/// <summary>
		/// Allows you to combine two objects and return the resulting string value
		/// </summary>
		/// <param name="leftSide">The object to appear on the left side of the concattenated string</param>
		/// <param name="rightSide">The object to appear on the right side of the concattenated string</param>
		/// <returns>A combined string representation of the two objects entered</returns>
		public string Concat(object leftSide, object rightSide)
		{
			return string.Concat(leftSide, rightSide);
		}

		#region Reflection

		/// <summary>
		/// Helper method which reflects on the current object and writes out a table. 
		/// </summary>
		public string Reflect(object obj)
		{
			return Reflect(obj, false);
		}

		/// <summary>
		/// Helper method which reflects on the current object and writes out a table. 
		/// </summary>
		public string Reflect(object obj, bool showTypes)
		{
			if (obj == null)
				return "Null Object";

			StringBuilder sb = new StringBuilder();
			sb.Append("<table>");
			sb.AppendFormat("<caption>{0}</caption>", obj.GetType().Name);
			sb.Append("<tr><th>Property</th>");
			if (showTypes)
			{
				sb.Append("<th>Type</th>");

			}

			sb.Append("</tr>");

			PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

			foreach (PropertyInfo pi in properties)
			{
				if (pi.CanRead)
				{
					if (pi.PropertyType == typeof(IList))
					{
						IList list = pi.GetValue(obj, null) as IList;
						if (list == null || list.Count == 0)
						{
							if (showTypes)
								sb.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", pi.Name, pi.PropertyType);
							else
								sb.AppendFormat("<tr><td>{0}</td></tr>", pi.Name);
						}
						else
						{
							if (showTypes)
								sb.AppendFormat("<tr><td>{0}</td><td>{1} ({2})</td></tr>", pi.Name, pi.PropertyType, list[0].GetType().Name);
							else
								sb.AppendFormat("<tr><td>{0}</td></tr>", pi.Name);

						}
					}
					else
					{
						if (showTypes)
							sb.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", pi.Name, pi.PropertyType);
						else
							sb.AppendFormat("<tr><td>{0}</td></tr>", pi.Name);
					}
				}
			}

			sb.Append("</table>");

			return sb.ToString();
		}

		#endregion

		#endregion

		#region Gravatar

		private static string defaultGravatar = null;
		//private static string defaultEnocodedGravatar  = null;

		/// <summary>
		/// Generates a gravtar image
		/// </summary>
		/// <param name="email"></param>
		/// <param name="name">Users name used for alt attribute</param>
		/// <param name="ip">Users IP Address</param>
		/// <returns></returns>
		public string Gravatar(string email, string name, string ip)
		{
			return Gravatar(email, name, ip, new Hashtable());
		}

		/// <summary>
		/// Generates a gravtar image
		/// </summary>
		public string Gravatar(string email, string name, string ip, IDictionary attributes)
		{
			string size = attributes["size"] as string;
			if (size == null)
				size = "80";
			else
				attributes.Remove("size");

			string image = GravatarImage(email, ip, size);
			return string.Format("<img src=\"{0}\" {1} width=\"{2}\" height=\"{2}\" alt=\"{3} avatar\" />", image, GetAttributes(attributes), size, name);
		}

		//Based on code by Jon Galloway
		//http://weblogs.asp.net/jgalloway/archive/2007/09/23/adding-gravatars-to-your-asp-net-site-in-a-few-lines-of-code.aspx

		/// <summary>
		/// Generates the url for the Gravtar image
		/// </summary>
		/// <param name="email">address used to look up a user's Gravtar</param>
		/// <param name="ip">users IP Address to use if Gravatar does not exist</param>
		/// <param name="size">size of image</param>
		/// <returns></returns>
		public string GravatarImage(string email, string ip, string size)
		{
			if (defaultGravatar == null)
			{
				defaultGravatar = FullUrl(VirtualPathUtility.ToAbsolute("~/__utility/img/IdenticonHandler.ashx"));
				//defaultEnocodedGravatar = HttpUtility.UrlEncode(defaultGravatar);
			}

			string identicon =
				 string.Format("{0}?code={1}&size={2}", defaultGravatar, Docuverse.Identicon.IdenticonUtil.Code(ip), size);


			if (string.IsNullOrEmpty(email))
			{
				return identicon;
			}

			identicon = HttpUtility.UrlEncode(identicon);

			string hash = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(email.Trim(), "MD5").Trim().ToLower();
			return string.Format("http://www.gravatar.com/avatar/{0}?amp;r=g&amp;s={2}&amp;d={1}", hash, identicon, size);
		}

		#endregion

		#region Logo

		/// <summary>
		/// Renders an IMG with the logo.
		/// </summary>
		public string LogoImage
		{
			get { return "<img style=\"border: none;\" id=\"graffiti_logo\" alt=\"Powered by Graffiti CMS\" src=\"" + VirtualPathUtility.ToAbsolute("~/__utility/img/logo.png") + "\" />"; }
		}

		public string Logo
		{
			get
			{
				return "<a title=\"Powered by Graffiti CMS\" href=\"http://graffiticms.codeplex.com\">" + LogoImage + "</a>";
			}
		}

		#endregion

		#region Forms

		/// <summary>
		/// Returns the template to the comment form (.view)
		/// </summary>
		/// <returns></returns>
		public string CommentForm()
		{
			return ViewManager.RenderTemplate(HttpContext.Current, GraffitiContext.Current, "~/__utility/forms/comment.view");
		}

		/// <summary>
		/// Renders the comment button wired up to the ajax handler.
		/// </summary>
		/// <returns></returns>
		public string CommentButton()
		{
			return CommentButton(new Hashtable());
		}

		/// <summary>
		/// Renders the comment button and allows additional attriubtes to be included. 
		/// </summary>
		/// <param name="dictionary"></param>
		/// <returns></returns>
		public string CommentButton(IDictionary dictionary)
		{
			if (!dictionary.Contains("value"))
				dictionary["value"] = "Submit Comment";

			if (!dictionary.Contains("id"))
				dictionary["id"] = "commentbutton";

			return
				 string.Format(
					  "<input type=\"button\" {0} onclick=\"Comments.submitComment('{1}');\" />", GetAttributes(dictionary), new Urls().Ajax);
		}

		/// <summary>
		/// Returns the contact form template (.view)
		/// </summary>
		/// <returns></returns>
		public string ContactForm()
		{
			return ViewManager.RenderTemplate(HttpContext.Current, GraffitiContext.Current, "~/__utility/forms/contact.view");
		}

		/// <summary>
		/// Renders the comment button wired up to the ajax handler.
		/// </summary>
		/// <returns></returns>
		public string ContactButton()
		{
			return ContactButton(new Hashtable());
		}

		/// <summary>
		/// Renders the comment button and allows additional attriubtes to be included. 
		/// </summary>
		/// <param name="dictionary"></param>
		/// <returns></returns>
		public string ContactButton(IDictionary dictionary)
		{
			if (!dictionary.Contains("value"))
				dictionary["value"] = "Send";

			if (!dictionary.Contains("id"))
				dictionary["id"] = "contactbutton";


			return
				 string.Format(
					  "<input type=\"button\" {0} onclick=\"Contact.submitMessage('{1}');\" />", GetAttributes(dictionary), new Urls().Ajax);
		}

		#endregion

		#region Spinner

		/// <summary>
		/// Returns the Url of the Spinner.gif image
		/// </summary>
		public string Spinner
		{
			get { return VirtualPathUtility.ToAbsolute("~/__utility/img/spinner.gif"); }
		}

		/// <summary>
		/// Renders an image element containing the Spinner Image.
		/// </summary>
		public string SpinnerImage
		{
			get { return "<img src=\"" + Spinner + "\" alt=\"Spinner\" />"; }
		}

		#endregion



		#region Favicon

		/// <summary>
		/// Writes out the link information for a favicon
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public string Favicon(string path)
		{
			if (path.StartsWith("~/"))
				path = VirtualPathUtility.ToAbsolute(path);

			return "<link rel=\"shortcut icon\" href=\"" + path + "\" type=\"image/x-icon\" />";

		}

		/// <summary>
		/// Writes out the default favicon
		/// </summary>
		/// <returns></returns>
		public string Favicon()
		{
			var icon = ConfigurationManager.AppSettings["Graffiti:Icon"];
			if (string.IsNullOrEmpty(icon))
				icon = "~/__utility/img/favicon.ico";
			return Favicon(icon);
		}

		#endregion

		#region Dates
		/// <summary>
		/// Gets a DateTime from a string
		/// </summary>
		/// <param name="dt">string to try to parse</param>
		/// <returns>Valid DateTime for string value or DateTime.MinValue</returns>
		private DateTime GetDateTimeFromString(string dt)
		{
			DateTime date = DateTime.MinValue;
			if (!String.IsNullOrEmpty(dt))
				DateTime.TryParse(dt, out date);

			return date;
		}

		/// <summary>
		/// Gets the formatted date (from SiteSettings) for the datetime
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public string FormattedDate(string dt)
		{
			return FormattedDate(GetDateTimeFromString(dt));
		}

		/// <summary>
		/// Gets the formatted date (from SiteSettings) for the datetime
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public string FormattedDate(DateTime dt)
		{
			return dt.ToString(SiteSettings.Get().DateFormat);
		}

		/// <summary>
		/// Gets the formatted time (from SiteSettings) for the datetime
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public string FormattedTime(string dt)
		{
			return FormattedTime(GetDateTimeFromString(dt));
		}

		/// <summary>
		/// Gets the formatted time (from SiteSettings) for the datetime
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public string FormattedTime(DateTime dt)
		{
			return dt.ToString(SiteSettings.Get().TimeFormat);
		}

		/// <summary>
		/// Gets the formatted date + seperator + formatted time (from SiteSettings) for the datetime
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public string FormattedTime(string dt, string seperator)
		{
			return FormattedDateTime(GetDateTimeFromString(dt), seperator);
		}

		/// <summary>
		/// Gets the formatted date + seperator + formatted time (from SiteSettings) for the datetime
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="seperator"></param>
		/// <returns></returns>
		public string FormattedDateTime(DateTime dt, string seperator)
		{
			return FormattedDate(dt) + seperator + FormattedTime(dt);
		}
		#endregion

		#region Private Helpers

		/// <summary>
		/// Helper to convert an IDictionary to list of html attributes
		/// </summary>
		/// <param name="ht"></param>
		/// <returns></returns>
		private static string GetAttributes(IDictionary ht)
		{
			if (ht == null || ht.Count == 0)
				return string.Empty;

			StringBuilder sb = new StringBuilder();
			foreach (string key in ht.Keys)
			{
				sb.AppendFormat(" {0} = \"{1}\" ", key, ht[key]);
			}

			return sb.ToString();
		}

		#endregion
	}

	/// <summary>
	/// Used to return a safe link for chalk rendering
	/// </summary>
	public class Link
	{
		private string _url;

		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}

		private string _name;

		public string Text
		{
			get { return _name; }
			set { _name = value; }
		}

		private bool _isSelected;

		public bool IsSelected
		{
			get { return _isSelected; }
			set { _isSelected = value; }
		}

		private DynamicNavigationType _navigationType;

		public DynamicNavigationType NavigationType
		{
			get { return _navigationType; }
			set { _navigationType = value; }
		}


		private int _categoryId;

		public int CategoryId
		{
			get { return _categoryId; }
			set { _categoryId = value; }
		}

		private int _postId;

		public int PostId
		{
			get { return _postId; }
			set { _postId = value; }
		}

		public Category Category
		{
			get { return CategoryId > 0 ? new CategoryController().GetCachedCategory(CategoryId, true) : null; }
		}

		public Post Post
		{
			get { return PostId > 0 ? Post.GetCachedPost(PostId) : null; }
		}

	}
}