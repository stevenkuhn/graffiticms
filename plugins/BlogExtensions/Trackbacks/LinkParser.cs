using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Graffiti.Core;

namespace Graffiti.BlogExtensions
{
	/// <summary>
	/// Parses a post for links and sends trackbacks/pingbacks where possible
	/// </summary>
	public class LinkParser
	{

		private string _siteName = null;
		private string _postTitle = null;
		private string _postLink = null;
		private string _postBody = null;
		private string _excerpt = null;
		private string _baseUrl = null;

		private static readonly Regex linksRegex = new Regex(@"(?:[hH][rR][eE][fF]\s*=)(?:[\s""']*)(?!#|[Mm]ailto|[lL]ocation.|[jJ]avascript|.*css|.*this\.)(.*?)(?:[\s>""'])"
							 , RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		/// Creates a new instance of LinkParser setting the values needed to complete all the TrackBacks and PingBacks
		/// </summary>
		public LinkParser(string siteName, string postTitle, string postLink, string postBody, string excerpt, string baseUrl)
		{
			this._siteName = siteName;
			this._postTitle = postTitle;
			this._postLink = postLink;
			this._postBody = postBody;
			this._excerpt = excerpt;
			this._baseUrl = baseUrl;
		}


		public static void CheckPost(Post post)
		{
			LinkParser lp = new LinkParser(SiteSettings.Get().Title, post.Title, new Macros().FullUrl(post.Url), post.Body, post.Excerpt("", "", "", 300), SiteSettings.BaseUrl);
			ManagedThreadPool.QueueUserWorkItem(new WaitCallback(lp.CheckPost));
		}

		/// <summary>
		/// Creates an array of links based on the post's body. Then walks through each link and attempts to send trackbacks/pingbacks
		/// </summary>
		public void CheckPost(object state)
		{
			List<string> links = GetLinks(_postBody, _baseUrl);
			foreach (string externalUrl in links)
			{
				if (string.IsNullOrEmpty(externalUrl))
					continue;

				try
				{
					WebHeaderCollection headers = null;
					string pageText = null;

					using (HttpWebResponse response = GRequest.GetResponse(externalUrl, _postLink))
					{
						headers = response.Headers;
						pageText = GRequest.GetPageText(response);
						response.Close();
					}

					if (!string.IsNullOrEmpty(pageText))
					{
						bool pingSent = false;

						// First try to send a TrackBack if the required RDF info was embedded in the HTML
						TrackBackSender trackBack = new TrackBackSender(pageText, externalUrl, _siteName, _postTitle, _postLink, _excerpt);
						pingSent = trackBack.SendTrackBackPing();

						// If the Trackback attempt failed, try to do a Pingback
						// (if the required PingBack XML-RPC service URL exists in the HTTP Header or HTML Head)
						if (!pingSent)
						{
							PingBackSender pingBack = new PingBackSender(headers, pageText, externalUrl, _postTitle, _postLink);
							pingSent = pingBack.SendPingbackPing();
						}

					}
				}
				catch (System.Exception ex)
				{
					string message = String.Format("Trackback/Pingback attempt to the url [{0}] failed for post {1} while retrieving the remote document. Error message returned was: {2}.", externalUrl, this._postTitle, ex.Message);
					Log.Warn("Trackback/Pingback Error", message);
				}
			}

		}

		/// <summary>
		/// Gets a list of all of the valid html links from a string
		/// </summary>
		/// <param name="text"></param>
		internal static List<string> GetLinks(string text, string baseUrl)
		{
			text = Util.FullyQualifyRelativeUrls(text, baseUrl);

			List<string> links = new List<string>();

			Match m;
			string link = null;
			for (m = linksRegex.Match(text); m.Success; m = m.NextMatch())
			{

				if (m.Groups.ToString().Length > 0)
				{

					link = m.Groups[1].ToString();
					if (!links.Contains(link))
					{
						links.Add(link);
					}
				}
			}
			return links;
		}

		/// <summary>
		/// Retrieves a remote html page and determines if it contains a link to a given target url. Also gets the page title.
		/// </summary>
		internal static bool SourceContainsTarget(string sourceURI, string targetURI, out string pageTitle)
		{
			pageTitle = string.Empty;
			Uri baseUri = new Uri(sourceURI);
			string baseUrl = string.Format("{0}://{1}{2}", baseUri.Scheme, baseUri.Host, baseUri.Port == 80 ? string.Empty : ":" + baseUri.Port.ToString());

			string page = null;
			HttpWebRequest request = GRequest.CreateSafeRequest(sourceURI, targetURI);
			request.MaximumAutomaticRedirections = 3;
			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			{
				if (response != null)
				{
					if (GRequest.IsContentTypeValid(response.ContentType, GRequest.ContentTypeCategory.HTML) && response.ContentLength < 300000)
						page = GRequest.GetPageText(response);

					response.Close();
				}
			}

			if (string.IsNullOrEmpty(page))
				return false;

			if (page.IndexOf(targetURI) < 0)
			{
				// The direct link to the Graffiti post was not found in the body of the source post
				// BUT, the source may have linked to a url that redirected to the Graffiti post (i.e. Feedburner url)
				List<string> links = LinkParser.GetLinks(page, baseUrl);
				bool targetLinkFound = false;

				foreach (string externalUrl in links)
				{
					if (string.IsNullOrEmpty(externalUrl))
						continue;

					try
					{
						string responseUrl = null;
						HttpWebRequest request2 = GRequest.CreateSafeRequest(externalUrl, targetURI);
						request2.MaximumAutomaticRedirections = 3;
						using (HttpWebResponse response2 = request2.GetResponse() as HttpWebResponse)
						{
							if (response2 != null)
							{
								if (GRequest.IsContentTypeValid(response2.ContentType, GRequest.ContentTypeCategory.HTML) && response2.ContentLength < 300000)
									responseUrl = response2.ResponseUri.ToString();

								response2.Close();
							}
						}

						if (responseUrl != null && string.Compare(targetURI, responseUrl, StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							targetLinkFound = true;
							break;
						}

					}
					catch { }
				}

				if (!targetLinkFound)
					return false;
			}

			// Look for the HTML Title of the remote page
			string pat = @"<head.*?>.*<title.*?>(.*)</title.*?>.*</head.*?>";
			Regex reg = new Regex(pat, RegexOptions.IgnoreCase | RegexOptions.Singleline);
			Match m = reg.Match(page);
			if (m.Success)
			{
				pageTitle = HttpUtility.HtmlDecode(Graffiti.Core.Util.RemoveHtml(m.Result("$1").Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim(), 250));
			}

			return true;
		}




	}
}
