<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" CodeBehind="FileBrowser.aspx.cs" Inherits="Graffiti.Web.graffiti_admin.site_options.utilities.FileBrowser" Title="Graffiti FileBrowser" %>
<%@ Register TagPrefix="Z" TagName="FileBrowser" Src="~/graffiti-admin/site-options/utilities/FileBrowser.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<h1>File Browser</h1>
<Z:FileBrowser runat="server" />

</asp:Content>