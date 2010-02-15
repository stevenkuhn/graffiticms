using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Graffiti.Core
{
	public class WYWIWYGFormElement : FormElement
	{
		public WYWIWYGFormElement(string name, string desc, string tip) : base(name, desc, tip)
		{
		}

		public override void Write(StringBuilder sb, NameValueCollection nvc)
		{
			GraffitiEditor.SetupScripts(HttpContext.Current.Handler as System.Web.UI.Page);
			GraffitiEditor editor = new GraffitiEditor();
			editor.ToolbarSet = "Simple";
			editor.Width = new Unit(600);
			editor.Height = new Unit(200);
			editor.Text = HttpUtility.HtmlEncode(nvc[Name]);
			editor.ID = Name;
			sb.Append(editor.GenerateHTML());
		}
	}
}