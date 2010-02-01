<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" CodeBehind="logviewer.aspx.cs" Inherits="Graffiti.Web.graffiti_admin.site_options.utilities.logviewer" Title="Graffiti Log Viewer" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<h1>Your Event Logs</h1>
<Z:Breadcrumbs runat="server" SectionName="Logs" />
<div id = "messages_form">
    
    <div id="post_form_container" class="FormBlock">
        
    <div style="margin-top: 20px;">
    
    <asp:MultiView runat="Server" ID ="LogViews" ActiveViewIndex="0">
        <asp:View runat="Server" ID="InfoView">    
           <div class="TabPaneTabSet" style="overflow:auto;">
                <div class="TabPaneTabSelected" style="float:left;">Information</div>
                <div class="TabPaneTab" style="float:left;"><a href="?type=2">Warning</a></div>
                <div class="TabPaneTab" style="float:left;"><a href="?type=3">Error</a></div>
            </div>
    </asp:View>

        <asp:View runat="Server" ID="WarningView">    
           <div class="TabPaneTabSet" style="overflow:auto;">
                <div class="TabPaneTab" style="float:left;"><a href="logviewer.aspx">Information</a></div>
                <div class="TabPaneTabSelected" style="float:left;">Warning</div>
                <div class="TabPaneTab" style="float:left;"><a href="?type=3">Error</a></div>
            </div>
    </asp:View>
    
            <asp:View runat="Server" ID="ErrorView">    
           <div class="TabPaneTabSet" style="overflow:auto;">
                <div class="TabPaneTab" style="float:left;"><a href="logviewer.aspx">Information</a></div>
                <div class="TabPaneTab" style="float:left;"><a href="?type=2">Warning</a></div>
                <div class="TabPaneTabSelected" style="float:left;">Error</div>
            </div>
    </asp:View>
    
    </asp:MultiView>
    
    
    
    </div>
    
    <Z:Repeater runat="Server" ID = "LogList">
        <NoneTemplate>
        <z:StatusMessage ID="StatusMessage1" runat="Server" Text="There are no log entries matching your request." Type="Warning" />
    </NoneTemplate>
    <HeaderTemplate>
        <table id="postList">
        <thead>
            <tr align="left">
                <th>Title</th>
                <th width="150px" style="text-align: left;">When</th>
            </tr>
        </thead>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td>
                <b><%# Server.HtmlEncode(Eval("Title")  as string)%></b><br />
                <%# Util.ConvertTextToHTML(Eval("Message") as string)%>
            </td>
            <td>
                <%# Eval("CreatedOn")%>
            </td>
        </tr>
    
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="alt">
            <td>
                <b><%# Server.HtmlEncode(Eval("Title")  as string)%></b><br />
                <%# Util.ConvertTextToHTML(Eval("Message") as string)%>
            </td>
            <td>
                <%# Eval("CreatedOn")%>
            </td>
        </tr>
    
    </AlternatingItemTemplate>    
    <FooterTemplate>
        </table>
    </FooterTemplate>
    
    </Z:Repeater>
    <asp:Literal runat="Server" ID = "pager" />
    </div>
</div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="SideBarRegion" Runat="Server">
<div id="sidebar"><div class="gutter">
<div class="box">
    <h3>About this Page</h3>
    <p>Graffiti records (logs) information about things it does for you behind the scenes. Most of the time you can just ignore this information, but in 
    the event you are trying to diagnose a problem, the logs will likely be a handy tool. 
    </p>
    <p>
    There are three types of logs:
    </p>
   
    <ol>
        <li><em>Information</em>: Nice to know information and status updates</li>
        <li><em>Warning</em>: Something did not go according to plan, but generally there is nothing to worry about</li>
        <li><em>Error</em>: Graffiti encountered a problem which may be affecting the site or a particular feature</li>
    </ol>
    <p>
    By default, Graffiti will automatically delete log entries that are older than a couple of days.
    </p>
</div>
<div style="clear: both;"></div>
</div></div>
</asp:Content>
