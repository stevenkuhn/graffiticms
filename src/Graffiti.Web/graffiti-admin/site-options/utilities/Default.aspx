<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" Title="Graffiti Utilities" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">
<h1>Your Site Utilities</h1>
<Z:Breadcrumbs runat="server" SectionName="Utilities" />
<h3>What would you like to change?</h3>
    
<div class="listboxes">
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='RebuildPages.aspx';">
            <div class="title">
                Rebuild Pages
            </div>
            <div class="body" style="height: 175px; padding: 10px;">
                This utility will rebuild your Category, Post, and Tag pages. This is sometimes needed when you move from one server to another (or deploy a local site
                to a live server).
            </div>
            <div class="commands"></div>
        </div>
    </div>
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='logviewer.aspx';">
        <div class="title">
            Logs
        </div>
        <div class="body" style="height: 175px; padding: 10px;">
            Graffiti records information on things that happen behind the scenes for you. You can use this information to help diagnose potential site issues.
        </div>
        <div class="commands"></div>
        </div>
    </div>
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='FileBrowser.aspx';">
        <div class="title">
            File Browser
        </div>
        <div class="body" style="height: 175px; padding: 10px;">
            Allows you to browse/upload/modify files on your server.
        </div>
        <div class="commands"></div>
        </a>
        </div>
    </div>
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='migrator/';">
        <div class="title">
            Migrator
        </div>
        <div class="body" style="height: 175px; padding: 10px;">
            Allows you to import posts and comments from other platforms.
        </div>
        <div class="commands"></div>
        </a>
        </div>
    </div> 
</div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
</asp:Content>

<script runat="Server">

void Page_Load(object sender, EventArgs e)
{
    LiHyperLink.SetNameToCompare(Context, "settings");
}

</script>