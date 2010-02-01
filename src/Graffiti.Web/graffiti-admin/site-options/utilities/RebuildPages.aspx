<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" Title="Rebuild Pages" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<script runat="Server">
    void Page_Load(object sender, EventArgs e)
    {
        Util.CanWriteRedirect(Context);
         LiHyperLink.SetNameToCompare(Context,"settings");
    }
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<script type="text/javascript">

var Rebuilder =
{
	start : function()
	{
	    $$('#btn').value = 'Building';
	    $$('#btn').disabled = true;
	    $$('#Status').innerHTML = 'Started, building categories';
	
	    Rebuilder.buildCategories();
	}
	
	, buildCategories : function()
	{
	     $.ajax(
	      {
	        type:'POST',
	        dataType: 'text',
	        data: "{}",
	        url: '<%= new Urls().AdminAjax %>?command=buildCategoryPages',
	        //parameters:  {t:typeName},
	        success: function(transport)
	        {
	            var response = transport || "no response text";
	            if(response == 'Success')
	            {
	                
	                $$('#Status').innerHTML = "Categories are completed, building post and tag pages";
	                setTimeout("Rebuilder.buildPages(1)",1000)
	            }
	            else
	            {
	                alert(response);
	            }
	
	        },
	        error: function(){ alert('Something went wrong...') }
	      });
	}
	
	, buildPages : function(pageNumber)
	{
	    $$('#Status').innerHTML = 'Building pages (and tags) ' + (((pageNumber * 20) - 20) + 1) + ' to ' + (pageNumber * 20);
	
	     new $.ajax(
	      {
	        type:'POST',
	        url: '<%= new Urls().AdminAjax %>?command=buildPages',
	        data:  {p:pageNumber},
	        success: function(transport)
	        {
	            var response = transport || 'no response text';
	            if(response == 'Next')
	            {
	                //alert(pageNumber);
	                $$('#Status').innerHTML = 'Pages (and tags) ' + (((pageNumber * 20) - 20) + 1) + ' to ' + (pageNumber * 20) + ' are finished building';
	                pageNumber = pageNumber + 1;
	                setTimeout("Rebuilder.buildPages(" + pageNumber + ")",1000)
	
	            }
	            else if(response == 'Success')
	            {
	                $$('#btn').disabled = false;
	                $$('#btn').value = 'Start Building';
	                $$('#Status').innerHTML = 'Finished Building';            
	            }
	            else
	            {
	                alert(response);
	            }
	
	        },
	        error: function(){ alert('Something went wrong...') }
	      });
	}
}

</script>

<h1>Rebuild Your Site Pages</h1>
<Z:Breadcrumbs runat="server" SectionName="RebuildPages" />

<div id = "messages_form">
    
    <div id="post_form_container" class="FormBlock">
        
        <div style="padding-top:10px">
        <p>You can use this utility to rebuild the pages in your site. You may need to do this if there was an error saving/updating a post or if you 
        are moving your site to another server.</p>
        <p>Once you click the button below, please <strong>do not</strong> refresh the page or navigate away from this page until it is finished.</p>
        <p>
        <input type="button" id="btn" value = "Start Building" onclick="Rebuilder.start(); return false;" /> <span id ="Status" style="padding-left:20px; font-weight:bold"></span>
        </p>
        </div>
    </div>
</div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
</asp:Content>

