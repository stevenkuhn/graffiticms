using System;
using System.Collections.Generic;
using System.Xml;

namespace Graffiti.Core.Marketplace
{
    public class MessageInfoCollection : List<MessageInfo>
    {
        public MessageInfoCollection()
        {
        }

        public MessageInfoCollection(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                MessageInfo messageInfo = new MessageInfo(node);
                Add(messageInfo);
            }
        }
    }
}
