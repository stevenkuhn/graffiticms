using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Graffiti.Core.Marketplace
{
    public class CreatorInfoCollection : Dictionary<string, CreatorInfo>
    {
        public CreatorInfoCollection()
        {
        }

        public CreatorInfoCollection(IEnumerable<XElement> nodes)
        {
            foreach (XElement node in nodes)
            {
                CreatorInfo creatorInfo = new CreatorInfo(node);
                Add(creatorInfo.Id, creatorInfo);
            }
        }
    }
}
