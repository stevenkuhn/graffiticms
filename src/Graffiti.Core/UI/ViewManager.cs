using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Caching;

namespace Graffiti.Core
{
	/// <summary>
	/// Manages the loading, caching, and execution of views (and layouts)
	/// </summary>
	public static class ViewManager
	{
		public static bool Exists(string theme, string name)
		{
			return File.Exists(GetFilePath(theme, name));
		}

		/// <summary>
		/// returns the file path for a file in the theme directory. This file may not exist
		/// </summary>
		/// <param name="theme">Name of the theme</param>
		/// <param name="name">Name of the file (no .view)</param>
		public static string GetFilePath(string theme, string name)
		{
			return HttpContext.Current.Server.MapPath(string.Format("~/files/themes/{0}/{1}", theme, name));
		}

		public static string GetAbsoluateFilePath(string theme, string name)
		{
			return VirtualPathUtility.ToAbsolute(string.Format("~/files/themes/{0}/{1}", theme, name));
		}

		public static string[] GetAllViews(string theme)
		{
			List<string> views = new List<string>();
			foreach (FileInfo file in (new DirectoryInfo(GetThemePath(theme))).GetFiles("*.view"))
			{
				views.Add(file.Name);
			}
			return views.ToArray();
		}

		public static string GetThemePath(string theme)
		{
			return HttpContext.Current.Server.MapPath(string.Format("~/files/themes/{0}", theme));
		}

		/// <summary>
		/// Loads the contents of the file into memory. This file is cached and has a dependency on it
		/// </summary>
		public static string LoadFile(string theme, string view)
		{
			string path = GetFilePath(theme, view);
			return LoadFile(path);
		}

		private static string LoadFile(string path)
		{
			string fileContent = ZCache.Get<string>(path);
			if (fileContent == null)
			{
				using (StreamReader sr = new StreamReader(path))
				{
					fileContent = sr.ReadToEnd();
				}

				ZCache.MaxCache(path, fileContent, new CacheDependency(path));
			}

			return fileContent;
		}

		public static string RenderTemplate(HttpContext httpContext, GraffitiContext graffitiContext, string theme, string file)
		{
			try
			{
				return TemplateEngine.Evaluate(LoadFile(theme, file), graffitiContext);
			}
			catch (Exception ex)
			{
				if (httpContext.Request.IsAuthenticated)
					return string.Format("<p>The view {0} ({2}) could not be rendered</p><p>{1}</p>", file, ex, theme);
				else
					return "Content could not be rendered";
			}
		}

		public static string RenderTemplate(HttpContext httpContext, GraffitiContext graffitiContext, string virtualPath)
		{
			try
			{
				return TemplateEngine.Evaluate(LoadFile(httpContext.Server.MapPath(virtualPath)), graffitiContext);
			}
			catch (Exception ex)
			{
				if (httpContext.Request.IsAuthenticated)
					return string.Format("<p>The view {0} (NOT THEMED) could not be rendered</p><p>{1}</p>", virtualPath, ex);
				else
					return "Content could not be rendered";
			}
		}

		/// <summary>
		/// Renders the contents of a view and a layout into the response stream
		/// </summary>
		public static void Render(HttpContext httpContext, GraffitiContext graffitiContext, string theme)
		{
			httpContext.Response.ClearContent();
			try
			{
				graffitiContext["childContent"] = TemplateEngine.Evaluate(LoadFile(theme, graffitiContext.View), graffitiContext);
			}
			catch (DirectoryNotFoundException exDNF)
			{
				Log.Error("Site theme error", "The {0} theme seems to be missing. Graffiti CMS can not find a folder for that theme. Error details: {1}", theme, exDNF.Message);
				if (httpContext.Request.IsAuthenticated)
					httpContext.Response.Write(
						 string.Format("<h1>The {0} theme seems to be missing</h1><p>Graffiti CMS can not find a folder for that theme. Please check your files or <a href=\"{2}\">select a different theme</a>.</p><p>Error Details: {1}</p>", theme, exDNF.Message, new Urls().Admin));
				else
					httpContext.Response.Write("<h1>Site Theme Error</h1><p>Please try again later or contact the site administrator.</p>");
				return;
			}
			catch (Exception ex)
			{
				Log.Error("Site view file rendering error", "The view {0} ({1}) could not be rendered. Error details: {2}", graffitiContext.View, theme, ex.Message);
				if (httpContext.Request.IsAuthenticated)
					graffitiContext["childContent"] =
						 string.Format("<p>The view {0} ({2}) could not be rendered</p><p>Error Details: {1}</p>", graffitiContext.View, ex, theme);
				else
					graffitiContext["childContent"] = "Content could not be rendered";
			}

			try
			{
				TemplateEngine.Evaluate(httpContext.Response.Output, LoadFile(theme, graffitiContext.Layout), graffitiContext);
			}
			catch (Exception ex)
			{
				Log.Error("Site layout rendering error", "The layout {0} ({1}) could not be rendered. Error details: {2}", graffitiContext.Layout, theme, ex.Message);
				if (httpContext.Request.IsAuthenticated)
					httpContext.Response.Write(
						 string.Format("<p>The layout {0} ({2}) could not be rendered</p><p>{1}</p>", graffitiContext.Layout, ex, theme));
				else
					httpContext.Response.Write("<h1>Site Error</h1><p>Please try again later or contact the site administrator.</p>");

			}
		}
	}
}