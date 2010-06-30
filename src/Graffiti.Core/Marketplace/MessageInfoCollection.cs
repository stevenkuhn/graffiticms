using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Graffiti.Core.Marketplace
{
    public class MessageInfoCollection : List<MessageInfo>
    {
        public MessageInfoCollection()
        {
        }

        public MessageInfoCollection(IEnumerable<XElement> nodes)
        {
            foreach (XElement node in nodes)
            {
                MessageInfo messageInfo = new MessageInfo(node);
                Add(messageInfo);
            }
        }
    }
}
