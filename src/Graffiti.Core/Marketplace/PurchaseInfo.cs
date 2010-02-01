using System;
using System.Xml;

namespace Graffiti.Core.Marketplace
{
    public class PurchaseInfo
    {
        private double _price = 0.0;
        private string _buyUrl = string.Empty;

        public PurchaseInfo(XmlNode node)
        {
            XmlNode n = node.SelectSingleNode("price");
            if (n != null)
                _price = double.Parse(n.InnerText);

            n = node.SelectSingleNode("buyUrl");
            if (n != null)
                _buyUrl = n.InnerText;
        }

        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public string FormattedPrice
        {
            get
            {
                if (_price <= 0.0)
                    return "FREE";

                return string.Format("{0:C}", _price);
            }
        }

        public string BuyUrl
        {
            get { return _buyUrl; }
            set { _buyUrl = value; }
        }
    }
}
