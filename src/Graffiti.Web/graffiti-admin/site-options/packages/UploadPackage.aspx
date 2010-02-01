<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminModal.master" Title="Upload Package" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Text" %>
<%@ Import Namespace="Graffiti.Core" %>

<asp:Content ContentPlaceHolderID="HeaderRegion" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyRegion" runat="server">

<div class="ModalFormBlock">

    <asp:Label ID="lblError" runat="server" style="color: Red;" />
    
    <h2>Package file:</h2>
    <asp:FileUpload ID="PackageUploader" runat="server" />
   
    <h2>Package Name: <span style="color: #777; font-size: 80%;">(optional)</span></h2>
    <asp:TextBox runat="Server" id="txtPackageName" TabIndex="1" Columns="32" MaxLength="256" />
    <h2 style="font-size: 90%;"><i>Only enter the Package Name if you want to override the value from the uploaded file.</i></h2>

    <h2></h2>
    <asp:CheckBox ID="OverrideFiles" runat="Server" Text="Override existing files" />
    <h2 style="font-size: 90%;"><i>Any files that are overwritten will be restored if you uninstall this package.</i></h2>
   
    <div class="submit">
        <asp:Button ID="UploadPackage" Text="Upload" runat="server" OnClick="Upload_Click" />
        <a href="javascript:window.parent.Telligent_Modal.Close();">(Cancel)</a>
    </div>
</div>

<script runat="Server">
    
    void Page_Load(object sender, EventArgs e)
    {
        txtPackageName.Focus();
    }

    protected void Upload_Click(object sender, EventArgs e)
    {
        string packageName = String.Empty;

        if (!PackageUploader.HasFile)
        {
            lblError.Text = "Select a file to upload.";
            return;
        }
        else
        {
            try
            {
                int fileLength = PackageUploader.PostedFile.ContentLength;

                Byte[] input = new Byte[fileLength];
                Stream stream = PackageUploader.FileContent;
                stream.Read(input, 0, fileLength);

                UTF8Encoding utf = new UTF8Encoding();
                string encodedFile = utf.GetString(input);

                if (String.IsNullOrEmpty(txtPackageName.Text))
                    packageName = PackageSettings.ToDisk(encodedFile, OverrideFiles.Checked, null);
                else
                    packageName = PackageSettings.ToDisk(encodedFile, OverrideFiles.Checked, txtPackageName.Text);

                ClientScript.RegisterStartupScript(this.GetType(), "close-and-postback",
                            "window.parent.location.href = \"Default.aspx?pu=" + Server.UrlEncode(packageName) + "\";", true);
            }
            catch (Exception exc)
            {
                lblError.Text = "Error uploading Package. " + exc.Message;
            }
        }
    }
   
</script>

</asp:Content>
