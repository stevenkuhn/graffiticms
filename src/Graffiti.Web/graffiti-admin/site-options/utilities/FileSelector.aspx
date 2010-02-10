<%@ Page Language="C#" AutoEventWireup="true" Title="Graffiti FileBrowser" %>
<%@ Register TagPrefix="Z" TagName="FileBrowser" Src="~/graffiti-admin/site-options/utilities/FileBrowser.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="overflow: hidden;">
<head runat="server">
	<link href="../../common/css/main.css" runat="Server" type="text/css" media="all" rel="Stylesheet" />
	<%= new Macros().GraffitiJavaScript %>
</head>
<body style="overflow: hidden; min-height: 0;">
	<Glow:Modal runat="server" CssClasses="Modal,Modal1,Modal2,Modal3,Modal4,Modal5"
		TitleCssClasses="ModalTitle" CloseCssClasses="ModalClose" ContentCssClasses="ModalContent"
		FooterCssClasses="ModalFooter" ResizeCssClasses="ModalResize" MaskCssClasses="ModalMask"
		LoadingUrl="~/graffiti-admin/loading.htm" />
	<form id="ModalMasterForm" runat="server">
	<div id="modalcontent" style="min-height: 0;">
		<div class="modalgutter" style="margin-top: -15px; min-height: 0;">

			<script type="text/javascript">
				// <!--
				function selectFile(url) {
					window.opener.CKEDITOR.tools.callFunction(<%= Request.QueryString["CKEditorFuncNum"] ?? "window.top.opener.CKEDITOR.dialog.getCurrent().getParentEditor()._.filebrowserFn" %>, url);
					window.top.close();
					window.top.opener.focus();
				}
				// -->
			</script>

			<Z:FileBrowser runat="server" OnClientFileClickedFunction="selectFile" IncludeUtilityBreadCrumbs="false"
				ContentHeightOffset="70" EnableResizeToHeight="true" />
		</div>
	</div>
	</form>
</body>
</html>
