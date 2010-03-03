<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_site_options_configuration_Default" Title="Graffiti - Site Configuration" Codebehind="Default.aspx.cs" %>

<asp:Content ContentPlaceHolderID="MainRegion" Runat="Server">

<h1>Your Site Configuration</h1>

<Z:Breadcrumbs runat="server" SectionName="Configuration" />
<z:StatusMessage runat="server" ID ="Message" />

<div id = "messages_form">
    
    <div id="post_form_container" class="FormBlock">
        <h3>General Configuration</h3>
        <h2><asp:CheckBox ID="chkUseExternalJQuery" Text="Use Microsoft For JQuery" runat="Server" /> <span class="form_tip">Determines if Graffiti will use a local copy of JQuery or the one served from Microsoft.</span></h2>
        <h2><asp:CheckBox ID="chkUseProxy" Text="Use Proxy Server" runat="Server" /> <span class="form_tip">Determines if Graffiti needs to use a proxy server to make local/outside request.</span></h2>
        
        <div id="proxySettings" runat="server">
            <h2>Proxy Host:</h2>
            <asp:TextBox CssClass = "large" runat="Server" id="txtProxyHost" />  
        
            <h2>Proxy Port:</h2>
            <asp:TextBox CssClass = "large" runat="Server" id="txtProxyPort" style="width: 75px;" />  
            
            <h2>Proxy Username:</h2>
            <asp:TextBox CssClass = "large" runat="Server" id="txtProxyUsername" />  
            
            <h2>Proxy Password:</h2>
            <asp:TextBox CssClass = "large" runat="Server" id="txtProxyPassword" />  

            <h2><asp:CheckBox ID="chkBypassProxyOnLocal" Text="Bypass Proxy For Local Request" runat="Server" /></h2>
        </div>
        
        <h2><asp:CheckBox ID="chkGenerateFolders" Text="Generate Folders for Posts/Categories/Tags (Legacy)" runat="Server" /> <span class="form_tip">This setting is only needed if you are not running IIS7. (you will need to restart graffiti for the changes to take effect)</span></h2>
        
        <h3 style="margin-top: 15px;">View Configuration</h3>
        <h2><asp:CheckBox ID="chkCacheViews" Text="Cache Views" runat="Server" /> <span class="form_tip"> Developer/Designer Flag. When unchecked, Graffiti will not cache the views which makes adding new ones much easier.</span></h2>
        
        <h2>Date Format: <span class="form_tip">Formats any dates in your view when $macros.FormattedDate() is called.</span></h2>
        <asp:TextBox CssClass = "large" runat="Server" id="txtDateFormat" />  
        
        <h2>Time Format: <span class="form_tip">Formats any dates in your view when $macros.FormattedTime() is called.</span></h2>
        <asp:TextBox CssClass = "large" runat="Server" id="txtTimeFormat" />
        
        <h3 style="margin-top: 15px;">Data Configuration</h3>
        <h2><asp:CheckBox ID="chkFilterUncategorizedPostsFromLists" Text="Include Uncategorized Posts with Lists" runat="Server" /> <span class="form_tip">Determines if Graffiti should add uncategorized post to it's standard aggregate lists (such as the homepage).</span></h2>
        <h2><asp:CheckBox ID="chkIncludeChildPosts" Text="Include Child Posts with Parent List" runat="Server" /> <span class="form_tip">If checked, displays child category posts with the parent categories lists.</span></h2>
        <h2>Page Size: <span class="form_tip">Determines how many posts to display in a single list (before a pager is used for example).</span></h2>
        <asp:TextBox CssClass = "large" runat="Server" id="txtPageSize" style="width: 75px;" />
    </div>
    
    <div class="submit">
        <div id="buttons">
            <asp:Button runat="Server" ID="ConfigurationSave" Text = "Update Configuration" OnClick="ConfigurationSave_Click" TabIndex="9" />
            <asp:HyperLink ID ="Cancel_Edit" Text = "(Cancel)" NavigateUrl="~/graffiti-admin/site-options/" runat="Server" TabIndex="10" />
        </div>
    </div> 
</div>

<script language="javascript">
    $('#<%= chkUseProxy.ClientID %>').click(function() {
    $('#<%= proxySettings.ClientID %>').slideToggle();
    });
</script>

</asp:Content>


