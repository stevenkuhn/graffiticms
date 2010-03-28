using System;
using System.Collections.Specialized;
using System.Security;
using System.Text;
using System.Web;
using RssToolkit.Rss;

namespace Graffiti.Core
{
	[WidgetInfo("3ec475ab-cd5c-47f6-8e37-e7752a46cc5a", "Twitter", "Twitter messages")]
	public class TwitterWidget : WidgetFeed
	{
		public string UserName { get; set; }

		private int _itemsToDisplay = 3;
		public int ItemsToDisplay
		{
			get { return _itemsToDisplay; }
			set { _itemsToDisplay = value; }
		}

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
							string desc = channel.Items[i].Description;
							int index = desc.IndexOf(":");

              sb.Append("<li class=\"tweet\">" + Util.FormatLinks(HttpUtility.HtmlEncode(HttpUtility.HtmlDecode(((index > -1) ? desc.Substring(index + 1).Trim() : desc)))) + "</li>");

						}
					}

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
			lfe.Add(new ListItemFormElement("3", "3", true));
			lfe.Add(new ListItemFormElement("5", "5"));
			fec.Add(lfe);

			return fec;
		}

		protected override NameValueCollection DataAsNameValueCollection()
		{
			NameValueCollection nvc = base.DataAsNameValueCollection();
			nvc["username"] = UserName;
			nvc["itemsToDisplay"] = ItemsToDisplay.ToString();

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