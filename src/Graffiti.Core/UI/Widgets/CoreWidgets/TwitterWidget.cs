using System;
using System.Collections.Specialized;
using System.Security;
using System.Text;
using System.Web;
using RssToolkit.Rss;
using System.Text.RegularExpressions;

namespace Graffiti.Core
{
	[WidgetInfo("3ec475ab-cd5c-47f6-8e37-e7752a46cc5a", "Twitter", "Twitter messages")]
	public class TwitterWidget : WidgetFeed
	{
		public TwitterWidget()
		{
			DisplayFollowMe = true;
			ItemsToDisplay = 3;
		}

		public string UserName { get; set; }

		public int ItemsToDisplay { get; set; }

		public bool DisplayFollowMe { get; set; }

		public override string FeedUrl
		{
			get { return "http://twitter.com/statuses/user_timeline/" + UserName + ".rss"; }
		}

		public override string RenderData()
		{
			StringBuilder sb = new StringBuilder("<ul>");

			if (!string.IsNullOrEmpty(UserName))
			{
				try
				{
					RssChannel channel = this.Document();
					if (channel != null && channel.Items != null)
					{
						int min = Math.Min(channel.Items.Count, ItemsToDisplay);
						for (int i = 0; i < min; i++)
						{
							sb.Append("<li class=\"tweet\">");

							// remove username prefix (if it exists)
							var desc = channel.Items[i].Description;
							var index = desc.IndexOf(":");
							desc = (index > -1) ? desc.Substring(index + 1).Trim() : desc;

							// format any links in the description and 
							desc = Util.FormatLinks(HttpUtility.HtmlEncode(HttpUtility.HtmlDecode(desc))).Trim();

							// replace all '@twittername' with link to user's twitter profile
							var regex = new Regex(@"\B@([_a-z0-9]+)", RegexOptions.IgnoreCase);
							sb.Append(regex.Replace(desc, string.Format("@<a href=\"http://twitter.com/{0}\">{0}</a>", "$1")));

							// add link to tweet with the relative date of tweet
							var date = channel.Items[i].PubDateParsed;
							var span = DateTime.Now.ToUniversalTime().Subtract(date);

							string relativeDate;
							if (span.TotalMinutes < 1)
								relativeDate = "less than a minute ago";
							else if (span.TotalMinutes < 2)
								relativeDate = "about a minute ago";
							else if (span.TotalHours < 1)
								relativeDate = string.Concat((int)span.TotalMinutes, " minutes ago");
							else if (span.TotalHours < 2)
								relativeDate = "about an hour ago";
							else if (span.TotalDays < 1)
								relativeDate = string.Concat((int)span.TotalHours, " hours ago");
							else if (span.TotalDays < 2)
								relativeDate = "1 day ago";
							else
								relativeDate = string.Concat((int)span.TotalDays, " days ago");

							sb.AppendFormat(" - <a href=\"{0}\">{1}</a></li>", channel.Items[i].Link, relativeDate);
						}
					}

					if (DisplayFollowMe)
						sb.Append("<li class=\"twitterlink\"><a href=\"http://twitter.com/" + UserName + "\">Follow Me on Twitter</a></li>");
				}
				catch (Exception)
				{
				}
				sb.Append("</ul>\n");
			}
			return sb.ToString();
		}

		public override string Title
		{
			get
			{
				if (string.IsNullOrEmpty(base.Title))
					base.Title = "My Tweets";

				return base.Title;
			}
			set
			{
				base.Title = value;
			}
		}

		public override string Name
		{
			get
			{
				return "Twitter";
			}
		}

		protected override FormElementCollection AddFormElements()
		{
			FormElementCollection fec = new FormElementCollection();
			fec.Add(AddTitleElement());
			fec.Add(new TextFormElement("username", "UserName", "(your twitter username)"));
			ListFormElement lfe = new ListFormElement("itemsToDisplay", "Number of Tweets", "(how many tweets do you want to display?)");
			lfe.Add(new ListItemFormElement("1", "1"));
			lfe.Add(new ListItemFormElement("2", "2"));
			lfe.Add(new ListItemFormElement("3", "3", true));
			lfe.Add(new ListItemFormElement("4", "4"));
			lfe.Add(new ListItemFormElement("5", "5"));
			fec.Add(lfe);
			fec.Add(new CheckFormElement("displayFollowMe", "Display 'Follow Me on Twitter' link", null, true));
			return fec;
		}

		protected override NameValueCollection DataAsNameValueCollection()
		{
			NameValueCollection nvc = base.DataAsNameValueCollection();
			nvc["username"] = UserName;
			nvc["itemsToDisplay"] = ItemsToDisplay.ToString();
			nvc["displayFollowMe"] = DisplayFollowMe.ToString();

			return nvc;
		}

		public override StatusType SetValues(HttpContext context, NameValueCollection nvc)
		{
			StatusType statusType = base.SetValues(context, nvc);
			if (statusType == StatusType.Success)
			{
				if (string.IsNullOrEmpty(nvc["username"]))
				{
					SetMessage(context, "Please enter a twitter username");
					return StatusType.Error;
				}

				ItemsToDisplay = Int32.Parse(nvc["itemsToDisplay"]);
				UserName = nvc["username"];
				DisplayFollowMe = !string.IsNullOrEmpty(nvc["displayFollowMe"]) && 
					(nvc["displayFollowMe"] == "checked" || nvc["displayFollowMe"] == "on");

				try
				{
					RegisterForSyndication();
				}
				catch (Exception ex)
				{
					statusType = StatusType.Error;
					SetMessage(context, ex.Message);
				}
			}

			return statusType;
		}

	}
}