using System;
using System.Xml;

namespace Graffiti.Core.Marketplace
{
    public class MessageInfo
    {
        private int _id = 0;
        private string _text = string.Empty;

        public MessageInfo(XmlNode node)
        {
            XmlAttribute a = node.Attributes["id"];
            if (a != null)
                _id = int.Parse(a.Value);

            XmlNode n = node.SelectSingleNode("text");
            if (n != null)
                _text = n.InnerXml;
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
    }
}
