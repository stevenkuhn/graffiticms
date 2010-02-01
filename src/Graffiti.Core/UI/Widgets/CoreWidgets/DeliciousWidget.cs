using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using RssToolkit.Rss;

namespace Graffiti.Core
{
    [WidgetInfo("31ae0b55-6d93-4af7-98d8-04c648540654","Del.icio.us", "Recent Del.icio.us Links")]
    public class DeliciousWidget : WidgetFeed
    {
        private string _UserName;

        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        private int _itemsToDisplay = 5;

        public int ItemsToDisplay
        {
            get { return _itemsToDisplay; }
            set { _itemsToDisplay = value; }
        }
	

        public override string FeedUrl
        {
            get { return "http://del.icio.us/rss/" + UserName; }
        }

        public override string RenderData()
        {
            StringBuilder sb = new StringBuilder("<ul>");

            if (!string.IsNullOrEmpty(UserName))
            {
                try
                {
                    RssChannel channel = Document();
                    if (channel != null && channel.Items != null)
                    {
                        int min = Math.Min(channel.Items.Count, ItemsToDisplay);
                        for (int i = 0; i < min; i++)
   
                            sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", channel.Items[i].Link,
                                            channel.Items[i].Title);
                            
                     
                    }
                }
                catch(Exception)
                {
                }
                sb.Append("</ul>\n");

                //sb.AppendFormat("<p><a href=\"http://del.icio.us/{0}\">See more on Del.icio.us</a></p>", UserName);
            }
            return sb.ToString();
        }

        public override string Title
        {
            get
            {
                if (string.IsNullOrEmpty(base.Title))
                    base.Title = "My Del.icio.us Links";

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
                return "Del.ico.us";
            }
        }

        protected override FormElementCollection AddFormElements()
        {
            FormElementCollection fec = new FormElementCollection();
            fec.Add(AddTitleElement());
            fec.Add(new TextFormElement("username", "UserName", "(your Del.icio.us username)"));
            ListFormElement lfe = new ListFormElement("itemsToDisplay", "Number of Links", "(how many links do you want to display?)");
            lfe.Add(new ListItemFormElement("1","1"));
            lfe.Add(new ListItemFormElement("3", "3"));
            lfe.Add(new ListItemFormElement("5", "5", true));
            lfe.Add(new ListItemFormElement("10", "10"));
            fec.Add(lfe);
            return fec;
        }

        protected override NameValueCollection DataAsNameValueCollection()
        {
            NameValueCollection nvc =  base.DataAsNameValueCollection();
            nvc["username"] = UserName;
            nvc["itemsToDisplay"] = ItemsToDisplay.ToString();

            return nvc;
        }

        public override StatusType SetValues(HttpContext context, NameValueCollection nvc)
        {
            StatusType statusType = base.SetValues(context, nvc);
            if(statusType == StatusType.Success)
            {
                ItemsToDisplay = Int32.Parse(nvc["itemsToDisplay"]);
                UserName = nvc["username"];

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