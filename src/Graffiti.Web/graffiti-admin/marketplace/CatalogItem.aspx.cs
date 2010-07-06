using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Graffiti.Core;
using Graffiti.Core.Marketplace;
using Glow = Telligent.Glow;

public partial class graffiti_admin_marketplace_CatalogItem : AdminControlPanelPage
{
    private CatalogType _catalogType = CatalogType.All;
    private int _itemId = 0;
    protected string _itemTypeName = string.Empty;

    protected void Page_Init(object sender, EventArgs e)
    {
        string catalog = Request.QueryString["catalog"];
        if (!string.IsNullOrEmpty(catalog))
        {
            try { _catalogType = (CatalogType)Enum.Parse(typeof(CatalogType), catalog); }
            catch { }
        }

        string item = Request.QueryString["item"];
        if (!string.IsNullOrEmpty(item))
        {
            try { _itemId = int.Parse(item); }
            catch { }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        switch (_catalogType)
        {
            case CatalogType.Widgets:
                LiHyperLink.SetNameToCompare(Context, "presentation");
                PageBreadcrumbs.SectionName = Breadcrumbs.Section.WidgetMarketplace;
                _itemTypeName = "Widget";
                break;
            case CatalogType.Themes:
                LiHyperLink.SetNameToCompare(Context, "presentation");
                PageBreadcrumbs.SectionName = Breadcrumbs.Section.ThemeMarketplace;
                _itemTypeName = "Theme";
                break;
            case CatalogType.Plugins:
                LiHyperLink.SetNameToCompare(Context, "settings");
                PageBreadcrumbs.SectionName = Breadcrumbs.Section.PluginMarketplace;
                _itemTypeName = "Plugin";
                break;
            case CatalogType.All:
                Response.Redirect(new Urls().AdminMarketplaceHome);
                break;
        }

        SetControlVisibility();
    }

    public void SetControlVisibility()
    {
        AdditionalConfiguration.Visible = Item.RequiresManualIntervention;

        Email.Visible = (Item.Creator != null) && Item.Creator.DisplayEmail;

        Statistics.Visible = (Item.Statistics.ViewCount + Item.Statistics.DownloadCount > 0);
        Views.Visible = (Item.Statistics.ViewCount > 0);
        Downloads.Visible = (Item.Statistics.DownloadCount > 0);

        InstallButton.Visible = DownloadButton.Visible = BuyButton.Visible = false;
        if (Item.Purchase.Price > 0)
            BuyButton.Visible = true;
        else if (Item.RequiresManualIntervention)
            DownloadButton.Visible = true;
        else
            InstallButton.Visible = true;

        InstallButton.Text = "Install " + _itemTypeName;
        DownloadButton.Text = "Downlaod " + _itemTypeName;
        BuyButton.Text = "Buy " + _itemTypeName;
    }

    protected void Install_Click(object sender, EventArgs e)
    {
        bool success = false;

        try
        {

            switch (_catalogType)
            {
                case CatalogType.Widgets:
                    string widgetFileName = string.Format("{0}/{1}", Server.MapPath("~/bin"), Item.FileName).Replace(".zip", ".dll");
                    new WebClient().DownloadFile(Item.DownloadUrl, widgetFileName);
                    break;
                case CatalogType.Themes:
                    string themeFilename = string.Format("{0}/{1}", Server.MapPath("~/files/themes"), Item.FileName);
                    new WebClient().DownloadFile(Item.DownloadUrl, themeFilename);
                    UTF8Encoding utf = new UTF8Encoding();
                    string encodedFile = utf.GetString(File.ReadAllBytes(themeFilename));
                    ThemeConverter.ToDisk(encodedFile, Server.MapPath("~/files/themes/"), true, Item.Name);
                    File.Delete(themeFilename);
                    break;
                case CatalogType.Plugins:
                    string pluginFilename = string.Format("{0}/{1}", Server.MapPath("~/bin"), Item.FileName).Replace(".zip", ".dll");
                    new WebClient().DownloadFile(Item.DownloadUrl, pluginFilename);
                    break;
            }


            success = true;
        }
        catch { }

        if (success)
        {
            InstallButton.Enabled = false;
            CancelButton.Enabled = false;

            Message.Type = StatusType.Success;
            Message.Text = string.Format("This {0} has been successfully installed.", _itemTypeName);
        }
        else
        {
            InstallButton.Text = "Try Again";
            Message.Type = StatusType.Error;
            Message.Text = string.Format("An error has occurred during the downloading of this {0}.", _itemTypeName);
        }
    }

    protected ItemInfo Item
    {
        get { return Marketplace.Catalogs[_catalogType].Items[_itemId]; }
    }
}
