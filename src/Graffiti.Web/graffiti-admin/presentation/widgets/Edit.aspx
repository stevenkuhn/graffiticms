<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_presentation_widgets_Edit" Title="Untitled Page" Codebehind="Edit.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<h1><asp:Literal ID = "PageTitle" runat="Server" Text="Write a post" /></h1> 
<div id = "messages_form">
    
    
    
    
    <Z:Breadcrumbs runat="server" SectionName="WidgetEdit" />
    <Z:StatusMessage runat="Server" ID = "Message"  Type="Success"/>
    
    <div id="post_form_container" class="FormBlock abc">
    
        <asp:Literal runat="Server" ID = "FormRegion" />
    
    </div>
    
<div class="submit">
    <div id="buttons">
        <asp:Button ID="Publish_Button" runat="Server" Style="font-weight:bold" Text = "Update" OnClick="EditWidget_Click" TabIndex="81" />
        <asp:HyperLink ID ="Cancel_Edit" Text = "(Cancel)" NavigateUrl="~/graffiti-admin/presentation/widgets/" runat="Server" />
    </div>
</div>    
    
 </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
    <div id="sidebar"><div class="gutter">
    <div class="box">
        <h3>About This Page</h3>
        <p>The editing experience is managed by each individual widget, so it may vary. For widget specific questions, please consult the widget author.</p>
    </div>
    <div class="box">
        <h3>Widget Editing Tip</h3>
        <p>Some widgets may not require any editing.</p>
    </div>
    <div style="clear: both;"></div>
    </div></div>
</asp:Content>

