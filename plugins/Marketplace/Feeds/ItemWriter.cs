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
	public class ItemWriter : BaseFeedWriter
	{
        private int _categoryId = 0;

        public ItemWriter(string parentCategory)
		{
            Category category = new CategoryController().GetCachedCategoryByLinkName(parentCategory, true);

            if (category != null)
                this._categoryId = category.Id;
        }

        protected override string CacheKey
        {
            get { return string.Format("MarketPlugin-ItemWriter-{0}", _categoryId); }
        }

		protected override string BuildFeed()
		{
			StringWriter sw = new StringWriter();
			sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
			XmlTextWriter writer = new XmlTextWriter(sw);

			writer.WriteStartElement("items");

            foreach (Post post in GetPosts())
            {
                writer.WriteStartElement("itemInfo");
                writer.WriteAttributeString("id", post.Id.ToString());
                writer.WriteAttributeString("categoryId", post.CategoryId.ToString());
                writer.WriteAttributeString("creatorId", post.UserName);
                writer.WriteElementString("name", post.Name);
                writer.WriteElementString("description", post.Body);
                writer.WriteElementString("version", post.Custom("Version"));
                writer.WriteElementString("size", post.Custom("Size"));
                writer.WriteElementString("downloadUrl", post.Custom("FileName"));
                writer.WriteElementString("screenshotUrl", post.Custom("ImageLarge"));
                writer.WriteElementString("iconUrl", post.ImageUrl);
                writer.WriteElementString("worksWithMajorVersion", post.Custom("RequiredMajorVersion"));
                writer.WriteElementString("worksWithMinorVersion", post.Custom("RequiredMinorVersion"));
                writer.WriteElementString("requiresManualIntervention", post.Custom("RequiresManualIntervention"));
                writer.WriteElementString("isApproved", post.IsPublished.ToString());
                writer.WriteElementString("dateAdded", post.Published.ToUniversalTime().ToString("u"));

                writer.WriteStartElement("statisticsInfo");
                writer.WriteElementString("downloadCount", post.Views.ToString());
                writer.WriteElementString("viewCount", post.Views.ToString());
                writer.WriteEndElement(); // End statisticsInfo

                writer.WriteStartElement("purchaseInfo");
                writer.WriteElementString("price", post.Custom("Price"));
                writer.WriteElementString("buyUrl", post.Custom("BuyUrl"));
                writer.WriteEndElement(); // End purchaseInfo

                writer.WriteStartElement("tags");
                foreach (string tag in Util.ConvertStringToList(post.TagList))
                {
                    writer.WriteElementString("tag", tag);
                }
                writer.WriteEndElement(); // End tags

                writer.WriteEndElement(); // End itemInfo
            }

			writer.WriteEndElement(); // End items

			return sw.ToString();
		}

        private IEnumerable GetPosts()
        {
            PostCollection posts = new Data().PostsByCategory(_categoryId, 100);

            if (posts != null && posts.Count > 0)
            {
                // Retrieve Marketplace details for posts



                foreach (Post p in posts)
                {
                    yield return p;
                }
            }
            
        }


        /*
        public void GetMarketplaceData()
        {

            using (SqlConnection connection = CreateMarketplaceConnection())
            {
                using (SqlCommand command = new SqlCommand("marketplace_items_get", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@categoryId", SqlDbType.Int)).Value = _categoryId;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                    return new ItemWriter(reader).WriteXml();
                }
            }
        }
        */

	}
}
