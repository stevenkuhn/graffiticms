using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;

namespace GraffitiClient.API
{
    public class CategoryResourceProxy : ResourceProxy<Category>
    {
        internal CategoryResourceProxy(string username, string password, string baseUrl)
            : base(username, password, baseUrl)
        {
        }

        public override Category Get(int id)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc["id"] = id.ToString();
            PagedList<Category> list = Get(nvc);

            if (list.Count > 0)
                return list[0];

            return null;
        }

        public Category Get(string name)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc["name"] = name;
            PagedList<Category> list = Get(nvc);

            if (list.Count > 0)
                return list[0];

            return null;
        }

        public override PagedList<Category> Get(NameValueCollection nvc)
        {
            XmlDocument doc = GetXML(nvc);

            return GetCategories(doc);
        }

        public override int Create(Category t)
        {
            string xml = GetXML(t);
            XmlDocument doc = SendXML(xml, false);
            int id = -1;

            bool result = CheckResult(doc, "true", out id);

            //if (!result)
            //    throw new Exception("The category was not successfully created");

            //if (id <= 0)
            //    throw new Exception("The category was not successfully created");

            return id;

        }

        public override bool Update(Category t)
        {
            string xml = GetXML(t);
            XmlDocument doc = SendXML(xml, false);
           
            return CheckResult(doc, "true");

        }

        public override bool Delete(int id)
        {
            XmlDocument doc = SendXML("<category id = \"" + id + "\" />",true);

            return CheckResult(doc, "deleted");
        }

        private static PagedList<Category> GetCategories(XmlDocument doc)
        {
            PagedList<Category> categories = new PagedList<Category>();

            XmlNode rootNode = doc.SelectSingleNode("/categories");

            categories.PageIndex = Int32.Parse(rootNode.Attributes["pageIndex"].Value);
            categories.PageSize = Int32.Parse(rootNode.Attributes["pageSize"].Value);
            categories.TotalRecords = Int32.Parse(rootNode.Attributes["totalCategories"].Value);
            

            XmlNodeList nodes = doc.SelectNodes("/categories/category");
            foreach (XmlNode node in nodes)
            {
                Category c = GetCategory(node);

                XmlNodeList subCategories = node.SelectNodes("subCategories/category");
                if(subCategories != null)
                {
                    foreach(XmlNode child in subCategories)
                    {
                        c.Children.Add(GetCategory(child));
                    }
                }

                categories.Add(c);
            }


            return categories;
        }

        private static Category GetCategory(XmlNode node)
        {
            Category c = new Category();
            c.Id = Int32.Parse(node.Attributes["id"].Value);

            c.Name = node.SelectSingleNode("name").InnerText;
            c.LinkName = node.SelectSingleNode("linkName").InnerText;
            c.Url = node.SelectSingleNode("url").InnerText;
            c.PostCount = Int32.Parse(node.SelectSingleNode("postCount").InnerText);
            c.SortOrder = (SortOrderType) Int32.Parse(node.SelectSingleNode("sortOrderType").InnerText);
            c.ParentId = Int32.Parse(node.SelectSingleNode("parentId") != null ? node.SelectSingleNode("parentId").InnerText : "0");
            c.MetaDescription = node.SelectSingleNode("metaDescription").InnerText != "" ? node.SelectSingleNode("metaDescription").InnerText : null;
            c.MetaKeywords = node.SelectSingleNode("metaKeywords").InnerText != "" ? node.SelectSingleNode("metaKeywords").InnerText : null;
            c.FeedBurnerUrl = node.SelectSingleNode("feedBurnerUrl").InnerText != "" ? node.SelectSingleNode("feedBurnerUrl").InnerText : null;

            XmlNode body = node.SelectSingleNode("body");
            if (body != null)
                c.Body = body.InnerText;
            return c;
        }

        private static string GetXML(Category category)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter writer = new XmlTextWriter(sw);

            writer.WriteStartElement("category");
            writer.WriteAttributeString("id", category.Id.ToString());
            writer.WriteElementString("parentId", category.ParentId.ToString());
            writer.WriteElementString("linkName", category.LinkName);
            writer.WriteElementString("name", category.Name);
            writer.WriteElementString("body", category.Body);
            writer.WriteElementString("sortOrderType", ((int)category.SortOrder).ToString());
            writer.WriteElementString("metaDescription", category.MetaDescription);
            writer.WriteElementString("metaKeywords", category.MetaKeywords);
            writer.WriteElementString("feedBurnerUrl", category.FeedBurnerUrl);
            writer.WriteEndElement();

            writer.Close();
            sw.Close();

            return sb.ToString();
        }
    }
}