<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_presentation_navigation_Default" Title="Graffiti - Smart Navigation" Codebehind="Default.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<script type="text/javascript">

$(document).ready(function()
{
   setupNavigation();
});

function setupNavigation()
{
    var cl = $$('<%=the_Categories.ClientID %>');
    
    if(cl.options.length == 0)
    {
        var option = new Option();
        option.value = -1;
        option.text = '--No Categories--';        
        cl.options[0] = option;
        
        $('AddCategoryBTN').disabled = true;
    }
    
    var pl = $$('<%=the_Posts.ClientID %>');
    
    if(pl.options.length == 0)
    {
        var option = new Option();
        option.value = -1;
        option.text = '--No Posts--';        
        pl.options[0] = option;
        
        $('AddPostBTN').disabled = true;
    }    
}

function saveNavigationUpdates()
{
    var idArray = new Array();
    var lbarClientID = <%= lbar.ClientID %>;
     for (var i = 0; i < lbarClientID.GetItemCount(); i++)
     {
        idArray[idArray.length] = <%= lbar.ClientID %>.GetItemAtIndex(i).Value;
     }

      $.ajax(
      {
        type:'POST',
        url:'<%= new Urls().AdminAjax %>?command=reOrderNavigation',
        dataType:'text',
        data:  {navItems:idArray.join('&')},
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
             }
        },
        error: function(){ alert('Something went wrong...') }
      });        
}

function reAddItem(list,name,id, btn)
{
    var the_List = $$(list);
    
    if(the_List.options.length == 1)
    {
        if(the_List.options[0].text.startsWith('--'))
        {
            the_List.options[0] = null;
        }
    }
    
    var option = new Option();
    option.text = name;
    option.value = id;
    
    the_List.options[the_List.options.length] = option;
    
    $(btn).disabled = false;
}

function remove_Link(type, name, id)
{
    if(!confirm('Are you sure you want to delete this navigation link?'))
        return;

    removeItem(type, id);
    
    if(type == 'Category')
    {
        reAddItem('<%= the_Categories.ClientID %>',name, type + '-' + id, 'AddCategoryBTN');
    }
    else if(type == 'Post')
    {
        reAddItem('<%= the_Posts.ClientID %>',name, type + '-' + id, 'AddPostBTN');
    }
}

function removeItem(type, id)
{
    
     var lbarClientID = <%= lbar.ClientID %>;
     for (var i = 0; i < lbarClientID.GetItemCount(); i++)
     {
        if (lbarClientID.GetItemAtIndex(i).Value == type + '-' + id)
        {
            lbarClientID.RemoveItem(lbarClientID.GetItemAtIndex(i));
            lbarClientID.Refresh();
            break;
        }
     }

        
      $.ajax(
      {
        type:'POST',
        dataType:'text',
        url:'<%= new Urls().AdminAjax %>?command=deleteTextLink',
        data:  {id:id},
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
             }
        },
        error: function(){ alert('Something went wrong...') }
      });    
}


//var base_ListItem_Template = new Template('<div style=\"border: solid 1px #999; padding: 4px;\"><strong>#{title} (#{type})</strong><div style=\"text-align:right;\"><a title=\"Delete Link\" href=\"javascript:void();\" onclick=\"javascript:remove_Link( &#39;#{type}&#39;,&#39;#{text}&#39;, &#39;#{lid}&#39;); return false;\">Delete</a></div></div>');
function AddCategory()
{
    var list = $$('<%= the_Categories.ClientID %>');
    var option = list.options[list.selectedIndex];
    var lbarClientID = <%= lbar.ClientID %>;

    AddItem('Category', option.value.substring(9));
    lbarClientID.AddItem(new Telligent_OrderedListItem(option.value, option.text, parseTemplate($("#NavBarTemplate").html(),{type: 'Category', title: option.text, text: option.text, lid: option.value.substring(9)})));
    lbarClientID.Refresh();
    lbarClientID.SelectItem(lbarClientID.GetItemCount() - 1, true);

    list.options[list.selectedIndex] = null;
    setupNavigation();
}

function AddPost()
{
    var list = $$('<%= the_Posts.ClientID %>');
    var option = list.options[list.selectedIndex];
    var lbarClientID = <%= lbar.ClientID %>;
    
    AddItem('Post', option.value.substring(5));
    lbarClientID.AddItem(new Telligent_OrderedListItem(option.value, option.text, parseTemplate($("#NavBarTemplate").html(),{type:'Post', title: option.text, text: option.text, lid: option.value.substring(5)})));
    lbarClientID.Refresh();
    lbarClientID.SelectItem(lbarClientID.GetItemCount() - 1, true);
    
    list.options[list.selectedIndex] = null;
    setupNavigation();
}

function AddItem(type, id)
{
      $.ajax(
      {
        type:'POST',
        dataType:'text',
        url:'<%= new Urls().AdminAjax %>?command=addNavigationItem',
        data:  {id:id, type:type},
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
             }
        },
        error: function(){ alert('Something went wrong...') }
      });   
}

function AddLink()
{
    
    var text = $$('tb_Text');
    var link = $$('tb_Link');
    
      $.ajax(
      {
        type:'POST',
        dataType:'text',
        url:'<%= new Urls().AdminAjax %>?command=createTextLink',
        data:  {text:text.value,href:link.value},
        success: function(transport)
        {
             var response = transport || "no response text";  
             if(response.length == 36)
             {
                <%= lbar.ClientID %>.AddItem(new Telligent_OrderedListItem("Link-" + response, escape(text.value), parseTemplate($("#NavBarTemplate").html(),{type:'Link', title:text.value, text: escape(text.value), lid: response })));
                <%= lbar.ClientID %>.Refresh();
                <%= lbar.ClientID %>.SelectItem(<%= lbar.ClientID %>.GetItemCount() - 1, true);
                text.value = '';
                link.value = '';
                setupNavigation();
                post_update();
             }
             else
             {
                alert(response);
             }            
             
             
        },
        error: function(){ alert('Something went wrong...') }
      });  

}

function pre_update()
{
     $('#status_update').show();
     $('#status_desc').hide();
}

function post_update()
{
     $$('status_time').innerHTML = new Date().toLocaleString();
     $('#status_desc').show();
     $('#status_update').hide();
}



</script>

<script id="NavBarTemplate" type="text/html">
<div style="border: solid 1px #999; padding: 4px;">
    <strong><#= title #> (<#= type #>)</strong>
    <div style="text-align:right;">
        <a title="Delete Link" href="javascript:void();" onclick="javascript:remove_Link( '<#= type #>','<#= text #>', '<#= lid #>'); return false;">Delete</a>
    </div>
</div>
</script>

<h1>Navigation</h1>
<div id = "messages_form">


<Z:Breadcrumbs runat="server" SectionName="Navigation" />

<div id="post_form_container" class="FormBlock abc" style="min-height: 350px; min-width: 600px; margin-top: -15px;">

        <div id="left-sidebar" style="float:left;width:300px;padding-top:20px; padding-right: 15px; position: relative;border-right: solid 1px #999; min-height: 350px;">
           
            <Glow:TabbedPanes runat="server" PanesCssClass="TabPane"   TabSetCssClass="TabPaneTabSet"    TabCssClasses="TabPaneTab"    TabSelectedCssClasses="TabPaneTabSelected"    TabHoverCssClasses="TabPaneTabHover">
                <Glow:TabbedPane runat="server">
                    <Tab>Categories</Tab>
                    <Content>
                        <h2> Categories:</h2>
                        <asp:DropDownList Width="200px" ID = "the_Categories" runat="Server" DataTextField="name" DataValueField="Id" />
                        <h2><input type="button" value = "Add to Navigation" id="AddCategoryBTN" onclick="AddCategory();" /></h2>
                    </Content>
                </Glow:TabbedPane>
                <Glow:TabbedPane runat="server">
                    <Tab>Posts</Tab>
                    <Content>
                        <h2> Posts:</h2>
                        <asp:DropDownList Width="200px" ID = "the_Posts" runat="Server" DataTextField="Title" DataValueField="Id" />
                        <h2><input type="button" value = "Add to Navigation" id="AddPostBTN" onclick="AddPost();" /></h2>
                    </Content>
                </Glow:TabbedPane>
                <Glow:TabbedPane runat="server">
                    <Tab>Custom Links</Tab>
                    <Content>
                        <h2>Text: </h2>
                        <input type="text" class="tiny" id = "tb_Text" />

                        <h2>Link: </h2>
                        <input type="text" class="tiny" id = "tb_Link" />            
                        
                        <h2><input type="button" value = "Add to Navigation" onclick="AddLink();" /></h2>
                    </Content>
                </Glow:TabbedPane>
            </Glow:TabbedPanes>
         </div>

        <div id="current-navigation" style="float:left;min-height: 350px; padding-left: 40px;">
            <h2>Navigation Links<br /><span class="form_tip">Rearrange, add, or delete navigation links.</span></h2>

            <Glow:OrderedList ID="lbar" runat="server" Width="250px" Height="300px" OnItemMovedClientFunction="saveNavigationUpdates" EnableDeleteButton="false" EnableMoveDownButton="false" EnableMoveUpButton="false" />
            
         </div>

        <div id="test" style="clear:both;"></div>
</div>  
  
    <div class="submit">
    <div id="buttons">
        <em>Changes are saved automatically. Just drag and drop, we will do the rest.</em>
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
        <p>Smart Navigation allows you to control the links which are displayed on your sites main navigation element 
        (note: not all themes will use this tool).</p>
        <p>By default, all of your top level categories will be displayed in alphabetical order. 
        From there, you can add/remove categories, uncategorized posts, or even add a custom link to 
        another page in your site (or an external link). 
        Finally, you can simply drag and drop the "boxes" to resort how the links are presented.</p>
    </div>
    <div class="box">    
        <h3>Tips</h3>
        <p>
            <ul>
                <li>If you would like to have an about page in your navigation, create an uncategorized post called "About" and then add it to the smart navigation</li>
                <li>Add a link back to your home page, by adding "Home" as the text and "/" as the link.</li>
            </ul>
        </p>
    </div>
    <div style="clear: both;"></div>
    </div></div>
</asp:Content>

