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
		public DateTimeFormElement(string name, string desc, string tip) : base(name, desc, tip)
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

			// add the script portion ONLY for the ajax call
			Page p = HttpContext.Current.Handler as Page;
			if (p == null)
			{
				string js = "ds" + guid + " = new Telligent_DateTimeSelector('ds" + guid + "', 'ds_state" + guid + "', '<January,February,March,April,May,June,July,August,September,October,November,December> <1-31> <1900-3000> <01-12>:<00-59> <AM,PM>', 2, 0, 1, 3, 4, 5, true, true, null);";

				sb.Append("startscript:" + js + ":endscript");
			}
		}
	}
}