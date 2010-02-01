using System;
using System.IO;
using System.Net;
using System.Security;
using System.Text;

namespace Graffiti.Core
{
    public class GRequest
    {

        #region Private Fields

        private const int defaultTimeout = 60000;
        protected static int defaultMaxAutoRedirects = 10;
        private static string referer = "Graffiti CMS";
        private static readonly string userAgent = String.Format("Graffiti CMS {0} (build {1}; {2}; .NET CLR {3};)", SiteSettings.Version, SiteSettings.BuildNumber, Environment.OSVersion.ToString(), Environment.Version.ToString());
        protected static WebProxy webProxy = null;

        #endregion

        #region Constructor
        static GRequest()
        {
            if (!string.IsNullOrEmpty(SiteSettings.Get().ProxyHost))
            {
                WebProxy proxy = new WebProxy(SiteSettings.Get().ProxyHost, SiteSettings.Get().ProxyPort);

                proxy.BypassProxyOnLocal = SiteSettings.Get().ProxyBypassOnLocal;

                if (SiteSettings.Get().ProxyUsername != string.Empty)
                    proxy.Credentials = new NetworkCredential(SiteSettings.Get().ProxyUsername, SiteSettings.Get().ProxyPassword);

                WebProxy = proxy;
            }
        }
        #endregion

        #region Public Properties
        public static string UserAgent
        {
            get { return userAgent; }
        }

        #endregion

        #region CreateRequest(...)

        /// <summary>
        /// Creates a new HttpRequest with the default Referral value
        /// </summary>
        public static HttpWebRequest CreateRequest(string url)
        {
            return CreateRequest(url, referer);
        }

        /// <summary>
        /// Creates a new general purpose HttpRequest and sets the referral value.
        /// </summary>
        public static HttpWebRequest CreateRequest(string url, string referral)
        {
            ICredentials credentials = null;

            //note this code will not work if there is an @ or : in the username or password
            if (url.IndexOf('@') > 0)
            {
                string[] urlparts = url.Split('@');
                if (urlparts.Length >= 2)
                {
                    string[] userparts = urlparts[0].Split(':');

                    if (userparts.Length == 3)
                    {
                        //string protocol = userparts[0];
                        string username = userparts[1].TrimStart('/');
                        string password = userparts[2];

                        credentials = new NetworkCredential(username, password);
                        url = url.Replace(string.Format("{0}:{1}@", username, password), "");
                    }

                }
            }
            else
            {
                credentials = CredentialCache.DefaultCredentials;
            }

            WebRequest req;

            // This may throw a SecurityException if under medium trust... should set it to null so it will return instead of error out.
            try { req = WebRequest.Create(url); }
            catch (SecurityException) { req = null; }

            HttpWebRequest wreq = req as HttpWebRequest;
            if (null != wreq)
            {
                wreq.UserAgent = userAgent;
                wreq.Referer = referral;

                // Set some reasonable limits on resources used by this request
                wreq.Timeout = defaultTimeout;
                wreq.MaximumAutomaticRedirections = defaultMaxAutoRedirects;
                wreq.KeepAlive = false;

                if (credentials != null)
                    wreq.Credentials = credentials;

                if (HasWebProxy)
                    wreq.Proxy = WebProxy;
            }
            return wreq;
        }

        #endregion

        #region Proxies

        public static WebProxy WebProxy
        {
            get { return webProxy; }
            set { webProxy = value; }
        }

        public static bool HasWebProxy
        {
            get { return webProxy != null; }
        }

        #endregion

        #region GetResponse(...)
        /// <summary>
        /// Gets the HttpResponse using the default referral
        /// </summary>
        public static HttpWebResponse GetResponse(string url)
        {
            return GetResponse(url, referer);
        }

        /// <summary>
        /// Gets the HttpResponse using the supplied referral
        /// </summary>
        public static HttpWebResponse GetResponse(string url, string referral)
        {
            HttpWebRequest request = CreateRequest(url, referral);
            return (HttpWebResponse)request.GetResponse();
        }
        #endregion

        #region GetPageText(...)
        /// <summary>
        /// Gets the full text at the url parameter
        /// </summary>
        public static string GetPageText(string url)
        {
            return GetPageText(url, referer);
        }

        /// <summary>
        /// Gets the full text at the url parameter
        /// </summary>
        public static string GetPageText(string url, string referral)
        {
            HttpWebResponse response = GetResponse(url, referral);
            return GetPageText(response);
        }

        /// <summary>
        /// Gets the full text at the url parameter
        /// </summary>
        public static string GetPageText(HttpWebResponse response)
        {
            using (Stream s = response.GetResponseStream())
            {
                string enc = response.ContentEncoding.Trim();
                if (enc == "")
                    enc = "us-ascii";
                Encoding encode = System.Text.Encoding.GetEncoding(enc);
                using (StreamReader sr = new StreamReader(s, encode))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        #endregion

        #region Safe/Restricted methods for Trackbacks & Pingbacks

        /// <summary>
        /// Gets the full text at the url parameter in a safe way to prevent DoS attacks for trackbacks
        /// </summary>
        public static string GetSafePageText(string url, string referral)
        {
            HttpWebResponse response = GetSafeResponse(url, referral);
            if (response != null)
            {
                if (IsContentTypeValid(response.ContentType, ContentTypeCategory.HTML) && response.ContentLength < 300000)
                    return GetPageText(response);

                response.Close();
            }
            return null;
        }

        /// <summary>
        /// Gets the HttpResponse using the supplied referral
        /// </summary>
        public static HttpWebResponse GetSafeResponse(string url, string referral)
        {
            HttpWebRequest request = CreateSafeRequest(url, referral);
            return (HttpWebResponse)request.GetResponse();
        }

        /// <summary>
        /// Creates a new restricted HttpRequest for trackbacks and pingbacks, and sets the referral value.
        /// </summary>
        public static HttpWebRequest CreateSafeRequest(string url, string referral)
        {
            HttpWebRequest wreq = CreateRequest(url, referral);
            if (wreq != null)
            {
                wreq.Timeout = 15000;
                wreq.MaximumAutomaticRedirections = 1;
                wreq.MaximumResponseHeadersLength = 32;
            }

            return wreq;
        }

        public static bool IsContentTypeValid(string contentType, ContentTypeCategory desiredContentType)
        {
            if (!string.IsNullOrEmpty(contentType))
            {
                // ContentType header may be returned in the format "text/html; ...."
                string ct = contentType.IndexOf(';') == -1 ? contentType.Trim().ToLower() : contentType.Substring(0, contentType.IndexOf(';')).Trim().ToLower();

                switch (desiredContentType)
                {
                    case ContentTypeCategory.HTML:
                        {
                            if (ct.CompareTo("text/html") == 0
                                || ct.CompareTo("text/xhtml") == 0
                                || ct.CompareTo("application/xhtml+xml") == 0
                                || ct.CompareTo("text/plain") == 0
                                || ct.CompareTo("text/sgml") == 0
                                || ct.CompareTo("text/xml") == 0)
                                return true;
                            break;
                        }
                }
            }
            return false;
        }

        public enum ContentTypeCategory
        {
            HTML
        }

        #endregion

    }
}
