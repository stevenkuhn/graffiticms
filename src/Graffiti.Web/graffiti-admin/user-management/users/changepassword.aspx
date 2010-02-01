<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" Title="Change Password" %>
<script runat="Server" >
protected override void OnInit(EventArgs e)
{
    base.OnInit(e);

    if (SiteSettings.Get().RequireSSL)
    {
        Util.RedirectToSSL(Context);
    }
}

void Page_Load(object sender, EventArgs e)
{
    if(Request.QueryString["user"] == null)
        Response.Redirect("~/graffiti-admin/user-management/users/changepassword.aspx?user=" + GraffitiUsers.Current.Name);

    Cancel_Password.NavigateUrl = "~/graffiti-admin/user-management/?user=" + Request.QueryString["user"];

    IGraffitiUser theUser = GraffitiUsers.GetUser(Request.QueryString["user"]);
    if (theUser.Name != GraffitiUsers.Current.Name)
    {
        if (!GraffitiUsers.IsUserInRole(GraffitiUsers.Current.Name, GraffitiUsers.AdminRole))
            throw new Exception("Invalid permissions");
        else
        {
            CurrentMessage.Visible = false;
            PageText.Text = "Change " + theUser.ProperName + "'s password.";
        }
    }
}

void Password_Update(object sender, EventArgs e)
{
    if (IsValid)
    {
        IGraffitiUser user = GraffitiUsers.GetUser(Request.QueryString["user"]);

        if (user.Name == GraffitiUsers.Current.Name)
        {
            if (txtPassword.Text != txtPassword2.Text)
            {
                Message.Text = "The new passwords do not match";
                Message.Type = StatusType.Error;
                return;
            }
            
            if (!GraffitiUsers.ChangePassword(user.Name, CurrentPassword.Text, txtPassword.Text))
            {
                Message.Text = "The password update was not successful";
                Message.Type = StatusType.Warning;
                return;
            }
        }
        else
        {
            if (!GraffitiUsers.ChangePassword(user.Name, txtPassword.Text))
            {
                Message.Text = "The password update was not successful";
                Message.Type = StatusType.Warning;
                return;
            }
        }
        


        Response.Redirect("~/graffiti-admin/user-management/?user=" + Request.QueryString["user"]);
        
    }
}

</script>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<h1><asp:Literal ID ="PageText" runat="Server" Text="Change Your Password" /></h1>

<Z:Breadcrumbs runat="server" SectionName="ChangePassword" />
<z:StatusMessage runat="Server" ID = "Message" />

<div id = "messages_form">
    
    <div id="post_form_container" class="FormBlock">
    
    <div id="CurrentMessage" runat="Server">
    <h2>Current Password:</h2>
    <asp:TextBox runat="Server" ID = "CurrentPassword" TextMode="Password" CssClass="small" />
    </div>
    
    <h2>New Password: <asp:RequiredFieldValidator ControlToValidate ="txtPassword" Display="Dynamic" runat="Server">Please add a new password</asp:RequiredFieldValidator> </h2>
    <asp:TextBox runat="Server" ID = "txtPassword" TextMode="Password" CssClass="small" />
    
    <h2>Confirm Password:
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate ="txtPassword2" Display="Dynamic" runat="Server">Please add a new password</asp:RequiredFieldValidator>
    <asp:CompareValidator ControlToCompare="txtPassword" ControlToValidate="txtPassword2" Operator="Equal" runat="server">Your passwords do not match</asp:CompareValidator>  </h2>
    <asp:TextBox runat="Server" ID = "txtPassword2" TextMode="Password" CssClass="small" />
    
    </div>
    
    <div class="submit">
        <div id="buttons">
            <asp:Button runat="Server" ID="SettingsSave" Text = "Update Password" OnClick ="Password_Update" />
            <asp:HyperLink ID ="Cancel_Password" Text = "(Cancel)"  runat="Server" />
        </div>
    </div>     
    
</div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
</asp:Content>
