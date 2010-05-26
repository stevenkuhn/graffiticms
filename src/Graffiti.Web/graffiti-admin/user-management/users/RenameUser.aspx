<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminModal.master" Title="Rename User" Inherits="Graffiti.Core.AdminControlPanelPage" %>
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
        }
        
    }

    protected void RenameUser_Click(object sender, EventArgs e)
    {
    
        GraffitiUsers.RenameUser(theUser.Name, NewUserName.Text);
        ClientScript.RegisterStartupScript(this.GetType(), "close-and-refresh",
                string.Format("window.parent.location.href = '{0}?user={1}';", VirtualPathUtility.ToAbsolute("~/graffiti-admin/user-management/users/"), NewUserName.Text), true);
    }

</script>

<asp:Content ContentPlaceHolderID="HeaderRegion" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyRegion" runat="server">

<div class="ModalFormBlock">

    <asp:PlaceHolder id="RenameUserPanel" runat="server">
    <h2>Enter the new username for this user</h2>
    <asp:TextBox id="NewUserName" runat="server" MaxLength="256" CssClass="small" />
    </asp:PlaceHolder>
    
    <div class="submit">
        <asp:Button ID="RenameUser" Text="Rename User" runat="server" OnClick="RenameUser_Click" />
        <a href="javascript:window.parent.Telligent_Modal.Close();">(Cancel)</a>
        <br /><span id="Status">Once you click the button,
        please <strong>do not</strong> refresh the page
        or navigate away from this page until it is finished.</span></p>
    </div>
</div>

</asp:Content>
