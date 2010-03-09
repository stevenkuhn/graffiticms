<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminModal.master" Title="Delete User" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<%@ Import Namespace="Graffiti.Core" %>
<%@ Import Namespace="System.Collections.Generic" %>

<script runat="Server">

    IGraffitiUser theUser;
        
void Page_Load(object sender, EventArgs e)
{
    if (!GraffitiUsers.IsUserInRole(GraffitiUsers.Current.Name, GraffitiUsers.AdminRole))
        throw new Exception("Invalid permissions");

    if (Request.QueryString["user"] != null)
    {
        theUser = GraffitiUsers.GetUser(Request.QueryString["user"]);

        if (!IsPostBack)
        {
			  List<IGraffitiUser> users = GraffitiUsers.GetUsers("*");

			  if (GraffitiUsers.IsAdmin(theUser))
			  {
				  // Don't allow the last admin user to be deleted
				  List<IGraffitiUser> adminUsers = GraffitiUsers.GetUsers(GraffitiUsers.AdminRole);
				  if (adminUsers.Count <= 1)
				  {
					  UserMessage.Type = StatusType.Error;
					  UserMessage.Text = "There is only one existing admin user. You must have at least one administrator.";
					  AssignUser.Visible = false;
					  DeleteUser.Visible = false;
					  return;
				  }
				  
			  }
			  
            if (users.Count > 1)
            {
                UserMessage.Type = StatusType.Warning;
                UserMessage.Text = string.Format("The user <b>{0}</b> will be deleted.", theUser.Name);

                foreach (IGraffitiUser user in users)
                {
                    if (user.Name != theUser.Name)
                        NewUser.Items.Add(new ListItem(user.ProperName, user.Name));
                }

            }
            else
            {
                UserMessage.Type = StatusType.Error;
                UserMessage.Text = "There is only one existing user. You must have at least one user.";
                AssignUser.Visible = false;
                DeleteUser.Visible = false;
            }
            
        }
    }
        
}

protected void DeleteUser_Click(object sender, EventArgs e)
{
    IGraffitiUser newUser = GraffitiUsers.GetUser(NewUser.SelectedValue);
    string errorMessage = null;
    bool result = GraffitiUsers.DeleteUser(theUser, newUser, out errorMessage);

    if (result)
    {
        ClientScript.RegisterStartupScript(this.GetType(), "close-and-refresh",
                "window.parent.location.href = '" + VirtualPathUtility.ToAbsolute("~/graffiti-admin/user-management/") + "';", true);
    }
    else
    {
        UserMessage.Type = StatusType.Error;
        UserMessage.Text = string.Format("The user <b>{0}</b> could not be deleted.<br />\n{1}", theUser.Name, errorMessage);
    }
}

</script>

<asp:Content ContentPlaceHolderID="HeaderRegion" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyRegion" runat="server">

<div class="ModalFormBlock">

    <Z:StatusMessage runat="server" ID="UserMessage" />
    
    <asp:PlaceHolder id="AssignUser" runat="server">
    <p>All posts created by the user that is being deleted will be re-assigned to an existing user.</p>
    <h2>Re-assign content to:</h2>
    <asp:DropDownList id="NewUser" runat="server" />
    </asp:PlaceHolder>
    
    <div class="submit">
        <asp:Button ID="DeleteUser" Text="Delete User" runat="server" OnClick="DeleteUser_Click" />
        <a href="javascript:window.parent.Telligent_Modal.Close();">(Cancel)</a>
    </div>
</div>

</asp:Content>
