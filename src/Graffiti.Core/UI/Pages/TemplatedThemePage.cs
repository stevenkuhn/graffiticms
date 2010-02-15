using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;

namespace Graffiti.Core
{
	/// <summary>
	/// Base page class used by all Graffiti pages. 
	/// </summary>
	public abstract class TemplatedThemePage : GraffitiPage
	{
		public int CategoryID = -1;
		public int PostId = -1;
		public string Name = null;
		public string TagName, RedirectUrl, PostName, CategoryName, MetaDescription, MetaKeywords;

		private bool _isInexed = true;
		public virtual bool IsIndexable
		{
			get { return _isInexed; }
			set { _isInexed = value; }
		}


		private static Dictionary<string, bool> fileCache = new Dictionary<string, bool>();

		/// <summary>
		/// Checks to see if an optionally named/existing theme exists.
		/// </summary>
		protected bool ViewExists(string viewName)
		{
			if (SiteSettings.Get().CacheViews)
			{
				if (!fileCache.ContainsKey(ThemeName + ":" + viewName))
				{
					fileCache[ThemeName + ":" + viewName] = ViewManager.Exists(ThemeName, viewName);
				}

				return fileCache[ThemeName + ":" + viewName];
			}
			else
				return ViewManager.Exists(ThemeName, viewName);
		}

		/// <summary>
		/// We use a 1 based page index
		/// </summary>
		protected int PageIndex
		{
			get { return Int32.Parse(Context.Request.QueryString["p"] ?? "1"); }
		}

		/// <summary>
		/// Current view
		/// </summary>
		protected virtual string ViewName { get { return ViewLookUp(".view", null); } }

		protected virtual string ViewLookUp(string baseName, string defaultViewName)
		{
			if (defaultViewName == null)
				return "index" + baseName;
			else
				return defaultViewName;
		}

		/// <summary>
		/// Overridable in subclasses. Should be used to add custom data to the graffitiContext.
		/// </summary>
		/// <param name="graffitiContext">The context passed to the view at runtime.</param>
		protected virtual void LoadContent(GraffitiContext graffitiContext)
		{
		}

		/// <summary>
		/// Used by page templates to set default values
		/// </summary>
		protected virtual void Initialize()
		{
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Initialize();

			if (string.IsNullOrEmpty(RedirectUrl))
			{

				GraffitiContext graffitiContext = GraffitiContext.Current;
				SetContextDefault(graffitiContext, ViewName);

				LoadContent(graffitiContext);

				if (!RolePermissionManager.GetPermissions(CategoryID, GraffitiUsers.Current, graffitiContext["where"].ToString() == "home" ||
																		  graffitiContext["where"].ToString() == "search").Read)
					Response.Redirect(ResolveUrl("~/access-denied/"));

				ViewManager.Render(Context, graffitiContext, ThemeName);
			}
			else
			{
				RedirectTo(VirtualPathUtility.ToAbsolute(RedirectUrl));
			}
		}

		protected void RedirectTo(string absoluteUrl)
		{
			string fullUrl = new Uri(Context.Request.Url, absoluteUrl).ToString();

			Context.Response.RedirectLocation = fullUrl;
			Context.Response.StatusCode = 301;
			Context.Response.End();
		}

		static readonly Macros macros = new Macros();

		/// <summary>
		/// Loads/sets all global default content
		/// </summary>
		/// <param name="graffitiContext">Current context</param>
		/// <param name="view">Name of the view requested</param>
		protected virtual void SetContextDefault(GraffitiContext graffitiContext, string view)
		{
			graffitiContext["request"] = Context.Request;
			graffitiContext["response"] = Context.Response;
			graffitiContext["url"] = Context.Request.RawUrl.ToLower().Replace(Util.DEFAULT_PAGE_LOWERED, string.Empty);
			graffitiContext["pageIndex"] = Int32.Parse(Context.Request.QueryString["p"] ?? "1");
			graffitiContext["isUser"] = Context.Request.IsAuthenticated;
			graffitiContext["user"] = GraffitiUsers.Current;
			graffitiContext["categoryID"] = CategoryID;
			graffitiContext["postID"] = PostId;
			graffitiContext["tagName"] = TagName;
			graffitiContext.Layout = ViewLookUp(".layout.view", "layout.view");
			graffitiContext.View = view;
			SetDataTypeHelpers(graffitiContext);

			graffitiContext.RegisterOnRequestDelegate("header", NamedViewLoader);
			graffitiContext.RegisterOnRequestDelegate("navigation", NamedViewLoader);
			graffitiContext.RegisterOnRequestDelegate("sidebar", NamedViewLoader);
			graffitiContext.RegisterOnRequestDelegate("left-sidebar", NamedViewLoader);
			graffitiContext.RegisterOnRequestDelegate("right-sidebar", NamedViewLoader);
			graffitiContext.RegisterOnRequestDelegate("footer", NamedViewLoader);
		}

		protected void SetDataTypeHelpers(GraffitiContext graffitiContext)
		{
			object[] builtInHelpers = new object[]
			{
				new StaticAccessorHelper<Byte>(),
				new StaticAccessorHelper<SByte>(),
				new StaticAccessorHelper<Int16>(),
				new StaticAccessorHelper<Int32>(),
				new StaticAccessorHelper<Int64>(),
				new StaticAccessorHelper<UInt16>(),
				new StaticAccessorHelper<UInt32>(),
				new StaticAccessorHelper<UInt64>(),
				new StaticAccessorHelper<Single>(),
				new StaticAccessorHelper<Double>(),
				new StaticAccessorHelper<Boolean>(),
				new StaticAccessorHelper<Char>(),
				new StaticAccessorHelper<Decimal>(),
				new StaticAccessorHelper<String>(),
				new StaticAccessorHelper<Guid>(),
				new StaticAccessorHelper<DateTime>()
			};

			foreach (object helper in builtInHelpers)
			{
				graffitiContext[helper.GetType().GetGenericArguments()[0].Name] = helper;
			}
		}

		protected virtual object NamedViewLoader(string key, GraffitiContext graffitiContext)
		{
			string the_View_Name = ViewLookUp("." + key + ".view", key + ".view");
			return macros.LoadThemeView(the_View_Name);
		}

		/// <summary>
		/// Current theme
		/// </summary>
		protected virtual string ThemeName
		{
			get { return GraffitiContext.Current.Theme; }
		}
	}
}