<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminModal.master" Title="Delete Role" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<%@ Import Namespace="Graffiti.Core" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="Server">
        
	void Page_Load(object sender, EventArgs e)
	{
		if (!GraffitiUsers.IsAdmin(GraffitiUsers.Current))
			throw new Exception("Invalid permissions");
	}

	protected void DeleteRole_Click(object sender, EventArgs e)
	{
		string roleName = Request.QueryString["role"];
		if (!string.IsNullOrEmpty(roleName))
			GraffitiUsers.DeleteRole(HttpUtility.HtmlDecode(HttpUtility.UrlDecode(roleName)));

		ClientScript.RegisterStartupScript(this.GetType(), "close-and-refresh",
			 "window.parent.location.href = '" + VirtualPathUtility.ToAbsolute("~/graffiti-admin/user-management/roles") + "';", true);
	}

</script>

<asp:Content ContentPlaceHolderID="HeaderRegion" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="BodyRegion" runat="server">
	<div class="ModalFormBlock">
		<Z:StatusMessage runat="server" ID="RoleMessage" />
		<asp:PlaceHolder ID="AssignRole" runat="server">
			<p>
				This role will be permanently deleted. Any users that were in this role will have
				their permissions removed.</p>
		</asp:PlaceHolder>
		<div class="submit">
			<asp:Button ID="DeleteRole" Text="Delete Role" runat="server" OnClick="DeleteRole_Click" />
			<a href="javascript:window.parent.Telligent_Modal.Close();">(Cancel)</a>
		</div>
	</div>
</asp:Content>
