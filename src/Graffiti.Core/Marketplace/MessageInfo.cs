using System;
using System.Xml.Linq;

namespace Graffiti.Core.Marketplace
{
    public class MessageInfo
    {
        private int _id = 0;
        private string _title = string.Empty;
        private string _text = string.Empty;

        public MessageInfo(XElement node)
        {
            string value;

            if (node.TryGetAttributeValue("id", out value))
                _id = int.Parse(value);

            XElement n = node.Element("title");
            if (n != null && n.TryGetValue(out value))
                _title = value;

            n = node.Element("text");
            if (n != null && n.TryGetValue(out value))
                _text = value;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

    }
}
