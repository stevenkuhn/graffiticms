using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using NVelocity;

namespace Graffiti.Core
{
	public delegate object OnRequestDelegate(string key, GraffitiContext context);


	/// <summary>
	/// Summary description for GraffitiContext
	/// </summary>
	public class GraffitiContext : VelocityContext
	{
		private static readonly bool _isFullTrust = false;
		static GraffitiContext()
		{
			/*
			_isFullTrust = Util.IsFullTrust;

			if (!_isFullTrust)
			{
				Log.Warn("Security", "Your site is operating under Medium trust. Custom toolbox items cannot be loaded");
			}
			*/
		}

		private Dictionary<string, OnRequestDelegate> _LateBoundContextItems = new Dictionary<string, OnRequestDelegate>();

		private HttpContext _context = null;
		//private ToolboxContext toolboxContext = null;

		[ThreadStatic]
		private static GraffitiContext currentContext = null;

		//private RequestToolboxContext rtc = null;
		private ApplicationToolboxContext atc = null;

		private GraffitiContext(HttpContext context)
		{
			_context = context;

			atc = ToolboxManager.GetApplicationToolbox();
			currentContext = this;
		}

		public override object InternalGet(string key)
		{
			object obj = null;
			//if (rtc != null)
			//    obj = rtc.Get(key);

			if (atc != null)
				obj = atc.Get(key);

			if (obj == null)
				obj = base.InternalGet(key);

			if (obj != null && obj is OnRequestDelegate)
			{
				obj = ((OnRequestDelegate)obj).Invoke(key, this);
				Put(key, obj);
			}

			if (obj == null)
			{
				if (_LateBoundContextItems.ContainsKey(key))
				{
					OnRequestDelegate lbc = _LateBoundContextItems[key];
					obj = lbc.Invoke(key, this);
					Put(key, obj);
				}
			}

			return obj;
		}

		public void RegisterOnRequestDelegate(string key, OnRequestDelegate item)
		{
			_LateBoundContextItems[key] = item;
		}


		public object this[string key]
		{
			get { return Get(key); }
			set { Put(key, value); }
		}

		public static GraffitiContext Current
		{
			get { return currentContext; }
		}

		public static void Unload()
		{
			currentContext = null;
		}

		public static GraffitiContext Create(HttpContext context)
		{
			GraffitiContext gContext = new GraffitiContext(context);

			// Allow plugins to modify the GraffitiContext after it is loaded
			Graffiti.Core.Events.Instance().ExecuteLoadGraffitiContext(gContext);

			return gContext;
		}

		private string _theme;

		public string Theme
		{
			get { return _theme ?? SiteSettings.Get().Theme; }
			set { _theme = value; }
		}


		public string View
		{
			get { return Get("view") as string; }
			set { Put("view", value); }
		}

		public string Layout
		{
			get { return Get("layout") as string; }
			set { Put("layout", value); }
		}

		public int PageIndex
		{
			get { return (int)(Get("pageIndex") ?? -1); }
			set { Put("pageIndex", value); }
		}

		public int TotalRecords
		{
			get { return (int)(Get("totalRecords") ?? -1); }
			set { Put("totalRecords", value); }
		}

		public int PageSize
		{
			get { return (int)(Get("pageSize") ?? 10); }
			set { Put("pageSize", value); }
		}

		public ThemeConfigurationData ThemeConfiguration
		{
			get
			{
				ThemeConfigurationData config = Get("themeConfiguration") as ThemeConfigurationData;
				if (config == null)
				{
					config = ObjectManager.Get<ThemeConfigurationData>(ThemeConfigurationData.GetKey(this.Theme));
					config.Theme = this.Theme;
				}

				return config;
			}
		}
	}


	public class ApplicationToolboxContext : VelocityContext
	{

	}

	public class RequestToolboxContext : VelocityContext
	{

	}

	public class PageTemplateToolboxContext : VelocityContext
	{
		public PageTemplateToolboxContext()
		{
			Put("date", DateTime.Now);
			Put("macros", new Macros());
			Put("settings", SiteSettings.Get());
		}
	}

	public class EmailTemplateToolboxContext : VelocityContext
	{
		public EmailTemplateToolboxContext()
		{
			Put("date", DateTime.Now);
			Put("macros", new Macros());
			Put("settings", SiteSettings.Get());
		}
	}

}