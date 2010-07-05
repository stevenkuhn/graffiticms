<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_presentation_widgets_Default" Title="Manage Widgets" Codebehind="Default.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">
<script type="text/javascript">

function refresh()
{
    window.location.href = window.location.href;
}

function saveWidgetUpdates(list)
{
    updateWidgetOrder(list.id, Sortable.serialize(list));
}

function addWidgetNode(text,id,list)
{
    list.AddItem(new Telligent_OrderedListItem(id, text, parseTemplate($("#WidgetTemplate").html(),{title: text, wid: id})));
    list.Refresh();
    list.SelectItem(list.GetItemCount() - 1, true);
}

function addNewWidget()
{
    pre_update();

    var awClientID = <%= AvailableWidgets.ClientID %>;
    var qbarClientID = <%= qbar.ClientID %>;
    var lbarClientID = <%= lbar.ClientID %>;
    var rbarClientID = <%= rbar.ClientID %>;
    var lbarSelectedIndex = lbarClientID.GetSelectedIndex();
    var rbarSelectedIndex = rbarClientID.GetSelectedIndex();
    var newWidgetUniqueId = awClientID.GetValue();

    $.ajax(
    {
        type:'POST',
        url:'<%= new Urls().AdminAjax %>?command=createdWidget',
        dataType:'text',
        data:  {id:newWidgetUniqueId},
        success: function(transport)
        {
            var response = transport || "no response text";
            if(response.length == 36)
            {
                addWidgetNode(awClientID.GetText(awClientID.GetSelectedIndex()), response, qbarClientID);
                awClientID.SetSelectedIndex(0);
                post_update();
                if(lbarSelectedIndex =! -1)
                  lbarClientID.SelectItem(lbarSelectedIndex);
                if(rbarSelectedIndex =! -1)
                  rbarClientID.SelectItem(rbarSelectedIndex);
            }
            else
            {
                awClientID.SetSelectedIndex(0);
                alert(response);
            }
        },
        error: function(){ alert('Something went wrong...'); awClientID.SetSelectedIndex(0); }
    });
}

function itemAddedOrMoved(listName, list)
{
     pre_update();
     
     var idArray = new Array();
     for (var i = 0; i < list.GetItemCount(); i++)
     {
        idArray[idArray.length] = list.GetItemAtIndex(i).Value;
     }

      $.ajax(
      {
        type:'POST',
        url:'<%= new Urls().AdminAjax %>?command=updateWidgetsOrder',
        dataType:'text',
        data:  {id:listName,list:idArray.join('&')},
        success: function(transport)
        {
             var response = transport || "no response text";             
             post_update();
        },
        error: function(){ alert('Something went wrong...') }
      });
}

function deleteWidget(wid)
{
      if(!confirm('Are you sure you want to delete this widget? This action cannot be undone'))
        return;
        
        pre_update();

      $.ajax(
      {
        type:'POST',
        dataType:'text',
        url:'<%= new Urls().AdminAjax %>?command=deleteWidget',
        data:  {id:wid},
        success: function(transport)
        {
             var response = transport || "no response text";
             var qbarClientID = <%= qbar.ClientID %>;
             var lbarClientID = <%= lbar.ClientID %>;
             var rbarClientID = <%= rbar.ClientID %>;

             doDeleteWidget( qbarClientID, wid );
             doDeleteWidget( lbarClientID, wid );
             doDeleteWidget( rbarClientID, wid );

             post_update();
        },
        error: function(){ alert('Something went wrong...') }
      });
}

function doDeleteWidget(item, wid)
{
	var i;
	for (i = 0; i < item.GetItemCount(); i++)
		if (item.GetItemAtIndex(i).Value == wid)
		{
			item.RemoveItem(item.GetItemAtIndex(i));
			item.Refresh();
			break;
		}
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

</script>

<script id="WidgetTemplate" type="text/html">
<div style="border: solid 1px #999; padding: 4px;">
    <b><#= title #></b>
    <div style="text-align:right;"><a title="Edit Widget" href="javascript:void();" onclick="window.location='edit.aspx?id=<#= wid #>'">Edit</a> | <a title="Delete Widget" href="javascript:void();" onclick="deleteWidget('<#= wid #>'); return false;">Delete</a>
    </div>
</div>
</script>

<h1>Manage Your Widgets</h1> 
<div id = "messages_form">
    
    <Z:Breadcrumbs runat="server" SectionName="Widget" />
    <Z:StatusMessage runat="Server" ID = "Message"  Type="Success"/>
    <Glow:Modal ID="Modal" runat="server" />

    
    <div id="post_form_container" class="FormBlock abc" style="min-height:300px;min-width:690px; margin: -15px 0 0 0;">
        
        <div id="queue-sidebar" style="float:left;width:29%; max-width:210px;border-right: solid 1px #999; padding-right: 2%; margin-right: 2%;">
            <div style="height: 120px; margin-top: 0; padding-top: 10px;">
                <h2 style="font-weight: bold;">
                    Available Widgets<br />
                </h2>
                <div style="color: #666; font-size: 11px; line-height: normal; padding-top: 3px;">Select a widget from the list to configure it.</div>
            
                <div style="padding-top: 8px;">
                    <Glow:DropDownList ID="AvailableWidgets" runat="Server" onchange="addNewWidget();" Width="160px" SelectListWidth="250" ShowHtmlWhenSelected="false" />
                </div>
                <div style="padding-bottom: 8px;">
                    <a href="javascript:Telligent_Modal.Open('<%= new Urls().AdminMarketplace("Widgets") %>', 600, 475, refresh);">Search online widgets...</a>
            </div>

            <Glow:OrderedList ID="qbar" runat="server" Width="80%" Height="300px" OnItemAddedClientFunction="new Function('list', 'itemAddedOrMoved(\'qbar\',list);')" OnItemMovedClientFunction="new Function('list', 'itemAddedOrMoved(\'qbar\',list);')" OnItemRemovedClientFunction="new Function('list', 'itemAddedOrMoved(\'qbar\',list);')" DraggableOrderedListIds="lbar,rbar" EnableDeleteButton="false" EnableMoveDownButton="false" EnableMoveUpButton="false" />
        </div>
        
        <div style="margin-right: 30px; height: 74px;">
            <h2 style="font-weight: bold; padding-top: 10px; margin-top: 0;">
                Live Widgets<br />
            </h2>
            <div style="color: #666; font-size: 11px; line-height: normal; padding-top: 3px;">Below are widgets that are active on your site. You can drag and drop widgets from either sidebar.</div>
        </div>        

        <div id="left-sidebar" style="float:left;width:33%;max-width:230px;">
            <h2 style="height:45px;">Left Sidebar<br /><span class="form_tip">Widgets shown on the left sidebar.</span></h2>
            <Glow:OrderedList ID="lbar" runat="server" Width="80%" Height="300px" OnItemAddedClientFunction="new Function('list', 'itemAddedOrMoved(\'lbar\',list);')" OnItemMovedClientFunction="new Function('list', 'itemAddedOrMoved(\'lbar\',list);')" OnItemRemovedClientFunction="new Function('list', 'itemAddedOrMoved(\'lbar\',list);')" DraggableOrderedListIds="qbar,rbar" EnableDeleteButton="false" EnableMoveDownButton="false" EnableMoveUpButton="false" />
        </div>

        <div id="right-sidebar" style="float:left;width:33%;max-width:230px;">
            <h2 style="height:45px;">Right Sidebar<br /><span class="form_tip">Widgets shown on the right sidebar.</span></h2>
            <Glow:OrderedList ID="rbar" runat="server" Width="80%" Height="300px" OnItemAddedClientFunction="new Function('list', 'itemAddedOrMoved(\'rbar\',list);')" OnItemMovedClientFunction="new Function('list', 'itemAddedOrMoved(\'rbar\',list);')" OnItemRemovedClientFunction="new Function('list', 'itemAddedOrMoved(\'rbar\',list);')" DraggableOrderedListIds="lbar,qbar" EnableDeleteButton="false" EnableMoveDownButton="false" EnableMoveUpButton="false" />
        </div>    

        <div style="clear:both;"></div>
    </div>
    
    <div class="submit">
    <div id="buttons">
        <em>Changes are saved automatically. Just drag and drop, and we'll do the rest.</em>
        <br />
        <span id="status_update" style="display:none;"><img src="<%= VirtualPathUtility.ToAbsolute("~/graffiti-admin/common/img/spinner.gif") %>" alt="status" /> Saving Changes</span>
        <span id="status_desc" style="display:none;">Last updated at <span id="status_time"></span></span>
    </div>
</div>     
    
</div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
    <div id="sidebar"><div class="gutter">
    <div class="box">
	    <h3>About This Page</h3>
	    <p>Widgets are small pieces of content which exist in the sidebar of your site. Using this functionality you can add, remove, and rearrange each of the items.</p>
	    <p>While this is a powerful feature, not every site will implement it. In addition, there is no guarantee a site theme will implement either sidebar.</p>
	    <p><b>Note: </b>Graffiti will attempt to auto detect if the current theme uses this feature and if one or more sidebars exist. This feature has not yet been implemented.</p>
    </div>
    <div style="clear: both;"></div>
    </div></div>
</asp:Content>

