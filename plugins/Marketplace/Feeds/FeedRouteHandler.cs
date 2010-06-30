using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Graffiti.Core;

namespace Graffiti.Marketplace
{
	public class FeedRouteHandler : IRouteHandler
	{

		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
            string feedType = requestContext.RouteData.Values["FeedType"] != null ? requestContext.RouteData.Values["FeedType"].ToString() : null;
            string categoryName = requestContext.RouteData.Values["CategoryName"] != null ? requestContext.RouteData.Values["CategoryName"].ToString() : null;
            string feed = requestContext.RouteData.Values["Feed"] != null ? requestContext.RouteData.Values["Feed"].ToString() : null;

            switch (feedType)
			{
                case "catalogs":
                    if (categoryName == null || feed == null)
                        return new CatalogWriter();
                    switch (feed)
                    {
                        case "categories":
                            return new CategoryWriter(categoryName);
                        case "items":
                            return new ItemWriter(categoryName);
                        case "messages":
                            return new MessageWriter(categoryName);
                    }
                    return new CatalogWriter();


                case "messages":
				    return new MessageWriter();
                case "creators":
					return new CreatorWriter();
			}

            throw new HttpException(404, "Page not found");

		}

	}
}
