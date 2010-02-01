<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_posts_HomePostSortOrder" Title="Manage Sort Order" Codebehind="default.aspx.cs" %>
<%@ Register TagPrefix="Z" Assembly="Graffiti.Core" Namespace="Graffiti.Core" %>
<%@ Register TagPrefix="Glow" Assembly="Telligent.Glow" Namespace="Telligent.Glow" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<script type="text/javascript">

function saveNavigationUpdates()
{
    pre_update();

    var idArray = new Array();
    var lbarClientID = <%= Posts.ClientID %>;
    for (var i = 0; i < lbarClientID.GetItemCount(); i++)
    {
        idArray[idArray.length] = lbarClientID.GetItemAtIndex(i).Value;
    }

    $.ajax(
    {
        type:'POST',
        dataType:'text',
        url:'<%= new Urls().AdminAjax %>?command=reOrderHomePosts',
        data:  {posts:idArray.join('&')},
        success: function(transport)
        {
            var response = transport || "no response text"; 
            if(response == "Success")
            {
                post_update();
            }
            else
            {
                alert(response);
                post_reset();
            }
        },
        error: function(){ alert('Something went wrong...'); post_reset(); }
    });        
}

function saveHomeSortStatus(isChecked)
{
    pre_update();

    $.ajax(
    {
        type:'POST',
        dataType:'text',
        url:'<%= new Urls().AdminAjax %>?command=saveHomeSortStatus',
        data:  {ic:isChecked},
        success: function(transport)
        {
            var response = transport || "no response text"; 
            if(response == "Success")
            {
                post_update();
            }
            else
            {
                alert(response);
                post_reset();
            }
        },
        error: function(){ alert('Something went wrong...'); post_reset(); }
    });        
}

function pre_update()
{
     $('status_update').show();
     $('status_desc').hide();
}

function post_update()
{
     $$('status_time').innerHTML = new Date().toLocaleString();
     $('status_desc').show();
     $('status_update').hide();
}

function post_reset()
{
    $('status_update').hide();
}

</script>

   <h1><asp:Literal ID = "PageTitle" runat="Server" Text="Order Posts" /></h1> 
    
    <Z:Breadcrumbs runat="server" SectionName="SortHomePosts" />
    <Z:StatusMessage runat="Server" ID = "Message"  Type="Success" />
     
    <div class="FormBlock">
    
        <h2><input type="checkbox" id="home_sort_setting" <%= SiteSettings.Get().UseCustomHomeList ? "checked = \"checked\"" : "" %> onclick="saveHomeSortStatus(this.checked);" /> <label>Enable Home Page Overrides</label> <br /><span class="form_tip">When enabled, the default list of posts on your site's home page will be ordered by the list below. To add new posts to the list, click &quot;Include on Home Page&quot; on the settings tab when writing or editing a post.</span></h2> 
    
        <h3>Set the Custom Post Sort Order</h3>
        
        <Glow:OrderedList runat="server" Width="400px" Height="300px" OnItemMovedClientFunction="saveNavigationUpdates" EnableDeleteButton="false" EnableMoveDownButton="false" EnableMoveUpButton="false" ID="Posts" />
        
    </div>
    
    <div class="submit">
        <div id="buttons">
            <em>Changes are saved automatically. Just drag and drop, we will do the rest.</em>
            <br />
            <span id="status_update" style="display:none;"><img src="<%= VirtualPathUtility.ToAbsolute("~/graffiti-admin/common/img/spinner.gif") %>" alt="status" /> Saving Changes</span>
            <span id="status_desc" style="display:none;">Last updated at <span id="status_time"></span></span>
        </div>  
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
</asp:Content>
