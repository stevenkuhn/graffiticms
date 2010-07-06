<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminModal.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="graffiti_admin_marketplace_Default" Title="Graffiti Marketplace" %>
<asp:Content ContentPlaceHolderID="BodyRegion" Runat="Server">
<div id = "messages_form">

    <Z:StatusMessage runat="Server" ID="Message" Visible="False"/>

    <div id="post_form_container" class="FormBlock abc" style="min-height:300px;min-width:550px;">

        <img id="MarketplaceImage" runat="server" src="../common/img/marketplace.gif" visible="false" style="padding: 30px;" />

        <div id="mp_categories">
            <asp:Repeater ID="catalogList" runat="server">
                <HeaderTemplate>
                    <ul>
                        <li class="first">Catalogs</li>
                </HeaderTemplate>
                <ItemTemplate>
                    <li>
                        <a href="catalog.aspx?catalog=<%# Eval("Type") %>" title="<%# Eval("Description") %>">
                            <%# Eval("Name") %>
                        </a>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
        </div>

        <div style="clear:both;"></div>
    </div>

</div>

</asp:Content>
