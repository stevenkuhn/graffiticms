using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web;
using Graffiti.Core.Marketplace;

namespace Graffiti.Core
{
    internal class QueryStringKey
    {
        public const string Author = "author";
        public const string CategoryId = "categoryId";
        public const string IncludeChildCategories = "includeChildCategories";
        public const string Status = "status";
        public const string IsDeleted = "isDeleted";
        public const string PageIndex = "pageIndex";
        public const string PageSize = "pageSize";
        public const string Id = "id";
        public const string PostId = "postId";
        public const string IPAddress = "ipAddress";
        public const string Name = "name";
        public const string IsPublished = "isPublished";
        public const string Spam = "spam";
        public const string Theme = "theme";
        public const string User = "user";
        public const string Role = "role";
        public const string Revision = "revision";
        public const string ParentId = "parentId";
    }

    public class Breadcrumbs : WebControl
    {
        private Section _sectionName;

        public Section SectionName
        {
            get { return _sectionName; }
            set { _sectionName = value; }
        }

        public enum Section
        {
            ThemeEdit = 1,
            Widget,
            WidgetEdit,
            SiteSettings,
            Configuration,
            Comments,
            CustomFields,
            Themes,
            Categories,
            SiteComments,
            Navigation,
            UserManagement,
            Roles,
            ChangePassword,
            PlugIns,
            PlugInsEdit,
            Packages,
            EmailSettings,
            WidgetMarketplace,
            ThemeMarketplace,
            PluginMarketplace,
            SortPosts,
            SortHomePosts,
            ConfigureTheme,
            Licensing,
            RebuildPages,
            Logs,
            Migrator,
            Utilities
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // make sure the section is set
            if (_sectionName == 0)
                throw new Exception("The SectionName was not provided.");
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write(GetBreadCrumbs());
        }

        private string GetBreadCrumbs()
        {
            Urls urls = new Urls();
            StringBuilder crumbs = new StringBuilder();

            if (this.Page.MasterPageFile.EndsWith("AdminModal.master"))
                crumbs.Append("<div class=\"breadcrumbs_modal\">");
            else
                crumbs.Append("<div class=\"breadcrumbs\">");

            switch (_sectionName)
            {
                case Section.ThemeEdit:
                    {
                        crumbs.Append(GetHyperLink("Presentation", ResolveUrl("~/graffiti-admin/presentation/"), true));
                        crumbs.Append(GetHyperLink("Themes", ResolveUrl("~/graffiti-admin/presentation/themes/"), true));

                        string theme = HttpContext.Current.Request.QueryString[QueryStringKey.Theme];
                        crumbs.Append(GetHyperLink(theme, String.Format("EditTheme.aspx?{0}={1}", QueryStringKey.Theme, theme), false));
                    }
                    break;

                case Section.ConfigureTheme:
                    {
                        crumbs.Append(GetHyperLink("Presentation", ResolveUrl("~/graffiti-admin/presentation/"), true));
                        crumbs.Append(GetHyperLink("Themes", ResolveUrl("~/graffiti-admin/presentation/themes/"), true));

                        string theme = HttpContext.Current.Request.QueryString[QueryStringKey.Theme];
                        crumbs.Append(GetHyperLink(theme, String.Format("EditTheme.aspx?{0}={1}", QueryStringKey.Theme, theme), true));

                        crumbs.Append(GetHyperLink("Configure Theme", ResolveUrl("~/graffiti-admin/presentation/themes/ConfigureTheme.aspx?" + QueryStringKey.Theme + "=" + theme), false));
                    }
                    break;

                case Section.Widget:

                    crumbs.Append(GetHyperLink("Presentation", ResolveUrl("~/graffiti-admin/presentation/"), true));
                    crumbs.Append(GetHyperLink("Widgets", ResolveUrl("~/graffiti-admin/presentation/widgets/"), true));

                    break;

                case Section.WidgetEdit:

                    crumbs.Append(GetHyperLink("Presentation", ResolveUrl("~/graffiti-admin/presentation/"), true));
                    crumbs.Append(GetHyperLink("Widgets", ResolveUrl("~/graffiti-admin/presentation/widgets/"), true));

                    Widget widget = Widgets.Fetch(new Guid(HttpContext.Current.Request.QueryString[QueryStringKey.Id]));
                    crumbs.Append(GetHyperLink(widget.Name, String.Format("edit.aspx?{0}={1}", QueryStringKey.Id, widget.Id), false));
                    
                    break;

                case Section.SiteSettings:

                    crumbs.Append(GetHyperLink("Site Options", ResolveUrl("~/graffiti-admin/site-options/"), true));
                    crumbs.Append(GetHyperLink("Settings", ResolveUrl("~/graffiti-admin/site-options/settings/"), false));

                    break;

                case Section.Configuration:

                    crumbs.Append(GetHyperLink("Site Options", ResolveUrl("~/graffiti-admin/site-options/"), true));
                    crumbs.Append(GetHyperLink("Configuration", ResolveUrl("~/graffiti-admin/site-options/configuration/"), false));

                    break;

                case Section.Utilities:

                    crumbs.Append(GetHyperLink("Site Options", ResolveUrl("~/graffiti-admin/site-options/"), true));
                    crumbs.Append(GetHyperLink("Utilities", ResolveUrl("~/graffiti-admin/site-options/utilities/"), false));

                    break;

                case Section.RebuildPages:

                    crumbs.Append(GetHyperLink("Site Options", ResolveUrl("~/graffiti-admin/site-options/"), true));
                    crumbs.Append(GetHyperLink("Utilities", ResolveUrl("~/graffiti-admin/site-options/utilities/"), true));
                    crumbs.Append(GetHyperLink("Rebuild Pages", ResolveUrl("~/graffiti-admin/site-options/utilities/RebuildPages.aspx"), false));

                    break;

                case Section.Logs:

                    crumbs.Append(GetHyperLink("Site Options", ResolveUrl("~/graffiti-admin/site-options/"), true));
                    crumbs.Append(GetHyperLink("Utilities", ResolveUrl("~/graffiti-admin/site-options/utilities/"), true));
                    crumbs.Append(GetHyperLink("Logs", ResolveUrl("~/graffiti-admin/site-options/utilities/LogViewer.aspx"), false));

                    break;

                case Section.Migrator:

                    crumbs.Append(GetHyperLink("Site Options", ResolveUrl("~/graffiti-admin/site-options/"), true));
                    crumbs.Append(GetHyperLink("Utilities", ResolveUrl("~/graffiti-admin/site-options/utilities/"), true));
                    crumbs.Append(GetHyperLink("Migrator", ResolveUrl("~/graffiti-admin/site-options/utilities/migrator/"), false));

                    break;

                case Section.Comments:

                    crumbs.Append(GetHyperLink("Site Options", ResolveUrl("~/graffiti-admin/site-options/"), true));
                    crumbs.Append(GetHyperLink("Comments", ResolveUrl("~/graffiti-admin/site-options/comments/"), false));

                    break;


                case Section.CustomFields:

                    crumbs.Append(GetHyperLink("Site Options", ResolveUrl("~/graffiti-admin/site-options/"), true));
                    crumbs.Append(GetHyperLink("Custom Fields", ResolveUrl("~/graffiti-admin/site-options/custom-fields/"), true));

                    string customFieldId = HttpContext.Current.Request.QueryString[QueryStringKey.Id];
                    int fieldCategoryId = int.Parse(HttpContext.Current.Request.QueryString["category"] ?? "-1");

                    if (!String.IsNullOrEmpty(customFieldId))
                    {
                        CustomFormSettings csf = CustomFormSettings.Get(fieldCategoryId, false);

                        CustomField cf = null;
                        Guid g = new Guid(customFieldId);
                        foreach (CustomField cfx in csf.Fields)
                        {
                            if (cfx.Id == g)
                            {
                                cf = cfx;
                                break;
                            }
                        }

                        if (cf != null)
                        {
                            crumbs.Append(GetHyperLink(cf.Name, ResolveUrl("~/graffiti-admin/site-options/custom-fields/?id=" + cf.Id), false));
                        }
                    }

                    break;

                case Section.Themes:

                    crumbs.Append(GetHyperLink("Presentation", ResolveUrl("~/graffiti-admin/presentation/"), true));
                    crumbs.Append(GetHyperLink("Themes", ResolveUrl("~/graffiti-admin/presentation/themes/"), false));

                    break;

                case Section.SortHomePosts:

                    crumbs.Append(GetHyperLink("Site Options", ResolveUrl("~/graffiti-admin/site-options/"), true));
                    crumbs.Append(GetHyperLink("Home Page", ResolveUrl("~/graffiti-admin/site-options/homesort/"), false));

                    break;

				case Section.Licensing:

					crumbs.Append(GetHyperLink("Site Options", ResolveUrl("~/graffiti-admin/site-options/"), true));
					crumbs.Append(GetHyperLink("Licensing", ResolveUrl("~/graffiti-admin/site-options/licensing/"), false));

					break;

                case Section.Categories:
                    {
                        string id = HttpContext.Current.Request.QueryString[QueryStringKey.Id];

                        if (String.IsNullOrEmpty(id))
                            return string.Empty;

                        List<Category> categories = new List<Category>();

                        Category c = new Category(id);
                        categories.Add(c);

                        Category parent;

                        if (c.ParentId != -1)
                        {
                            parent = c;

                            bool noMoreParents = false;

                            while (!noMoreParents)
                            {
                                parent = new Category(parent.ParentId);
                                if (parent.Id != 0)
                                {
                                    categories.Insert(0, parent);
                                }
                                else
                                {
                                    noMoreParents = true;
                                }
                            }
                        }

                        crumbs.Append(GetHyperLink("Categories", ResolveUrl("~/graffiti-admin/categories/"), true));

                        int counter = 0;
                        int catCount = categories.Count;

                        foreach (Category tempcat in categories)
                        {
                            counter++;

                            bool addArrow = counter == catCount ? false : true;

                            crumbs.Append(GetHyperLink(tempcat.Name, ResolveUrl("~/graffiti-admin/categories/?id=" + tempcat.Id), addArrow));
                        }
                    }
                    break;

                case Section.SortPosts:
                    {
                        string id = HttpContext.Current.Request.QueryString[QueryStringKey.Id];

                        if (String.IsNullOrEmpty(id))
                            return string.Empty;

                        List<Category> categories = new List<Category>();

                        Category c = new Category(id);
                        categories.Add(c);

                        Category parent;

                        if (c.ParentId != -1)
                        {
                            parent = c;

                            bool noMoreParents = false;

                            while (!noMoreParents)
                            {
                                parent = new Category(parent.ParentId);
                                if (parent.Id != 0)
                                {
                                    categories.Insert(0, parent);
                                }
                                else
                                {
                                    noMoreParents = true;
                                }
                            }
                        }

                        crumbs.Append(GetHyperLink("Categories", ResolveUrl("~/graffiti-admin/categories/"), true));
                        foreach (Category tempcat in categories)
                        {
                            crumbs.Append(GetHyperLink(tempcat.Name, ResolveUrl("~/graffiti-admin/categories/?id=" + tempcat.Id), true));
                        }

                        crumbs.Append(GetHyperLink("Order Posts", ResolveUrl("~/graffiti-admin/categories/PostSortOrder.aspx?id=" + id), false));
                    }
                    break;


                case Section.SiteComments:

                    string commentId = HttpContext.Current.Request.QueryString[QueryStringKey.Id];

                    if (String.IsNullOrEmpty(commentId))
                        return string.Empty;

                    Comment comment = new Comment(commentId);

                    crumbs.Append(GetHyperLink("Comments", ResolveUrl("~/graffiti-admin/comments/"), true));
                    crumbs.Append(GetHyperLink(comment.Name + " @ " + comment.Published, ResolveUrl("~/graffiti-admin/comments/?id=" + comment.Id), false));

                    break;

                case Section.Navigation:

                    crumbs.Append(GetHyperLink("Presentation", ResolveUrl("~/graffiti-admin/presentation/"), true));
                    crumbs.Append(GetHyperLink("Navigation", ResolveUrl("~/graffiti-admin/presentation/navigation/"), false));

                    break;

                case Section.UserManagement:

                    crumbs.Append(GetHyperLink("User Management", ResolveUrl("~/graffiti-admin/user-management/"), true));

                    string user = HttpContext.Current.Request.QueryString[QueryStringKey.User];

                    if (!String.IsNullOrEmpty(user))
                    {
                        crumbs.Append(GetHyperLink("Users", ResolveUrl("~/graffiti-admin/user-management/users"), true));

                        IGraffitiUser graffitiUser = GraffitiUsers.GetUser(user);
                        crumbs.Append(GetHyperLink(graffitiUser.Name, ResolveUrl("~/graffiti-admin/user-management/users/?user=" + graffitiUser.Name), false));
                    }
                    else
                    {
                        crumbs.Append(GetHyperLink("Users", ResolveUrl("~/graffiti-admin/user-management/users"), false));
                    }
                    
                    break;

                case Section.Roles:

                    crumbs.Append(GetHyperLink("User Management", ResolveUrl("~/graffiti-admin/user-management/"), true));

                    string role = HttpUtility.HtmlEncode(HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString[QueryStringKey.Role]));

                    if (!String.IsNullOrEmpty(role))
                    {
                        crumbs.Append(GetHyperLink("Roles", ResolveUrl("~/graffiti-admin/user-management/roles"), true));

                        crumbs.Append(GetHyperLink(role, ResolveUrl("~/graffiti-admin/user-management/roles/?role=" + role), false));
                    }
                    else
                    {
                        crumbs.Append(GetHyperLink("Roles", ResolveUrl("~/graffiti-admin/user-management/roles"), false));
                    }

                    break;

                case Section.ChangePassword:

                    string cpUser = HttpContext.Current.Request.QueryString[QueryStringKey.User];

                    if (String.IsNullOrEmpty(cpUser))
                        return string.Empty;

                    IGraffitiUser graffitiUser1 = GraffitiUsers.GetUser(cpUser);

                    crumbs.Append(GetHyperLink("User Management", ResolveUrl("~/graffiti-admin/user-management/"), true));
                    crumbs.Append(GetHyperLink("Users", ResolveUrl("~/graffiti-admin/user-management/users/"), true));
                    crumbs.Append(GetHyperLink(graffitiUser1.Name, ResolveUrl("~/graffiti-admin/user-management/users/?user=" + graffitiUser1.Name), true));

                    crumbs.Append(GetHyperLink("Change Password", ResolveUrl("~/graffiti-admin/user-management/users/changepassword.aspx?user=" + graffitiUser1.Name), false));

                    break;


                case Section.PlugIns:

                    crumbs.Append(GetHyperLink("Site Options", ResolveUrl("~/graffiti-admin/site-options/"), true));
                    crumbs.Append(GetHyperLink("Plug-Ins", ResolveUrl("~/graffiti-admin/site-options/plug-ins/"), false));

                    break;

                case Section.PlugInsEdit:

                    crumbs.Append(GetHyperLink("Site Options", ResolveUrl("~/graffiti-admin/site-options/"), true));
                    crumbs.Append(GetHyperLink("Plug-Ins", ResolveUrl("~/graffiti-admin/site-options/plug-ins/"), true));

                    EventDetails ed = Graffiti.Core.Events.GetEvent(HttpContext.Current.Request.QueryString["t"]);

                    crumbs.Append(GetHyperLink(ed.Event.Name, ResolveUrl("~/graffiti-admin/site-options/plug-ins/edit.aspx?t=") + HttpContext.Current.Request.QueryString["t"], false));

                    break;

                case Section.Packages:

                    crumbs.Append(GetHyperLink("Site Options", ResolveUrl("~/graffiti-admin/site-options/"), true));
                    crumbs.Append(GetHyperLink("Packages", ResolveUrl("~/graffiti-admin/site-options/packages/"), false));

                    break;

                case Section.EmailSettings:

                    crumbs.Append(GetHyperLink("Site Options", ResolveUrl("~/graffiti-admin/site-options/"), true));
                    crumbs.Append(GetHyperLink("Email Settings", ResolveUrl("~/graffiti-admin/site-options/email-settings/"), false));

                    break;

                case Section.WidgetMarketplace:

                    crumbs.Append(GetHyperLink("All Widgets", urls.AdminMarketplace("Widgets"), true));

                    CatalogInfo widgets = Marketplace.Marketplace.Catalogs[CatalogType.Widgets];

                    int categoryId = 0;
                    string category = HttpContext.Current.Request.QueryString["category"];
                    if (!string.IsNullOrEmpty(category))
                    {
                        try { categoryId = int.Parse(category); }
                        catch {}
                    }

                    if ((categoryId != 0) && widgets.Categories.ContainsKey(categoryId))
                    {
                        CategoryInfo categoryInfo = widgets.Categories[categoryId];
                        crumbs.Append(GetHyperLink(categoryInfo.Name, urls.AdminMarketplace("Widgets") + "&category=" + categoryInfo.Id.ToString(), false));
                    }

                    string creatorId = string.Empty;
                    if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["creator"]))
                        creatorId = HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString["creator"]);

                    if (!string.IsNullOrEmpty(creatorId) && (Marketplace.Marketplace.Creators.ContainsKey(creatorId)))
                    {
                        CreatorInfo creatorInfo = Marketplace.Marketplace.Creators[creatorId];
                        crumbs.Append(GetHyperLink(creatorInfo.Name, urls.AdminMarketplace("Widgets") + "&creator=" + HttpUtility.UrlEncode(creatorInfo.Id), false));
                    }

                    int itemId = 0;
                    string item = HttpContext.Current.Request.QueryString["item"];
                    if (!string.IsNullOrEmpty(item))
                    {
                        try { itemId = int.Parse(item); }
                        catch { }
                    }

                    if ((itemId != 0) && (widgets.Items.ContainsKey(itemId)))
                    {
                        ItemInfo itemInfo = widgets.Items[itemId];
                        CategoryInfo categoryInfo = itemInfo.Category;
                        crumbs.Append(GetHyperLink(categoryInfo.Name, urls.AdminMarketplace("Widgets") + "&category=" + categoryInfo.Id.ToString(), true));
                        crumbs.Append(GetHyperLink(itemInfo.Name, urls.AdminMarketplaceItem("Widgets", itemInfo.Id), false));
                    }

                    break;

                case Section.ThemeMarketplace:

                    crumbs.Append(GetHyperLink("All Themes", urls.AdminMarketplace("Themes"), true));

                    CatalogInfo themeCatalog = Marketplace.Marketplace.Catalogs[CatalogType.Themes];

                    categoryId = 0;
                    category = HttpContext.Current.Request.QueryString["category"];
                    if (!string.IsNullOrEmpty(category))
                    {
                        try { categoryId = int.Parse(category); }
                        catch { }
                    }

                    if ((categoryId != 0) && (themeCatalog.Categories.ContainsKey(categoryId)))
                    {
                        CategoryInfo categoryInfo = themeCatalog.Categories[categoryId];
                        crumbs.Append(GetHyperLink(categoryInfo.Name, urls.AdminMarketplace("Themes") + "&category=" + categoryInfo.Id.ToString(), false));
                    }

                    creatorId = string.Empty;
                    if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["creator"]))
                        creatorId = HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString["creator"]);

                    if (!string.IsNullOrEmpty(creatorId) && (Marketplace.Marketplace.Creators.ContainsKey(creatorId)))
                    {
                        CreatorInfo creatorInfo = Marketplace.Marketplace.Creators[creatorId];
                        crumbs.Append(GetHyperLink(creatorInfo.Name, urls.AdminMarketplace("Themes") + "&creator=" + HttpUtility.UrlEncode(creatorInfo.Id), false));
                    }

                    itemId = 0;
                    item = HttpContext.Current.Request.QueryString["item"];
                    if (!string.IsNullOrEmpty(item))
                    {
                        try { itemId = int.Parse(item); }
                        catch { }
                    }

                    if ((itemId != 0) && (themeCatalog.Items.ContainsKey(itemId)))
                    {
                        ItemInfo itemInfo = themeCatalog.Items[itemId];
                        CategoryInfo categoryInfo = itemInfo.Category;
                        crumbs.Append(GetHyperLink(categoryInfo.Name, urls.AdminMarketplace("Themes") + "&category=" + categoryInfo.Id.ToString(), true));
                        crumbs.Append(GetHyperLink(itemInfo.Name, urls.AdminMarketplaceItem("Themes", itemInfo.Id), false));
                    }

                    break;

                case Section.PluginMarketplace:

                    crumbs.Append(GetHyperLink("All Plugins", urls.AdminMarketplace("Plugins"), true));

                    CatalogInfo plugins = Marketplace.Marketplace.Catalogs[CatalogType.Plugins];

                    categoryId = 0;
                    category = HttpContext.Current.Request.QueryString["category"];
                    if (!string.IsNullOrEmpty(category))
                    {
                        try { categoryId = int.Parse(category); }
                        catch { }
                    }

                    if ((categoryId != 0) && plugins.Categories.ContainsKey(categoryId))
                    {
                        CategoryInfo categoryInfo = plugins.Categories[categoryId];
                        crumbs.Append(GetHyperLink(categoryInfo.Name, urls.AdminMarketplace("Plugins") + "&category=" + categoryInfo.Id.ToString(), false));
                    }

                    creatorId = string.Empty;
                    if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["creator"]))
                        creatorId = HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString["creator"]);

                    if (!string.IsNullOrEmpty(creatorId) && (Marketplace.Marketplace.Creators.ContainsKey(creatorId)))
                    {
                        CreatorInfo creatorInfo = Marketplace.Marketplace.Creators[creatorId];
                        crumbs.Append(GetHyperLink(creatorInfo.Name, urls.AdminMarketplace("Plugins") + "&creator=" + HttpUtility.UrlEncode(creatorInfo.Id), false));
                    }

                    itemId = 0;
                    item = HttpContext.Current.Request.QueryString["item"];
                    if (!string.IsNullOrEmpty(item))
                    {
                        try { itemId = int.Parse(item); }
                        catch { }
                    }

                    if ((itemId != 0) && (plugins.Items.ContainsKey(itemId)))
                    {
                        ItemInfo itemInfo = plugins.Items[itemId];
                        CategoryInfo categoryInfo = itemInfo.Category;
                        crumbs.Append(GetHyperLink(categoryInfo.Name, urls.AdminMarketplace("Plugins") + "&category=" + categoryInfo.Id.ToString(), true));
                        crumbs.Append(GetHyperLink(itemInfo.Name, urls.AdminMarketplaceItem("Plugins", itemInfo.Id), false));
                    }

                    break;

                // more breadcrumb logic here, add a value to the enum
            }

            crumbs.Append("</div>");

            return crumbs.ToString();
        }

        private string GetHyperLink(string description, string url, bool addSeperator)
        {
            return String.Format("<a href=\"{0}\">{1}</a>{2}", url, description, addSeperator ? GetSeperator() : "");
        }

        private string GetSeperator()
        {
            return "<span class=\"seperator\">></span>";
        }
    }
}
