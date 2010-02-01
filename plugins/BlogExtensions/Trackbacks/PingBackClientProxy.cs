using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Graffiti.Core;
using CookComputing.XmlRpc;

namespace Graffiti.BlogExtensions
{
	/// <summary>
	/// PingBack XML-RPC client proxy
	/// </summary>
	public class PingBackClientProxy : XmlRpcClientProtocol
	{
		public PingBackClientProxy(string remoteServerURI)
		{
			this.UserAgent = GRequest.UserAgent;
			this.Timeout = 60000;
			this.Url = remoteServerURI;

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

		/// <summary>
		/// Sends a PingBack request using XML-RPC 
		/// </summary>
		/// <param name="sourceURI">The absolute URI of the post on the source page containing the link to the target site.</param>
		/// <param name="targetURI">The absolute URI of the target of the link, as given on the source page.</param>
		[XmlRpcMethod("pingback.ping", Description = "Notifies the server that a link has been added to sourceURI, pointing to targetURI.")]
		[return: XmlRpcReturnValue(Description = "A Message String")]
		public string Ping(string sourceURI, string targetURI)
		{
			return (string)Invoke("Ping", new object[] { sourceURI, targetURI });
		}

	}
}
