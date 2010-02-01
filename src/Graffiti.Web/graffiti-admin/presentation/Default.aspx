<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" Title="Presentation" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<%@ Import Namespace="System.IO" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<h1>Your Site's Presentation</h1>

<h3>What would you like to change?</h3>

<div class="listboxes">
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='themes/';">
        <div class="title">
            Themes
        </div>
        <div class="body" style="height:105px; padding: 10px;">
            Themes allow you to modify the look and feel of your website. You can choose from a list of existing themes, or create your own. 
        </div>
        <div class="commands"></div>
        </div>
    </div>
    <div class="nonnested" style="float: left; width: 200px; margin-right: 15px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='widgets/';">
        <div class="title">
            Widgets
        </div>
        <div class="body" style="height:105px; padding: 10px;">
            Widgets are small pieces of content which exist in the sidebar of your site. There are many widgets to choose from built in to Graffiti.
        </div>
        <div class="commands"></div>
        </div>
    </div>
    <div class="nonnested" style="float: left; width: 200px; margin-bottom: 25px;">
        <div class="boxlink" onmouseover="this.className='boxlink highlight';" onmouseout="this.className='boxlink';" onclick="window.location='navigation/';">
        <div class="title">
            Navigation
        </div>
        <div class="body" style="height:105px; padding: 10px;">
            Navigation allows you to control the links which are displayed on your sites main navigation element. You can add new links or adjust the order of existing categories.
        </div>
        <div class="commands"></div>
        </div>
    </div>
</div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
    <div id="sidebar"><div class="gutter">
    <div class="box">
        <h3>About This Page</h3>
        <p>This page is your starting point for managing how users see and interact with your site. Graffiti provides you with 
        some very helpful tools to customize your site on the fly. However, it is very important to note that some themes may choose
        to not support features like widgets and custom navigation bars.</p>
    </div>
    <div style="clear: both;"></div>
    </div></div>
</asp:Content>