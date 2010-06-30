using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Graffiti.Core;
using Graffiti.Core.Marketplace;
using Glow = Telligent.Glow;

public partial class graffiti_admin_presentation_widgets_CatalogItem : AdminControlPanelPage
{
    private int _catalogId = 1001;
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

        Email.Visible = Item.Creator.DisplayEmail;
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
    }

    protected void Install_Click(object sender, EventArgs e)
    {
        bool success = false;
        string targetFilename = string.Format("{0}/{1}", Server.MapPath("~/bin"), Item.FileName).Replace(".zip", ".dll");

        try
        {
            new WebClient().DownloadFile(Item.DownloadUrl, targetFilename);
            success = true;
        }
        catch { }

        if (success)
        {
            InstallButton.Enabled = false;
            CancelButton.Enabled = false;

            Message.Type = StatusType.Success;
            Message.Text = "This widget has been successfully installed.";
        }
        else
        {
            InstallButton.Text = "Try Again";
            Message.Type = StatusType.Error;
            Message.Text = "An error has occurred during the downloading of this widget.";
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
