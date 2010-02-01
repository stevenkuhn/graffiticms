using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Web;

namespace Graffiti.Core
{
    public class WYWIWYGFormElement : FormElement
    {
        public WYWIWYGFormElement(string name, string desc, string tip)
            : base(name, desc, tip)
        {
        }

        public override void Write(StringBuilder sb, NameValueCollection nvc)
        {
            sb.Append("\n<h2>");
            sb.Append(Description);
            sb.Append(": " + SafeToolTip(false));
            sb.Append("</h2><div>");

            sb.AppendFormat(
                "<input type=\"hidden\" id=\"{0}\" name=\"{0}\" value=\"{1}\" />",
                Name, HttpUtility.HtmlEncode(nvc[Name]));

            sb.AppendFormat("<input type=\"hidden\" id=\"{0}___Config\" value=\"\" />", Name);
            sb.AppendFormat(
                "<iframe id=\"{0}___Frame\" src=\"{1}editor/teeditor.html?InstanceName={0}&amp;Toolbar=Simple\" width=\"600px\" height=\"200px\" frameborder=\"no\" scrolling=\"no\"></iframe>", Name, VirtualPathUtility.ToAbsolute(ConfigurationManager.AppSettings["Telligent_Glow_Editor:BasePath"]));
            sb.Append("</div>");
        }
    }
}