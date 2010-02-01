using System.Collections.Specialized;
using System.Text;

namespace Graffiti.Core
{

    public class TextAreaFormElement : FormElement
    {
        public TextAreaFormElement(string name, string desc, string tip, int rows)
            : base(name, desc, tip)
        {
            _rows = rows;
        }

        private int _rows;

        public override void Write(StringBuilder sb, NameValueCollection nvc)
        {
            sb.Append("\n<h2>");
            sb.Append(Description);
            sb.Append(": " + SafeToolTip(false));
            sb.Append("</h2>");
            sb.AppendFormat("<textarea cols=\"60\" rows=\"{1}\"  id=\"{0}\" name=\"{0}\" class=\"large\" >{2}</textarea>",
                            Name, _rows, nvc[Name]);

            sb.Append("\n");
        }
    }
}