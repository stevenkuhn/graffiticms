using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Graffiti.Core
{
    public class FileBrowser : Control 
    {
        public static void RegisterJavaScript(Page page)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("var fileBrowserCallback = null;\n");
            sb.Append("function OpenFileBrowser(callbackFunction){\n");
            sb.Append("fileBrowserCallback = callbackFunction;\n");
            sb.AppendFormat("var url = '{0}';\n", VirtualPathUtility.ToAbsolute("~/graffiti-admin/site-options/utilities/FileSelector.aspx"));
            sb.AppendFormat("url += '?path=files\\\\media';");
            sb.Append("window.open(url, 'TEBrowseWindow', \"toolbar=no,status=no,resizable=yes,dependent=yes,scrollbars=yes,width=\" + (screen.width * 0.7) + \",height=\" + (screen.height * 0.7) + \",left=\" + ((screen.width * 0.3) / 2) + \",top=\" + ((screen.height * 0.3) / 2));\n");
            sb.Append("}\n");

            sb.Append("function SetUrl(url)\n");
            sb.Append("{\n");
            sb.Append("      fileBrowserCallback(url);\n");
            sb.Append("}\n");

            page.ClientScript.RegisterClientScriptBlock(typeof(FileBrowser), "FileBrowser", sb.ToString(), true);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            RegisterJavaScript(this.Page);

            base.Render(writer);
        }
    }
}
