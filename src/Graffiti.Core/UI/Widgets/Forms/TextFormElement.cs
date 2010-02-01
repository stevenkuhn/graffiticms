using System.Collections.Specialized;
using System.Text;


namespace Graffiti.Core
{

    public class TextFormElement : FormElement
    {
        public TextFormElement(string name, string desc, string tip)
            : base(name, desc, tip)
        {
        }


        public override void Write(StringBuilder sb, NameValueCollection nvc)
        {
            sb.Append("\n<h2>");
            sb.Append(Description);
            sb.Append(": " + SafeToolTip(false));
            sb.Append("</h2>");
            sb.AppendFormat("<input type=\"text\" id=\"{0}\" name=\"{0}\" class=\"large\" value=\"{1}\" />",
                            Name, nvc[Name]);
            sb.Append("\n");
        }
    }

    public class MessageElement : FormElement
    {
        public MessageElement(string name, string desc, string tip)
            : base(name, desc, tip)
        {
        }


        public override void Write(StringBuilder sb, NameValueCollection nvc)
        {
            sb.Append("\n<h2>");
            sb.Append(Description);
            sb.Append("<br /> " + SafeToolTip(false));
            sb.Append("</h2>");
           sb.Append("\n");
        }
    }

}