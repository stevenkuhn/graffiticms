using System.Web;
using System.Web.Compilation;
using System.Web.Routing;

namespace Graffiti.Core
{
	public class TagHandler : IRouteHandler
	{
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			string tagName = requestContext.RouteData.Values["tagname"] != null ? requestContext.RouteData.Values["tagname"].ToString() : null;

			TagPage tp = new TagPage();
			tp.TagName = tagName;
			tp.MetaDescription = "Posts and articles tagged as " + tagName;
			tp.MetaKeywords = tagName;

			return tp;
		}
	}
}