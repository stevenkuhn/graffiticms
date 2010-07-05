using System;
using System.Xml.Linq;

namespace Graffiti.Core.Marketplace
{
    public class CreatorInfo
    {
        private string _id = string.Empty;
        private string _name = string.Empty;
        private string _email = string.Empty;
        private bool _displayEmail = false;
        private string _bio = string.Empty;
        private string _url = string.Empty;
        private ItemInfoCollection _items;

        public CreatorInfo(XElement node)
        {
            string value;

            if (node.TryGetAttributeValue("id", out value))
                _id = value;

            XElement n = node.Element("name");
            if (n != null && n.TryGetValue(out value))
                _name = value;

            n = node.Element("email");
            if (n != null && n.TryGetValue(out value))
            {
                _email = value;

                if (n.TryGetAttributeValue("display", out value))
                    _displayEmail = bool.Parse(value);
            }

            n = node.Element("bio");
            if (n != null && n.TryGetValue(out value))
                _bio = value;

            n = node.Element("url");
            if (n != null && n.TryGetValue(out value))
                _url = value;

        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Bio
        {
            get { return _bio; }
            set { _bio = value; }
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

        public ItemInfoCollection GetItems(CatalogType catType)
        {
            if (_items == null)
            {
                _items = new ItemInfoCollection();
                foreach (ItemInfo item in Marketplace.Catalogs[catType].Items.Values)
                {
                    if (Util.AreEqualIgnoreCase(item.CreatorId, Id))
                        _items.Add(item.Id, item);
                }
            }

            return _items;
        }
    }
}
