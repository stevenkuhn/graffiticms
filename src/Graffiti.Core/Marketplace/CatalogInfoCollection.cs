using System;
using System.Collections.Generic;
using System.Xml;

namespace Graffiti.Core.Marketplace
{
    public class CatalogInfoCollection : Dictionary<int, CatalogInfo>
    {
        public CatalogInfoCollection()
        {
        }

        public CatalogInfoCollection(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                CatalogInfo catalogInfo = new CatalogInfo(node);
                Add(catalogInfo.Id, catalogInfo);
            }
        }
    }
}
