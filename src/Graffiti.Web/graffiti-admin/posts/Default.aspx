<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_posts_Default" Title="Graffiti Posts" Codebehind="Default.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<script type="text/javascript">

var tab = '<%= Request.QueryString["status"] %>';
var category = '<%= Request.QueryString["category"] %>';
var author = '<%= Request.QueryString["author"] %>';

function categoryselect(category)
{
    if(tab == '')
        tab = '1';
    
    var url = '?status=' + tab;

    if(category != '0')
        url += '&category=' + category
        
    if(author != '')
        url += "&author=" + author;
        
    window.location = url;
}

function tabselect(tab)
{
    var url = '?status=' + tab;
    
    if(category != '')
        url += '&category=' + category
        
    if(author != '')
        url += "&author=" + author;
        
    window.location = url;
}

function authorselect(author)
{
    if(tab == '')
        tab = '1';
    
    var url = '?status=' + tab;
    
    if(author != '0')
        url += '&author=' + author;
        
    if(category != '')
        url += "&category=" + category;
        
    window.location = url;
}

</script>

<h1><asp:Label ID="lblPageTitle" runat="server" /></h1>

<div style="margin-top: 20px;">
    <asp:MultiView runat="Server" ID ="PostsLinks" ActiveViewIndex="0">
        <asp:View runat="Server" ID="Published">
            <div class="TabPaneTabSet" style="overflow:auto;">
                <div class="TabPaneTabSelected" style="float:left;">Published</div>
                <div class="TabPaneTab" style="float:left;"><a href="javascript:tabselect('2')">Draft<asp:Literal ID="Draft1" runat="server" /></a></div>
                <div class="TabPaneTab" style="float:left;" id="PendingReviewArea1" runat="server"><a href="javascript:tabselect('3')">Pending Review<asp:Literal ID="PendingReview1" runat="server" /></a></div>
                <div class="TabPaneTab" style="float:left;" id="RequiresChangesArea1" runat="server"><a href="javascript:tabselect('4')">Requires Changes<asp:Literal ID="RequiresChanges1" runat="server" /></a></div>
                <div class="TabPaneTab" style="float:left;"><a href="javascript:tabselect('-1')">Deleted</a></div>
            </div>
        </asp:View>
        <asp:View runat="Server" ID="Draft">
            <div class="TabPaneTabSet" style="overflow:auto;">
                <div class="TabPaneTab" style="float:left;"><a href="javascript:tabselect('1')">Published</a></div>
                <div class="TabPaneTabSelected" style="float:left;">Draft<asp:Literal ID="Draft2" runat="server" /></div>
                <div class="TabPaneTab" style="float:left;" id="PendingReviewArea2" runat="server"><a href="javascript:tabselect('3')">Pending Review<asp:Literal ID="PendingReview2" runat="server" /></a></div>
                <div class="TabPaneTab" style="float:left;" id="RequiresChangesArea2" runat="server"><a href="javascript:tabselect('4')">Requires Changes<asp:Literal ID="RequiresChanges2" runat="server" /></a></div>
                <div class="TabPaneTab" style="float:left;"><a href="javascript:tabselect('-1')">Deleted</a></div>
            </div>
        </asp:View>
        <asp:View runat="Server" ID="PendingReview">
            <div class="TabPaneTabSet" style="overflow:auto;">
                <div class="TabPaneTab" style="float:left;"><a href="javascript:tabselect('1')">Published</a></div>
                <div class="TabPaneTab" style="float:left;"><a href="javascript:tabselect('2')">Draft<asp:Literal ID="Draft3" runat="server" /></a></div>
                <div class="TabPaneTabSelected" style="float:left;" id="PendingReviewArea3" runat="server">Pending Review<asp:Literal ID="PendingReview3" runat="server" /></div>
                <div class="TabPaneTab" style="float:left;" id="RequiresChangesArea3" runat="server"><a href="javascript:tabselect('4')">Requires Changes<asp:Literal ID="RequiresChanges3" runat="server" /></a></div>
                <div class="TabPaneTab" style="float:left;"><a href="javascript:tabselect('-1')">Deleted</a></div>
            </div>
        </asp:View>
        <asp:View runat="Server" ID="RequiresChanges">
            <div class="TabPaneTabSet" style="overflow:auto;">
                <div class="TabPaneTab" style="float:left;"><a href="javascript:tabselect('1')">Published</a></div>
                <div class="TabPaneTab" style="float:left;"><a href="javascript:tabselect('2')">Draft<asp:Literal ID="Draft4" runat="server" /></a></div>
                <div class="TabPaneTab" style="float:left;" id="PendingReviewArea4" runat="server"><a href="javascript:tabselect('3')">Pending Review<asp:Literal ID="PendingReview4" runat="server" /></a></div>
                <div class="TabPaneTabSelected" style="float:left;" id="RequiresChangesArea4" runat="server">Requires Changes<asp:Literal ID="RequiresChanges4" runat="server" /></div>
                <div class="TabPaneTab" style="float:left;"><a href="javascript:tabselect('-1')">Deleted</a></div>
            </div>
        </asp:View>
        <asp:View runat="Server" ID="Deleted">
            <div class="TabPaneTabSet" style="overflow:auto;">
                <div class="TabPaneTab" style="float:left;"><a href="javascript:tabselect('1')">Published</a></div>
                <div class="TabPaneTab" style="float:left;"><a href="javascript:tabselect('2')">Draft<asp:Literal ID="Draft5" runat="server" /></a></div>
                <div class="TabPaneTab" style="float:left;" id="PendingReviewArea5" runat="server"><a href="javascript:tabselect('3')">Pending Review<asp:Literal ID="PendingReview5" runat="server" /></a></div>
                <div class="TabPaneTab" style="float:left;" id="RequiresChangesArea5" runat="server"><a href="javascript:tabselect('4')">Requires Changes<asp:Literal ID="RequiresChanges5" runat="server" /></a></div>
                <div class="TabPaneTabSelected" style="float:left;">Deleted</div>
            </div>
        </asp:View>
    </asp:MultiView>
</div>

<div style="padding: 10px 0px 5px 0;" runat="Server">

    <div id="comment_status" class="infomessage" style="display: none;"></div>   
    <Z:StatusMessage runat="Server" ID = "PostUpdateStatus" />

    <Z:Repeater ShowHeaderFooterOnNone="false" ID="PostList" runat="Server" OnItemCreated="PostList_OnItemCreated">
    <NoneTemplate>
        <z:StatusMessage ID = "NoneMessage" runat="Server" Text="" Type="Warning" />
    </NoneTemplate>
    <HeaderTemplate>
        <table id="postList">
    </HeaderTemplate>
    <ItemTemplate>
        <tr id="post-<%# Eval("Id") %>"<%# IsAltRow( Container.ItemIndex, ((int)Eval("Id")) ) %>>
            <td style="padding: 4px; padding-top: 7px; padding-bottom: 7px;">
                <a style="font-size: 185%;" href="write/?id=<%# Eval("Id") %><% if (Request.QueryString["status"] != null) { %>&status=<%=Request.QueryString["status"]%><% } if (Request.QueryString["category"] != null) { %>&category=<%=Request.QueryString["category"]%><% } if (Request.QueryString["author"] != null) { %>&author=<%=Request.QueryString["author"]%><% } %>"><%# Eval("Title") %></a><br />
                <span style="line-height: 18px;">By <strong><%# Eval("User.ProperName")%></strong> on <%# Eval("Published", "{0:ddd MMM d, yyyy h:mm tt}")%>, revision: <%# Eval("Version") %></span><br />
                <span style="line-height: 13px;">Category: <%# GetCategoryLink(Eval("Category.Name").ToString(), Eval("Category.Url").ToString()) %></span><br />
            </td>
            <td style="<%= ReportStyle %>">
                <span style="line-height: 13px;"><a href="<%= ResolveUrl("~/") %>graffiti-admin/reporting/views/post/?id=<%# Eval("Id") %>">Reports</a></span>
            </td>
            <td width="200px" style="text-align: center; vertical-align: middle;">
                <asp:Literal ID="CommentCounts" runat="server" />
            </td>
            <td width="125px" style="text-align: center; vertical-align: middle;">
                <% if (Request.QueryString["status"] != "-1") {%>
                    <a href="<%# Eval("Url") %>" style="font-size: 120%;">View</a>
                <% } %>
                <asp:placeholder ID="delete" runat="server">
                    <% if(Request.QueryString["status"] != "-1") {%> 
                    | <a style="font-size: 120%;" href="javascript:void(0);" onclick="Posts.deletePost('<%=new Urls().AdminAjax%>', <%# Eval("Id") %>,'post-<%# Eval("Id") %>'); return false;">Delete</a> 
                   <% } else { %>
                    <a style="font-size: 120%;" href="javascript:void(0);" onclick="Posts.unDeletePost('<%=new Urls().AdminAjax%>', <%# Eval("Id") %>,'post-<%# Eval("Id") %>'); return false;">Un-Delete</a> 
                    | <a style="font-size: 120%;" href="javascript:void(0);" onclick="Posts.permanentDeletePost('<%=new Urls().AdminAjax%>', <%# Eval("Id") %>,'post-<%# Eval("Id") %>'); return false;">Destroy</a> 
                   <% }  %>
               </asp:placeholder>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
    </Z:Repeater>

    <asp:Literal runat="Server" ID = "Pager" />

</div>
</asp:Content>

<asp:Content ContentPlaceHolderID="SideBarRegion" Runat="Server">
<div id="sidebar"><div class="gutter">
<div class="box">
    <h3>Categories</h3>
    <Z:Repeater ShowHeaderFooterOnNone="false" ID="rptCategories" runat="Server" OnItemDataBound="rptCategories_ItemDataBound">
        <HeaderTemplate>
            <ul class="categorylist" style="margin: 5px 0 5px 0; font-size: 110%;">
            <li<%# IsSelectedCategory("0") %> style="margin-left: -7px; margin-right: -7px;">
                <a href="javascript:categoryselect('0')">All Categories</a>
            </li>
        </HeaderTemplate>
        <ItemTemplate>
            <li<%# IsSelectedCategory(Eval("ID").ToString()) %> style="font-size: 110%; margin-left: -7px; margin-right: -7px; margin-top: 2px;">
                <asp:hyperlink id="parentCategory" runat="server" />
            </li>
                <Z:Repeater ID="rptCategoriesNested" runat="server">
                    <ItemTemplate>
                        <li<%# IsSelectedCategory(Eval("ID").ToString()) %>>
                        <div style="position: absolute; margin-left: -5px;">
                            <asp:image id="TreeImage" runat="server" Height="22px" Width="11px" />
                        </div>
                        <a style="padding-left: 10px;" href="javascript:categoryselect('<%# Eval("ID") %>')"><%# Eval("Name") %> (<%# Eval("Count") %>)</a>
                        </li>
                    </ItemTemplate>
                </Z:Repeater>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </Z:Repeater>
</div>

<div class="box">
    <h3>Authors</h3>
    <Z:Repeater ShowHeaderFooterOnNone="false" ID="rptAuthors" runat="Server">
        <HeaderTemplate>
            <ul class="categorylist" style="margin: 5px 0 5px 0; font-size: 110%;">
            <li<%# IsSelectedAuthor("0") %> style="margin-left: -7px; margin-right: -7px;">
                <a href="javascript:authorselect('0')">All Authors</a>
            </li>
        </HeaderTemplate>
        <ItemTemplate>
            <li<%# IsSelectedAuthor(Eval("ID").ToString()) %> style="font-size: 110%; margin-left: -7px; margin-right: -7px; margin-top: 2px;">
                <a href="javascript:authorselect('<%# Eval("ID") %>')"><%# Eval("Name") %> (<%# Eval("Count") %>)</a>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </Z:Repeater>
</div>

<div style="clear: both;"></div>
</div></div>
</asp:Content>

