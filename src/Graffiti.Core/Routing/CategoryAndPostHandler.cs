using System;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;

namespace Graffiti.Core
{
	public class CategoryAndPostHandler : IRouteHandler
	{
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			string param1 = requestContext.RouteData.Values["param1"] != null ? requestContext.RouteData.Values["param1"].ToString() : null;
			string param2 = requestContext.RouteData.Values["param2"] != null ? requestContext.RouteData.Values["param2"].ToString() : null;
			string param3 = requestContext.RouteData.Values["param3"] != null ? requestContext.RouteData.Values["param3"].ToString() : null;

			if (!String.IsNullOrEmpty(param3))
			{
				var post = GetPost(param3);
				if (post != null)
					return post;
			}
			else if (!String.IsNullOrEmpty(param2))
			{
				// either a sub-category or post request
				var category = GetCategory(string.Format("{0}/{1}", param1, param2));
				if (category != null)
					return category;

				var post = GetPost(param2);
				if (post != null)
					return post;
			}
			else if (!String.IsNullOrEmpty(param1))
			{
				// either a category or post request
				var category = GetCategory(param1);
				if (category != null)
					return category;

				var post = GetPost(param1);
				if (post != null)
					return post;
			}

			throw new HttpException(404, "Page not found");
		}

		public CategoryPage GetCategory(string param)
		{
			var category = new CategoryController().GetCachedCategoryByLinkName(param, true);

			if (category != null)
			{
				var categoryPage = new CategoryPage();

				categoryPage.CategoryID = category.Id;
				categoryPage.CategoryName = category.LinkName;
				categoryPage.MetaDescription = category.MetaDescription;
				categoryPage.MetaKeywords = category.MetaKeywords;

				return categoryPage;
			}

			return null;
		}

		public PostPage GetPost(string param)
		{
			var post = new Data().GetPost(param);

			if (post != null)
			{
				var postPage = new PostPage();

				postPage.PostId = post.Id;
				postPage.CategoryID = post.CategoryId;
				postPage.CategoryName = new CategoryController().GetCachedCategory(post.CategoryId, false).LinkName;
				postPage.PostName = post.Name;
				postPage.Name = post.Name;
				postPage.MetaDescription = post.MetaDescription;
				postPage.MetaKeywords = post.MetaKeywords;

				return postPage;
			}

			return null;
		}
	}
}
