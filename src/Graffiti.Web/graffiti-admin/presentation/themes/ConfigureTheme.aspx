<%@ Page Language="C#"MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" CodeBehind="ConfigureTheme.aspx.cs" Inherits="Graffiti.Web.graffiti_admin.presentation.themes.ConfigureTheme" %>

<asp:Content ContentPlaceHolderID="MainRegion" runat="server">

    <h1>Configure Theme</h1>
    <Z:Breadcrumbs runat="server" SectionName="ConfigureTheme" />
    
    <div id="messages_form">
        <asp:PlaceHolder runat="Server" ID="ConfigWrapper">
            <div id="post_form_container" class="FormBlock">
                <CSDynConfig:ConfigurationForm runat="server" id="ConfigurationForm"
                    RenderGroupsInTabs="true" 
                    PanesCssClass="TabPane"
	                TabSetCssClass="TabPaneTabSet"
	                TabCssClasses="TabPaneTab"
	                TabSelectedCssClasses="TabPaneTabSelected"
	                TabHoverCssClasses="TabPaneTabHover"
	                >
	                <PropertyFormGroupHeaderTemplate>
	                    <Z:StatusMessage runat="Server" ID="Message" />
	                    <div style="margin-top: 1em;"><CSDynConfig:PropertyGroupData Property="Description" runat="server" /></div>
	                </PropertyFormGroupHeaderTemplate>
	                <PropertyFormSubGroupHeaderTemplate>
	                    <h2 style="font-size: 120%; font-weight: bold; margin-top: 2em;"><CSDynConfig:PropertySubGroupData Property="Name" runat="server" /></h2>
	                    <CSDynConfig:PropertySubGroupData Property="Description" runat="server" />
	                </PropertyFormSubGroupHeaderTemplate>
	                <PropertyFormPropertyTemplate>
	                    <h2><CSDynConfig:PropertyData Property="Name" runat="server" />: <span class="form_tip"><CSDynConfig:PropertyData Property="Description" runat="server" /></span></h2>
                        <CSDynConfig:PropertyControl runat="server" />
	                </PropertyFormPropertyTemplate>
                </CSDynConfig:ConfigurationForm>
                
                <div class="submit">
                    <asp:Button ID="Save" Text="Save" runat="server" OnClick="Save_Click" />
                    <a href="Default.aspx">(Cancel)</a>
                    <asp:LinkButton ID="RestoreDefaults" Text="(Restore Defaults)" runat="server" OnClick="RestoreDefaults_Click" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="Server" ID="NoConfigWrapper">
            <Z:StatusMessage runat="server" ID="Message2" />
        </asp:PlaceHolder>
    </div>

</asp:Content>