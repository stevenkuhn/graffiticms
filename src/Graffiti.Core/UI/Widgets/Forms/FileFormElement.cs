using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Graffiti.Core
{
    public class FileFormElement : FormElement
    {
        public FileFormElement(string name, string desc, string tip)
            : base(name, desc, tip)
        {
        }

        public override void Write(StringBuilder sb, NameValueCollection nvc)
        {
            if (HttpContext.Current.Handler is Page)
                FileBrowser.RegisterJavaScript((Page) HttpContext.Current.Handler);

            sb.Append("\n<h2>");
            sb.Append(Description);
            sb.Append(": " + SafeToolTip(false));
            sb.Append("</h2>");
            sb.AppendFormat("<input type=\"text\" id=\"{0}\" name=\"{0}\" class=\"small\" value=\"{1}\" /> <input class=\"inputbutton\" type=\"button\" value=\"Select ...\" onclick=\"OpenFileBrowser(new Function('url', '$(\\'{0}\\').value = url;'));return false\" />",
                            Name, nvc[Name]);
            sb.Append("\n");
        }
    }
}