using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using Graffiti.Core;
using Graffiti.Core.Marketplace;
using Glow = Telligent.Glow;

public partial class graffiti_admin_presentation_themes_Catalog : AdminControlPanelPage
{
    private int _catalogId = 1002;
    private int _categoryId = 0;
    private int _creatorId = 0;

    protected void Page_Init(object sender, EventArgs e)
    {
        string category = Request.QueryString["category"];
        if (!string.IsNullOrEmpty(category))
        {
            try { _categoryId = int.Parse(category); }
            catch { }
        }

        string creator = Request.QueryString["creator"];
        if (!string.IsNullOrEmpty(creator))
        {
            try { _creatorId = int.Parse(creator); }
            catch { }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context, "presentation");

        if (!IsPostBack)
        {
            try
            {
                MessageInfo messageInfo = null;
                if (Catalog.Messages.Count > 0)
                    messageInfo = Catalog.Messages[0];
                else if (Marketplace.Messages.Count > 0)
                    messageInfo = Marketplace.Messages[0];

                if (messageInfo != null)
                {
                    MarketplaceMessage.Visible = true;
                    MarketplaceMessage.Type = StatusType.Information;
                    MarketplaceMessage.Text = messageInfo.Text;
                }

                categoryList.DataSource = Catalog.Categories.Values;
                categoryList.DataBind();

                itemList.DataSource = Catalog.Items.Values;
                if (_categoryId > 0)
                    itemList.DataSource = Category.Items.Values;
                else if (_creatorId > 0)
                    itemList.DataSource = Creator.GetItems(Catalog.Id).Values;

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
                breadcrumbs.Visible = false;
                categoryList.Visible = false;
                itemList.Visible = false;
            }
        }
    }

    protected CatalogInfo Catalog
    {
        get { return Marketplace.Catalogs[_catalogId]; }
    }

    protected CategoryInfo Category
    {
        get { return Catalog.Categories[_categoryId]; }
    }

    protected CreatorInfo Creator
    {
        get { return Marketplace.Creators[_creatorId]; }
    }
}
