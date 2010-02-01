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
using System.Net.Mail;

public partial class graffiti_admin_site_options_email_settings : AdminControlPanelPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context,"settings");

        ClientScript.RegisterStartupScript(this.GetType(), "set-fields", "toggleServerAuthFields()", true);
        if (!IsPostBack)
        {
            SiteSettings settings = SiteSettings.Get();
            txtServerName.Text = settings.EmailServer;
            txtFrom.Text = settings.EmailFrom;
            RequiresAuthentication.Checked = settings.EmailServerRequiresAuthentication;
            UseSSL.Checked = settings.EmailRequiresSSL;
            txtPort.Text = settings.EmailPort.ToString();

            if (RequiresAuthentication.Checked)
            {
                txtUser.Text = settings.EmailUser;
                txtPassword.Text = settings.EmailPassword;
            }

            PageTemplateToolboxContext ptc = new PageTemplateToolboxContext();
            ptc.Put("comment", this);
        }
    }

    protected void SettingsSave_Click(object sender, EventArgs e)
    {
        try
        {
            SiteSettings settings = SiteSettings.Get();
            settings.EmailServer = txtServerName.Text;
            settings.EmailFrom = txtFrom.Text;
            settings.EmailRequiresSSL = UseSSL.Checked;
            settings.EmailPort = Convert.ToInt32(txtPort.Text);

            settings.EmailServerRequiresAuthentication = RequiresAuthentication.Checked;
            if (RequiresAuthentication.Checked)
            {
                settings.EmailUser = txtUser.Text;
                settings.EmailPassword = txtPassword.Text;
            }

            settings.Save();

            Message.Text = "Your settings have been updated!";
            Message.Type = StatusType.Success;
        }
        catch (Exception ex)
        {
            Message.Text = "Your settings could not be updated. Reason: " + ex.Message;
            Message.Type = StatusType.Error;
        }
    }

    protected void SendTestEmail_Click(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(txtTestEmail.Text))
        {
            Message.Text = "Please enter an address to send the test message to";
            Message.Type = StatusType.Error;
            return;
        }

        try
        {
            MailMessage mm = new MailMessage();
            mm.To.Add(new MailAddress(txtTestEmail.Text));
            mm.From = new MailAddress(txtFrom.Text, "GraffitiAdmin");
            mm.Subject = "Graffiti Test Email";
            mm.Body = "This email was sent to test your email settings configured in Graffiti. If you are seeing this message, your settings worked!<br /><br />Graffiti Admin";
            mm.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.Host = txtServerName.Text;
            client.Port = Convert.ToInt32(txtPort.Text);

            if (RequiresAuthentication.Checked)
                client.Credentials = new System.Net.NetworkCredential(txtUser.Text, txtPassword.Text);

            if(UseSSL.Checked)
                client.EnableSsl = true;

            client.Send(mm);

            Message.Text = "Your email was succesfully sent. Check the Inbox for <b>" + txtTestEmail.Text + "</b> to make sure the message arrived.";
            Message.Type = StatusType.Success;
        }
        catch (Exception ex)
        {
            Message.Text = "The email message could not be sent: " + ex.Message;
            Message.Type = StatusType.Error;
        }
    }
}

