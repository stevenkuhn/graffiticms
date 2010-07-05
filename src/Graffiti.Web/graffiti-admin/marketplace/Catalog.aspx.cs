using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using Graffiti.Core;
using Graffiti.Core.Marketplace;
using Glow = Telligent.Glow;

public partial class graffiti_admin_marketplace_Catalog : AdminControlPanelPage
{
    private CatalogType _catalogType = CatalogType.All;
    private int _categoryId = 0;
    private string _creatorId = string.Empty;

    protected void Page_Init(object sender, EventArgs e)
    {
        string catalog = Request.QueryString["catalog"];
        if (!string.IsNullOrEmpty(catalog))
        {
            try { _catalogType = (CatalogType)Enum.Parse(typeof(CatalogType), catalog); }
            catch { }
        }

        string category = Request.QueryString["category"];
        if (!string.IsNullOrEmpty(category))
        {
            try { _categoryId = int.Parse(category); }
            catch { }
        }

        string creator = Request.QueryString["creator"];
        if (!string.IsNullOrEmpty(creator))
            _creatorId = HttpUtility.UrlDecode(creator);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        switch (_catalogType)
        {
            case CatalogType.Widgets:
                LiHyperLink.SetNameToCompare(Context, "presentation");
                PageBreadcrumbs.SectionName = Breadcrumbs.Section.WidgetMarketplace;
                break;
            case CatalogType.Themes:
                LiHyperLink.SetNameToCompare(Context, "presentation");
                PageBreadcrumbs.SectionName = Breadcrumbs.Section.ThemeMarketplace;
                break;
            case CatalogType.Plugins:
                LiHyperLink.SetNameToCompare(Context, "settings");
                PageBreadcrumbs.SectionName = Breadcrumbs.Section.PluginMarketplace;
                break;
            case CatalogType.All:
                Response.Redirect(new Urls().AdminMarketplaceHome);
                break;
        }


        if (!IsPostBack)
        {
            try
            {
                CatalogInfo catalog = null;

                if (Marketplace.Catalogs.ContainsKey(_catalogType))
                    catalog = Marketplace.Catalogs[_catalogType];

                MessageInfo messageInfo = null;
                if (catalog.Messages.Count > 0)
                    messageInfo = catalog.Messages[0];
                else if (Marketplace.Messages.Count > 0)
                    messageInfo = Marketplace.Messages[0];

                if (messageInfo != null)
                {
                    MarketplaceMessage.Visible = true;
                    MarketplaceMessage.Type = StatusType.Information;
                    MarketplaceMessage.Text = messageInfo.Text;
                }

                categoryList.DataSource = catalog.Categories.Values;
                categoryList.DataBind();

                itemList.DataSource = catalog.Items.Values;
                if (_categoryId > 0)
                {
                    CategoryInfo category = catalog.Categories[_categoryId];
                    itemList.DataSource = category.Items.Values;
                }
                else if (!string.IsNullOrEmpty(_creatorId))
                {
                    CreatorInfo creator = Marketplace.Creators[_creatorId];
                    itemList.DataSource = creator.GetItems(_catalogType).Values;
                }

                itemList.DataBind();
            }
            catch (Exception ex)
            {
                string messageText = "An unexpected error has occurred connecting to the marketplace. The <a href=\"http://extendgraffiti.com/\" target=\"_blank\">Graffiti Marketplace</a> is where you can find new themes, widgets, and plugins. Please try again later.";
                if (ex is System.Security.SecurityException)
                    messageText = "Your security settings do not allow you to access the marketplace from within the Control Panel. To find new themes, widgets, and plugins, please visit the <a href=\"http://extendgraffiti.com/\" target=\"_blank\">Graffiti Marketplace</a>.";

                Message.Type = StatusType.Error;
                Message.Text = messageText;
                Message.Visible = true;
                MarketplaceImage.Visible = true;
                PageBreadcrumbs.Visible = false;
                categoryList.Visible = false;
                itemList.Visible = false;
            }
        }
    }

}
