using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Graffiti.Core;

namespace Graffiti.BlogExtensions
{
	public class TrackBackSender
	{

		#region Private Fields
		string _pageText = null;
		string _externalUrl = null;
		string _postTitle = null;
		string _postLink = null;
		string _excerpt = null;
		string _siteName = null;

		private static readonly Regex trackBackRDFRegex = new Regex(@"<rdf:\w+\s[^>]*?>(</rdf:rdf>)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Regex trackBackPingRegex = new Regex("trackback:ping=\"([^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		#endregion

		#region Constructor
		public TrackBackSender(string pageText, string externalUrl, string siteName, string postTitle, string postLink, string excerpt)
		{
			this._pageText = pageText;
			this._externalUrl = externalUrl;
			this._siteName = siteName;
			this._postTitle = postTitle;
			this._postLink = postLink;
			this._excerpt = excerpt;
		}
		#endregion

		#region public bool SendTrackBackPing()
		/// <summary>
		/// If the current site/link supports a TrackBack, we will ping the site here
		/// </summary>
		/// <param name="trackBackItem"></param>
		public bool SendTrackBackPing()
		{
			try
			{
				string trackBackItem = GetTrackBackText(_pageText, _externalUrl, _postLink);
				if (!string.IsNullOrEmpty(trackBackItem))
				{
					if (!trackBackItem.ToLower().StartsWith("http://") && !trackBackItem.ToLower().StartsWith("https://"))
						trackBackItem = "http://" + trackBackItem;

					string parameters = "title=" + HttpUtility.UrlEncode(HttpUtility.HtmlDecode(_postTitle)) + "&url=" + HttpUtility.UrlEncode(_postLink) + "&blog_name=" + HttpUtility.UrlEncode(HttpUtility.HtmlDecode(_siteName)) + "&excerpt=" + HttpUtility.UrlEncode(_excerpt);
					byte[] payload = Encoding.UTF8.GetBytes(parameters);

					HttpWebRequest request = GRequest.CreateRequest(trackBackItem, _postLink);
					request.Method = "POST";
					request.ContentLength = payload.Length;
					request.ContentType = "application/x-www-form-urlencoded";
					request.KeepAlive = false;
					request.AllowAutoRedirect = true;
					request.MaximumAutomaticRedirections = 3;

					using (Stream st = request.GetRequestStream())
					{
						st.Write(payload, 0, payload.Length);
						st.Close();

						using (WebResponse response = request.GetResponse())
						{
							response.Close();
						}
					}

					string message = String.Format("Trackback sent to {0}.", _externalUrl);
					Log.Info("Trackback Sent", message);

					return true;
				}
			}
			catch (System.Exception ex)
			{
				string message = String.Format("Trackback attempt to {0} failed. Error message returned was: {1}.", _externalUrl, ex.Message);
				Log.Warn("Trackback Error", message);
			}

			return false;
		}
		#endregion

		#region Private Methods
		private static string GetTrackBackText(string pageText, string url, string PostUrl)
		{
			if (!Regex.IsMatch(pageText, PostUrl, RegexOptions.IgnoreCase | RegexOptions.Singleline))
			{
				Match m;

				for (m = trackBackRDFRegex.Match(pageText); m.Success; m = m.NextMatch())
				{
					if (m.Groups.ToString().Length > 0)
					{

						string text = m.Groups[0].ToString();
						if (text.IndexOf(url) > 0)
						{
							Match m2 = trackBackPingRegex.Match(text);
							if (m2.Success)
							{
								return m2.Result("$1");
							}

							return text;
						}
					}
				}
			}

			return null;
		}
		#endregion

	}
}
