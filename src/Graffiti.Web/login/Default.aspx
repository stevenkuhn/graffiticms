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

protected void Page_Load(object sender, EventArgs e)
{
    
    if(!IsPostBack)
        UserName.Focus();
}
    
protected void login_click(object sender, EventArgs e)
{
    IGraffitiUser gu = GraffitiUsers.Login(UserName.Text, Password.Text);
    //Graffiti.Core.User user = Graffiti.Core.User.Login(UserName.Text, Password.Text);
    if (gu == null)
    {
        message.Text = "Sorry, not valid credentials";
        message.ForeColor = System.Drawing.Color.Red;
    }
    else
    {
        FormsAuthentication.SetAuthCookie(gu.Name, RememberMe.Checked);
        
        Response.Redirect(Request.QueryString["returnurl"] ?? "~/");
    }
}

protected void btnForgotPassword_Click(object sender, EventArgs e)
{
    IGraffitiUser gu = GraffitiUsers.GetUser(UserName.Text);

    if (string.IsNullOrEmpty(UserName.Text))
    {
        message.Text = "Please enter a username";
        message.ForeColor = System.Drawing.Color.Red;
        return;
    }
    
    if (gu == null)
    {
        message.Text = "Username not found.";
        message.ForeColor = System.Drawing.Color.Red;
    }
    else
    {
        try
        {
            EmailTemplateToolboxContext ptc = new EmailTemplateToolboxContext();
            ptc.Put("datetime", DateTime.Now);
            ptc.Put("url", "http://" + Request.Url.Host + ResolveUrl("~/login/reset-password/?key=" + gu.UniqueId + "&usr=" + gu.Name));
            ptc.Put("sitesettings", SiteSettings.Get());

            EmailTemplate template = new EmailTemplate();
            template.To = gu.Email;
            template.Context = ptc;
            template.Subject = "Forgot Password";
            template.TemplateName = "ForgotPassword.view";
            Emailer.Send(template);

            message.Text = "You have been emailed instructions to reset your password.";
            message.ForeColor = System.Drawing.Color.Green;
        }
        catch (Exception)
        {
            message.Text = "An email could not be sent to the email address on file. Contact the administrator of this site or check email settings in the control panel.";
            message.ForeColor = System.Drawing.Color.Red;
        }
    }
}
</script>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Graffiti - Login</title>

    <style type="text/css">
    body 
    {
        font-family: verdana, arial, helvetica, sans-serif;
        background-color: #aaa;
        text-align: center;
    }
    	
    .box 
    {
        margin: auto;
        margin-top: 100px;
        padding: 7px;
        font-size: 12px;
        line-height: 1.5em;
        color: #333;
        background-color:#fff;
        border: solid 1px #777;
        width: 300px;
        text-align: left;
    }
    .box h3 
    {
        margin: -7px -7px 7px -7px;
	    line-height: 28px;
	    padding-left: 12px;
	    font-size: 14px;
	    font-weight: bold;
	    color: #fff;
	    background-color: #777;
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
                <h3>Graffiti - Please Login</h3>
                <table style="margin: auto;">
                    <tr>
                        <td>User Name:</td>
                        <td><asp:TextBox runat="Server" ID = "UserName" style="width: 130px;" /></td>
                    </tr>
                    <tr>
                        <td>Password:</td>
                        <td><asp:TextBox runat="Server" ID = "Password" TextMode="password" style="width: 130px;" /></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td style="text-align: center; padding-top: 5px;"><asp:CheckBox ID = "RememberMe" runat="server" Text = "Remember Me" /></td>
                    </tr>
                    <tr>
                        <td style="vertical-align: bottom;"><asp:LinkButton ID="btnForgotPassword" runat="server" Text="Forgot Password?" OnClick="btnForgotPassword_Click" /></td>
                        <td style="text-align: right; padding-top: 8px;"><asp:Button runat="Server" ID = "login" OnClick="login_click" Text="Login" /></td>
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