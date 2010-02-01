<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" Title="Graffiti Site Options" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">
<h1>Your Site Options</h1>

<h3>What would you like to change?</h3>
    
<div class="listboxes">
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='settings/';">
        <div class="title">
            Settings
        </div>
        <div class="body" style="height: 175px; padding: 10px;">
            Site settings are generic items you can configure on your site. You can change your site's Title/TagLine and add additional rendering options.
        </div>
        <div class="commands"></div>
        </div>
    </div>
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='configuration/';">
        <div class="title">
            Configuration
        </div>
        <div class="body" style="height: 175px; padding: 10px;">
            This section allows you to configure how your site works. You can specify settings such as using WWW in urls, SSL for security pages, and configuring a proxy server.
        </div>
        <div class="commands"></div>
        </div>
    </div>
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='comments/';">
        <div class="title">
            Comments
        </div>
        <div class="body" style="height: 175px; padding: 10px;">
            This section allows you to configure settings for comments posted to your site. You can specify how many days users can comment on posts and add Akismet to prevent spam.
        </div>
        <div class="commands"></div>
        </div>
    </div>
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='custom-fields/';">
        <div class="title">
            Custom Fields
        </div>
        <div class="body" style="height: 175px; padding: 10px;">
            Custom fields allow you to store additional information about your post and are available when creating or editing a post.
        </div>
        <div class="commands"></div>
        </div>
    </div>
    
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='HomeSort/';">
        <div class="title">
            Home Page
        </div>
        <div class="body" style="height: 175px; padding: 10px;">
            Enables overriding which posts are displayed on your site's home page and the sort order of the posts. 
        </div>
        <div class="commands"></div>
        </div>
    </div>    
    
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='plug-ins/';">
        <div class="title">
            Plug-Ins
        </div>
        <div class="body" style="height: 175px; padding: 10px;">
            Plug-Ins are configurable extensions to Graffiti.
        </div>
        <div class="commands"></div>
        </div>
    </div>
    
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='packages/';">
        <div class="title">
            Packages
        </div>
        <div class="body" style="height: 175px; padding: 10px;">
            Packages are collections of Widgets, Themes, Plug-ins, Chalk Extensions and other Graffiti files.
        </div>
        <div class="commands"></div>
        </div>
    </div>
    
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='email-settings/';">
        <div class="title">
            Email Settings
        </div>
        <div class="body" style="height: 175px; padding: 10px;">
            Email Settings are where you configure the SMTP settings to send outbound emails from Graffiti.
        </div>
        <div class="commands"></div>
        </div>
    </div>
    
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='utilities/';">
        <div class="title">
            Utilities
        </div>
        <div class="body" style="height: 175px; padding: 10px;">
            Utilities are tools and information which can help you manage your site better.
        </div>
        <div class="commands"></div>
        </div>
    </div>    

    
    <div style="clear: both;"></div>
</div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
</asp:Content>


