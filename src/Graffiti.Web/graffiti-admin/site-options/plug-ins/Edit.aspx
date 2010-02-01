<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_presentation_plugins_Edit" Title="Graffiti Edit Plug-In" Codebehind="Edit.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<div id = "messages_form">
    
    
    <h1><asp:Literal ID = "PageTitle" runat="Server" /></h1> 
    <Z:Breadcrumbs runat="server" SectionName="PlugInsEdit" />
    <Z:StatusMessage runat="Server" ID = "Message"  Type="Success"/>
    
    <div id="post_form_container" class="FormBlock abc">
    
        <asp:Literal runat="Server" ID = "FormRegion" />
    
    </div>
    
<div class="submit">
    <div id="buttons">
        <asp:Button ID="Publish_Button" runat="Server" Style="font-weight:bold" Text = "Update" OnClick="EditWidget_Click" TabIndex="81" />
        <asp:HyperLink ID ="Cancel_Edit" Text = "(Cancel)" NavigateUrl="~/graffiti-admin/site-options/plug-ins/" runat="Server" />
    </div>
</div>    
    
 </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
    <div id="sidebar"><div class="gutter">
    <div class="box">
        <h3>About This Page</h3>
        <p>The editing expereince is managed by each individual widget, so it may vary. For widget specific questions, please consult the widget author.</p>
    </div>
    <div class="box">    
        <h3>Widget Editing Tip</h3>
        <p>Some widgets may not require any editing.</p>
    </div>
    <div style="clear: both;"></div>
    </div></div>
</asp:Content>

