<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" enableEventValidation="False" AutoEventWireup="true" Inherits="graffiti_admin_presentation_category_widget" Title="Graffiti - Category Widget" Codebehind="EditCategoryWidget.aspx.cs" %>
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
    var pl = $$('<%=the_Categories.ClientID %>');
    
    if(pl.options.length == 0)
    {
        var option = new Option();
        option.value = -1;
        option.text = '--No Categories--';        
        pl.options[0] = option;
        
        $('AddPostBTN').disabled = true;
    }    
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
    
    for (var i = 0; i < <%= existing_items.ClientID %>.GetItemCount(); i++)
     {
        if (<%= existing_items.ClientID %>.GetItemAtIndex(i).Value == id)
        {
            <%= existing_items.ClientID %>.RemoveItem(<%= existing_items.ClientID %>.GetItemAtIndex(i))
            <%= existing_items.ClientID %>.Refresh();
            break;
        }
     }
}

function remove_Link(name, id)
{
     reAddItem('<%= the_Categories.ClientID %>', name, id, 'AddPostBTN');
}

function AddPost()
{
    var list = $$('<%= the_Categories.ClientID %>');
    var option = list.options[list.selectedIndex];
    var id = option.value;
    var existingItems = <%= existing_items.ClientID %>;
    
    existingItems.AddItem(new Telligent_OrderedListItem(id, option.text, parseTemplate($("#LITemplate").html(),{text: option.text, lid: id})));
    existingItems.Refresh();
    existingItems.SelectItem(existingItems.GetItemCount() - 1, true);

    list.options[list.selectedIndex] = null;
    setupNavigation();
}

</script>
<script id="LITemplate" type="text/html">
<div style="border: solid 1px #999; padding: 4px;">
	<strong><#= text #></strong>
	<div style="text-align:right;">
		<a title="Delete Link" href="javascript:void();" onclick="remove_Link('<#= text #>', '<#= lid #>'); return false;">Delete</a>
	</div>
</div>
</script>


<h1>Category Widget</h1>
<div id = "messages_form">
    <Z:StatusMessage runat="Server" ID = "Message"  Type="Success"/>

	<div id="post_form_container" class="FormBlock abc" style="min-height:450px">

    <h2>Title: <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtTitle" Text="Please enter a Title" runat ="Server" /></h2>
    <asp:TextBox CssClass = "large" runat="Server" id="txtTitle" TabIndex="1" MaxLength="256" />  

        <div id="left-sidebar" style="float:left;width:300px;">
            <h2 style="height:25px;">Available Categories</h2>
            <asp:DropDownList Width="200px" ID = "the_Categories" runat="Server" DataTextField="Name" DataValueField="Id" />
            <input type="button" value = "Add" id="AddPostBTN" onclick="AddPost();" />
            </h2>            
         </div>

        <div id="current-navigation" style="float:left;width:250px;">
            <h2 style="height:25px;">Selected Categories</h2>
            <Glow:OrderedList ID="existing_items" runat="server" Width="250px" Height="300px" EnableDeleteButton="false" EnableMoveDownButton="false" EnableMoveUpButton="false" />
        </div>

        <div id="test" style="clear:both;"></div>
        

</div>  
  
<div class="submit">
    <div id="buttons">
        <asp:Button ID="Publish_Button" runat="Server" Style="font-weight:bold" Text = "Update"  TabIndex="81" OnClick="PageWidget_Save_Click" />
        <asp:HyperLink ID ="Cancel_Edit" Text = "(Cancel)" NavigateUrl="~/graffiti-admin/presentation/widgets/" runat="Server" />
    </div>
</div>  
         
</div>

</div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
    <div id="sidebar"><div class="gutter">
    <div class="box">
        <h3>About This Page</h3>
        <p>The Category Widget allows you to add one or more widgets to your side bar with your favorite categories.</p>
        <p>You have completed control over which categories are list and their order</p>
    </div>
    <div class="box">    
        <h3>Tip</h3>
        <p>
            By default, all categories are list with the title of <em>Categories</em>. If you keep the <em>Selected List</em> empty, Graffiti will always display all of your categories for you.
        </p>
    </div>
    <div style="clear: both;"></div>
    </div></div>
</asp:Content>

