using System;
using System.Collections.Generic;
using System.Xml;

namespace Graffiti.Core.Marketplace
{
    public class CategoryInfoCollection : Dictionary<int, CategoryInfo>
    {
        public CategoryInfoCollection()
        {
        }

        public CategoryInfoCollection(CatalogInfo catalogInfo, XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                CategoryInfo categoryInfo = new CategoryInfo(catalogInfo, node);
                Add(categoryInfo.Id, categoryInfo);
            }
        }
    }
}
