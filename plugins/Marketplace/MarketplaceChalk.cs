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

            // If the "download" querystring attrib exists, stream the download file
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["download"]))
            {
                // ToDo: render iframe or script to call the DownloadHandler
            }

            return string.Empty;
        }


    }
}
