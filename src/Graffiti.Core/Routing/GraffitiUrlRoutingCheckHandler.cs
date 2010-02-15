using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

namespace Graffiti.Core
{
	public class GraffitiUrlRoutingCheckRouteHandler : IRouteHandler
	{

		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return new GraffitiUrlRoutingCheckHandler();
		}

	}

	public class GraffitiUrlRoutingCheckHandler : IHttpHandler
	{

		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			context.Response.StatusCode = 204; // 204 == NoContent
			context.Response.AddHeader("GraffitiCMS-UrlRouting", "true");
			context.Response.End();
		}

	}
}
