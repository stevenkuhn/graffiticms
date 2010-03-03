using System;
using System.Configuration;
using System.Web;
using System.Collections.Generic;

namespace Graffiti.Core
{

    /// <summary>
    /// SiteSettings is a global item that contains information about the entire site. You can access this object from the data object ($data.Site).
    /// </summary>
    [Serializable]
    public class SiteSettings : IMenuItem
    {
        private bool? _useProxyServer;
        private string _proxyHost;
        private int? _proxyPort;
        private string _proxyUsername;
        private string _proxyPassword;
        private bool? _proxyBypassOnLocal;
        private string _dateFormat;
        private string _timeFormat;
        private bool? _cacheViews;
		  private bool? _useExternalJQuery;
        private bool? _filterUncategorizedPostsFromLists;
        private bool? _includeChildPosts;
        private int? _pageSize;

        public bool RequireWWW
        {
            get
            {
                bool requireWWW;

                Boolean.TryParse(ConfigurationManager.AppSettings["Graffiti::RequireWWW"], out requireWWW);

                return requireWWW;
            }
        }

        public bool RequireSSL
        {
            get
            {
                bool requireSSL;

                Boolean.TryParse(ConfigurationManager.AppSettings["Graffiti::RequireSSL"], out requireSSL);

                return requireSSL;
            }
        }

        public bool UseProxyServer
        {
            get
            {
                if(_useProxyServer.HasValue) return _useProxyServer.Value;

                return !String.IsNullOrEmpty(ConfigurationManager.AppSettings["Graffiti:Proxy:ProxyHost"] ?? "");
            }
            set
            {
                _useProxyServer = value;
            }
        }

        public string ProxyHost
        {
            get
            {
                if (!String.IsNullOrEmpty(_proxyHost)) return _proxyHost;

                return ConfigurationManager.AppSettings["Graffiti:Proxy:ProxyHost"] ?? "";
            }
            set
            {
                _proxyHost = value;
            }
        }
 
        public int ProxyPort
        {
            get
            {
                if (_proxyPort.HasValue) return _proxyPort.Value;

                int proxyPort;

                Int32.TryParse(ConfigurationManager.AppSettings["Graffiti:Proxy:ProxyPort"], out proxyPort);

                return proxyPort == 0 ? 80 : proxyPort;
            }
            set
            {
                _proxyPort = value;
            }
        }

        public string ProxyUsername
        {
            get
            {
                if (!String.IsNullOrEmpty(_proxyUsername)) return _proxyUsername;

                return ConfigurationManager.AppSettings["Graffiti:Proxy:ProxyUsername"] ?? "";
            }
            set
            {
                _proxyUsername = value;
            }
        }

        public string ProxyPassword
        {
            get
            {
                if (!String.IsNullOrEmpty(_proxyPassword)) return _proxyPassword;

                return ConfigurationManager.AppSettings["Graffiti:Proxy:ProxyPassword"] ?? "";
            }
            set
            {
                _proxyPassword = value;
            }
        }

        public bool ProxyBypassOnLocal
        {
            get
            {
                if (_proxyBypassOnLocal.HasValue) return _proxyBypassOnLocal.Value;

                bool proxyBypassOnLocal;

                Boolean.TryParse(ConfigurationManager.AppSettings["Graffiti:Proxy:ProxyBypassOnLocal"], out proxyBypassOnLocal);

                return proxyBypassOnLocal;
            }
            set
            {
                _proxyBypassOnLocal = value;
            }
        }

        public string DateFormat
        {
            get
            {
                if (!String.IsNullOrEmpty(_dateFormat)) return _dateFormat;

                return ConfigurationManager.AppSettings["Graffiti:Format:Date"] ?? "dddd, MMMM dd yyyy";
            }
            set
            {
                _dateFormat = value;
            }
        }

        public string TimeFormat
        {
            get
            {
                if (!String.IsNullOrEmpty(_timeFormat)) return _timeFormat;

                return ConfigurationManager.AppSettings["Graffiti:Format:Time"] ?? "h:mm tt";
            }
            set
            {
                _timeFormat = value;
            }
        }

        public bool CacheViews
        {
            get
            {
                if (_cacheViews.HasValue) return _cacheViews.Value;

                bool cacheViews;

                if(String.IsNullOrEmpty(ConfigurationManager.AppSettings["Graffiti:Views:Cache"])) return true;
                
                Boolean.TryParse(ConfigurationManager.AppSettings["Graffiti:Views:Cache"], out cacheViews);

                return cacheViews;
            }
            set
            {
                _cacheViews = value;
            }
        }

        public bool UseExternalJQuery
        {
            get
            {
					if (_useExternalJQuery.HasValue) return _useExternalJQuery.Value;

                return true;
            }
            set
            {
					_useExternalJQuery = value;
            }
        }

        public bool GenerateFolders { get; set; }

        public bool FilterUncategorizedPostsFromLists
        {
            get
            {
                if (_filterUncategorizedPostsFromLists.HasValue) return _filterUncategorizedPostsFromLists.Value;

                bool filterUncategorizedPostsFromLists;

                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["Graffiti:Data:FilterUncategorizedPostsFromLists"])) return true;

                Boolean.TryParse(ConfigurationManager.AppSettings["Graffiti:Data:FilterUncategorizedPostsFromLists"], out filterUncategorizedPostsFromLists);

                return filterUncategorizedPostsFromLists;
            }
            set
            {
                _filterUncategorizedPostsFromLists = value;
            }
        }

        public bool IncludeChildPosts
        {
            get
            {
                if (_includeChildPosts.HasValue) return _includeChildPosts.Value;

                bool includeChildPosts;

                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["Graffiti:Category:IncludeChildPosts"])) return true;

                Boolean.TryParse(ConfigurationManager.AppSettings["Graffiti:Category:IncludeChildPosts"], out includeChildPosts);

                return includeChildPosts;
            }
            set
            {
                _includeChildPosts = value;
            }
        }

        public int PageSize
        {
            get
            {
                if (_pageSize.HasValue) return _pageSize.Value;

                int pageSize;

                Int32.TryParse(ConfigurationManager.AppSettings["Graffiti:Data:PageSize"], out pageSize);

                return pageSize == 0 ? 10 : pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }



        public static readonly string Version = "1.3 Alpha" ;

        public static readonly string BuildNumber = typeof (SiteSettings).Assembly.GetName().Version.ToString();

        public static readonly string VersionDescription =
            string.Format("Graffiti CMS {0} (build {1})", Version, BuildNumber);

        public static string DefaultPassword { get { return ConfigurationManager.AppSettings["Graffiti:User:DefaultPassword"]; } }

        public static int DestroyDeletedPostsOlderThanDays { get { return Int32.Parse(ConfigurationManager.AppSettings["Graffiti:Data:DestroyDeletedPostsOlderThanDays"] ?? "7"); } }

        public static DateTime CurrentUserTime
        {
            get { return DateTime.Now.AddHours(Get().TimeZoneOffSet); }
        }

		  public static bool UrlRoutingSupported
		  {
			  get
			  {
				  HttpApplicationState application = HttpContext.Current.Application;
				  Object objUrlRoutingEnabled = application["UrlRoutingEnabled"];
				  if (objUrlRoutingEnabled == null)
					  application["UrlRoutingEnabled"] = Util.CheckUrlRoutingSupport();


				  return (bool)application["UrlRoutingEnabled"];
			  }
		  }

        /// <summary>
        /// Url to append to absolute urls in posts or rss.
        /// </summary>
        public static string BaseUrl
        {
            get
            {
                string url = ConfigurationManager.AppSettings["Graffiti:BaseUrl"];
                if (url == null)
                {
                    HttpContext context = HttpContext.Current;
                    if (context != null)
                    {
                        string portInfo = context.Request.Url.Port == 80 ? string.Empty : ":" + context.Request.Url.Port;
                        url = string.Format("{0}://{1}{2}", context.Request.Url.Scheme, context.Request.Url.Host, portInfo);
                    }
                }

                return url;
            }
        }

        private string _title = "Graffiti CMS";

        /// <summary>
        /// Site title
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _tagLine = "A Graffiti CMS powered site";

        /// <summary>
        /// Site short description 
        /// </summary>
        public string TagLine
        {
            get { return _tagLine; }
            set { _tagLine =  value; }
        }

        private string _ExternalFeedUrl = null;

        /// <summary>
        /// Feedburner url
        /// </summary>
        public string ExternalFeedUrl
        {
            get { return _ExternalFeedUrl; }
            set { _ExternalFeedUrl = string.IsNullOrEmpty(value) ? null : value; }
        }

        private string _theme = "default";

        /// <summary>
        /// Current site theme. 
        /// </summary>
        public string Theme
        {
            get { return _theme; }
            set { _theme = value; }
        }

        private string _copyright = "";

        /// <summary>
        /// Site copyright 
        /// </summary>
        public string CopyRight
        {
            get { return _copyright; }
            set { _copyright = string.IsNullOrEmpty(value) ? null : value; }
        }

        private double  _timeZoneOffSet = 0;


        /// <summary>
        /// Timezone offset. 
        /// </summary>
        public double  TimeZoneOffSet
        {
            get { return _timeZoneOffSet; }
            set { _timeZoneOffSet = value; }
        }

        private string _header;

        /// <summary>
        /// Additional HTML/Javascript for the page header
        /// </summary>
        public string Header
        {
            get { return _header; }
            set { _header = string.IsNullOrEmpty(value) ? null : value; }
        }

        private string _stats;

        /// <summary>
        /// Javascript for the site statistics. 
        /// </summary>
        public string WebStatistics
        {
            get { return _stats; }
            set { _stats = string.IsNullOrEmpty(value) ? null : value; }
        }

        private int _featuredId;

        /// <summary>
        /// Id of the sites featured post. 
        /// </summary>
        public int FeaturedId
        {
            get { return _featuredId; }
            set { _featuredId = value; }
        }

        private string _emailServer = "localhost";

        /// <summary>
        /// SMTP Server address
        /// </summary>
        public string EmailServer
        {
            get { return _emailServer; }
            set { _emailServer = string.IsNullOrEmpty(value) ? null : value; }
        }

        private string _emailFrom = "graffiti@yoursite.com";

        /// <summary>
        /// From address on system emails. 
        /// </summary>
        public string EmailFrom
        {
            get { return _emailFrom; }
            set { _emailFrom = string.IsNullOrEmpty(value) ? null : value; }
        }

        private bool _emailRequiresSSL;

        /// <summary>
        /// If the server requires ssl for outgoing message.
        /// </summary>
        public bool EmailRequiresSSL
        {
            get { return _emailRequiresSSL; }
            set { _emailRequiresSSL = value; }
        }

        private bool _emailServerRequiresAuthentication;

        /// <summary>
        /// Does the SMTP Server require a password.
        /// </summary>
        public bool EmailServerRequiresAuthentication
        {
            get { return _emailServerRequiresAuthentication; }
            set { _emailServerRequiresAuthentication = value; }
        }

        private int _emailPort = 25;

        /// <summary>
        /// The port to send the email from.
        /// </summary>
        public int EmailPort
        {
            get { return _emailPort; }
            set { _emailPort = value; }
        }

        private string _emailUser;

        /// <summary>
        /// SMTP server user name
        /// </summary>
        public string EmailUser
        {
            get { return _emailUser; }
            set { _emailUser = string.IsNullOrEmpty(value) ? null : value; }
        }

        private string _emailPassword;

        /// <summary>
        /// Password for the email server (to send only)
        /// </summary>
        public string EmailPassword
        {
            get { return _emailPassword; }
            set { _emailPassword = string.IsNullOrEmpty(value) ? null : value; }
        }

        private string  _metaDescription;

        /// <summary>
        /// Global Meta Description
        /// </summary>
        public string  MetaDescription
        {
            get { return _metaDescription; }
            set { _metaDescription = value; }
        }

        private string _metaKeyWords;

        /// <summary>
        /// Global Meta Keywords
        /// </summary>
        public string MetaKeywords
        {
            get { return _metaKeyWords; }
            set { _metaKeyWords = value; }
        }

        private bool _useCustomHomeList;

        /// <summary>
        /// Should a custom list of posts be used on the home page?
        /// </summary>
        public bool UseCustomHomeList
        {
            get { return _useCustomHomeList; }
            set { _useCustomHomeList = value; }
        }

        private bool _displayGraffitiLogo = true;

        /// <summary>
        /// Shows the graffiti logo
        /// </summary>
        public bool DisplayGraffitiLogo
        {
            get { return _displayGraffitiLogo; }
            set { _displayGraffitiLogo = value; }
        }


        public void Save()
        {
            ObjectManager.Save(this,"sitesettings");
        }

        public static SiteSettings Get()
        {
            return ObjectManager.Get<SiteSettings>("sitesettings");
        }

        #region IMenuItem Members

        public List<MenuItem> GetMenuItems()
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            menuItems.Add(new MenuItem("Title", "$data.Site.Title", "Your site title", "Settings"));
            menuItems.Add(new MenuItem("TagLine", "$data.Site.TagLine", "Your site\'s tagline", "Settings"));
            menuItems.Add(new MenuItem("ExternalFeedUrl", "$data.Site.ExternalFeedUrl", "Your feedburner (optional) url", "Settings"));
            menuItems.Add(new MenuItem("Theme", "$data.Site.Theme", "Your current theme", "Settings"));
            menuItems.Add(new MenuItem("CopyRight", "$data.Site.CopyRight", "Your site\'s copyright notice", "Settings"));
            menuItems.Add(new MenuItem("Header", "$data.Site.Header", "Any optional tagselements you wish to render in the site\'s header", "Settings"));
            menuItems.Add(new MenuItem("WebStatistics", "$data.Site.WebStatistics", "Renders any site tracking (Google Analytics) code", "Settings"));

            return menuItems;
        }

        #endregion
    }
}