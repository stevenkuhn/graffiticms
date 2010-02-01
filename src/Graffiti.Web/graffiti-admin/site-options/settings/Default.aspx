<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_site_options_Default" Title="Graffiti - Site Options" Codebehind="Default.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<h1>Your Site Options</h1>

<Z:Breadcrumbs runat="server" SectionName="SiteSettings" />
<z:StatusMessage runat="Server" ID = "Message" />

<div id = "messages_form">
    
    <div id="post_form_container" class="FormBlock">
    
        <h2>Title: </h2>
        <asp:TextBox CssClass = "large" runat="Server" id="txtTitle" TabIndex="1" />   

        <h2>TagLine: </h2>
        <asp:TextBox CssClass = "large" runat="Server" id="txtTagline" TabIndex="2" />    

        <h2>Copyright: <span class="form_tip">(HTML is allowed)</span></h2>
        <asp:TextBox TextMode = "MultiLine" Columns = "60" Rows = "5" runat="Server" id="txtCopyright" TabIndex="3" /> 
        
        <h2>Meta Description: <span class="form_tip">(The site wide meta description)</span></h2>
        <asp:TextBox CssClass = "large" runat="Server" id="txtMetaScription" TabIndex="3" TextMode="MultiLine" Rows="3" MaxLength="255" /> 

        <h2>Meta Keywords: <span class="form_tip">(The side wide meta keywords)</span></h2>
        <asp:TextBox CssClass = "large" runat="Server" id="txtKeywords" TabIndex="4" MaxLength="255" />    
        
        <h2>TimeZone OffSet: <span class="form_label">(What time is it for you locally?)</span>
        </h2><asp:DropDownList runat="Server" ID = "TimeOffSet" Style="width:auto;" TabIndex="5" />

        <h2>FeedBurner Url: <span class="form_label">(See <a href="http://feedburner.com" target="_blank">FeedBurner.com</a> for more details)</span> </h2>
        <asp:TextBox CssClass = "large" runat="Server" id="txtFeedBurner" TabIndex="6" />            
        
        <h2>Web Statistics: <span class="form_label">(This is usually <a href="http://google.com/analytics" target="_blank">Google Analytics</a> javascript)</span></h2>
        <asp:TextBox TextMode = "MultiLine" Columns = "60" Rows = "5" runat="Server" id="txtStats" TabIndex="7" />
        
        <h2>Header: <span class="form_label">Additional JavaScript or Meta tags you wish to render in the header of your site.</span></h2>
        <asp:TextBox TextMode = "MultiLine" Columns = "60" Rows = "5" runat="Server" id="txtHeader" TabIndex="8" />
    
        <asp:PlaceHolder runat="server" id="LicensedOptions">
        <h2>
            <asp:CheckBox runat="server" id="GraffitiLogo" Text="Display Graffiti Logo" TabIndex="10" /><br />
            <span class="form_tip">If checked, the Graffiti logo is displayed in the site footer.</span>
        </h2>
        </asp:PlaceHolder>

    </div>
    
    <div class="submit">
        <div id="buttons">
            <asp:Button runat="Server" ID="SettingsSave" Text = "Update Settings" OnClick="SettingsSave_Click" TabIndex="9" />
            <asp:HyperLink ID ="Cancel_Edit" Text = "(Cancel)" NavigateUrl="~/graffiti-admin/site-options/" runat="Server" TabIndex="10" />
        </div>
    </div> 
</div>

</asp:Content>

