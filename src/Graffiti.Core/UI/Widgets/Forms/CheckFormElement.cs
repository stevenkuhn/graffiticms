using System.Collections.Specialized;
using System.Text;

namespace Graffiti.Core
{
    /// <summary>
    /// Enables adding checkboxes to a dyanmic form.
    /// </summary>
    public class CheckFormElement : FormElement
    {
        public CheckFormElement(string name, string desc, string tip, bool defaultValue)
            : base(name, desc, tip)
        {
            _defaultValue = defaultValue;
        }

        private bool _defaultValue;

        public override void Write(StringBuilder sb, NameValueCollection nvc)
        {
            sb.Append("\n<h2>");
            string checkValue = nvc[Name];

            if (checkValue == "checked" || checkValue == "on")
                checkValue = true.ToString();

            if (string.IsNullOrEmpty(checkValue))
                checkValue = false.ToString();

            bool isChecked = bool.Parse(checkValue ?? _defaultValue.ToString());

            sb.Append("</h2>");

            sb.AppendFormat("<input type=\"checkbox\" id=\"{0}\" name=\"{0}\" {2} /> {1}",
                            Name, Description,
                            isChecked ? "checked = \"checked\" " : null
                );

            sb.Append(SafeToolTip(true));
            sb.Append("\n");
        }
    }
}