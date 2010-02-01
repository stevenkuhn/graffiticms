using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;

namespace Graffiti.Core
{
	public class CategoryAndPostHandler : IRouteHandler
	{
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			string path = requestContext.RouteData.Values["path"] != null ? requestContext.RouteData.Values["path"].ToString() : null;
			string[] pathParts = GetPaths(path);

			if (pathParts.Length > 2)
			{
				// Assume it is a post if more than 2 levels deep for now
				// ToDo: Needs to be reworked to support n-level categories
				var post = GetPost(pathParts[pathParts.Length - 1]);
				if (post != null)
					return post;
			}
			else if (pathParts.Length == 2)
			{
				// either a sub-category or post request
				var category = GetCategory(string.Format("{0}/{1}", pathParts[0], pathParts[1]));
				if (category != null)
					return category;

				var post = GetPost(pathParts[1]);
				if (post != null)
					return post;
			}
			else if (pathParts.Length == 1)
			{
				// either a category or post request
				var category = GetCategory(pathParts[0]);
				if (category != null)
					return category;

				var post = GetPost(pathParts[0]);
				if (post != null)
					return post;
			}

			throw new HttpException(404, "Page not found");
		}

		public string[] GetPaths(string path)
		{
			string[] pathArray;

			if (string.IsNullOrEmpty(path))
				pathArray = new string[0];
			else
			{
				pathArray = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

				for (int i = 0; i < pathArray.Length; i++)
				{
					if (pathArray[i].IndexOfAny(new char[] { '<', '>', '"' }) > -1)
						pathArray[i] = HttpUtility.HtmlEncode(pathArray[i]);
				}
			}

			return pathArray;
		}

		// Not used yet, will be needed to support n-level categories
		public List<Category> GetCategories(string path)
		{
			CategoryController cc = new CategoryController();
			var categoryStack = new List<Category>() { cc.GetUnCategorizedCategory() };

			string[] pathArray = GetPaths(path);
			string lastCategoryPath = pathArray.LastOrDefault();

			foreach (string linkName in pathArray)
			{
				if (string.IsNullOrEmpty(linkName))
					continue;

				Category parentCategory = categoryStack[categoryStack.Count - 1];
				Category category = cc.GetCachedCategoryByLinkName(string.Format("{0}/{1}", parentCategory.LinkName, linkName), parentCategory.Id, true);

				// If category cannot be found, we reached the end of the stack and remaining items must be content
				if (category == null)
					break;

				categoryStack.Add(category);
			}

			return categoryStack;
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