using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Graffiti.Core.Marketplace
{
    public class CatalogInfoCollection : Dictionary<CatalogType, CatalogInfo>
    {
        public CatalogInfoCollection()
        {
        }

        public CatalogInfoCollection(IEnumerable<XElement> nodes)
        {
            foreach (XElement node in nodes)
            {
                CatalogInfo catalogInfo = new CatalogInfo(node);
                Add(catalogInfo.Type, catalogInfo);
            }
        }
    }
}
