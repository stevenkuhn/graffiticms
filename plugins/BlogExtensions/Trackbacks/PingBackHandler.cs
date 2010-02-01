using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Graffiti.Core;
using DataBuddy;
using CookComputing.XmlRpc;

namespace Graffiti.BlogExtensions
{
	/// <summary>
	/// PingBack Service that receives and responds to XML-RPC PingBack requests.
	/// </summary>
	public class PingBackHandler : XmlRpcService, IPingBack
	{

		#region Error Codes

		// Unknown error
		protected readonly int errorCode_General = 0;

		// The source URI does not exis
		protected readonly int errorCode_SourceURIDoesNotExist = 16;

		// The source URI does not contain a link to the target URI, and so cannot be used as a source.
		protected readonly int errorCode_SourceDoesNotContainTarget = 17;

		// The specified target URI does not exist
		protected readonly int errorCode_TargetURIDoesNotExist = 32;

		// The specified target URI cannot be used as a PingBack target.
		protected readonly int errorCode_TargetURIInvalid = 33;

		// The pingback has already been registered
		protected readonly int errorCode_DuplicatePingBack = 48;

		// Access denied
		protected readonly int errorCode_AccessDenied = 49;

		#endregion

		#region Constructor
		public PingBackHandler()
		{
		}
		#endregion

		#region IPingBack Members

		/// <summary>
		/// Processes a PingBack request 
		/// </summary>
		/// <param name="sourceURI">The absolute URI of the post on the source page containing the link to the target site.</param>
		/// <param name="targetURI">The absolute URI of the target of the link, as given on the source page.</param>
		public string Ping(string sourceURI, string targetURI)
		{

			try
			{
				// Attempt to create the PingBack and save it to the database
				CreatePingBack(sourceURI, targetURI);
			}
			catch (XmlRpcFaultException XmlRpcEx)
			{
				string message = String.Format("Pingback request received but failed due to one or more reasons. Source Url: {0}; Target URL: {1}. Error message returned was: {2}.", sourceURI, targetURI, XmlRpcEx.FaultString);
				Log.Warn("Pingback Error", message);

				// If a XmlRpcFaultException was thrown then it was done deliberately
				//	Just rethrow it and the CookComputing.XmlRpc library will catch it and
				//	convert it into a XML-RPC Fault Code response.
				throw XmlRpcEx;
			}
			catch (System.Exception ex)
			{
				string message = String.Format("Pingback request received but failed due to one or more reasons. Source Url: {0}; Target URL: {1}. Error message returned was: {2}.", sourceURI, targetURI, ex.Message);
				Log.Warn("Pingback Error", message);

				// If any other exception was thrown then something unexpected happened.
				// Return a General error XML-RPC Fault Code to the client since we don't know exactly what went wrong.
				throw new XmlRpcFaultException(errorCode_General, "Unknown error occurred while processing Pingback.");
			}

			return "Thanks for the Pingback!";
		}

		#endregion

		#region Private Helper Methods

		private void CreatePingBack(string sourceURI, string targetURI)
		{
			// Check Parameters
			if (string.IsNullOrEmpty(sourceURI))
			{
				throw new XmlRpcFaultException(errorCode_SourceURIDoesNotExist, "No source URI parameter found, please try harder!");
			}
			if (string.IsNullOrEmpty(targetURI))
			{
				throw new XmlRpcFaultException(errorCode_TargetURIDoesNotExist, "The target URI does not exist!");
			}

			// Retrieve referenced post
			Post trackedEntry = null;
			try
			{
				trackedEntry = GetPostFromUrl(targetURI);
			}
			catch
			{
				throw new XmlRpcFaultException(errorCode_TargetURIInvalid, "The target URI is invalid.");
			}
			if (trackedEntry == null)
			{
				throw new XmlRpcFaultException(errorCode_TargetURIInvalid, "The target URI is invalid.");
			}

			// Check if trackbacks/pingbacks are enabled
			if (!trackedEntry.EnableComments || !trackedEntry.EnableNewComments)
			{
				throw new XmlRpcFaultException(errorCode_AccessDenied, "Pingbacks are not enabled.");
			}

			// Check if this is a duplicate pingback (or trackback)
			if (!IsNewTrackBack(trackedEntry.Id, sourceURI))
			{
				throw new XmlRpcFaultException(errorCode_DuplicatePingBack, "A pingback for this source URI already exists.");
			}

			// Retrieve the source document and check if it actually contains a link to the target
			string pageTitle = null;
			if (!LinkParser.SourceContainsTarget(sourceURI, new Macros().FullUrl(trackedEntry.Url), out pageTitle))
			{
				throw new XmlRpcFaultException(errorCode_SourceDoesNotContainTarget, "Sorry couldn't find a relevant link in " + sourceURI);
			}

			if (string.IsNullOrEmpty(pageTitle))
				throw new XmlRpcFaultException(errorCode_SourceDoesNotContainTarget, "Could not find a readable HTML title in the remote page at " + sourceURI);

			// Create the Trackback item
			Comment comment = new Comment();
			comment.IsTrackback = true;
			comment.PostId = trackedEntry.Id;
			comment.Name = pageTitle;
			comment.WebSite = sourceURI;
			comment.Body = "Pingback from " + pageTitle;
			comment.IPAddress = Context.Request.UserHostAddress;
			comment.Published = DateTime.Now.AddHours(SiteSettings.Get().TimeZoneOffSet);
			comment.Save();

			// Log success message to EventLog
			string message = String.Format("Pingback request received from {0} and saved to post {1}.", sourceURI, trackedEntry.Title);
			Log.Info("Pingback Received", message);

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

		private Post GetPostFromUrl(string url)
		{
			Post post = null;

			if (!string.IsNullOrEmpty(url))
			{
				string[] urlSegments = url.Split('/');
				if (urlSegments != null && urlSegments.Length > 1)
				{
					string postName = urlSegments[urlSegments.Length - 2];
					if (!string.IsNullOrEmpty(postName))
					{
						post = new Data().GetPost(postName);
					}
				}
			}

			return post;
		}

		#endregion

		#region Pingback Service Links
		public static string GeneratePingbackLinkElement()
		{
			return string.Format("<link rel=\"pingback\" href=\"{0}\" />", PingbackServiceUrl);
		}

		public static string PingbackServiceUrl
		{
			get { return new Macros().FullUrl(VirtualPathUtility.ToAbsolute("~/pingback.ashx")); }
		}
		#endregion

	}
}
