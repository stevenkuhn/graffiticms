using System;
using System.Xml;

namespace Graffiti.Core.Marketplace
{
    public class CategoryInfo
    {
        private CatalogInfo _catalog;
        private int _id = 0;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private ItemInfoCollection _items;

        public CategoryInfo(CatalogInfo catalog, XmlNode node)
        {
            _catalog = catalog;

            XmlAttribute a = node.Attributes["id"];
            if (a != null)
                _id = int.Parse(a.Value);

            XmlNode n = node.SelectSingleNode("name");
            if (n != null)
                _name = n.InnerText;

            n = node.SelectSingleNode("description");
            if (n != null)
                _description = n.InnerText;
        }

        public CatalogInfo Catalog
        {
            get { return _catalog; }
            set { _catalog = value; }
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

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public ItemInfoCollection Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ItemInfoCollection();
                    foreach (ItemInfo item in Catalog.Items.Values)
                    {
                        if (item.CategoryId == Id)
                            _items.Add(item.Id, item);
                    }
                }

                return _items;
            }
        }
    }
}
