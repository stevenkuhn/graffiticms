using System;
using System.Xml;

namespace Graffiti.Core.Marketplace
{
    public class CreatorInfo
    {
        private int _id = 0;
        private string _name = string.Empty;
        private string _location = string.Empty;
        private bool _displayLocation = false;
        private string _email = string.Empty;
        private bool _displayEmail = false;
        private string _url = string.Empty;
        private ItemInfoCollection _items;

        public CreatorInfo(XmlNode node)
        {
            XmlAttribute a = node.Attributes["id"];
            if (a != null)
                _id = int.Parse(a.Value);

            XmlNode n = node.SelectSingleNode("name");
            if (n != null)
                _name = n.InnerText;

            n = node.SelectSingleNode("location");
            if (n != null)
            {
                _location = n.InnerText;

                a = n.Attributes["display"];
                if (a != null)
                    _displayLocation = bool.Parse(a.Value);
            }

            n = node.SelectSingleNode("email");
            if (n != null)
            {
                _email = n.InnerText;

                a = n.Attributes["display"];
                if (a != null)
                    _displayEmail = bool.Parse(a.Value);
            }

            n = node.SelectSingleNode("url");
            if (n != null)
                _url = n.InnerText;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public bool DisplayLocation
        {
            get { return _displayLocation; }
            set { _displayLocation = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public bool DisplayEmail
        {
            get { return _displayEmail; }
            set { _displayEmail = value; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public ItemInfoCollection GetItems(int catalogId)
        {
            if (_items == null)
            {
                _items = new ItemInfoCollection();
                foreach (ItemInfo item in Marketplace.Catalogs[catalogId].Items.Values)
                {
                    if (item.CreatorId == Id)
                        _items.Add(item.Id, item);
                }
            }

            return _items;
        }
    }
}
