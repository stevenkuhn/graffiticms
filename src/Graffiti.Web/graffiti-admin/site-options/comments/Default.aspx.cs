using System;
using Graffiti.Core;

public partial class graffiti_admin_site_options_comments_Default : AdminControlPanelPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context, "settings");

        if(!IsPostBack)
        {
            CommentSettings settings = CommentSettings.Get();
            EnableComments.Checked = settings.EnableCommentsDefault;
            CommentDays.Items.FindByValue(settings.CommentDays.ToString()).Selected = true;
            txtEmail.Text = settings.Email;
            txtSpamScore.Text = settings.SpamScore.ToString();

            txtAkismetId.Text = settings.AkismetId;
            txtAkismetScore.Text = settings.AkismetScore.ToString();

            chkUseAkismet.Checked = settings.UseAkismet;

            if (settings.CommentDays > 0 && String.IsNullOrEmpty(settings.AkismetId))
            {
                Message.Text = "We noticed you have enabled new comments, but have not added Akismet. Hopefully that is what you are doing right now, if not, please think about it.";
                Message.Type = StatusType.Warning;
            }
        }

        string display = chkUseAkismet.Checked ? "display: block; padding-left: 20px;" : "display: none; padding-left: 20px;";
        akismetSettings.Attributes.Add("style", display);
    }

    protected void SettingsSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(txtAkismetId.Text.Trim()))
            {
                Macros m = new Macros();
                Joel.Net.Akismet akismet = new Joel.Net.Akismet(txtAkismetId.Text.Trim(), m.FullUrl(new Urls().Home), SiteSettings.Version);
                if (!akismet.VerifyKey())
                {
                    Message.Text = "Your Akismet key could not be verified. Please check it and re-enter it.";
                    Message.Type = StatusType.Error;
                    return;
                }
            }

            CommentSettings settings = CommentSettings.Get();
            settings.EnableCommentsDefault = EnableComments.Checked;
            settings.CommentDays = Int32.Parse(CommentDays.SelectedValue);
            settings.Email = txtEmail.Text;
            settings.SpamScore = Int32.Parse(txtSpamScore.Text);

            if (chkUseAkismet.Checked && String.IsNullOrEmpty(txtAkismetId.Text))
            {
                Message.Text = "Please provide your Akismet Id.";
                Message.Type = StatusType.Error;
                return;
            }

            settings.UseAkismet = chkUseAkismet.Checked;

            settings.AkismetId = txtAkismetId.Text;
            settings.AkismetScore = Int32.Parse(txtAkismetScore.Text);
            
            settings.Save();

            Message.Text = "Your Comment & Spam settings have been updated!";
            Message.Type = StatusType.Success;
        }
        catch(Exception ex)
        {
            Message.Text = "Your Comment & Spam settings could not be updated. Reason: " + ex.Message;
            Message.Type = StatusType.Error;
        }
    }
}
