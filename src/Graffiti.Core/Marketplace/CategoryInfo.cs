using System;
using System.Xml.Linq;

namespace Graffiti.Core.Marketplace
{
    public class CategoryInfo
    {
        private CatalogInfo _catalog;
        private int _id = 0;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private ItemInfoCollection _items;

        public CategoryInfo(CatalogInfo catalog, XElement node)
        {
            _catalog = catalog;

            string value;

            if (node.TryGetAttributeValue("id", out value))
                _id = int.Parse(value);

            XElement n = node.Element("name");
            if (n != null && n.TryGetValue(out value))
                _name = value;

            n = node.Element("description");
            if (n != null && n.TryGetValue(out value))
                _description = value;
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
