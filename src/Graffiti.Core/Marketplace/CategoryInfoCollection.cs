using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Graffiti.Core.Marketplace
{
    public class CategoryInfoCollection : Dictionary<int, CategoryInfo>
    {
        public CategoryInfoCollection()
        {
        }

        public CategoryInfoCollection(CatalogInfo catalogInfo, IEnumerable<XElement> nodes)
        {
            foreach (XElement node in nodes)
            {
                CategoryInfo categoryInfo = new CategoryInfo(catalogInfo, node);
                Add(categoryInfo.Id, categoryInfo);
            }
        }
    }
}
