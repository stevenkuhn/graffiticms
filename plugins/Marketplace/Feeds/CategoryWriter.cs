using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Graffiti.Core;

namespace Graffiti.Marketplace
{
    public class CategoryWriter : BaseFeedWriter
    {
        private string _parentCategoryName = null;

        public CategoryWriter(string parentCategory)
		{
            this._parentCategoryName = parentCategory;
        }

        protected override string CacheKey
        {
            get { return string.Format("MarketPlugin-CategoryWriter-{0}", _parentCategoryName ?? string.Empty); }
        }

		protected override string BuildFeed()
		{
			StringWriter sw = new StringWriter();
			sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
			XmlTextWriter writer = new XmlTextWriter(sw);

            writer.WriteStartElement("categories");

            foreach (Category category in GetCategories())
            {
                writer.WriteStartElement("categoryInfo");
                writer.WriteAttributeString("id", category.Id.ToString());
                writer.WriteElementString("name", category.FormattedName);
                writer.WriteElementString("description", Util.RemoveHtml(category.Body, 300));
                writer.WriteEndElement(); // End categoryInfo
            }

            writer.WriteEndElement(); // End categories

			return sw.ToString();
		}

        private IEnumerable GetCategories()
        {
            Category parentCategory = new CategoryController().GetCachedCategory(_parentCategoryName, true);

            if (parentCategory != null && parentCategory.HasChildren)
            {
                foreach (Category c in parentCategory.Children)
                {
                    yield return c;
                }
            }
        }

    }
}
