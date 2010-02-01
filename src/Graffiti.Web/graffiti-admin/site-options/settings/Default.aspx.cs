using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Graffiti.Core;

public partial class graffiti_admin_site_options_Default : AdminControlPanelPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context,"settings");

        if(!IsPostBack)
        {
            SiteSettings settings = SiteSettings.Get();
            txtTitle.Text = Server.HtmlDecode(settings.Title);
            txtTagline.Text = Server.HtmlDecode(settings.TagLine);
            txtCopyright.Text = settings.CopyRight;
            txtFeedBurner.Text = settings.ExternalFeedUrl;
            txtStats.Text = settings.WebStatistics;
            txtHeader.Text = settings.Header;
            txtMetaScription.Text = Server.HtmlDecode(settings.MetaDescription ?? string.Empty);
            txtKeywords.Text = Server.HtmlDecode(settings.MetaKeywords ?? string.Empty);

            DateTime dt = DateTime.Now;
            // there are timezones which differ by 15 minutes
            for(double i = -24; i < 24; i = i + .25)
            {
                ListItem li = new ListItem(dt.AddHours(i).ToString("ddd, dd MMMM yyyy HH:mm"), i.ToString());
                TimeOffSet.Items.Add(li);
            }

            ListItem liSelected = TimeOffSet.Items.FindByValue(settings.TimeZoneOffSet.ToString());
            if(liSelected != null)
                liSelected.Selected = true;
            else
                TimeOffSet.Items.FindByValue("0").Selected = true;

            GraffitiLogo.Checked = settings.DisplayGraffitiLogo;
        }
    }

    protected void SettingsSave_Click(object sender, EventArgs e)
    {
        try
        {
            SiteSettings settings = SiteSettings.Get();
            settings.Title = Server.HtmlEncode(txtTitle.Text.Trim());
            settings.TagLine = Server.HtmlEncode(txtTagline.Text.Trim());
            settings.CopyRight = txtCopyright.Text.Trim();
            settings.TimeZoneOffSet = double.Parse(TimeOffSet.SelectedValue);
            settings.ExternalFeedUrl = txtFeedBurner.Text.Trim();
            settings.WebStatistics = txtStats.Text.Trim();
            settings.Header = txtHeader.Text.Trim();
            settings.MetaDescription = Server.HtmlEncode(txtMetaScription.Text.Trim());
            settings.MetaKeywords = Server.HtmlEncode(txtKeywords.Text.Trim());

            settings.DisplayGraffitiLogo = GraffitiLogo.Checked;

            settings.Save();

            Message.Text = "Your settings have been updated!";
            Message.Type = StatusType.Success;
        }
        catch(Exception ex)
        {
            Message.Text = "Your settings could not be updated. Reason: " + ex.Message;
            Message.Type = StatusType.Error;
        }
    }
}
