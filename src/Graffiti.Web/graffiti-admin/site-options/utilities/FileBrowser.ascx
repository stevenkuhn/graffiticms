<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileBrowser.ascx.cs" Inherits="Graffiti.Web.FileBrowserControl" %>
<script type="text/javascript">
// <!--

window._fileBrowserSelectFileFunction = <%= this.OnClientFileClickedFunction %>;

function select(url)
{
    if (window._fileBrowserSelectFileFunction)
    {
        window._fileBrowserSelectFileFunction(url);
        return false;
    }
    
    return true;
}

var lastHeight = -1;
function resizeHeight()
{
    var height;

    if (typeof(window.innerHeight) == 'number') 
        height = window.innerHeight;
    else if (document.documentElement && document.documentElement.clientHeight) 
        height = document.documentElement.clientHeight;
    else if (document.body && document.body.clientHeight) 
        height = document.body.clientHeight;
		
   	height -= <%= this.ContentHeightOffset %>;
    if (height < 100)
        height = 100;
		
    if (lastHeight != height)
    {
        lastHeight = height;
        
        var area = document.getElementById('folderList');
        if (area)
            area.style.height = height + 'px';
            
        area = document.getElementById('fileList');
        if (area)
            area.style.height = height + 'px';
    }

    window.setTimeout(resizeHeight, 499);
}

<%= this.EnableResizeToHeight ? "window.setTimeout(resizeHeight, 499);" : "" %>

// -->
</script>

<style>
	#rightFileView
	{
		width: 100%;
		position: relative;
		float: left;
	}
	* html #rightFileView
	{
		width: 74.8%;
	}
</style>
<asp:Literal runat="Server" ID="theBreadCrumbs" />
<div id="messages_form" style="margin: -15px;">
	<div id="post_form_container" class="FormBlock">
		<div id="left-sidebar" style="float: left; width: 25%; border-right: solid 1px #999;">
			<h2 style="margin: 0 0 5px 0; padding: 5px 15px 5px 15px; background-color: #eee; font-weight: bold; border-bottom: solid 1px #999; color: #000;">
				Folders
			</h2>
			<div style="height: 475px; overflow: auto; padding: 0; margin: -5px 0 0 0; white-space: nowrap;" id="folderList">
				<Z:Repeater runat="Server" ID="FolderList" ShowHeaderFooterOnNone="False">
					<NoneTemplate>
						No folders at this level</NoneTemplate>
					<HeaderTemplate>
						<ul id="folderList">
					</HeaderTemplate>
					<FooterTemplate>
						</ul></FooterTemplate>
					<ItemTemplate>
						<li><a class="folder" href="<%# Eval("Url") %>"> <%# Eval("Name") %></a></li>
					</ItemTemplate>
				</Z:Repeater>
			</div>
		</div>
		<div id="current-navigation" style="margin: 0; padding: 0; overflow: hidden;">
			<div id="rightFileView">
				<asp:MultiView runat="Server" ID="FileViews">
					<asp:View runat="Server" ID="FileLists">
						<h2 style="margin: 0 0 5px 0; padding: 5px 15px 5px 15px; background-color: #eee;
							font-weight: bold; border-bottom: solid 1px #999; color: #000;">
							Files</h2>
						<div style="height: 475px; overflow: auto; padding: 0; margin: -5px 0 0 -5px;" id="fileList">
							<div style="float: left; width: 49%; overflow: hidden;">
								<asp:Repeater ID="LeftFiles" runat="Server">
									<HeaderTemplate>
										<ul id="leftFiles">
									</HeaderTemplate>
									<FooterTemplate>
										</ul></FooterTemplate>
									<ItemTemplate>
										<li><a class="<%# Eval("CssClass") %>" href="<%# Eval("Url") %>" onclick="<%# Eval("OnClick") %>">
											<%# Eval("Name") %></a></li>
									</ItemTemplate>
								</asp:Repeater>
							</div>
							<div style="float: left; width: 49%; overflow: hidden;">
								<asp:Repeater ID="RightFiles" runat="Server">
									<HeaderTemplate>
										<ul id="rightFiles">
									</HeaderTemplate>
									<FooterTemplate>
										</ul></FooterTemplate>
									<ItemTemplate>
										<li><a class="<%# Eval("CssClass") %>" href="<%# Eval("Url") %>" onclick="<%# Eval("OnClick") %>">
											<%# Eval("Name") %></a></li>
									</ItemTemplate>
								</asp:Repeater>
							</div>
						</div>
						<div style="clear: both;">
						</div>
						<div style="position: absolute; right: 15px; top: 3px;">
							<a href="javascript:void(0);" onclick="Telligent_Modal.Open('FileBrowser-AddFolder.aspx?Path=<%= Server.UrlEncode(Request.QueryString["path"] ?? "") %>', 320, 200, null); return false;">Create Folder</a> | 
							<a href="javascript:void(0);" onclick="Telligent_Modal.Open('FileBrowser-UploadFiles.aspx?Path=<%= Server.UrlEncode(Request.QueryString["path"] ?? "") %>', 500, 400, null); return false;">Add Files</a>
						</div>
					</asp:View>
					<asp:View runat="Server" ID="FileDetails">
						<h2 style="margin: 0 0 5px 0; padding: 5px 15px 5px 15px; background-color: #eee;
							font-weight: bold; border-bottom: solid 1px #999; color: #000;">
							File:
							<asp:HyperLink runat="Server" ID="FileDetailsName" /><asp:Literal runat="Server"
								ID="FileDetailsText" /></h2>
						<div style="padding: 0 15px;">
							<Z:StatusMessage runat="Server" ID="ActionMessage" />
							<ul style="list-style-type: none; margin: 0; padding: 0;">
								<li id="revsionLI" runat="Server">Revision:
									<asp:Literal runat="Server" ID="FileDetailsRevision" /></li>
								<li id="assemblyLI" runat="Server">Assembly Version:
									<asp:Literal runat="Server" ID="FileDetailsAssemblyVersion" /></li>
								<li>Size:
									<asp:Literal runat="Server" ID="FileDetailsSize" /></li>
								<li>Last Modified:
									<asp:Literal runat="Server" ID="FileDetailsLastModified" /></li>
							</ul>
							<div class="submit">
								<div id="buttons">
									<asp:Button runat="Server" ID="DownloadButton" OnClick="DownloadFile_Click" Text="Download"
										TabIndex="8" />
									<input type="button" value="Edit" id="EditButton" runat="Server" onclick="window.location = window.location + '&edit=true';" />
									<asp:Button runat="Server" ID="DeleteButton" OnClick="DeleteFile_Click" Text="Delete"
										TabIndex="8" />
								</div>
							</div>
						</div>
					</asp:View>
					<asp:View runat="Server" ID="FileEditor">
						<h2 style="margin: 0 0 5px 0; padding: 5px 15px 5px 15px; background-color: #eee;
							font-weight: bold; border-bottom: solid 1px #999; color: #000;">
							Editing:
							<asp:HyperLink runat="Server" ID="EditFileName" /></h2>
						<div style="padding: 0 15px;">
							<Z:StatusMessage runat="Server" ID="EditMessage" />
							<div id="VersionHistoryArea" runat="server" style="text-align: right; padding-bottom: 5px;
								padding-right: 3px;">
								<Glow:DropDownList runat="server" ID="VersionHistory" SelectListWidth="300" Width="150px"
									ShowHtmlWhenSelected="false" />
							</div>
							<asp:TextBox TextMode="MultiLine" runat="Server" ID="EditBox" Style="height: 400px;
								width: 100%;" CssClass="large" Columns="20" Rows="2" />
							<div class="submit">
								<div id="buttons">
									<asp:Button runat="Server" ID="Button1" Text="Save Changes" OnClick="SaveFile_Click"
										TabIndex="8" />
								</div>
							</div>
						</div>
					</asp:View>
				</asp:MultiView>
			</div>
		</div>
		<div style="clear: both;">
		</div>
	</div>
</div>
