using System;
using System.Xml.Linq;

namespace Graffiti.Core.Marketplace
{
    public class PurchaseInfo
    {
        private double _price = 0.0;
        private string _buyUrl = string.Empty;

        public PurchaseInfo(XElement node)
        {
            string value;

            XElement n = node.Element("price");
            if (n != null && n.TryGetValue(out value))
                _price = double.Parse(value);

            n = node.Element("buyUrl");
            if (n != null && n.TryGetValue(out value))
                _buyUrl = value;
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
