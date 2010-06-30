using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Graffiti.Core;

namespace Graffiti.Marketplace
{
    public class DownloadRouteHandler : IRouteHandler
	{

		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
            string postID = requestContext.RouteData.Values["PostID"] != null ? requestContext.RouteData.Values["PostID"].ToString() : null;
            if (!string.IsNullOrEmpty(postID))
            {

                return new DownloadHandler(postID);
            }

            throw new HttpException(404, "Page not found");
		}

	}
}
