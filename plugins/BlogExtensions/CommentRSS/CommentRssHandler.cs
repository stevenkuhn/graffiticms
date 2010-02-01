using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Routing;
using System.Xml;
using DataBuddy;
using Graffiti.Core;

namespace Graffiti.BlogExtensions
{
	public class CommentRssHandler : Page
	{

		protected override void OnLoad(EventArgs e)
		{
			SiteSettings settings = SiteSettings.Get();
			CommentCollection cc = new Data().RecentComments(Util.PageSize);

			DateTime lastModified = DateTime.Now;
			if (cc.Count > 0)
			{
				string lastMod = Context.Request.Headers["If-Modified-Since"];
				if (lastMod != null)
				{
					if (lastMod == cc[0].Published.AddHours(-1 * settings.TimeZoneOffSet).ToUniversalTime().ToString("r"))
					{
						Context.Response.StatusCode = 304;
						Context.Response.Status = "304 Not Modified";
						Context.Response.End();
					}
				}
				lastModified = cc[0].Published.AddHours(-1 * settings.TimeZoneOffSet);
				Context.Response.Clear();
				Context.Response.Cache.SetCacheability(HttpCacheability.Public);
				Context.Response.Cache.SetLastModified(lastModified);
				Context.Response.Cache.SetETag(lastModified.ToString());
			}

			Macros macros = new Macros();

			StringWriter sw = new StringWriter();
			sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
			XmlTextWriter writer = new XmlTextWriter(sw);

			writer.WriteStartElement("rss");
			writer.WriteAttributeString("version", "2.0");
			writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");

			writer.WriteStartElement("channel");
			writer.WriteElementString("title", settings.Title + ": All Comments");
			writer.WriteElementString("link", macros.FullUrl(new Urls().Home));
			writer.WriteElementString("description", settings.TagLine);
			writer.WriteElementString("generator", SiteSettings.VersionDescription);
			writer.WriteElementString("lastBuildDate", lastModified.AddHours(-1 * settings.TimeZoneOffSet).ToUniversalTime().ToString("r"));

			string baseUrl = SiteSettings.BaseUrl;
			foreach (Comment comment in cc)
			{
				string link = macros.FullUrl(comment.Url);
				string body = Util.FullyQualifyRelativeUrls(comment.Body, baseUrl);
				if (comment.IsTrackback)
					body = string.Format("{0}\n<br />\nTrackback url: {1}", body, comment.WebSite);

				writer.WriteStartElement("item");
				writer.WriteElementString("title", HttpUtility.HtmlDecode(comment.Title));
				writer.WriteElementString("link", link);
				writer.WriteElementString("pubDate", comment.Published.AddHours(-1 * settings.TimeZoneOffSet).ToUniversalTime().ToString("r"));

				writer.WriteStartElement("guid");
				writer.WriteAttributeString("isPermaLink", "true");
				writer.WriteString(link);
				writer.WriteEndElement();

				writer.WriteElementString("dc:creator", comment.Name);
				writer.WriteElementString("description", body);

				writer.WriteEndElement(); // End Item
			}

			writer.WriteEndElement(); // End Channel
			writer.WriteEndElement(); // End Document

			// save XML into response
			Context.Response.ContentEncoding = System.Text.Encoding.UTF8;
			Context.Response.ContentType = "application/rss+xml";
			Context.Response.Write(sw.ToString());

		}



	}
}
