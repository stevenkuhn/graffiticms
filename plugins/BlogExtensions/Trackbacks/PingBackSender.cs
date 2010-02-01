using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Graffiti.Core;
using CookComputing.XmlRpc;

namespace Graffiti.BlogExtensions
{
	public class PingBackSender
	{

		#region Private Fields
		WebHeaderCollection _headers = null;
		string _pageText = null;
		string _externalUrl = null;
		string _postTitle = null;
		string _postLink = null;

		private static readonly Regex pingbackLinkElementRegex = new Regex("<link rel=\"pingback\" href=\"([^\"]+)\" ?/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		#endregion

		#region Constructor
		public PingBackSender(WebHeaderCollection headers, string pageText, string externalUrl, string postTitle, string postLink)
		{
			this._headers = headers;
			this._pageText = pageText;
			this._externalUrl = externalUrl;
			this._postTitle = postTitle;
			this._postLink = postLink;
		}
		#endregion

		public bool SendPingbackPing()
		{
			try
			{
				string pingBackServerURI = GetPingBackServerURI(_headers, _pageText);
				if (!string.IsNullOrEmpty(pingBackServerURI))
				{
					PingBackClientProxy pingBackClient = new PingBackClientProxy(pingBackServerURI.Trim());
					pingBackClient.Ping(_postLink, _externalUrl);

					string message = String.Format("Pingback sent to {0} for post {1}.", this._externalUrl, this._postTitle);
					Log.Info("Pingback Sent", message);

					return true;
				}
			}
			catch (CookComputing.XmlRpc.XmlRpcException ex)
			{
				string message = String.Format("Pingback attempt to {0} failed for post [{1}]. Error message returned was: {2}.", this._externalUrl, this._postTitle, ex.Message);
				Log.Warn("Pingback Error", message);
			}
			catch (CookComputing.XmlRpc.XmlRpcFaultException ex)
			{
				string message = String.Format("Pingback attempt to {0} failed for post [{1}]. Error message returned was: {2}.", this._externalUrl, this._postTitle, ex.Message);
				Log.Warn("Pingback Error", message);
			}
			catch (System.Exception ex)
			{
				string message = String.Format("Pingback attempt to {0} failed for post [{1}]. Error message returned was: {2}.", this._externalUrl, this._postTitle, ex.Message);
				Log.Warn("Pingback Error", message);
			}

			return false;

		}

		#region Private Methods
		private static string GetPingBackServerURI(WebHeaderCollection headers, string pageText)
		{
			string pingBackServerURI = null;

			// First look for the X-Pingback HTTP Header
			if (headers != null && headers.HasKeys())
			{
				foreach (string key in headers.Keys)
				{
					if (key.ToLower().Trim() == "x-pingback")
					{
						pingBackServerURI = headers[key];
						break;
					}
				}
			}

			// If the HTTP header was not found, look for the PingBack <Link> element in the body
			if (string.IsNullOrEmpty(pingBackServerURI) && !string.IsNullOrEmpty(pageText))
			{
				Match m = pingbackLinkElementRegex.Match(pageText);
				if (m.Success)
					pingBackServerURI = m.Result("$1");
			}

			return pingBackServerURI;
		}

		#endregion

	}
}
