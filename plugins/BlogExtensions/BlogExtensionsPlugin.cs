using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.Routing;
using Graffiti.Core;

namespace Graffiti.BlogExtensions
{
	/// <summary>
	/// Graffiti CMS plugin that provides advanced blogging capabilities.
	/// </summary>
	public class BlogExtensionsPlugin : GraffitiEvent
	{

		#region Properties

		private const string _geoRSSCustomFieldName = "GeoRSS Location";

		private bool _enableCommentRSS = false;
		public bool EnableCommentRSS
		{
			get { return _enableCommentRSS; }
			set { _enableCommentRSS = value; }
		}

		private bool _enablePings = false;
		public bool EnablePings
		{
			get { return _enablePings; }
			set { _enablePings = value; }
		}

		private bool _enableTrackbacks = false;
		public bool EnableTrackbacks
		{
			get { return _enableTrackbacks; }
			set { _enableTrackbacks = value; }
		}

		private bool _enableGeoRSS = false;
		public bool EnableGeoRSS
		{
			get { return _enableGeoRSS; }
			set { _enableGeoRSS = value; }
		}

		private string _geoRSSLocation = string.Empty;
		public string GeoRSSLocation
		{
			get { return _geoRSSLocation; }
			set { _geoRSSLocation = value; }
		}

		private string _pingUrls = "http://rpc.pingomatic.com/RPC2";
		public string PingUrls
		{
			get { return _pingUrls; }
			set { _pingUrls = value; }
		}


		#endregion

		#region Init
		public override void Init(GraffitiApplication ga)
		{
			ga.AfterInsert += ga_AfterInsert;
			ga.AfterUpdate += ga_AfterUpdate;
			ga.BeginRequest += ga_BeginRequest;
			ga.RssNamespace += ga_RssNamespace;
			ga.RssItem += ga_RssItem;
			ga.RenderHtmlHeader += ga_RenderHtmlHeader;
			ga.RenderPostBody += ga_RenderPostBody;
			ga.UrlRoutingAdd += ga_UrlRoutingAdd;
		}

		#endregion

		#region GraffitiEvent Properties

		public override bool IsEditable
		{
			get { return true; }
		}

		public override string Name
		{
			get { return "Blog Extensions Plugin"; }
		}

		public override string Description
		{
			get
			{
				return "Extends Graffiti CMS with advanced blogging capabilities.";
			}
		}

		#endregion

		#region Configuration

		protected override FormElementCollection AddFormElements()
		{
			FormElementCollection fec = new FormElementCollection();
			fec.Add(new CheckFormElement("enableCommentRSS", "Enable Comment RSS Feed", "Generates a RSS feed that contains feedback received for all posts. Can be found at /feed/comments/", false));
			fec.Add(new CheckFormElement("enablePings", "Enable Pings", "Enables automatic notification to services that new content is available.", false));
			fec.Add(new CheckFormElement("enableTrackbacks", "Enable Trackbacks", "If enabled, Graffiti will automatically send trackbacks and pingbacks, and accept them for posts that have new comments enabled.", false));
			fec.Add(new CheckFormElement("enableGeoRSS", "Enable GeoRSS", "If enabled a custom field will be created to enter the geographic location of each post. If one is not entered, the default GeoRSS location (below) will be used.", false));
			fec.Add(new TextAreaFormElement("pingUrls", "Ping Service Urls", "Enter the URLs of all the services Graffiti should ping when you add or edit a post. Put each URL on a new line.", 3));
			fec.Add(new TextFormElement("geoRSSLocation", "Default GeoRSS Location (lattitude longitude)", "If a location is not entered in the custom GeoRSS field when writing a post, this value will be used in the GeoRSS element in RSS feeds. Enter a lattitude and longitude in the format: 45.256 -71.92."));

			return fec;
		}

		protected override NameValueCollection DataAsNameValueCollection()
		{
			NameValueCollection nvc = new NameValueCollection();
			nvc["enableCommentRSS"] = EnableCommentRSS.ToString();
			nvc["enablePings"] = EnablePings.ToString();
			nvc["enableTrackbacks"] = EnableTrackbacks.ToString();
			nvc["enableGeoRSS"] = EnableGeoRSS.ToString();

			nvc["pingUrls"] = PingUrls;
			nvc["geoRSSLocation"] = GeoRSSLocation;

			return nvc;
		}

		public override StatusType SetValues(System.Web.HttpContext context, NameValueCollection nvc)
		{

			EnableCommentRSS = ConvertStringToBool(nvc["enableCommentRSS"]);
			EnablePings = ConvertStringToBool(nvc["enablePings"]);
			EnableTrackbacks = ConvertStringToBool(nvc["enableTrackbacks"]);
			EnableGeoRSS = ConvertStringToBool(nvc["enableGeoRSS"]);

			PingUrls = nvc["pingUrls"];
			GeoRSSLocation = nvc["geoRSSLocation"];

			if (EnablePings && string.IsNullOrEmpty(PingUrls))
			{
				SetMessage(context, "Pings are enabled but no ping service urls were entered. Please enter one or ping service urls, or disable pings.");
				return StatusType.Error;
			}
			if (EnableGeoRSS && string.IsNullOrEmpty(GeoRSSLocation))
			{
				SetMessage(context, "GeoRSS is enabled but a default location was not entered. Please enter one or disable GeoRSS.");
				return StatusType.Error;
			}

			UrlRouting.Initialize();
			SetupCommentFeed();
			SetupGeoRSS();

			return StatusType.Success;
		}

		public override void EventEnabled()
		{
			UrlRouting.Initialize();
			SetupCommentFeed();
			SetupGeoRSS();
		}

		public override void EventDisabled()
		{
			SetupCommentFeed();
		}

		private bool ConvertStringToBool(string checkValue)
		{
			if (string.IsNullOrEmpty(checkValue))
				return false;
			else if (checkValue == "checked" || checkValue == "on")
				return true;
			else
				return bool.Parse(checkValue);
		}

		private void SetupCommentFeed()
		{
			if (EnableCommentRSS)
			{
				// Write out default folders/files if necessary
				if (SiteSettings.Get().GenerateFolders)
				{
					string absPath = HttpContext.Current.Server.MapPath("~/Feed/Comments/" + Util.DEFAULT_PAGE);
					FileInfo fi = new FileInfo(absPath);
					if (!fi.Directory.Exists)
					{
						fi.Directory.Create();
					}

					if (!fi.Exists || fi.Length == 0)
					{
						StreamWriter sw = new StreamWriter(absPath, false);
						sw.WriteLine("<%@ Page Language=\"C#\" AutoEventWireup=\"true\" Inherits=\"Graffiti.BlogExtensions.CommentRssHandler\" %>");
						sw.WriteLine("<%-- This file was generated at {0} by the Blog Extensions Plugin and should not be manually edited. --%>", DateTime.Now.ToString());
						sw.Close();
					}
				}
			}
			else
			{
				// Remove folder/file if necessary
				string dirPath = HttpContext.Current.Server.MapPath("~/Feed/Comments/");
				if (Directory.Exists(dirPath))
					Directory.Delete(dirPath);
			}
		}

		private void SetupGeoRSS()
		{
			if (!EnableGeoRSS)
				return;

			bool customFieldExists = false;
			CustomFormSettings cfs = CustomFormSettings.Get();
			if (cfs.Fields != null && cfs.Fields.Count > 0)
			{
				foreach (CustomField cf in cfs.Fields)
				{
					if (Util.AreEqualIgnoreCase(_geoRSSCustomFieldName, cf.Name))
					{
						customFieldExists = true;
						break;
					}
				}
			}

			if (!customFieldExists)
			{
				CustomField nfield = new CustomField();
				nfield.Name = _geoRSSCustomFieldName;
				nfield.Description = "The geographic location of this post in the format lattitude longitude. If no location is entered, the default location set in the Blog Extensions Plugin will be used.";
				nfield.Enabled = true;
				nfield.Id = Guid.NewGuid();
				nfield.FieldType = FieldType.TextBox;

				cfs.Name = "-1";
				cfs.Add(nfield);
				cfs.Save();
			}
		}

		#endregion

		#region Events

		void ga_AfterUpdate(DataBuddyBase dataObject, EventArgs e)
		{
			Post post = dataObject as Post;

			if (post != null)
			{
				// Check that post
				//  1. Is Published
				//  2. The current verison is set to be published
				//  3. Was not published within the last 10 seconds (some plugins update the post immediately after it is created, 
				//     which can cause double trackbacks/pings)
				if (post.IsPublished && post.PostStatus == PostStatus.Publish && post.Published.CompareTo(DateTime.UtcNow.AddSeconds(-10.0)) <= 0)
				{
					// Blog Pings
					if (EnablePings)
						XmlRpcPings.SendPings(post, PingUrls);

					// Check for links to send Trackbacks & Pingbacks
					if (EnableTrackbacks)
						LinkParser.CheckPost(post);
				}

			}
		}

		void ga_AfterInsert(DataBuddyBase dataObject, EventArgs e)
		{
			Post post = dataObject as Post;

			if (post != null)
			{
				// Check that post
				//  1. Is Published
				//  2. The current verison is set to be published
				//  3. Is not a future dated post
				if (post.IsPublished && post.PostStatus == PostStatus.Publish && post.Published.CompareTo(DateTime.UtcNow.AddMinutes(1.0)) <= 0)
				{
					// Blog Pings
					if (EnablePings)
						XmlRpcPings.SendPings(post, PingUrls);

					// Check for links to send Trackbacks & Pingbacks
					if (EnableTrackbacks)
						LinkParser.CheckPost(post);
				}

			}
		}

		void ga_UrlRoutingAdd(RouteCollection routes, EventArgs e)
		{
			if (EnableCommentRSS)
				routes.Add(new Route("feed/comments/", new CommentRssRouteHandler()));

			if (EnableTrackbacks)
			{
				routes.Add(new Route("trackback.ashx", new TrackbackRouteHandler()));
				routes.Add(new Route("pingback.ashx", new PingbackRouteHandler()));
			}

		}

		void ga_BeginRequest(object sender, EventArgs e)
		{
			HttpApplication app = sender as HttpApplication;
			HttpContext context = app.Context;
			bool generateFolders = SiteSettings.Get().GenerateFolders;
			string path = context.Request.Path.ToLower();

			if (EnableTrackbacks)
			{
				// Check if this is a Trackback or Pingback request
				if (generateFolders && path.Contains("/trackback.ashx"))
				{
					TrackBackHandler trackback = new TrackBackHandler();
					trackback.ProcessRequest(context);
					return;
				}
				else if (generateFolders && path.Contains("/pingback.ashx"))
				{
					IHttpHandler pingback = new PingBackHandler();
					pingback.ProcessRequest(context);
					return;
				}

				// Add Pingback HttpHeader if enabled
				context.Response.AddHeader("X-Pingback", PingBackHandler.PingbackServiceUrl);
			}

		}

		void ga_RssNamespace(XmlTextWriter writer, EventArgs e)
		{
			if (EnableGeoRSS)
			{
				writer.WriteAttributeString("xmlns:georss", "http://www.georss.org/georss");
			}
		}

		void ga_RssItem(XmlTextWriter writer, PostEventArgs e)
		{
			if (EnableGeoRSS)
			{
				string location = e.Post.Custom(_geoRSSCustomFieldName);
				if (string.IsNullOrEmpty(location))
					location = GeoRSSLocation;

				writer.WriteElementString("georss:point", location);
			}
		}

		void ga_RenderHtmlHeader(StringBuilder sb, EventArgs e)
		{
			// Add autodiscovery for CommentRSS feed
			if (EnableCommentRSS)
			{
				sb.AppendFormat("<link rel=\"alternate\" type=\"application/rss+xml\" title=\"{0}\" href=\"{1}\" />\n",
					  "Comments RSS Feed",
					  new Macros().FullUrl(VirtualPathUtility.ToAbsolute("~/feed/comments/")));
			}

			// Render Pingback Link element if enabled
			if (EnableTrackbacks)
			{
				sb.Append(PingBackHandler.GeneratePingbackLinkElement());
			}
		}

		void ga_RenderPostBody(StringBuilder sb, PostEventArgs e)
		{
			// Embed TrackBack RDF info for this post
			if (EnableTrackbacks && e.RenderLocation == PostRenderLocation.Web && e.Post.EnableComments && e.Post.EnableNewComments)
			{
				sb.Append(TrackBackHandler.GenerateTrackbackRDF(e.Post));
			}

		}

		#endregion



	}
}
