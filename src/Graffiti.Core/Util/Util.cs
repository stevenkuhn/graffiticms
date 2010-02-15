using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using DataBuddy;
using System.Web.UI;

namespace Graffiti.Core
{
    /// <summary>
    /// Summary description for Util
    /// </summary>
    public static class Util
    {
		public const string DEFAULT_PAGE = "Default.aspx";
		public const string DEFAULT_PAGE_LOWERED = "default.aspx";
        private static bool _canWrite = false;

        public static bool CanWrite(HttpContext cntx)
        {
            try
            {
                if (!_canWrite)
                {
                    string path = cntx.Server.MapPath("~/files/" + Guid.NewGuid() + ".txt");
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        sw.WriteLine("Temporary file generated by Graffiti CMS to test for write access. If you see this file, it is likely your server is not properly configured to support Graffiti. Please see http://docs.graffiticms.com for more help.");
                        sw.Close();
                    }

                    File.Delete(path);
                    
                    _canWrite = true;
                }
                return _canWrite;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public static void CanWriteRedirect(HttpContext context)
        {
            if(!CanWrite(context))
            {
                context.Response.Redirect("~/graffiti-admin/msg/cannot-write.aspx");
            }
        }

        public static bool IsAccess
        {
            get { return DataService.DataProviderType == typeof(MSAccessProvider); }
        }

        public static string GetFileText(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string text = sr.ReadToEnd();
                sr.Close();
                return text;
            }
        }

		public static string NormalizePath(string path)
		{
			if( string.IsNullOrEmpty( path ) ) return string.Empty;
			
			return path.Replace(
				( (Environment.OSVersion.Platform == PlatformID.Unix) ? '\\' : '/' )
				, Path.DirectorySeparatorChar
				);
		}

        public static string FullyQualifyRelativeUrls(string html, string baseUrl)
        {
            if (!string.IsNullOrEmpty(baseUrl))
            {
                if (!baseUrl.EndsWith("/"))
                    baseUrl += "/";

                html = Regex.Replace(html, "href=\"/", "href=\"" + baseUrl, RegexOptions.IgnoreCase);
                html = Regex.Replace(html, "src=\"/", "src=\"" + baseUrl, RegexOptions.IgnoreCase);

            }

            return html;
        }

        public static string ConvertTextToHTML(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                string html = HttpUtility.HtmlEncode(text);
                html = FormatLinks(html);
                html = ConvertTextToParagraph(html);

                return html;
            }

            return text;
        }

        public static string ConvertTextToParagraph(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                text = text.Replace("\r\n", "\n").Replace("\r", "\n");
                text += "\n\n";

                text = text.Replace("\n\n", "\n");

                string[] lines = text.Split('\n');

                StringBuilder paragraphs = new StringBuilder();

                foreach (string line in lines)
                {
                    if (line != null && line.Trim().Length > 0)
                        paragraphs.AppendFormat("<p>{0}</p>\n", line);
                }

                return paragraphs.ToString();
            }

            return text;
        }


        public static string FormatLinks(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;



            //Find any links
            string pattern = @"(\s|^)(http|ftp|https):\/\/[\w]+(.[\w]+)([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])(\s|$)";
            MatchCollection matchs;
            StringCollection uniqueMatches = new StringCollection();

            matchs = Regex.Matches(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            foreach (Match m in matchs)
            {
                if (!uniqueMatches.Contains(m.ToString()))
                {
                    string link = m.ToString().Trim();
                    if (link.Length > 30)
                    {
                        try
                        {
                            Uri u = new Uri(link);
                            string absolutePath = u.AbsolutePath.EndsWith("/")
                                            ? u.AbsolutePath.Substring(0, u.AbsolutePath.Length - 1)
                                            : u.AbsolutePath;

                            int slashIndex = absolutePath.LastIndexOf("/");
                            if (slashIndex > -1)
                                absolutePath = "/..." + absolutePath.Substring(slashIndex);

                            if (absolutePath.Length > 20)
                                absolutePath = absolutePath.Substring(0, 20);

                            link = u.Host + absolutePath;
                        }
                        catch
                        {
                        }
                    }
                    text = text.Replace(m.ToString(), " <a target=\"_blank\" href=\"" + m.ToString().Trim() + "\">" + link + "</a> ");
                    uniqueMatches.Add(m.ToString());
                }
            }

            return text;
        }

        public static AspNetHostingPermissionLevel GetCurrentTrustLevel()
        {
            foreach (AspNetHostingPermissionLevel trustLevel in
                    new AspNetHostingPermissionLevel[] {
                AspNetHostingPermissionLevel.Unrestricted,
                AspNetHostingPermissionLevel.High,
                AspNetHostingPermissionLevel.Medium,
                AspNetHostingPermissionLevel.Low,
                AspNetHostingPermissionLevel.Minimal 
            })
            {
                try
                {
                    new AspNetHostingPermission(trustLevel).Demand();
                }
                catch (System.Security.SecurityException)
                {
                    continue;
                }

                return trustLevel;
            }

            return AspNetHostingPermissionLevel.None;
        }

        public static bool IsFullTrust
        {
            get
            {
                AspNetHostingPermissionLevel level = GetCurrentTrustLevel();
                return level == AspNetHostingPermissionLevel.Unrestricted || level == AspNetHostingPermissionLevel.High;
            }
        }

        public static bool AreEqualIgnoreCase(string firstString, string secondString)
        {
            // if references match (or both are null), quickly return
            if (firstString == secondString) return true;

            // if one is null, return false
            if (firstString == null || secondString == null) return false;

            // with two different string instances, call Equals method 
            return firstString.Equals(secondString,
                StringComparison.InvariantCultureIgnoreCase);
        }

        public static int PageSize { get { return SiteSettings.Get().PageSize; } }

        public static string Pager(int pageIndex, int pageSize, int totalRecords, string cssClass, string qs)
        {
            return Pager(pageIndex, pageSize, totalRecords, cssClass, qs, "&larr; Older Posts", "Newer Posts &rarr;");
        }

        public static string Pager(int pageIndex, int pageSize, int totalRecords, string cssClass, string qs, string older, string newer)
        {
            if (totalRecords <= 0 || totalRecords <= pageSize)
                return string.Empty;

            if (string.IsNullOrEmpty(cssClass))
                cssClass = "navigation";

            if (qs != null)
                qs = qs + "&amp;p=";
            else
                qs = "?p=";

            int totalPagesAvailable = totalRecords / pageSize;

            if ((totalRecords % pageSize) > 0)
                totalPagesAvailable++;

            string linkFormat = "<a href = \"{0}{1}\">{2}</a>";

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<div class = \"{0}\">", cssClass);

            if (totalPagesAvailable > pageIndex)
            {
                sb.AppendFormat("<div class=\"previous\">" + linkFormat + "</div>", qs, pageIndex + 1, older);
            }

            if (pageIndex > 1)
            {
                sb.AppendFormat("<div class=\"next\">" + linkFormat + "</div>", qs, pageIndex - 1, newer);
            }

            sb.Append("</div>");

            return sb.ToString();
        }

        public static string UnCleanForUrl(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.Replace("-", " ");
        }

        public static string CleanForUrl(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text", "Text to clean is null or empty");

            text = Regex.Replace(text.ToLower(), "[^A-Za-z0-9-]+", "-", RegexOptions.IgnoreCase);
            text = Regex.Replace(text.ToLower(), "[-]{2,}", "-", RegexOptions.IgnoreCase);

            if (text.StartsWith("-"))
                text = text.Substring(1);

            if (text.EndsWith("-"))
                text = text.Substring(0, text.Length - 1);

            return text;
        }

        #region reusable regex's
        static Regex htmlRegex = new Regex("<[^>]+>|\\&nbsp\\;", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex spacer = new Regex(@"\s{2,}", RegexOptions.Compiled);
        static Regex isWhitespace = new Regex("[^\\w&;#]", RegexOptions.Singleline | RegexOptions.Compiled);
        #endregion

        public static string RemoveHtml(string html, int charLimit)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            string nonhtml = spacer.Replace(htmlRegex.Replace(html, " ").Trim(), " ");
            if (charLimit <= 0 || charLimit >= nonhtml.Length)
                return nonhtml;
            else
                return MaxLength(nonhtml, charLimit);
        }

        public static string Truncate(string text, int charLimit)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (charLimit >= text.Length)
                return text;

            return text.Substring(0, charLimit) + "...";
        }

        public static string MaxLength(string text, int charLimit)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (charLimit >= text.Length)
                return text;

            Match match = isWhitespace.Match(text, charLimit);
            if (!match.Success)
                return text;
            else
                return text.Substring(0, match.Index);
        }

        public static List<string> ConvertStringToList(string itemText)
        {
            List<string> list = new List<string>();


            if (!string.IsNullOrEmpty(itemText))
            {
                string[] items = itemText.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in items)
                {
                    bool found = false;
                    foreach (string existingItem in list)
                    {
                        if (existingItem.Equals(item, StringComparison.InvariantCultureIgnoreCase))
                        {
                            found = true;
                        }
                    }

                    if (!found)
                        list.Add(item);
                }
            }

            return list;
        }

        public static bool UseGoogleForjQuery
        {
            get
            {//<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"></script><script type="text/javascript">
                return SiteSettings.Get().UseGoogleForJQuery;               
            }
        }

        public static bool IsValidFileName(string name)
        {
            return !Regex.IsMatch(name, "^(" + ConfigurationManager.AppSettings["Graffiti::ExcludedNames"] + ")$", RegexOptions.IgnoreCase);
        }

        public static string[] Randomize(string[] original, int numberOfItemsToReturn)
        {
            //If there are no items or a single item, there is not much to do
            if (original == null || original.Length <= 1)
                return original;

            //the max items cannot be larger than the number of items we have to work with
            int max = (original.Length > numberOfItemsToReturn ? numberOfItemsToReturn : original.Length);

            //Create a list of hold our results
            string[] newArray = new string[max];

            //Be kind and do not edit the original array (also easier to remove from a collection)
            List<string> copy = new List<string>(original);

            //weakest link, Radmomizing the Radomizer....
            Random rnd = new Random(Guid.NewGuid().GetHashCode());

            //loop through N times and find an item. When we find one, we should remove it as an option
            for (int i = 0; i < max; i++)
            {
                int n = rnd.Next(0, copy.Count);
                newArray[i] = copy[n];
                copy.RemoveAt(n);
            }

            return newArray;
        }

        public static void CreateFile(string fileName, string fileData)
        {
            FileInfo fi = new FileInfo(fileName);

            if (!fi.Directory.Exists)
                fi.Directory.Create();

            using (StreamWriter sw = new StreamWriter(fi.FullName))
            {
                sw.Write(fileData);
            }
        }

        private static StringDictionary _mimeMap;

        static Util()
        {
            _mimeMap = new StringDictionary();

            _mimeMap.Add("csv", "application/vnd.ms-excel");
            _mimeMap.Add("css", "text/css");
            _mimeMap.Add("js", "text/javascript");
            _mimeMap.Add("doc", "application/msword");
            _mimeMap.Add("gif", "image/gif");
            _mimeMap.Add("bmp", "image/bmp");
            _mimeMap.Add("htm", "text/html");
            _mimeMap.Add("html", "text/html");
            _mimeMap.Add("jpeg", "image/jpeg");
            _mimeMap.Add("jpg", "image/jpeg");
            _mimeMap.Add("pdf", "application/pdf");
            _mimeMap.Add("png", "image/png");
            _mimeMap.Add("ppt", "application/vnd.ms-powerpoint");
            _mimeMap.Add("rtf", "application/msword");
            _mimeMap.Add("txt", "text/plain");
            _mimeMap.Add("xls", "application/vnd.ms-excel");
            _mimeMap.Add("xml", "text/xml");
            _mimeMap.Add("wmv", "video/x-ms-wmv");
            _mimeMap.Add("wma", "video/x-ms-wmv");
            _mimeMap.Add("mpeg", "video/mpeg");
            _mimeMap.Add("mpg", "video/mpeg");
            _mimeMap.Add("mpa", "video/mpeg");
            _mimeMap.Add("mpe", "video/mpeg");
            _mimeMap.Add("mov", "video/quicktime");
            _mimeMap.Add("qt", "video/quicktime");
            _mimeMap.Add("avi", "video/x-msvideo");
            _mimeMap.Add("asf", "video/x-ms-asf");
            _mimeMap.Add("asr", "video/x-ms-asf");
            _mimeMap.Add("asx", "video/x-ms-asf");
            _mimeMap.Add("swf", "application/x-shockwave-flash");
            _mimeMap.Add("vm", "vm/text");
        }


        public static string GetMapping(string filename)
        {
            string result = null;
            int idx = filename.LastIndexOf('.');

			if (idx > 0 && idx > filename.LastIndexOf(Path.DirectorySeparatorChar))
                result = _mimeMap[filename.Substring(idx + 1).ToLower(CultureInfo.InvariantCulture)];

            if (result == null)
                return "application/octet-stream";
            else
                return result;
        }

        public static void RedirectToSSL(HttpContext context)
        {
            if (!context.Request.IsSecureConnection)
            {
                string url = new Macros().FullUrl(context.Request.RawUrl);
                context.Response.Redirect("https://" + url.Substring(7));
            }
        }

        public static Control FindControlRecursive(Control root, string id)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root", "Cannot be null.");
            }

            if (root.ID == id)
            {
                return root;
            }

            foreach (Control c in root.Controls)
            {
                Control t = FindControlRecursive(c, id);
                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }

		public static bool CheckUrlRoutingSupport()
		{
			Macros macros = new Macros();
			try
			{
				HttpWebResponse response = GRequest.GetResponse(macros.FullUrl(VirtualPathUtility.ToAbsolute("~/__utility/GraffitiUrlRoutingCheck")));

				if (response.StatusCode == HttpStatusCode.NoContent)
				{
					string headerValue = response.Headers.Get("GraffitiCMS-UrlRouting");
					if (!string.IsNullOrEmpty(headerValue))
					{
						bool urlRoutingSupported = false;
						if (bool.TryParse(headerValue, out urlRoutingSupported))
							return urlRoutingSupported;
					}
				}
			}
			catch { }

			return false;
		}

    }
}