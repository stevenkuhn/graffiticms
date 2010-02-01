using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Graffiti.Core;
using DataBuddy;

namespace Graffiti.BlogExtensions
{
	public class TrackBackHandler : IHttpHandler
	{

		private static readonly string successResponseXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<response>\n<error>0</error>\n</response>";
		private static readonly string failureResponseXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<response>\n<error>1</error>\n<message>{0}</message>\n</response>";

		#region Constructor
		public TrackBackHandler()
		{
		}
		#endregion

		#region Public methods
		public void ProcessRequest(HttpContext context)
		{
			Macros macros = new Macros();
			context.Response.ContentType = "text/xml";

			int postId = 0;
			try { postId = int.Parse(context.Request.QueryString["id"]); }
			catch { }

			if (postId <= 0)
				TrackbackResponse(context, "PostId is invalid or missing");

			if (context.Request.HttpMethod == "POST")
			{
				string title = SafeParam(context, "title");
				string excerpt = SafeParam(context, "excerpt");
				string url = SafeParam(context, "url");
				string blog_name = SafeParam(context, "blog_name");

				try
				{
					// Check if params are valid
					if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(blog_name) || string.IsNullOrEmpty(excerpt))
					{
						TrackbackResponse(context, "One or more parameters are invalid or missing");
					}

					Post trackedEntry = Post.GetCachedPost(postId);
					if (trackedEntry == null)
					{
						TrackbackResponse(context, "The link does not exist");
						return;
					}

					if (!trackedEntry.EnableComments || !trackedEntry.EnableNewComments)
					{
						TrackbackResponse(context, "Trackbacks are not enabled");
						return;
					}

					if (!IsNewTrackBack(trackedEntry.Id, url))
					{
						TrackbackResponse(context, "Trackbacks already exists");
						return;
					}

					string pageTitle = null;
					if (!LinkParser.SourceContainsTarget(url, macros.FullUrl(trackedEntry.Url), out pageTitle))
					{
						TrackbackResponse(context, "Sorry couldn't find a relevant link in " + url);
					}

					if (string.IsNullOrEmpty(pageTitle))
					{
						TrackbackResponse(context, "Could not find a readable HTML title in the remote page at " + url);
						return;
					}

					if (!string.IsNullOrEmpty(excerpt))
						excerpt = Util.RemoveHtml(excerpt, 250);

					// Create the Trackback item
					Comment comment = new Comment();
					comment.IsTrackback = true;
					comment.PostId = trackedEntry.Id;
					comment.Name = title;
					comment.WebSite = url;
					comment.Body = excerpt;
					comment.IPAddress = context.Request.UserHostAddress;
					comment.Published = DateTime.Now.AddHours(SiteSettings.Get().TimeZoneOffSet);
					comment.Save();

					// Log success message to EventLog
					string message = String.Format("Trackback request received from {0} and saved to post {1}.", url, trackedEntry.Title);
					Log.Info("Trackback Received", message);

					context.Response.Write(successResponseXML);
					context.Response.End();
				}
				catch (System.Threading.ThreadAbortException) { }
				catch (System.Exception ex)
				{
					if (ex.Message != null)
						TrackbackResponse(context, string.Format("Error occurred while processing Trackback: {0}", ex.Message));
					else
						TrackbackResponse(context, "Unknown error occurred while processing Trackback.");
				}

			}
		}

		public bool IsReusable
		{
			get { return true; }
		}

		#endregion

		#region Private Helper Methods

		private void TrackbackResponse(HttpContext context, string errorMessage)
		{
			// Log trackback failure message
			string message = String.Format("Trackback request received but failed due to one or more reasons. Request URL: {0}. Error message returned was: {1}.", context.Request.RawUrl, errorMessage);
			Log.Warn("Trackback Error", message);

			context.Response.Write(string.Format(failureResponseXML, errorMessage));
			context.Response.End();
		}

		private string SafeParam(HttpContext context, string pName)
		{
			if (context.Request.Form[pName] != null)
				return context.Request.Form[pName];
			return string.Empty;
		}

		private bool IsNewTrackBack(int postId, string trackbackUrl)
		{
			CommentCollection trackbacks = new CommentCollection();
			Query q = Comment.CreateQuery();
			q.AndWhere(Comment.Columns.PostId, postId);
			q.AndWhere(Comment.Columns.IsDeleted, false);
			q.AndWhere(Comment.Columns.IsTrackback, true);
			q.OrderByAsc(Comment.Columns.Published);
			trackbacks.LoadAndCloseReader(q.ExecuteReader());

			foreach (Comment trackback in trackbacks)
			{
				if (string.Compare(trackbackUrl, trackback.WebSite, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return false;
				}
			}

			return true;
		}

		#endregion

		#region GenerateTrackbackRDF(...)
		public static string GenerateTrackbackRDF(Post post)
		{
			Macros macros = new Macros();
			StringBuilder sb = new StringBuilder();

			string postUrl = macros.FullUrl(post.Url);
			string pingUrl = macros.FullUrl(string.Format("{0}?id={1}", VirtualPathUtility.ToAbsolute("~/trackback.ashx"), post.Id));

			sb.Append("\n<!--\n");
			sb.Append("<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"\n");
			sb.Append("xmlns:dc=\"http://purl.org/dc/elements/1.1/\"\n");
			sb.Append("xmlns:trackback=\"http://madskills.com/public/xml/rss/module/trackback/\">\n");
			sb.Append("<rdf:Description\n");
			sb.AppendFormat("rdf:about=\"{0}\"\n", postUrl);
			sb.AppendFormat("dc:identifier=\"{0}\"\n", postUrl);
			sb.AppendFormat("dc:title=\"{0}\"\n", post.Title);
			sb.AppendFormat("trackback:ping=\"{0}\" />\n", pingUrl);
			sb.Append("</rdf:RDF>\n");
			sb.Append("-->\n");

			return sb.ToString();
		}
		#endregion

	}
}
