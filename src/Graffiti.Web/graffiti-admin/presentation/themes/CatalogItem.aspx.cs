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

public partial class graffiti_admin_presentation_themes_CatalogItem : AdminControlPanelPage
{
    private int _catalogId = 1002;
    private int _itemId = 0;

    protected void Page_Init(object sender, EventArgs e)
    {
        string item = Request.QueryString["item"];
        if (!string.IsNullOrEmpty(item))
        {
            try { _itemId = int.Parse(item); }
            catch { }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context, "presentation");
        SetControlVisibility();
    }

    public void SetControlVisibility()
    {
        AdditionalConfiguration.Visible = Item.RequiresManualIntervention;

        Location.Visible = Item.Creator.DisplayLocation;
        Email.Visible = Item.Creator.DisplayEmail;

        Statistics.Visible = (Item.Statistics.ViewCount + Item.Statistics.DownloadCount + Item.Statistics.RatingCount > 0);
        Views.Visible = (Item.Statistics.ViewCount > 0);
        Downloads.Visible = (Item.Statistics.DownloadCount > 0);
        Rating.Visible = (Item.Statistics.RatingCount > 0);

        InstallButton.Visible = DownloadButton.Visible = BuyButton.Visible = false;
        if (Item.Purchase.Price > 0)
            BuyButton.Visible = true;
        else if (Item.RequiresManualIntervention)
            DownloadButton.Visible = true;
        else
            InstallButton.Visible = true;
    }

    protected void Install_Click(object sender, EventArgs e)
    {
        bool success = false;
        string targetFilename = string.Format("{0}/{1}", Server.MapPath("~/files/themes"), Item.FileName);

        try
        {
            new WebClient().DownloadFile(Item.DownloadUrl, targetFilename);

            UTF8Encoding utf = new UTF8Encoding();
            string encodedFile = utf.GetString(File.ReadAllBytes(targetFilename));
            ThemeConverter.ToDisk(encodedFile, Server.MapPath("~/files/themes/"), true, Item.Name);
            File.Delete(targetFilename);

            success = true;
        }
        catch { }

        if (success)
        {
            InstallButton.Enabled = false;
            CancelButton.Enabled = false;

            Message.Type = StatusType.Success;
            Message.Text = "This theme has been successfully installed.";
        }
        else
        {
            InstallButton.Text = "Try Again";
            Message.Type = StatusType.Error;
            Message.Text = "An error has occurred during the downloading of this theme.";
        }
    }

    protected CatalogInfo Catalog
    {
        get { return Marketplace.Catalogs[_catalogId]; }
    }

    protected ItemInfo Item
    {
        get { return Catalog.Items[_itemId]; }
    }
}
