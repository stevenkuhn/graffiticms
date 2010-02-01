using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;


namespace Graffiti.Core
{
    /// <summary>
    /// Enables adding a dropdown list to a dynamic form.
    /// </summary>
    public class ListFormElement : FormElement
    {
        public ListFormElement(string name, string desc, string tip)
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
            sb.Append(Description);

            sb.AppendFormat(": <select id=\"{0}\" name=\"{0}\" style=\"width:150px;margin-left:5px;\">",
                            Name);



            foreach (ListItemFormElement li in _items)
            {
                sb.AppendFormat("<option value=\"{0}\" {2}>{1}</option>", li.Value,
                                li.Text,
                                checkedValue == li.Value ? "selected=\"selected\"" : null
                    );
            }

            sb.Append("</select>");

            sb.Append(SafeToolTip(true));
            sb.Append("</h2>");
            sb.Append("\n");
        }
    }

    [Serializable]
    public class ListItemFormElement
    {

        public ListItemFormElement()
        {
        }

        public ListItemFormElement(string text, string val)
            : this(text, val, false)
        {
        }

        public ListItemFormElement(string text, string val, bool isDefault)
        {
            _text = text;
            _value = val;
            _isDefault = isDefault;
        }

        private string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _text;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private bool _isDefault;

        public bool IsDefault
        {
            get { return _isDefault; }
            set { _isDefault = value; }
        }


    }
}