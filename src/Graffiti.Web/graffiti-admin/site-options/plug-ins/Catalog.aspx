<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminModal.master" AutoEventWireup="true" CodeBehind="Catalog.aspx.cs" Inherits="graffiti_admin_presentation_plugins_Catalog" Title="Plugin Marketplace" %>
<asp:Content ContentPlaceHolderID="BodyRegion" Runat="Server">
<div id = "messages_form">

    <Z:Breadcrumbs runat="server" ID="breadcrumbs" SectionName="PluginMarketplace" />
    <Z:StatusMessage runat="Server" ID="Message" Visible="False"/>
    <Z:StatusMessage runat="server" ID="MarketplaceMessage" />

    <div id="post_form_container" class="FormBlock abc" style="min-height:300px;min-width:550px;">

        <img id="MarketplaceImage" runat="server" src="../../common/img/marketplace.gif" visible="false" style="padding: 30px;" />

        <div id="mp_categories">
            <asp:Repeater ID="categoryList" runat="server">
                <HeaderTemplate>
                    <ul>
                        <li class="first">Categories</li>
                </HeaderTemplate>
                <ItemTemplate>
                    <li>
                        <a href="?category=<%# Eval("Id") %>" title="<%# Eval("Description") %>">
                            <%# Eval("Name") %>
                        </a>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
        </div>

        <div>
            <asp:DataList ID="itemList" runat="server" RepeatDirection="Horizontal" RepeatColumns="3">
                <ItemTemplate>
                    <div id="mp_itemlist">
                        <a href="CatalogItem.aspx?item=<%# Eval("Id") %>" title="<%# Eval("Description") %>">
                            <img alt="<%# Eval("Description") %>" border="0" src="<%# Eval("IconUrl") %>" height="90" width="120" />
                            <div><%# Util.Truncate(Eval("Name").ToString(), 15)%></div>
                        </a>
                    </div>
                </ItemTemplate>
            </asp:DataList>
        </div>


        <div style="clear:both;"></div>
    </div>

</div>

</asp:Content>
