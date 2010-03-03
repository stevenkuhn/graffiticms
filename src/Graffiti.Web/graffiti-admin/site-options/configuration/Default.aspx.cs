using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using Graffiti.Core;

public partial class graffiti_admin_site_options_configuration_Default : AdminControlPanelPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context, "settings");

        if (!IsPostBack)
        {
            SiteSettings settings = SiteSettings.Get();

				chkUseExternalJQuery.Checked = settings.UseExternalJQuery;
            chkUseProxy.Checked = settings.UseProxyServer;
            txtProxyHost.Text = settings.ProxyHost;
            txtProxyPort.Text = settings.ProxyPort.ToString();
            txtProxyUsername.Text = settings.ProxyUsername;
            txtProxyPassword.Text = settings.ProxyPassword;
            chkBypassProxyOnLocal.Checked = settings.ProxyBypassOnLocal;
            chkCacheViews.Checked = settings.CacheViews;
            txtDateFormat.Text = settings.DateFormat;
            txtTimeFormat.Text = settings.TimeFormat;
            chkFilterUncategorizedPostsFromLists.Checked = settings.FilterUncategorizedPostsFromLists;
            chkIncludeChildPosts.Checked = settings.IncludeChildPosts;
            txtPageSize.Text = settings.PageSize.ToString();
            chkGenerateFolders.Checked = settings.GenerateFolders;
        }

        string display = chkUseProxy.Checked ? "display: block; padding-left: 20px;" : "display: none; padding-left: 20px;";
        proxySettings.Attributes.Add("style", display);
    }

    protected void ConfigurationSave_Click(object sender, EventArgs e)
    {
        try
        {
            SiteSettings settings = SiteSettings.Get();

				settings.UseExternalJQuery = chkUseExternalJQuery.Checked;
            settings.UseProxyServer = chkUseProxy.Checked;
            settings.ProxyHost = txtProxyHost.Text;
            settings.ProxyPort = Int32.Parse(txtProxyPort.Text);
            settings.ProxyUsername = txtProxyUsername.Text;
            settings.ProxyPassword = txtProxyPassword.Text;
            settings.ProxyBypassOnLocal = chkBypassProxyOnLocal.Checked;
            settings.CacheViews = chkCacheViews.Checked;
            settings.DateFormat = txtDateFormat.Text;
            settings.TimeFormat = txtTimeFormat.Text;
            settings.FilterUncategorizedPostsFromLists = chkFilterUncategorizedPostsFromLists.Checked;
            settings.IncludeChildPosts = chkIncludeChildPosts.Checked;
            settings.PageSize = Int32.Parse(txtPageSize.Text);
            settings.GenerateFolders = chkGenerateFolders.Checked;

            settings.Save();

            Message.Text = "Your configuration has been updated!";
            Message.Type = StatusType.Success;
        }
        catch (Exception ex)
        {
            Message.Text = "Your configuration could not be updated. Reason: " + ex.Message;
            Message.Type = StatusType.Error;
        }
    }

}

