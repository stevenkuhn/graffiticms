using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

namespace Graffiti.BlogExtensions
{
	public class PingbackRouteHandler : IRouteHandler
	{

		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return new PingBackHandler();
		}

	}
}
