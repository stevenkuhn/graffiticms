using System.Collections.Generic;
using RssToolkit.Rss;

namespace Graffiti.Core
{
    /// <summary>
    /// Feeds is a helper class for retrieving data about other feeds.
    /// </summary>
    [Chalk("feeds")]
    public class Feeds
    {
        public RssChannel GetDocument(string url)
        {
            Feed feed = FeedManager.GetFeed(url);
            if (feed != null)
                return feed.Document.Channel;

            return new RssChannel();
        }

        public List<RssItem> GetItems(string url)
        {
            return GetDocument(url).Items;
        }
    }
}