using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Graffiti.Core;

namespace Graffiti.Marketplace
{
    /// <summary>
    /// Collection of Marketplace related Chalk extensions
    /// </summary>
    [Chalk("marketplace")]
    public class MarketplaceChalk
    {

        public string DownloadLink(Post post)
        {
            return VirtualPathUtility.ToAbsolute("~/download/") + post.Id.ToString();
        }

        public string FileDownloader(Post post)
        {
            // If the "download" querystring attrib exists, stream the download file
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["download"]))
            {
                return string.Format("<iframe src=\"{0}\" width=\"1\" height=\"1\"></iframe>", "");
            }

            return string.Empty;
        }

        public ItemStatistics PostItemStatistics(Post post)
        {
            if (post != null)
                return DataHelper.GetMarketplacePostStats(post.Id);

            return new ItemStatistics();
        }
    }
}
