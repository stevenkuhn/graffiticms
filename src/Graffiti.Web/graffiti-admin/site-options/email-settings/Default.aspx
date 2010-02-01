<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" Inherits="graffiti_admin_site_options_email_settings" AutoEventWireup="true" Title="Graffiti - Email Settings" Codebehind="Default.aspx.cs" %>
<%@ Register TagPrefix="Z" Assembly="Graffiti.Core" Namespace="Graffiti.Core" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<script language="javascript">

function toggleServerAuthFields()
{
    var chk = document.getElementById('<%= RequiresAuthentication.ClientID %>');
    
    var user = document.getElementById('<%= txtUser.ClientID %>');
    var pass = document.getElementById('<%= txtPassword.ClientID %>');
    
    user.disabled = !chk.checked;
    pass.disabled = !chk.checked;
}

</script>

<h1>Email Settings</h1>

<Z:Breadcrumbs runat="server" SectionName="EmailSettings" />
<z:StatusMessage runat="Server" ID = "Message" />

<div id = "messages_form">
    
    <div id="post_form_container" class="FormBlock">
    
    <h2>Server: </h2>
    <asp:TextBox CssClass = "small" runat="Server" id="txtServerName" TabIndex="1" />
    
    <h2>Port:</h2> 
    <asp:TextBox CssClass = "small" style="width: 75px;" runat="Server" id="txtPort" TabIndex="1" />
    (25 is the default for most email servers)
    
    <h2>From: </h2>
    <asp:TextBox CssClass = "small" runat="Server" id="txtFrom" TabIndex="2" />   

    <h2 style="margin-top: 20px;">
        <asp:CheckBox ID="RequiresAuthentication" runat="server" Text="Server requires authentication" onclick="toggleServerAuthFields()" TabIndex="3"/>    
    </h2>
    <h2>
        <asp:CheckBox ID="UseSSL" runat="server" Text="Use SSL" TabIndex="4"/>
    </h2>
 
    <h2>User: </h2>
    <asp:TextBox CssClass = "small" runat="Server" id="txtUser" TabIndex="5" />   
    
    <h2>Password: </h2>
    <asp:TextBox CssClass = "small" runat="Server" id="txtPassword" TabIndex="6" />   
 
    <h2>Test your settings:</h2>
    <p class="submit">
        Send a test email to <asp:TextBox runat="Server" id="txtTestEmail" TabIndex="7" />  
        <asp:button id="SendTestEmail" runat="server" Text="Send email" OnClick="SendTestEmail_Click" />
    </p>
    </div>

    
    <div class="submit">
        <div id="buttons">
            <asp:Button runat="Server" ID="SettingsSave" Text = "Update Settings"  OnClick="SettingsSave_Click" TabIndex="6" />
            <asp:HyperLink ID ="Cancel_Edit" Text = "(Cancel)" NavigateUrl="~/graffiti-admin/site-options/" runat="Server" TabIndex="7" />
        </div>
    </div> 
</div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
</asp:Content>

