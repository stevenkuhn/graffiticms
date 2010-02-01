using System;
using System.Web.Routing;

namespace Graffiti.Core
{
	public static class UrlRouting
	{
		public static void Initialize()
		{
			RegisterRoutes(RouteTable.Routes);

		}

		private static void RegisterRoutes(RouteCollection routes)
		{

			// Do not use url routing for requests with .axd/.asmx/.ico extensions
			// This prevents ASP.NET from having to do File.Exists check
			routes.Add("ignoreAXD", new Route("{resource}.axd/{*pathInfo}", new StopRoutingHandler()));
			routes.Add("ignoreASMX", new Route("{resource}.asmx/{*pathInfo}", new StopRoutingHandler()));
			routes.Add("ignoreICO", new Route("{resource}.ico/{*pathInfo}", new StopRoutingHandler()));
			// Ignore all of admin section
			routes.Add("ignoreADMIN", new Route("graffiti-admin/{*pathInfo}", new StopRoutingHandler()));

			routes.Add("TagPage", new Route("tags/{TagName}/", new TagHandler()));
			routes.Add("TagFeed", new Route("tags/{TagName}/feed/", new RssHandler()));

			routes.Add("SiteFeed", new Route("feed/", new RssHandler()));
			routes.Add("Category1Feed", new Route("{CategoryOne}/feed/", new RssHandler()));
			routes.Add("Category2Feed", new Route("{CategoryOne}/{CategoryTwo}/feed/", new RssHandler()));

			// Allow plugins to add Routes before the default CategoryAndPost one
			Events.Instance().ExecuteUrlRoutingAdd(routes);

			routes.Add("CategoryAndPost", new Route("{*path}", new CategoryAndPostHandler()));
		}

		public static void AddRoute(Route route)
		{
			// Ensure route does not already exist
			foreach (Route r in RouteTable.Routes)
			{
				if (r.Url == route.Url)
					return;
			}

			// Add new routes before the last default rout (CategoryAndPost)
			RouteTable.Routes.Insert(RouteTable.Routes.Count - 1, route);
		}

	}
}