<%@ Page Language="C#" enableEventValidation="false" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_posts_write_Default" Title="Write a Post" Codebehind="Default.aspx.cs" %>
<%@ Register TagPrefix="Z" Assembly="Graffiti.Core" Namespace="Graffiti.Core" %>
<%@ Register TagPrefix="Glow" Assembly="Telligent.Glow" Namespace="Telligent.Glow" %>
<%@ Register TagPrefix="GlowEditor" Assembly="Telligent.Glow.Editor" Namespace="Telligent.Glow.Editor" %>
<%@ Import namespace="System.Collections.Generic"%>
<%@ Import namespace="Telligent.Glow"%>
<%@ Import namespace="DataBuddy"%>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">

<script type="text/javascript">

function GetMergeContent()
{
    return Telligent_EditorAPI.GetInstance( '<%= txtContent.ClientID %>' ).GetXHTML(false) + Telligent_EditorAPI.GetInstance( '<%= txtContent_extend.ClientID %>' ).GetXHTML(false);
}

function RedirectToVersionedPost(versionLink)
{
    if(versionLink != null)
        window.location = versionLink;
    
}

function Publish_Status_Change()
{
   var list = $$('#<%= PublishStatus.ClientID %>');
   var btn = $$('#<%= Publish_Button.ClientID %>');
   
   var val = list.options[list.selectedIndex].value;
   
   if(val == 1)
        btn.value = 'Publish';
   else if(val == 2)
        btn.value = 'Save as Draft';
   else if(val == 3)
        btn.value = 'Save for review';
   else if(val == 4)
        btn.value = 'Request changes';
        
}

function toggleExtendedBody()
{
    var eBody = $$('#extended_body');
    if (eBody.style.position == 'absolute')
    {
        eBody.style.position = 'static';
        eBody.style.visibility = 'visible';
    }
    else
    {
        eBody.style.position = 'absolute';
        eBody.style.visibility = 'hidden';
    }   
}

var categoryLastValue = '<%= CategoryList.SelectedValue %>';
var customFieldValues = new Object();
var customFieldNameRe = new RegExp('^[0-9a-f]{8}\-[0-9a-f]{4}\-[0-9a-f]{4}\-[0-9a-f]{4}\-[0-9a-f]{12}$', 'i');
function categoryChanged(categorySelect)
{
    if(categorySelect[categorySelect.selectedIndex].value == -1)
    {
        $('#category_form').show()
    }
    else
    {
        $('#category_form').hide();
    }

    categoryLastValue = categorySelect[categorySelect.selectedIndex].value;
    
    // Check if the user has publish permissions to the new category
    var statusList = $$('#<%= PublishStatus.ClientID %>');
    $.ajax({
        type: 'POST',
        url: '<%= new Urls().AdminAjax %>?command=checkCategoryPermission&permission=Publish&category=' + categoryLastValue,
        data: customFieldValues,
        dataType: 'text',
        success: function(transport)
        {
            var response = transport || "";
            if (response == 'true')
            {
                var publishExists = 0;
                for (var i=0;i < statusList.options.length;i++) {
                    if (statusList.options[i].value == 1) {
                        publishExists = 1;
                    }
                }
                if (publishExists == 0)
                {
                    insertOption(statusList, 'Published', '1');
                }
                statusList.options[0].selected = true;
            }
            else
            {

                for(i=statusList.length - 1; i>=0; i--) {
                    if (statusList.options[i].value == 1) {
                        if (statusList.options[i].selected) {
                            statusList.selectedIndex = i+1;
                        }
                        statusList.options[i] = null;
                    }
                }
            }
            
            Publish_Status_Change();
        },
        error: function() { alert('Something went wrong...') }
    });

    addUpdateCustomFieldValues();    
    $$('#customfields').innerHTML = 'Loading... please wait';
    
    new $.ajax(
    {
        type: 'POST',
        dataType: 'text',
        url: '<%= new Urls().AdminAjax %>?command=categoryForm&category=' + categorySelect[categorySelect.selectedIndex].value + "&post=" + <%= Request.QueryString["id"] ?? "-1" %>,
        data: customFieldValues,
        success: function(transport)
        {
            var response = transport || "";
            if (response == '')
            {
                $$('#customfields').innerHTML = '&nbsp;';
                <%= TabbedPanes1.ClientID %>.GetTabAtIndex(3).Disabled = true;
                <%= TabbedPanes1.ClientID %>.Refresh();
            }
            else
            {
                var runScripts = true;
                while(runScripts)
                {
                    if(response.indexOf("startscript:") > 0)
                    {
                        var cmd = response.substring(response.indexOf("startscript:") + 12, response.indexOf(":endscript"));
                        setTimeout("RunScript(\"" + cmd + "\");", 1);
                        response = response.replace("startscript:" + cmd + ":endscript", ""); 
                    }
                    else
                    {
                        runScripts = false;
                    }
                }
  
                $('#customfields').html(response);
                <%= TabbedPanes1.ClientID %>.GetTabAtIndex(3).Disabled = false;
                <%= TabbedPanes1.ClientID %>.Refresh();
            }
        },
        error: function() { alert('Something went wrong...') }
    });
}

function addUpdateCustomFieldValues()
{
    for (var i = 0; i < document.forms[0].length; i++) 
	{
		var element = document.forms[0].elements[i];
		if (element.name && customFieldNameRe.test(element.name)) 
		{   
			var elementValue = '';
			if (element.nodeName == 'INPUT') 
			{
				var elementType = element.type.toLowerCase();
				if (elementType == 'text' || elementType == 'password' || elementType == 'hidden') 
					elementValue = element.value;
				else if ((elementType == 'checkbox' || elementType == 'radio') && element.checked) 
					elementValue = element.value;
			}
			else if (element.nodeName == 'SELECT' || element.nodeName == 'TEXTAREA') 
				elementValue = element.value;
			
			customFieldValues[element.name] = elementValue;
		}
	}
}

function updatePostImageUrl(url)
{
       $$('#<%= postImage.ClientID %>').value = url;
}

function showMergeWindow()
{
    var w = window.open('../postmerge.aspx?id=<%= Request.QueryString["id"] %>','merger','width=600,height=400,toolbar=no,menubar=no,location=no,directories=no,resizable=yes,scrollbars=yes');
    w.focus();
}

function publishDateChanged()
{
    $$('#dateChangeFlag').value = 'true';
}

function GoToTab(index)
{
    <%= TabbedPanes1.ClientID %>.SelectTab(<%= TabbedPanes1.ClientID %>.GetTabAtIndex(index));
}

function insertOption(theSel, newText, newValue)
{
  if (theSel.length == 0) {
    var newOpt1 = new Option(newText, newValue);
    theSel.options[0] = newOpt1;
    theSel.selectedIndex = 0;
  } else if (theSel.selectedIndex != -1) {
    var selText = new Array();
    var selValues = new Array();
    var selIsSel = new Array();
    var newCount = -1;
    var newSelected = -1;
    var i;
    for(i=0; i<theSel.length; i++)
    {
      newCount++;
      if (newCount == theSel.selectedIndex) {
        selText[newCount] = newText;
        selValues[newCount] = newValue;
        selIsSel[newCount] = false;
        newCount++;
        newSelected = newCount;
      }
      selText[newCount] = theSel.options[i].text;
      selValues[newCount] = theSel.options[i].value;
      selIsSel[newCount] = theSel.options[i].selected;
    }
    for(i=0; i<=newCount; i++)
    {
      var newOpt = new Option(selText[i], selValues[i]);
      theSel.options[i] = newOpt;
      theSel.options[i].selected = selIsSel[i];
    }
  }
}
</script>

<Z:FileBrowser runat="Server" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<h1><asp:Literal ID = "PageTitle" runat="Server" Text="Write a post" /></h1> 
    <div id = "messages_form">
    
        <asp:PlaceHolder runat="Server" ID = "FormWrapper">
 
             <div id="post_form_container" class="FormBlock" style="padding-top:20px; height: 1%; position: relative;">
             <Glow:TabbedPanes ID="TabbedPanes1" runat="server" PanesCssClass="TabPane"   TabSetCssClass="TabPaneTabSet"    TabCssClasses="TabPaneTab"    TabSelectedCssClasses="TabPaneTabSelected"    TabHoverCssClasses="TabPaneTabHover">
      <Glow:TabbedPane ID="TabbedPane1" runat="server">
        <Tab>Content</Tab>
          <Content>
          <div style="padding-top: 8px;"></div>
          <Z:StatusMessage runat="server" ID="Message1" Type="Success" />
           <h2>Title: <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtTitle" Text="Please enter a Title" runat ="Server" /></h2>
            <asp:TextBox CssClass = "large" runat="Server" id="txtTitle" TabIndex="1" MaxLength="256" />  

            <h2>Category: 
                <asp:DropDownList runat="Server" ID = "CategoryList" Style="width:200px;" TabIndex="2" DataTextField="Name" DataValueField="Id" onchange="categoryChanged(this);">
                </asp:DropDownList>
                
                <span id="category_form" style="display:none;">
                    | <asp:TextBox id = "newCategory" runat="server" /><!--<input type="button" onclick="addNewCategory(); return false;" value="Add New Category" />-->
                </span>
            </h2>


            <h2>Body: <span class="form_tip">(<a href="javascript:void();" onclick="javascript:toggleExtendedBody(); return false;">extended body</a>)</span></h2>
            <GlowEditor:Editor ID="txtContent" ToolbarSet="Simple" runat="server" Width="100%" Height="300px" TabIndex="4" />

            <div id="extended_body">
                <h2>Extended Body:</h2>
                <GlowEditor:Editor ID ="txtContent_extend" ToolbarSet="Simple" runat="Server" Width= "100%" Height="300px" TabIndex="5" />
            </div>            
        </Content>
          
      </Glow:TabbedPane>

      <Glow:TabbedPane ID="TabbedPane2" runat="server">
            <Tab>Settings</Tab>
            <Content>
            <div style="padding-top: 8px;"></div>
            <Z:StatusMessage runat="server" ID="Message2" Type="Success" />
                   <h2>Tags: <span class="form_tip">(seperate tags with a comma)</span></h2>
                   <Z:TagTextBox CssClass="large" runat="Server" id="txtTags" TabIndex="6" MaxLength="255" />

                   <h2>Name: <span class="form_tip">(name of the post. Only letters and numbers. All other characters will be removed)</span></h2>
                    <asp:TextBox CssClass = "large" runat="Server" id="txtName" TabIndex="7" MaxLength="255" />  


                    

                    <h2>Publish Date:</h2>
                    <Glow:DateTimeSelector runat="server" TabIndex="8" id="PublishDate"  DateTimeFormat="MMMM d yyyy hh:mm tt" ShowCalendarPopup="true" OnChangedClientFunction="publishDateChanged" />

                    <h2><asp:CheckBox ID="EnableComments" TabIndex="9" Text="Enable Comments" runat="Server" /><br /><span class="form_tip">If comments are enabled site wide, this setting will allow you to disable them for the current post.</span></h2>
                    
                    <h2 id="useHomeSort" runat="Server"><asp:CheckBox ID="HomeSortOverride" TabIndex="10" Text="Include on the Home Page" runat="Server" Checked="false" /><br /><span class="form_tip">If home page overrides are enabled, do you want this post to be displayed on your site's home page?</span></h2>
                    
                    <h2 id="featuresiteRegion" runat="Server"><asp:CheckBox ID="FeaturedSite" Text="Featured Site Post" runat="Server" Checked="false" TabIndex="11"/><br /><span class="form_tip">Do you want to mark this as the site's featured post?</span></h2>
                    
                    <h2 id="featuredCategoryRegion" runat="Server"><asp:CheckBox ID="FeaturedCategory" TabIndex="12" Text="Featured Category Post" runat="Server" Checked="false" /><br /><span class="form_tip">Do you want to mark this as the selected category's featured post?</span></h2>

                    <h2 id="P_Status" runat="Server">Publish Status: 
                        <asp:DropDownList runat="Server" ID = "PublishStatus" Style="width:150px;" TabIndex="13" />
                    </h2>    


            </Content>
      </Glow:TabbedPane>
      
      <Glow:TabbedPane ID = "PostOptions" runat="Server">
        <Tab>Options</Tab>
        <Content>
        <div style="padding-top: 8px;"></div>
        <Z:StatusMessage runat="server" ID="Message3" Type="Success" />
                <h2>Meta Description: <span class="form_tip">(The meta description for this post)</span></h2>
                <asp:TextBox CssClass = "large" runat="Server" id="txtMetaScription" TabIndex="13" TextMode="MultiLine" Rows="3" MaxLength="255" /> 
   
                <h2>Meta Keywords: <span class="form_tip">(The meta keywords for this post)</span></h2>
                <asp:TextBox CssClass = "large" runat="Server" id="txtKeywords" TabIndex="14" MaxLength="255" /> 
                
                <h2>Image: <span class="form_tip">(an optional image to associate with this post. This image may not be used in all site themes)</span></h2>
                <asp:TextBox CssClass = "small" runat="Server" id="postImage" TabIndex="15" MaxLength="255"/> <input class="inputbutton" type="button" value="Select ..." onclick="OpenFileBrowser(updatePostImageUrl, 'image');return false" />
                
                <h2>Notes: <span class="form_tip">(notes about this post and/or revision)</span></h2>
                <asp:TextBox CssClass = "large" runat="Server" id="txtNotes" TextMode="MultiLine" Rows="16" TabIndex="17" />        
        
        </Content>
      
      </Glow:TabbedPane>
      
      <Glow:TabbedPane runat="server" ID="CustomFieldsTab">
        <Tab>Custom Fields</Tab>
        <Content>
            <div id="customfields">
                <asp:Literal runat="Server" ID = "the_CustomFields" />
            </div>  
        </Content>
      </Glow:TabbedPane>
      
      </Glow:TabbedPanes>

                <div id="VersionHistoryArea" visible="false" runat="server" style="position: absolute; right: 0px; top: 3px; z-index: 100; margin: 0; padding: 0;">
                    <table cellpadding="0" cellspacing="0" border="0"><tr>
                        <td><Glow:DropDownList runat="server" ID="VersionHistory" SelectListWidth="300" Width="150px" ShowHtmlWhenSelected="false" /></td>
                        <td>&nbsp;| <a href="#" onclick="showMergeWindow(); return false;">View Changes</a></td>
                    </tr></table>
                </div>
            </div>

            <div class="submit">
                <asp:Button ID="Publish_Button" runat="Server" Style="font-weight:bold;" Text = "Publish" OnClick="publish_return_click" OnClientClick="GoToTab(0);" TabIndex="81" />
                <asp:HyperLink ID ="Cancel_Edit" Text = "(Cancel)" NavigateUrl="~/graffiti-admin/posts/" runat="Server" TabIndex="82" />
            </div>   

        </asp:PlaceHolder>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
    <div id="sidebar"><div class="gutter">
			<div class="box">
				<h3>About This Page</h3>
                <p>This page allows you to create new content. Depending on your permissions, your content may need to be approved before it is published.</p>

                <p>Graffiti will <b>always</b> make a copy of your post before you change it. This way if anything goes wrong you can quickly rollback to a previous version using the version tool. </p>
                <p><em>Note: The version tool will be displayed below when more than one version of your post exists</em></p>

			</div>
    <div style="clear: both;"></div>
    </div></div>
</asp:Content>

