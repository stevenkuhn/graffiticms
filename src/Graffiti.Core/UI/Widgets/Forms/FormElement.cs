using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;


namespace Graffiti.Core
{
	public class FormElementCollection : List<FormElement>
	{

	}

	/// <summary>
	/// Root object for building a dynamic form element
	/// </summary>
	public abstract class FormElement
	{
		public string Name { get; private set; }
		public string Tip { get; private set; }
		public string Description { get; private set; }

		public FormElement(string name, string desc, string tip)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("Cannot be null or empty", "name");

			if (string.IsNullOrEmpty(desc))
				throw new ArgumentException("Cannot be null or empty", "desc");

			Name = name;
			Tip = tip;
			Description = desc;
		}

		/// <summary>
		/// Adds the tooltip only if it exists
		/// </summary>
		/// <param name="addBreak"></param>
		/// <returns></returns>
		protected virtual string SafeToolTip(bool addBreak)
		{
			if (!string.IsNullOrEmpty(Tip))
				return string.Format("{0}<span class=\"form_tip\">{1}</span>", addBreak ? "<br />" : null, Tip);

			return string.Empty;
		}

		public abstract void Write(StringBuilder sb, NameValueCollection nvc);
	}

}