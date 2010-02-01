<%@ Page Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (SiteSettings.Get().RequireSSL)
        {
            Util.RedirectToSSL(Context);
        }
    }
    protected void ChangePassword_Click(object sender, EventArgs e)
    {
        string user = Request.QueryString["usr"];
        string uniqueId = Request.QueryString["key"];

        IGraffitiUser gu = GraffitiUsers.GetUser(user);

        if (String.IsNullOrEmpty(NewPassword.Text) || String.IsNullOrEmpty(ConfirmPassword.Text))
        {
            message.Text = "Please enter a new password.";
            message.ForeColor = System.Drawing.Color.Red;
            return;
        }
        
        if (gu != null)
        {
            if (gu.UniqueId == new Guid(uniqueId))
            {
                if (NewPassword.Text == ConfirmPassword.Text)
                {
                    GraffitiUsers.ChangePassword(user, NewPassword.Text);
                }
                else
                {
                    message.Text = "Your passwords must match.";
                    message.ForeColor = System.Drawing.Color.Red;
                    return;
                }
            }
            else
            {
                message.Text = "Could not find user from provided key.";
                message.ForeColor = System.Drawing.Color.Red;
                return;
            }
        }
        else
        {
            message.Text = "Could not find user.";
            message.ForeColor = System.Drawing.Color.Red;
            return;
        }

        message.Text = "Password changed!<br />Redirecting in 3 seconds...";
        message.ForeColor = System.Drawing.Color.Green;

        IGraffitiUser guLogin = GraffitiUsers.Login(gu.Name, NewPassword.Text);
        FormsAuthentication.SetAuthCookie(guLogin.Name, false);
        
        ClientScript.RegisterStartupScript(this.GetType(), "goto-homepage", String.Format("setTimeout('window.parent.location.href = \"{0}\"', 3000)", ResolveUrl("~/")), true);
    }
</script>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Graffiti - Reset Password</title>

    <style type="text/css">
    body 
    {
        font-family: verdana, arial, helvetica, sans-serif;
        background-color: #ddd;
        text-align: center;
    }
    	
    .box 
    {
        margin: auto;
        margin-top: 100px;
        padding: 4px;
        font-size: 12px;
        line-height: 1.5em;
        color: #333;
        background-color:#fff;
        border: solid 5px #0080C3;
        width: 300px;
        text-align: left;
    }
    .box h3 
    {
        margin: 0px;
        margin-bottom: 10px;
        line-height: 25px;
        padding-left: 5px;
        font-size: 14px;
        font-weight: bold;
        color: #D75E09;
        background: #eee;
    }

    label
    {
        vertical-align: 2px;
    }
    </style>
</head>
<body>
<div id="content">
        <form id="form1" runat="server">
            <div class="box">
                <h3>Graffiti - Reset Password</h3>
                <table style="margin: auto;">
                    <tr>
                        <td>New Password:</td>
                        <td><asp:TextBox runat="Server" ID = "NewPassword" TextMode="password" style="width: 130px;" /></td>
                    </tr>
                    <tr>
                        <td>Confirm Password:</td>
                        <td><asp:TextBox runat="Server" ID = "ConfirmPassword" TextMode="password" style="width: 130px;" /></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td style="text-align: right; padding-top: 8px;"><asp:Button style="width: 130px;" runat="Server" ID = "btnChangePassword" Text="Change Password" OnClick="ChangePassword_Click" /></td>
                    </tr>
                </table>
                <div style="text-align: center; padding-top: 10px;">
                    <asp:Label runat="Server" ID="message" style="font-weight: bold;" />
                </div>
            </div>
        </form>
    </div>
</body>
</html>