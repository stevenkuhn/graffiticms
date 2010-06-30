using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Graffiti.Core;

namespace Graffiti.Marketplace
{
	public class CreatorWriter : BaseFeedWriter
	{

		public CreatorWriter()
		{
		}

        protected override string CacheKey
        {
            get { return "MarketPlugin-CreatorWriter"; }
        }


		protected override string BuildFeed()
		{
			StringWriter sw = new StringWriter();
			sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
			XmlTextWriter writer = new XmlTextWriter(sw);

			writer.WriteStartElement("creators");

            foreach(User user in GetCreators())
            {
                writer.WriteStartElement("creatorInfo");
                writer.WriteAttributeString("id", user.Name);
			    writer.WriteElementString("name",  user.ProperName ?? string.Empty);

			    writer.WriteStartElement("email");
                bool displayEmail = !string.IsNullOrEmpty(user.PublicEmail);
                writer.WriteAttributeString("display", displayEmail.ToString());
			    writer.WriteString(user.PublicEmail);
			    writer.WriteEndElement(); // End email

                writer.WriteElementString("bio", user.Bio ?? string.Empty);
                writer.WriteElementString("url", user.WebSite ?? string.Empty);
			    writer.WriteEndElement(); // End creatorInfo
            }

			writer.WriteEndElement(); // End creators

			return sw.ToString();
		}

        public IEnumerable GetCreators()
		{
            foreach (IGraffitiUser u in GraffitiUsers.GetUsers(MarketplacePlugin.MarketplaceCreatorsRoleName))
            {
                yield return u;
            }
		}

	}
}
