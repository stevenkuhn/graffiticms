using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.UI;
using System.IO;

namespace Graffiti.Core
{
    public class DateTimeFormElement : FormElement
    {
        public DateTimeFormElement(string name, string desc, string tip)
            : base(name, desc, tip)
        {
        }

        public override void Write(StringBuilder sb, NameValueCollection nvc)
        {
            sb.Append("\n<h2>");
            sb.Append(Description);
            sb.Append(": " + SafeToolTip(false));
            sb.Append("</h2><div>");

            string guid = System.Guid.NewGuid().ToString();
            guid = guid.Replace("-", "");

            sb.AppendFormat("<input type=\"text\" id=\"ds_state" + guid + "\" name=\"{0}\" value=\"{1}\" size=\"40\" />", Name, nvc[Name]);

            sb.Append("</div>");

            //sb.AppendFormat(
            //    "<input type=\"hidden\" id=\"{0}\" name=\"{0}\" value=\"{1}\" />",
            //    Name, HttpUtility.HtmlEncode(nvc[Name]));

            //sb.AppendFormat("<input type=\"hidden\" id=\"{0}___Config\" value=\"\" />", Name);
            //sb.AppendFormat(
            //    "<iframe id=\"{0}___Frame\" src=\"{1}editor/teeditor.html?InstanceName={0}&amp;Toolbar=Simple\" width=\"600px\" height=\"200px\" frameborder=\"no\" scrolling=\"no\"></iframe>", Name, VirtualPathUtility.ToAbsolute(ConfigurationManager.AppSettings["Telligent_Glow_Editor:BasePath"]));
            
            // ad the script portion ONLY for the ajax call
            Page p = HttpContext.Current.Handler as Page;
            if (p == null)
            {
                string js = "ds" + guid + " = new Telligent_DateTimeSelector('ds" + guid + "', 'ds_state" + guid + "', '<January,February,March,April,May,June,July,August,September,October,November,December> <1-31>, <1900-3000> at <1-12>:<00-59> <am,pm>', 2, 0, 1, 3, 4, 5, true, true, null);";

                sb.Append("startscript:" + js + ":endscript");
            }
        }
    }
}