using System;
using System.Collections.Generic;
using System.Xml;

namespace Graffiti.Core.Marketplace
{
    public class CreatorInfoCollection : Dictionary<int, CreatorInfo>
    {
        public CreatorInfoCollection()
        {
        }

        public CreatorInfoCollection(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                CreatorInfo creatorInfo = new CreatorInfo(node);
                Add(creatorInfo.Id, creatorInfo);
            }
        }
    }
}
