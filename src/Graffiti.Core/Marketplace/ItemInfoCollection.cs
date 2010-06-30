using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Graffiti.Core.Marketplace
{
    public class ItemInfoCollection : Dictionary<int, ItemInfo>
    {
        public ItemInfoCollection()
        {
        }

        public ItemInfoCollection(CatalogInfo catalogInfo, IEnumerable<XElement> nodes)
        {
            foreach (XElement node in nodes)
            {
                ItemInfo itemInfo = new ItemInfo(catalogInfo, node);
                Add(itemInfo.Id, itemInfo);
            }
        }
    }
}
