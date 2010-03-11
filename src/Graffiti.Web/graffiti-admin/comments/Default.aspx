<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_comments_Default" Title="Graffiti Comments" Codebehind="Default.aspx.cs" %>
<%@ Register TagPrefix="Z" Assembly="Graffiti.Core" Namespace="Graffiti.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">
<script language="javascript" type="text/javascript">

function toggleCheckAll()
{
    chkAll = $("#CheckAllComments");
    $('input[type=checkbox]').attr("checked",chkAll.attr("checked"));
}

function toggleCommentBody()
{
    hl = $("#commentBodyToggle");

    if(hl.html() == 'hide comment body')
    {
        hl.html('show comment body');
        $('table[name="CommentBody"]').hide();
    }
    else {
        hl.html('hide comment body');
        $('table[name="CommentBody"]').show();
    }
}

</script>

<h1><asp:Label ID="lblPageTitle" runat="server" /></h1>

<Z:Breadcrumbs runat="server" SectionName="SiteComments" />

<asp:MultiView runat="Server" ID="the_Views" ActiveViewIndex="0">
    <asp:View ID = "the_Comments" runat="Server">
    
        <div style="margin-top: 20px;">
            <asp:MultiView runat="Server" ID ="CommentLinks" ActiveViewIndex="0">
                <asp:View runat="Server" ID = "PublishedComments">
                    <div class="TabPaneTabSet" style="overflow:auto;">
                        <div class="TabPaneTabSelected" style="float:left;">Published</div>
                        <div class="TabPaneTab" style="float:left;"><a href="?a=f<%= IsPostBound() %>">Pending</a></div>
                        <div class="TabPaneTab" style="float:left;"><a href="?a=d<%= IsPostBound() %>">Deleted</a></div>
                    </div>
                </asp:View>
                <asp:View runat="Server" ID = "PendingComments">
                    <div class="TabPaneTabSet" style="overflow:auto;">
                        <div class="TabPaneTab" style="float:left;"><a href="?a=t<%= IsPostBound() %>">Published</a></div>
                        <div class="TabPaneTabSelected" style="float:left;">Pending</div>
                        <div class="TabPaneTab" style="float:left;"><a href="?a=d<%= IsPostBound() %>">Deleted</a></div>
                    </div>
                </asp:View>
                <asp:View runat="Server" ID = "DeletedComments">
                    <div class="TabPaneTabSet" style="overflow:auto;">
                        <div class="TabPaneTab" style="float:left;"><a href="?a=t<%= IsPostBound() %>">Published</a></div>
                        <div class="TabPaneTab" style="float:left;"><a href="?a=f<%= IsPostBound() %>">Pending</a></div>
                        <div class="TabPaneTabSelected" style="float:left;">Deleted</div>
                    </div>
                </asp:View>
            </asp:MultiView>
        </div>

        <div id = "comment_status" class="infomessage" style="display: none;"></div>
        
        <Z:StatusMessage runat="Server" ID = "CommentStatus" />

        <Z:Repeater runat="Server" ShowHeaderFooterOnNone = "false" ID = "CommentList" OnItemCreated="CommentList_OnItemCreated" OnItemCommand="CommentList_OnItemCommand">
            <NoneTemplate>
                <z:StatusMessage runat="Server" Text="Sorry, there are no comments to manage." Type="Warning" />
            </NoneTemplate>
            <HeaderTemplate>
                <table id="postList" style="margin-top: 15px;">
                <tr style="background: #ccc;">
                    <td style="padding: 5px;">
                        <input type="checkbox" id="CheckAllComments" onclick="toggleCheckAll();" />
                    </td>
                    <td style="padding: 5px;">
                        <a id="commentBodyToggle" class="a" onclick="toggleCommentBody();">hide comment body</a> 
                    </td>
                    <td colspan="2" width="380px" style="text-align: right; vertical-align: middle; padding: 5px;">
                        <asp:PlaceHolder id="bulkApprove" runat="server">
                            <asp:button ID="ApproveAllChecked" runat="server" Text="Approve all checked" CommandName="Approve" />
                        </asp:PlaceHolder>
                        <asp:PlaceHolder id="bulkDelete" runat="server">
                            <asp:button ID="DeleteAllChecked" runat="server" Text="Delete all checked" CommandName="Delete" />
                        </asp:PlaceHolder>
                        <asp:PlaceHolder id="bulkUndelete" runat="server">
                            <asp:button ID="UndeleteAllChecked" runat="server" Text="Undelete all checked" CommandName="Undelete" />
                        </asp:PlaceHolder>
                    </td>
                </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr id='comment-<%# Eval("Id") %>' <%# IsAltRow( Container.ItemIndex ) %>>
                    <td style="vertical-align: top; padding-top: 7px;">
                        <asp:HiddenField ID="CommentID" runat="server" />
                        <asp:CheckBox ID="CommentCheckbox" runat="server" />
                    </td>
                    <td style="padding: 4px; padding-top: 7px; padding-bottom: 7px;">
                        <span style="font-size: 130%;">Comment from <strong><%# new Macros().CommentLink(Container.DataItem as Comment) %></strong> - <a href='mailto:<%# Eval("Email") %>'><%# Eval("Email") %></a> - <span style="font-size: 80%;"><a target="_blank" href="http://ws.arin.net/cgi-bin/whois.pl?queryinput=<%# Eval("IPAddress") %>"><%# Eval("IPAddress") %></a></span></span><br />
                        <span style="line-height: 18px;">Post: <a href="<%# Eval("Post.Url") %>"><%# Eval("Post.Name") %></a></span><br />
                        <table name="CommentBody" cellpadding="0" cellspacing="0" style="table-layout: fixed;"><tr><td><span style="line-height: 13px;"><%# Eval("Body") %></span></td></tr></table>
                    </td>
                    <td style="text-align: center; vertical-align: middle;">
                        Received <%# Eval("Published", "{0:MMM d, yyyy h:mm tt}")%>
                    </td>
                    <td style="text-align: center; vertical-align: middle;">
                        <asp:PlaceHolder id="edit" runat="server">
                            <a style="font-size: 120%; <%= IsBold("EditBold") %>" href="?id=<%# Eval("Id") %>">Edit</a>
                        </asp:PlaceHolder>
                        <asp:Label ID="pipe1" runat="server" Text=" | " />
                        <asp:PlaceHolder id="approve" runat="server">
                            <a style="font-size: 120%; <%= IsBold("EditBold") %>" href="javascript:void(0);" onclick="Comments.approve('<%= new Urls().AdminAjax %>', <%# Eval("Id") %>,'comment-<%# Eval("Id") %>'); return false;">Approve</a>
                        </asp:PlaceHolder>
                        <asp:Label ID="pipe2" runat="server" Text=" | " />
                        <asp:PlaceHolder id="delete" runat="server">
                            <a style="font-size: 120%;" href="javascript:void(0);" onclick="Comments.deleteCommentWithStatus('<%= new Urls().AdminAjax %>', <%# Eval("Id") %>,'comment-<%# Eval("Id") %>'); return false;">Delete</a>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder id="undelete" runat="server">
                            <a style="font-size: 120%; font-weight: bold;" href="javascript:void(0);" onclick="Comments.unDelete('<%= new Urls().AdminAjax %>', <%# Eval("Id") %>, true); return false;">Un-Delete</a>
                        </asp:PlaceHolder>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </Z:Repeater>

        <asp:Literal runat="Server" ID = "Pager" />

    </asp:View>
    <asp:View ID = "Comment_Form" runat="Server">

	<div id = "messages_form">
	<div id="category_edit_form" class="FormBlock abc">

	<h2>Name:</h2>
	<asp:TextBox runat="Server" ID="txtName" CssClass="small" />

	<h2>Website:</h2>
	<asp:TextBox runat="Server" ID="txtSite" CssClass="small" />

	<h2>Email:</h2>
	<asp:TextBox runat="Server" ID="txtEmail" CssClass="small" />

	<h2>Comment:</h2>
	<Z:GraffitiEditor runat="server" ID="CommentEditor" Width="600px" ToolbarSet="Simple" TabIndex="3" />

	</div>

	<div class="submit">
		<div id="buttons">
			<asp:Button runat="Server" ID="CommentSave" Text = "Update Comment" OnClick="CommentSave_Click" />
			<asp:HyperLink ID ="Cancel_Edit" Text = "(Cancel)" NavigateUrl="~/graffiti-admin/comments/" runat="Server" />
		</div>
	</div>
</div>
    </asp:View>
</asp:MultiView>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
    <div id="sidebar"><div class="gutter">
    <div class="box">
        <h3>About This Page</h3>
        <p>This page lists all of the comments which have been made to your posts which accept comments. You can at anytime edit or remove (delete) a comment.</p>
        <p>Comments which are likely to be spam are available under the <em><a href="?a=f">Pending Comments</a></em> link. Pending comments older than 2 weeks will automatically be deleted for you.</p>
        <p>If you accidentally delete a comment, you can recover it on the deleted tab. <em><a href="?a=d">Deleted comments</a></em> will be permanently removed after 48 hours.</p>
    </div>
    <div class="box">    
        <h3>Short Cut</h3>
        <p>You can view the published and pending comments to any single post by clicking on the link from the <a href="../posts/">posts page</a>.</p>
    </div>
    <div style="clear: both;"></div>
    </div></div>
</asp:Content>

