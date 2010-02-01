<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminModal.master" Title="Upload File" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Text" %>
<%@ Import Namespace="Graffiti.Core" %>

<asp:Content ContentPlaceHolderID="HeaderRegion" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyRegion" runat="server">

<div class="ModalFormBlock">

    <asp:Label ID="lblError" runat="server" style="color: Red;" />
    
    <h2>File:</h2>
    <asp:FileUpload ID="FileUploader" runat="server" />
      
    <div class="submit">
        <asp:Button ID="UploadFile" Text="Upload" runat="server" OnClick="Upload_Click" />
        <a href="javascript:window.parent.Telligent_Modal.Close();">(Cancel)</a>
    </div>
</div>

<script runat="Server">

    protected void Upload_Click(object sender, EventArgs e)
    {
        string fileName = String.Empty;
        
        if (!FileUploader.HasFile)
        {
            lblError.Text = "Select a file to upload.";
            return;
        }

        string theme = Request.QueryString["theme"];

        string themePath = Server.MapPath("~/files/themes/" + theme + "/");
        string currentFile = Path.Combine(themePath, FileUploader.FileName);

        // make sure the file doesn't exist
        FileInfo fi = new FileInfo(currentFile);

        if (!FileFilters.IsEditable(currentFile))
        {
            lblError.Text = "This file name is not an Editable type. Have you configured Graffiti:FileBrowser:Editable in settings.config?";
            return;
        }

        if (fi.Exists)
        {
            lblError.Text = "This file already exist.";
            return;
        }

        try
        {
            int fileLength = FileUploader.PostedFile.ContentLength;

            Byte[] input = new Byte[fileLength];
            Stream stream = FileUploader.FileContent;
            stream.Read(input, 0, fileLength);

            UTF8Encoding utf = new UTF8Encoding();
            string encodedFile = utf.GetString(input);

            Util.CreateFile(currentFile, encodedFile);

            VersionStore.VersionFile(fi);

            ClientScript.RegisterStartupScript(this.GetType(), "close-and-postback",
                    "window.parent.location.href = 'EditTheme.aspx?theme=" + Request.QueryString["theme"] +
                    "&file=" + FileUploader.FileName + "'", true);
        }
        catch (Exception exc)
        {
            lblError.Text = "Error uploading file.";
        }
    }
   
</script>

</asp:Content>
