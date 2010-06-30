using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Graffiti.Core;

namespace Graffiti.Marketplace
{
	public class MessageWriter : BaseFeedWriter
	{
        private int _categoryId = 0;

		public MessageWriter() : this(string.Empty)
		{
		}

        public MessageWriter(string categoryName)
        {
            Category category = new CategoryController().GetCachedCategoryByLinkName(categoryName, true);

            if (category != null)
                this._categoryId = category.Id;
        }

        protected override string CacheKey
        {
            get { return string.Format("MarketPlugin-MessageWriter-{0}", _categoryId); }
        }

		protected override string BuildFeed()
		{

			StringWriter sw = new StringWriter();
			sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
			XmlTextWriter writer = new XmlTextWriter(sw);

			writer.WriteStartElement("messages");

            foreach (Post post in GetMessages())
            {
                writer.WriteStartElement("messageInfo");
                writer.WriteAttributeString("id", post.Id.ToString());
                writer.WriteElementString("title", post.Title);
                writer.WriteElementString("text", post.Body);
                writer.WriteEndElement(); // End messageInfo
            }

			writer.WriteEndElement(); // End messages

			return sw.ToString();
		}

        private IEnumerable GetMessages()
        {
            CategoryCollection categories = new CategoryController().GetTopLevelCachedCategories();
            foreach (Category c in categories)
            {
                if (c.Type == 1 && (_categoryId == 0 || _categoryId == c.Id))
                {
                    Category messagesCategory = c.Children.FirstOrDefault(cat => Util.AreEqualIgnoreCase(cat.Name, MarketplacePlugin.MessagesCategoryName));
                    if (messagesCategory != null)
                    {
                        foreach (Post p in new Data().PostsByCategory(messagesCategory, 15))
                        {
                            yield return p;
                        }
                    }
                }
            }
        }

	}
}
