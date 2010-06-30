using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Graffiti.Core
{

    public static class ExtensionMethods
    {

        public static bool TryGetValue(this XAttribute attribute, out string value)
        {
            if (attribute != null && !string.IsNullOrEmpty(attribute.Value))
            {
                value = attribute.Value;
                return true;
            }

            value = null;
            return false;
        }

        public static bool TryGetAttributeValue(this XElement element, string attributeName, out string value)
        {
            if (element != null)
            {
                XAttribute attrib = element.Attribute(attributeName);
                if (attrib != null)
                    return attrib.TryGetValue(out value);
            }

            value = null;
            return false;
        }

        public static bool TryGetValue(this XElement element, out string value)
        {
            if (element != null && !string.IsNullOrEmpty(element.Value))
            {
                value = element.Value;
                return true;
            }

            value = null;
            return false;
        }

    }
}
