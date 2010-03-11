using System.Collections.Specialized;
using System.Text;

namespace Graffiti.Core
{
	/// <summary>
	/// Enables adding checkboxes to a dyanmic form.
	/// </summary>
	public class CheckFormElement : FormElement
	{
		internal bool DefaultValue { get; private set; }
		internal bool IsNew { get; private set; }

		public CheckFormElement(string name, string desc, string tip, bool defaultValue)
			: this(name, desc, tip, defaultValue, false)
		{
		}

		public CheckFormElement(string name, string desc, string tip, bool defaultValue, bool isNew)
			: base(name, desc, tip)
		{
			DefaultValue = defaultValue;
			IsNew = isNew;
		}


		public override void Write(StringBuilder sb, NameValueCollection nvc)
		{
			sb.Append("\n<h2>");
			string checkValue = nvc[Name];

			bool isChecked = false;
			if (IsNew)
				isChecked = DefaultValue;
			else if (checkValue == "checked" || checkValue == "on")
				isChecked = true;
			else if (string.IsNullOrEmpty(checkValue))
				isChecked = false;
			else
				isChecked = bool.Parse(checkValue);

			sb.Append("</h2>");
			sb.AppendFormat("<input type=\"checkbox\" id=\"{0}\" name=\"{0}\" {2} /> {1}",
								 Name, Description,
								 isChecked ? "checked = \"checked\" " : null);
			sb.Append(SafeToolTip(true));
			sb.Append("\n");
		}
	}
}