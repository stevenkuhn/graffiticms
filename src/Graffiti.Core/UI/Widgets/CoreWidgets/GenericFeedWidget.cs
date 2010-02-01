using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using RssToolkit.Rss;

namespace Graffiti.Core
{
    [WidgetInfo("77400518-6bbc-40cf-97af-4346386e3f3e","Syndication Feed", "Displays the recent items from a RSS or Atom feed")]
    public class GenericFeedWidget : WidgetFeed
    {
        private int _itemsToDisplay = 3;

        public int ItemsToDisplay
        {
            get { return _itemsToDisplay; }
            set { _itemsToDisplay = value; }
        }

        public string FeedUri = null;
	

        public override string FeedUrl
        {
            get { return FeedUri; }
        }

        public override string RenderData()
        {
            if (string.IsNullOrEmpty(FeedUrl))
                return string.Empty;

            StringBuilder sb = new StringBuilder("<ul>");
            
            try
            {
                RssChannel channel = this.Document();
                if (channel != null && channel.Items != null)
                {
                    int min = Math.Min(channel.Items.Count, ItemsToDisplay);
                    for (int i = 0; i < min; i++)
                    {
                        sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", channel.Items[i].Link,
                                        HttpUtility.HtmlEncode(channel.Items[i].Title));
                    }
                }
            }
            catch (Exception)
            {
            }
            sb.Append("</ul>\n");

            return sb.ToString();
        }

        public override string Title
        {
            get
            {
                if (string.IsNullOrEmpty(base.Title))
                    base.Title = "Syndication Feed";

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
                return Title;
            }
        }

        protected override FormElementCollection AddFormElements()
        {
            FormElementCollection fec = new FormElementCollection();
            fec.Add(AddTitleElement());

            fec.Add(new TextFormElement("FeedUri", "Feed", "The Url of the feed you wish to display"));
            ListFormElement lfe = new ListFormElement("itemsToDisplay", "Number of Posts", "(how many posts do you want to display?)");
            lfe.Add(new ListItemFormElement("1","1"));
            lfe.Add(new ListItemFormElement("3", "3", true));
            lfe.Add(new ListItemFormElement("5", "5"));
            lfe.Add(new ListItemFormElement("7", "7"));
            fec.Add(lfe);

            return fec;
        }

        protected override NameValueCollection DataAsNameValueCollection()
        {
            NameValueCollection nvc =  base.DataAsNameValueCollection();
            nvc["FeedUri"] = FeedUri;
            nvc["itemsToDisplay"] = ItemsToDisplay.ToString();

            

            return nvc;
        }

        public override StatusType SetValues(HttpContext context, NameValueCollection nvc)
        {
            StatusType statusType = base.SetValues(context, nvc);
            if(statusType == StatusType.Success)
            {
                ItemsToDisplay = Int32.Parse(nvc["itemsToDisplay"]);
                FeedUri = nvc["FeedUri"];

  
                try
                {
                    RegisterForSyndication();
                }
                catch(Exception ex)
                {
                    statusType = StatusType.Error;
                    SetMessage(context,ex.Message);
                }
            }

            return statusType;
        }
	
    }
}