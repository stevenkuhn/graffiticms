<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileBrowser-UploadFiles.aspx.cs" Inherits="Graffiti.Web.FileBrowser_UploadFiles" MasterPageFile="~/graffiti-admin/common/AdminModal.master" Title="Add Files" %>
<%@ Import Namespace="System.IO" %>
<%@ Import namespace="DataBuddy"%>

<asp:Content ContentPlaceHolderID="HeaderRegion" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyRegion" runat="server">

<div class="ModalFormBlock" runat="server" id="FormWrapper">
    <asp:Label ID="lblError" runat="server" style="color: Red;" />
    
    <script type="text/javascript">
    
    function showError(uploadControl, code, file, message)
    {
        alert('An error occured while selecting or uploading files.\n\nFile:  ' + file.name + '\nMessage:  ' + message);
    }
    
    </script>

    <Glow:MultipleFileUpload runat="server" AllowedFileTypes="*.*" AutoUpload="true" AllowedNumberOfFiles="10" OnUploadErrorClientFunction="showError" AllowedFileTypesDescription="All Files" Width="97%" Height="230px" ID="Files" />
   
    <div class="submit">
        <asp:Button ID="Save" Text="Save" runat="server" OnClick="Save_Click" />
        <a href="javascript:window.parent.Telligent_Modal.Close();">(Cancel)</a>
    </div>
</div>

</asp:Content>