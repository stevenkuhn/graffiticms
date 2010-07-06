using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Graffiti.Core;

namespace Graffiti.Marketplace
{
	public class CatalogWriter : BaseFeedWriter
	{

		public CatalogWriter()
		{
		}

        protected override string CacheKey
        {
            get { return "MarketPlugin-CatalogWriter"; }
        }

		protected override string BuildFeed()
		{
			StringWriter sw = new StringWriter();
			sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
			XmlTextWriter writer = new XmlTextWriter(sw);

			writer.WriteStartElement("catalogs");

            foreach (Category category in GetCatalogCategories())
            {
                writer.WriteStartElement("catalogInfo");
                writer.WriteAttributeString("id", category.Id.ToString());
                writer.WriteElementString("name", category.FormattedName);
                writer.WriteElementString("description", Util.RemoveHtml(category.Body, 300));
                writer.WriteElementString("type", ((Graffiti.Core.Marketplace.CatalogType)category.Type).ToString());
                writer.WriteEndElement(); // End catalogInfo
            }

            writer.WriteEndElement(); // End catalogs
			return sw.ToString();
		}

        private IEnumerable GetCatalogCategories()
        {
            CategoryCollection categories = new CategoryController().GetTopLevelCachedCategories();
            foreach (Category c in categories)
            {
                if (c.Type > 0 && System.Enum.IsDefined(typeof(Graffiti.Core.Marketplace.CatalogType), c.Type))
                    yield return c;
            }
        }


	}
}
