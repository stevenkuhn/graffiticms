<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminModal.master" AutoEventWireup="true" CodeBehind="CatalogItem.aspx.cs" Inherits="graffiti_admin_marketplace_CatalogItem" Title="Graffiti Marketplace" %>
<asp:Content ContentPlaceHolderID="BodyRegion" Runat="Server">

<script language="javascript">
function downloadItem()
{
    window.location.href = '<%= Item.DownloadUrl %>';
}

function buyItem()
{
    window.open('<%= Item.Purchase.BuyUrl %>');
}
</script>

<div id="messages_form">

    <Z:Breadcrumbs runat="server" ID="PageBreadcrumbs" />
    <Z:StatusMessage runat="Server" ID="Message" Type="Success"/>

    <div id="post_form_container" class="FormBlock abc" style="min-height:260px;min-width:550px;">

        <div id="mp_item">
            <div class="pic">
                <img alt="<%= Item.Name %>" border="0" src="<%= Item.ScreenshotUrl %>" height="180" width="240" />
            </div>
            <div class="price"><strong><%= Item.Purchase.FormattedPrice %></strong></div>
        </div>

        <div>
            <p>
                <strong><%= Item.Name %></strong><br />
                <span style="font-size: smaller">Added on <%= Item.DateAdded.ToShortDateString() %></span><br />
            </p>
            <p><%= Item.Description %></p>
            <asp:PlaceHolder ID="AdditionalConfiguration" runat="server">
                <p style="font-size: smaller"><strong>Note:</strong> This item requires additional configuration outlined in the included readme.txt file.</p>
            </asp:PlaceHolder>
        </div>

        <div style="clear:both;"></div>

        <div>
            <p id="info1">
                <strong><%= _itemTypeName %> Info</strong><br />
                Tags: <strong><%= Item.Tags %></strong><br />
                Version: <strong><%= Util.Truncate(Item.Version, 25) %></strong><br />
                Size: <strong><%= Item.FormattedSize %></strong><br />
                Minimum Version: <strong>Graffiti <%= Item.WorksWithMajorVersion %>.<%= Item.WorksWithMinorVersion %></strong>
            </p>
            <p id="info2">
                <strong>Creator Info</strong><br />
                Name: <a href="Catalog.aspx?catalog=<%= Request.QueryString["catalog"] %>&amp;creator=<%= HttpUtility.UrlEncode(Item.Creator.Id) %>"><strong><%= Item.Creator.Name %></strong></a><br />
                Url: <a href="<%= Item.Creator.Url %>" title="<%= Item.Creator.Url %>" target="_blank"><strong><%= Util.Truncate(Item.Creator.Url, 25) %></strong></a><br />
                <asp:PlaceHolder ID="Email" runat="server">Email: <a href="mailto:<%= Item.Creator.Email %>?subject=<%= Item.Name %>" title="<%= Item.Creator.Email %>"><strong><%= Util.Truncate(Item.Creator.Email, 25) %></strong></a><br /></asp:PlaceHolder>
                Bio: <%= Util.Truncate(Item.Creator.Bio, 140) %><br />
            </p>
            <asp:PlaceHolder ID="Statistics" runat="server">
                <p id="info3">
                    <strong>Statistics</strong><br />
                    <asp:PlaceHolder ID="Views" runat="server">Views: <strong><%= Item.Statistics.ViewCount %></strong><br /></asp:PlaceHolder>
                    <asp:PlaceHolder ID="Downloads" runat="server">Downloads: <strong><%= Item.Statistics.DownloadCount %></strong><br /></asp:PlaceHolder>
                </p>
            </asp:PlaceHolder>
        </div>

        <div style="clear:both;"></div>
    </div>

    <div class="submit">
        <div id="buttons">
            <asp:Button ID="InstallButton" runat="server" Text="Install <%= _itemTypeName %>" OnClick="Install_Click" />
            <asp:Button ID="DownloadButton" runat="server" Text="Download <%= _itemTypeName %>" OnClientClick="downloadItem(); return false;" />
            <asp:Button ID="BuyButton" runat="server" Text="Buy <%= _itemTypeName %>" OnClientClick="buyItem(); return false;" />
            <asp:HyperLink ID="CancelButton" runat="server" Text="(Cancel)" NavigateUrl="Catalog.aspx" />
        </div>
    </div>

</div>

</asp:Content>
