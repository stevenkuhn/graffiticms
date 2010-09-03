using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;


namespace Graffiti.Core
{
    /// <summary>
    /// Enables adding a radio button to a dynamic form.
    /// </summary>
    public class RadioFormElement : FormElement
    {
        public RadioFormElement(string name, string desc, string tip)
            : base(name, desc, tip)
        {

        }

        private Queue<ListItemFormElement> _items = new Queue<ListItemFormElement>();


        public void Add(ListItemFormElement item)
        {
            _items.Enqueue(item);
        }

        public override void Write(StringBuilder sb, NameValueCollection nvc)
        {
            string checkedValue = nvc[Name];
            if (checkedValue == null)
            {
                foreach (ListItemFormElement li in _items)
                {
                    if (li.IsDefault)
                        checkedValue = li.Value;
                }
            }

            sb.Append("\n<h2>");
            sb.AppendFormat("{0}: ", Description);

            foreach (ListItemFormElement li in _items)
            {
                sb.AppendFormat("<label for=\"{1}\"><input type=\"radio\" name=\"{0}\" id=\"{1}\" value=\"{1}\"{3} />{2}</label>&nbsp;", Name, li.Value,
                                li.Text,
                                checkedValue == li.Value ? " checked=\"checked\"" : null
                    );
            }

            sb.Append(SafeToolTip(true));
            sb.Append("</h2>");
            sb.Append("\n");
        }
    }
}