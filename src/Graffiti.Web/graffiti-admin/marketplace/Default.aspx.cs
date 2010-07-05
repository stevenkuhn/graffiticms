using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using Graffiti.Core;
using Graffiti.Core.Marketplace;
using Glow = Telligent.Glow;

public partial class graffiti_admin_marketplace_Default : AdminControlPanelPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // LiHyperLink.SetNameToCompare(Context, "marketplace");

        if (!IsPostBack)
        {
            try
            {
                catalogList.DataSource = Marketplace.Catalogs.Values;
                catalogList.DataBind();

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
            }
        }
    }

}
