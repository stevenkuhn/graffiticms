<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master"
	Title="Rebuild Pages" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<script runat="Server">
	void Page_Load(object sender, EventArgs e)
	{
		Util.CanWriteRedirect(Context);
		LiHyperLink.SetNameToCompare(Context, "settings");

		if (SiteSettings.Get().GenerateFolders)
		{
			if (SiteSettings.UrlRoutingSupported)
			{
				WarningPanel.Visible = true;
				WarningLabel.Text = string.Format("Your web server suports URL Routing (urls without extensions), but the <a href=\"{0}\"><em>Generate Folders for Posts/Categories/Tags</em></a> setting is enabled. This is a legacy setting intended for older web servers without URL routing support. You probably want to disable this setting so you don't clutter things up.", VirtualPathUtility.ToAbsolute("~/graffiti-admin/site-options/configuration/"));
			}
		}
		else
		{
			RebuildPagesPanel.Visible = false;
			WarningPanel.Visible = true;
			WarningLabel.Text = string.Format("The <a href=\"{0}\"><em>Generate Folders for Posts/Categories/Tags</em></a> setting is disabled. This is a legacy setting intended for older web servers without URL routing support. You should enable that setting if you want to rebuild the folders and pages.", VirtualPathUtility.ToAbsolute("~/graffiti-admin/site-options/configuration/"));
		}
	}
</script>

<asp:Content ContentPlaceHolderID="HeaderRegion" runat="Server">
</asp:Content>

<asp:Content ContentPlaceHolderID="MainRegion" runat="Server">

<script type="text/javascript">
    var FeedFlusher =
	{
	    start: function() {
	        $$('#btn2').value = 'Flushing';
	        $$('#btn2').disabled = true;
	        $$('#Status2').innerHTML = 'Started, Flushing RSS Feeds...';

	        FeedFlusher.removeFeedData();
	    }

	    , removeFeedData: function() {
	        $.ajax(
				{
				    type: 'POST',
				    dataType: 'text',
				    data: "{}",
				    url: '<%= new Urls().AdminAjax %>?command=removeFeedData',
				    //parameters:  {t:typeName},
				    success: function(transport) {
				        var response = transport || "no response text";
				        if (response == 'Success') {
				            $$('#btn2').disabled = false;
				            $$('#btn2').value = 'Flush RSS Feeds';
				            $$('#Status2').innerHTML = 'Finished Flushing';

				        }
				        else {
				            alert(response);
				        }

				    },
				    error: function() { alert('Something went wrong...') }
				});
	    } 
	}
	
	
	var Rebuilder =
	{
		start: function() {
			$$('#btn').value = 'Building';
			$$('#btn').disabled = true;
			$$('#Status').innerHTML = 'Started, building main feed...';

			Rebuilder.buildMainFeed();
		}

		, buildMainFeed: function() {
			$.ajax(
				{
					type: 'POST',
					dataType: 'text',
					data: "{}",
					url: '<%= new Urls().AdminAjax %>?command=buildMainFeed',
					success: function(transport) {
						var response = transport || "no response text";
						if (response == 'Success') {

							$$('#Status').innerHTML = "Main feed completed. Now Building categories...";
							setTimeout("Rebuilder.buildCategories()", 1000)
						}
						else {
							alert(response);
						}

					},
					error: function() { alert('Something went wrong...') }
				});
		}

		, buildCategories: function() {
			$.ajax(
				{
					type: 'POST',
					dataType: 'text',
					data: "{}",
					url: '<%= new Urls().AdminAjax %>?command=buildCategoryPages',
					//parameters:  {t:typeName},
					success: function(transport) {
						var response = transport || "no response text";
						if (response == 'Success') {

							$$('#Status').innerHTML = "Categories are completed, building post and tag pages...";
							setTimeout("Rebuilder.buildPages(1)", 1000)
						}
						else {
							alert(response);
						}

					},
					error: function() { alert('Something went wrong...') }
				});
		}

		, buildPages: function(pageNumber) {
			$$('#Status').innerHTML = 'Building pages (and tags) ' + (((pageNumber * 20) - 20) + 1) + ' to ' + (pageNumber * 20);

			new $.ajax(
				{
					type: 'POST',
					url: '<%= new Urls().AdminAjax %>?command=buildPages',
					data: { p: pageNumber },
					success: function(transport) {
						var response = transport || 'no response text';
						if (response == 'Next') {
							//alert(pageNumber);
							$$('#Status').innerHTML = 'Pages (and tags) ' + (((pageNumber * 20) - 20) + 1) + ' to ' + (pageNumber * 20) + ' are finished building';
							pageNumber = pageNumber + 1;
							setTimeout("Rebuilder.buildPages(" + pageNumber + ")", 1000)

						}
						else if (response == 'Success') {
							$$('#btn').disabled = false;
							$$('#btn').value = 'Start Building';
							$$('#Status').innerHTML = 'Finished Building';
						}
						else {
							alert(response);
						}

					},
					error: function() { alert('Something went wrong...') }
				});
		}
	}
	</script>

	<h1>Rebuild Your Site Folders, Pages, and RSS Feeds</h1>
	<Z:Breadcrumbs runat="server" SectionName="RebuildPages" />
	<div id="messages_form">
		<asp:Panel ID="WarningPanel" runat="server" Visible="false" CssClass="infomessage">
			<asp:Label ID="WarningLabel" runat="server" />
		</asp:Panel>
		<asp:PlaceHolder ID="Panels" runat="server">
			<div id="post_form_container" class="FormBlock">
				<asp:PlaceHolder ID="RebuildPagesPanel" runat="server">
				    <div style="padding-top: 10px">
					    <p>
						    You can use this utility to rebuild the folders for all Posts/Categories/Tags
						    in your site. You may need to do this if there was an error saving/updating a post
						    or if you are moving your site to another server.</p>
					    <p>
						    Once you click the button below, please <strong>do not</strong> refresh the page
						    or navigate away from this page until it is finished.</p>
					    <p>
						    <input type="button" id="btn" value="Start Building" onclick="Rebuilder.start(); return false;" />
						    <span id="Status" style="padding-left: 20px; font-weight: bold"></span>
					    </p>
				    </div>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="FlushFeedsPanel" runat="server">
				    <div style="padding-top: 10px">
					    <p>
						    You can remove external RSS feeds from the database including those that may no longer be needed.
						    You may need to do this if an error is displayed in the log pertaining to a failed RSS feed update,
						    Or if some external RSS feeds are still being updated that are not used anywhere on your site.</p>
					    <p>
						    Once you click the button below, please <strong>do not</strong> refresh the page
						    or navigate away from this page until it is finished.</p>
					    <p>
						    <input type="button" id="btn2" value="Flush RSS Feeds" onclick="FeedFlusher.removeFeedData(); return false;" />
						    <span id="Status2" style="padding-left: 20px; font-weight: bold"></span>
					    </p>
				    </div>
				</asp:PlaceHolder>
			</div>
		</asp:PlaceHolder>
	</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" runat="Server">
</asp:Content>