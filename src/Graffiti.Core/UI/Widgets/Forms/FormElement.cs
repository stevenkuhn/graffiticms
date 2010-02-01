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
        public FormElement(string name, string desc, string tip)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Cannot be null or empty", "name");

            if (string.IsNullOrEmpty(desc))
                throw new ArgumentException("Cannot be null or empty", "desc");

            _name = name;
            _desc = desc;
            _tip = tip;
        }

        private string _name;
        private string _tip;

        public string Name
        {
            get { return _name; }
        }

        public string Tip
        {
            get { return _tip; }
        }

        private string _desc;

        public string Description
        {
            get { return _desc; }
        }

        /// <summary>
        /// Adds the tooltip only if it exists
        /// </summary>
        /// <param name="addBreak"></param>
        /// <returns></returns>
        protected virtual string SafeToolTip(bool addBreak)
        {
            if (!string.IsNullOrEmpty(Tip))
            {
                return string.Format("{0}<span class=\"form_tip\">{1}</span>", addBreak ? "<br />" : null, Tip);
            }

            return string.Empty;
        }

        public abstract void Write(StringBuilder sb, NameValueCollection nvc);

    }

}