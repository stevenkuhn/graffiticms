using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
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
            Macros macros = new Macros();
            string downloadUrlPrefix = macros.FullUrl(VirtualPathUtility.ToAbsolute("~/download/"));

			StringWriter sw = new StringWriter();
			sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
			XmlTextWriter writer = new XmlTextWriter(sw);
            
			writer.WriteStartElement("items");

            foreach (Post post in GetPosts())
            {
                Dictionary<int, ItemStatistics> stats = DataHelper.GetMarketplaceCategoryStats(_categoryId);

                writer.WriteStartElement("itemInfo");
                writer.WriteAttributeString("id", post.Id.ToString());
                writer.WriteAttributeString("categoryId", post.CategoryId.ToString());
                writer.WriteAttributeString("creatorId", post.Custom("Creator"));
                writer.WriteElementString("name", post.Title);
                writer.WriteElementString("description", Util.FullyQualifyRelativeUrls(post.Excerpt("", "", "Read More", 300), SiteSettings.BaseUrl));
                writer.WriteElementString("version", post.Custom("Version"));
                writer.WriteElementString("downloadUrl", downloadUrlPrefix + post.Id.ToString());
                if (!string.IsNullOrEmpty(post.Custom("ImageLarge")))
                    writer.WriteElementString("screenshotUrl", macros.FullUrl(post.Custom("ImageLarge")));
                if (!string.IsNullOrEmpty(post.ImageUrl))
                    writer.WriteElementString("iconUrl", macros.FullUrl(post.ImageUrl));
                writer.WriteElementString("worksWithMajorVersion", post.Custom("RequiredMajorVersion"));
                writer.WriteElementString("worksWithMinorVersion", post.Custom("RequiredMinorVersion"));
                writer.WriteElementString("requiresManualIntervention", post.Custom("RequiresManualIntervention") ?? "False" );
                writer.WriteElementString("isApproved", post.IsPublished.ToString());
                writer.WriteElementString("dateAdded", post.Published.ToUniversalTime().ToString("u"));

                writer.WriteStartElement("statisticsInfo");
                if (stats.ContainsKey(post.Id))
                    writer.WriteElementString("downloadCount", stats[post.Id].DownloadCount.ToString());
                else
                    writer.WriteElementString("downloadCount", "0");
                writer.WriteElementString("viewCount", post.Views.ToString());
                writer.WriteEndElement(); // End statisticsInfo

                writer.WriteStartElement("purchaseInfo");
                writer.WriteElementString("price", post.Custom("Price") ?? "0.0");
                if (!string.IsNullOrEmpty(post.Custom("BuyUrl")))
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
                foreach (Post p in posts)
                {
                    yield return p;
                }
            }
            
        }

	}
}
