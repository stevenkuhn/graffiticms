using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


namespace Graffiti.Core
{
    /// <summary>
    /// Summary description for PageWriter
    /// </summary>
    public static class PageWriter
    {
        public static void Write(string pageTemplateFile, string virtualPath, PageTemplateToolboxContext cntxt)
        {
            if (SiteSettings.Get().GenerateFolders)
            {
                string fileText =
                    Util.GetFileText(HttpContext.Current.Server.MapPath("~/__utility/pages/" + pageTemplateFile));
                //fileText = string.Format(fileText, list.ToArray());

                fileText = TemplateEngine.Evaluate(fileText, cntxt);

                string absPath = HttpContext.Current.Server.MapPath(virtualPath);
                FileInfo fi = new FileInfo(absPath);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }

                using (StreamWriter sw = new StreamWriter(absPath, false))
                {
                    sw.WriteLine(fileText);
                    sw.Close();
                }
            }
        }
    }
}