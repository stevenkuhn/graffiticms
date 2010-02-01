using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;

namespace Graffiti.Core.API
{
    /// <summary>
    /// Handles RESTFul Queries and Updates to Graffiti Categorories.
    /// </summary>
    public class CategoryResource : BaseServiceResource
    {
        protected override void HandleRequest(IGraffitiUser user, XmlTextWriter writer)
        {
            switch (Context.Request.HttpMethod.ToUpper())
            {
                case "GET":

                    CategoryController controller = new CategoryController();
                    CategoryCollection cc = null;
                    int count = 1;
                    if(Request.QueryString["id"] != null)
                    {
                        Category category = controller.GetCachedCategory(Int32.Parse(Request.QueryString["id"]), false);
                        cc = new CategoryCollection();
                        cc.Add(category);
                    }
                    else if (Request.QueryString["name"] != null)
                    {
                        Category category = controller.GetCachedCategory(Request.QueryString["name"], false);
                        cc = new CategoryCollection();
                        cc.Add(category);
                    }
                    else
                    {
                        cc = controller.GetAllTopLevelCachedCategories();
                        count = controller.GetAllCachedCategories().Count;
                    }
                    writer.WriteStartElement("categories");
                        writer.WriteAttributeString("pageIndex", "1");
                        writer.WriteAttributeString("pageSize", count.ToString() );
                        writer.WriteAttributeString("totalCategories", count.ToString());
                    
                        foreach(Category category in cc)
                        {
                            WriteCategoryToXML(category, writer);
                        }
                    writer.WriteEndElement();
                    writer.Close();

                    break;

                case "POST":

                    XmlDocument doc = new XmlDocument();
                    doc.Load(Request.InputStream);

                    if (Request.Headers["Graffiti-Method"] != "DELETE")
                    {
                        if (GraffitiUsers.IsAdmin(user))
                        {
                            string xml = CreateUpdateCategory(doc);
                            writer.WriteRaw(xml);
                        }
                        else
                        {
                            UnuathorizedRequest();
                        }
                    }
                    else
                    {
                        XmlAttribute categoryIdAttribute = doc.SelectSingleNode("/category").Attributes["id"];

                        foreach (Post p in PostCollection.FetchAll())
                        {
                            if (p.CategoryId == Int32.Parse(categoryIdAttribute.Value))
                            {
                                if (p.IsDeleted)
                                {
                                    Post.DestroyDeletedPost(p.Id);
                                }
                                else
                                {
                                    Response.StatusCode = 500;
                                    writer.WriteRaw("<error>You can not delete a category that contains post.</error>");
                                    return;
                                }
                            }
                        }

                        Category.Destroy(Int32.Parse(categoryIdAttribute.Value));
                        CategoryController.Reset();

                        writer.WriteRaw("<result id=\"" + Int32.Parse(categoryIdAttribute.Value) + "\">deleted</result>");
                    }
                    
                    break;

                default:


                    break;
            }
        }

        protected string CreateUpdateCategory(XmlDocument doc)
        {
            Category category = null;
            XmlAttribute categoryIdAttribute = doc.SelectSingleNode("/category").Attributes["id"];
            if (categoryIdAttribute == null)
                category = new Category();
            else
            {
                int pid = Int32.Parse(categoryIdAttribute.Value);
                if (pid > 0)
                    category = new Category(pid);
                else
                    category = new Category();
            }

            XmlNode node = doc.SelectSingleNode("/category");

            if(category.IsNew)
            {
                category.ParentId = GetNodeValue(node.SelectSingleNode("parentId"), 0);

                if(category.ParentId > 0)
                {
                    Category parentCategory = new CategoryController().GetCachedCategory(category.ParentId, true);
                    if (parentCategory == null)
                        throw new RESTConflict("The parent category " + category.ParentId + " does not exist");

                    if (parentCategory.ParentId > 0)
                        throw new RESTConflict(
                            "Graffiti only supports two levels of categories. Please choose a root category as the parent");
                }
            }

            category.Name = GetNodeValue(node.SelectSingleNode("name"), null);
            if (string.IsNullOrEmpty(category.Name))
                throw new RESTConflict("No name was specified for the category");
            category.Name = Server.HtmlEncode(category.Name);

            category.LinkName = GetNodeValue(node.SelectSingleNode("linkName"), Util.CleanForUrl(category.Name));
            category.LinkName = Server.HtmlEncode(category.LinkName);
            category.FormattedName = category.LinkName;


            category.Body = GetNodeValue(node.SelectSingleNode("body"), null);
            category.MetaDescription = GetNodeValue(node.SelectSingleNode("metaDescription"), null);
            category.MetaKeywords = GetNodeValue(node.SelectSingleNode("metaKeywords"), null);
            category.FeedUrlOverride = GetNodeValue(node.SelectSingleNode("feedBurnerUrl"), null);

            category.SortOrderTypeId = (GetNodeValue(node.SelectSingleNode("sortOrderType"), category.SortOrderTypeId));

            if (!Enum.IsDefined(typeof(SortOrderType), category.SortOrderTypeId))
                category.SortOrderTypeId = 0;
                        

            try
            {
                category.Save(GraffitiUsers.Current.Name);
                return "<result id = \"" + category.Id + "\">true</result>";
            }
            catch(Exception ex)
            {
                if (ex.Message.IndexOf("UNIQUE") > 0)
                    throw new RESTConflict("The suggested category name would lead to a duplicate category");

                throw;
            }
        }

        private static void WriteCategoryToXML(Category category, XmlTextWriter writer)
        {
            writer.WriteStartElement("category");
            writer.WriteAttributeString("id", category.Id.ToString());

            if(category.ParentId > 0)
                writer.WriteElementString("parentId", category.ParentId.ToString());

            writer.WriteElementString("name", HttpUtility.HtmlDecode( category.Name));
            writer.WriteElementString("linkName", HttpUtility.HtmlDecode(category.LinkName));
            writer.WriteElementString("url", new Macros().FullUrl(category.Url));
            writer.WriteElementString("postCount", category.PostCount.ToString());
            writer.WriteElementString("body", category.Body);
            writer.WriteElementString("sortOrderType", category.SortOrderTypeId.ToString());
            writer.WriteElementString("metaDescription", category.MetaDescription);
            writer.WriteElementString("metaKeywords", category.MetaKeywords);
            writer.WriteElementString("feedBurnerUrl", category.FeedUrlOverride);
            if(category.HasChildren)
            {
                writer.WriteStartElement("subCategories");
                foreach(Category child in category.Children)
                {
                    WriteCategoryToXML(child, writer);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
