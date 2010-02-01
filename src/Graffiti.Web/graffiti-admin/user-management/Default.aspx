<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" Title="User Management" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<%@ Import Namespace="System.IO" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<h1>User Management</h1>
<h3>What would you like to change?</h3>

<div class="listboxes">
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='users/';">
        <div class="title">
            Users
        </div>
        <div class="body" style="height:105px; padding: 10px;">
            Information about the users on your site and their Roles and Permissions.
        </div>
        <div class="commands"></div>
        </div>
    </div>
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='roles/';">
        <div class="title">
            Roles and Permissions
        </div>
        <div class="body" style="height:105px; padding: 10px;">
            Create custom roles to manage read, edit, and publish permissions on your site.
        </div>
        <div class="commands"></div>
        </div>
    </div>
</div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
    <div id="sidebar"><div class="gutter">
    <div class="box">
        <h3>About This Page</h3>
        <p>This page is your starting point for managing users and roles.</p>
    </div>
    <div style="clear: both;"></div>
    </div></div>
</asp:Content>