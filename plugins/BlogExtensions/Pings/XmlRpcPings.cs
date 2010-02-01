using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Web;
using CookComputing.XmlRpc;
using Graffiti.Core;

namespace Graffiti.BlogExtensions
{
	[XmlRpcMissingMapping(MappingAction.Ignore)]
	public struct PingResult
	{
		public XmlRpcBoolean flerror;
		public string message;
	}

	/// <summary>
	/// Enables pinging of community blogging sites such as weblogs.com after the weblog is updated.
	/// </summary>
	[Serializable]
	public class XmlRpcPings : XmlRpcClientProtocol
	{

		#region Private Fields

		private string siteName = string.Empty;
		private string siteUrl = string.Empty;
		private string siteFeedUrl = string.Empty;
		private string[] pingServiceUrls = null;

		#endregion

		#region Constructor

		public XmlRpcPings(string name, string url, string feedUrl, string[] pingUrls)
		{
			this.siteName = name;
			this.siteUrl = url;
			this.siteFeedUrl = feedUrl;
			this.pingServiceUrls = pingUrls;

			this.UserAgent = SiteSettings.VersionDescription;
			this.Timeout = 60000;

			// This improves compatibility with XML-RPC servers that do not fully comply with the XML-RPC specification.
			this.NonStandard = XmlRpcNonStandard.All;

			// Use a proxy server if one has been configured
			SiteSettings siteSettings = SiteSettings.Get();
			if (siteSettings.ProxyHost != string.Empty)
			{
				WebProxy proxy = new WebProxy(siteSettings.ProxyHost, siteSettings.ProxyPort);
				proxy.BypassProxyOnLocal = siteSettings.ProxyBypassOnLocal;

				if (siteSettings.ProxyUsername != string.Empty)
					proxy.Credentials = new NetworkCredential(siteSettings.ProxyUsername, siteSettings.ProxyPassword);

				this.Proxy = proxy;
			}
		}

		#endregion

		public static void SendPings(Post post, string pingUrls)
		{
			// Split ping urls into string array
			string[] pingUrlsArray = pingUrls.Split('\n');

			// Gather information to pass to ping service(s)
			string name = SiteSettings.Get().Title;
			string url = post.Category.IsUncategorized ? new Macros().FullUrl(new Urls().Home) : new Macros().FullUrl(post.Category.Url);
			string feedUrl = SiteSettings.Get().ExternalFeedUrl ?? new Macros().FullUrl(VirtualPathUtility.ToAbsolute("~/feed/"));

			XmlRpcPings pinger = new XmlRpcPings(name, url, feedUrl, pingUrlsArray);
			ManagedThreadPool.QueueUserWorkItem(new WaitCallback(pinger.SendPings));
		}

		public void SendPings(object state)
		{
			if (pingServiceUrls == null || pingServiceUrls.Length == 0)
				return;

			foreach (string pingUrl in pingServiceUrls)
			{
				if (!string.IsNullOrEmpty(pingUrl))
				{
					this.Url = pingUrl.Trim();
					bool result = false;
					string errorMessage = string.Empty;

					// Try to remember if the last ping to this URL supported the ExtendedPing spec, so we don't send out unnecessary pings repeatdly
					bool isExtendedPing = true;
					string lastPingResult = ZCache.Get<string>(CacheKey(pingUrl));
					if (!string.IsNullOrEmpty(lastPingResult))
					{
						try { isExtendedPing = bool.Parse(lastPingResult); }
						catch { }
					}

					if (isExtendedPing)
					{
						try
						{
							PingResult response = ExtendedPing(this.siteName, this.siteUrl, this.siteUrl, this.siteFeedUrl);
							if (response.flerror != null && response.flerror == true)
							{
								if (string.IsNullOrEmpty(response.message))
									errorMessage = "Remote weblogUpdates.extendedPing service indicated an error but provided no error message.";
								else
									errorMessage = response.message;
							}
							else
							{
								result = true;
							}
						}
						catch (CookComputing.XmlRpc.XmlRpcException ex)
						{
							errorMessage = ex.Message;
						}
						catch (System.Exception ex)
						{
							errorMessage = ex.Message;
						}

						// If the ExtendedPing request failed, try a basic ping to this url
						if (!result)
							isExtendedPing = false;
					}

					if (!isExtendedPing)
					{
						try
						{
							PingResult response = BasicPing(this.siteName, this.siteUrl);
							if (response.flerror != null && response.flerror == true)
							{
								if (string.IsNullOrEmpty(response.message))
									errorMessage = "Remote weblogUpdates.ping service indicated an error but provided no error message.";
								else
									errorMessage = response.message;
							}
							else
							{
								result = true;
							}
						}
						catch (CookComputing.XmlRpc.XmlRpcException ex)
						{
							errorMessage = ex.Message;
						}
						catch (System.Exception ex)
						{
							errorMessage = ex.Message;
						}
					}


					// Log succcess or failure to EventLog
					if (result)
					{
						// Remember whether extended or basic ping worked for this url in the future
						ZCache.InsertCache(CacheKey(pingUrl), isExtendedPing.ToString(), 43200);

						string message = String.Format("Blog Ping sent to {0}.", pingUrl);
						Log.Info("Ping Sent", message);
					}
					else
					{
						string message = String.Format("Blog Ping attempt to the url {0} failed. Error message returned was: {1}.", pingUrl, errorMessage);
						Log.Warn("Ping Error", message);
					}

				}
			}

		}

		private string CacheKey(string url)
		{
			return string.Format("BlogExtensions:Pings:{0}", url);
		}


		[XmlRpcMethod("weblogUpdates.ping")]
		public PingResult BasicPing(string name, string url)
		{
			return (PingResult)Invoke("BasicPing", new Object[] { name, url });
		}

		[XmlRpcMethod("weblogUpdates.extendedPing")]
		public PingResult ExtendedPing(string name, string url, string checkUrl, string feedUrl)
		{
			return (PingResult)Invoke("ExtendedPing", new Object[] { name, url, checkUrl, feedUrl });
		}

	}
}
