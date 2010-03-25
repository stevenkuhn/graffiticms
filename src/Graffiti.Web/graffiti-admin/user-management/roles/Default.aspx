<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_roles_Default" Title="Manage Roles" Codebehind="Default.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">

<Z:FileBrowser ID="FileBrowser1" runat="Server" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

   <h1><asp:Literal runat="Server" ID="PageText" Text="Manage Roles" /></h1> 
    
    <Z:Breadcrumbs runat="server" SectionName="Roles" />
    <Z:StatusMessage runat="Server" ID = "Message"  Type="Success" />
    
    <div id="new_role_container" class="FormBlock abc" runat="Server">
    
    <h3>Add a new role</h3>
    
    <h2>Role name: <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtRoleName" Text="Please enter a Role name" runat ="Server" /></h2>
    <asp:TextBox CssClass = "small" runat="Server" id="txtRoleName" TabIndex="1" MaxLength="256" />  
    
    <h2>Permissions</h2>
    <p style="float: left; margin-right: 10px; margin-top: 5px;"><asp:checkbox ID="read" runat="server" Text="Read" /></p>
    <p style="float: left; margin-right: 10px; margin-top: 5px;"><asp:checkbox ID="edit" runat="server" Text="Edit" /></p>
    <p style="float: left; margin-top: 5px;"><asp:checkbox ID="publish" runat="server" Text="Publish" /></p>
    
    <p style="clear: both;">
        <asp:Button runat="Server" ID="CreateRole" OnClick="CreateRole_Click" CssClass="button"  Text = "Add New Role" TabIndex="4" />
    </p>
    
    </div>
    
    <div id="role_edit_form" class="FormBlock abc" runat="Server">

    <table id="postList" style="font-size: 115%; width: 45%;">
        <tr style="font-weight: bold;">
            <td></td>
            <td style="text-align: center;">Read</td>
            <td style="text-align: center;">Edit</td>
            <td style="text-align: center;">Publish</td>
        </tr>
        <tr>
            <td style="font-size: 110%;">Role: <asp:Literal runat="Server" id="litExistingRoleName" /></td>
            <td style="text-align: center;"><asp:checkbox ID="readRolePermission" runat="server" /></td>
            <td style="text-align: center;"><asp:checkbox ID="editRolePermission" runat="server" /></td>
            <td style="text-align: center;"><asp:checkbox ID="publishRolePermission" runat="server" /></td>
        </tr>
        <tr>
            <td colspan="4"><h2 style="margin-bottom: 15px;"><span>The settings above apply to all categories. If you want to setup permissions by individual categories, you can do so below. (note: the permissions above will not be used if you choose to filter by categories)</span></h2></td>
        </tr>
    <Z:Repeater runat="Server" ID ="CategoryList" OnItemDataBound="CategoryList_OnItemDataBound">
        <HeaderTemplate>
            <tr style="font-weight: bold;">
                <td>Category</td>
                <td style="text-align: center;">Read</td>
                <td style="text-align: center;">Edit</td>
                <td style="text-align: center;">Publish</td>
            </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr id="cat-<%# Eval("Id") %>"<%# IsAltRow(Container.ItemIndex) %>>
                <td><asp:label ID="categoryName" runat="server" /><asp:HiddenField ID="categoryId" runat="server" /></td>
                <td style="text-align: center;"><asp:checkbox ID="readRoleCatPermission" runat="server"  /></td>
                <td style="text-align: center;"><asp:checkbox ID="editRoleCatPermission" runat="server"  /></td>
                <td style="text-align: center;"><asp:checkbox ID="publishRoleCatPermission" runat="server"  /></td>
            </tr>
        </ItemTemplate>
    </Z:Repeater>
    </table>
    
    <div class="submit">
        <asp:Button runat="Server" ID="EditRoles" CssClass="button"  Text = "Save" TabIndex="2" OnClick="EditRoles_Save" />
        <asp:HyperLink ID ="Cancel_Edit" Text = "(Cancel)" NavigateUrl="~/graffiti-admin/user-management/roles/" runat="Server" />
    </div>
    
    </div>    
    
    <div runat="Server" id ="Roles_List">
    
    <Z:Repeater runat="Server" ID ="Role_List">
        <HeaderTemplate>
            <h3>Existing roles</h3>
            <ul class="listboxes">
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <div class="nonnested">
                    <div class="title"><%# HttpUtility.HtmlEncode(Eval("RoleName").ToString()) %></div>
                    <div class="commands">
                    <a href="?role=<%# HttpUtility.UrlEncode(HttpUtility.HtmlEncode(Eval("RoleName").ToString())) %>">Edit</a>
                    <% if(CanDelete()) { %>
                    <span <%# IsNotSystemRole(Eval("RoleName").ToString()) %> style="padding: 0 4px 0 4px;">|</span>
                    <a <%# IsNotSystemRole(Eval("RoleName").ToString()) %> href="javascript:Telligent_Modal.Open('DeleteRole.aspx?role=<%# HttpUtility.UrlEncode(HttpUtility.UrlEncode(HttpUtility.HtmlEncode(Eval("RoleName").ToString()))) %>', 400, 200, null);">Delete</a>
                    <% }  %>
                    </div>
                </div>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </Z:Repeater>
    
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
</asp:Content>

