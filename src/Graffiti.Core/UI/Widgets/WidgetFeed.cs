using System;
using RssToolkit.Rss;

namespace Graffiti.Core
{
    public abstract class WidgetFeed: Widget
    {

        public abstract string FeedUrl { get;}

	
        protected void RegisterForSyndication()
        {
            string url = FeedUrl;

            if (string.IsNullOrEmpty(url))
                throw new Exception("The widget does not have the proper feed settings. FeedUrl is null or empty");

            Uri newUri;

            if (!Uri.TryCreate(url, UriKind.Absolute, out newUri))
                throw new Exception("Invalid Url format. The url " + FeedUrl + " is not valid");

            if (!FeedManager.RegisterFeed(FeedUrl))
                throw new Exception("The feed " + FeedUrl + " could not be registered");


        }

        protected RssChannel Document()
        {
            return FeedManager.GetFeed(FeedUrl).Document.Channel;
        }
	
    }
}