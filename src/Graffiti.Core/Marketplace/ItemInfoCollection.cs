using System;
using System.Collections.Generic;
using System.Xml;

namespace Graffiti.Core.Marketplace
{
    public class ItemInfoCollection : Dictionary<int, ItemInfo>
    {
        public ItemInfoCollection()
        {
        }

        public ItemInfoCollection(CatalogInfo catalogInfo, XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                ItemInfo itemInfo = new ItemInfo(catalogInfo, node);
                Add(itemInfo.Id, itemInfo);
            }
        }
    }
}
