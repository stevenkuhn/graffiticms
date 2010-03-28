<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminModal.master" Title="Upload Theme" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Text" %>
<%@ Import Namespace="Graffiti.Core" %>

<asp:Content ContentPlaceHolderID="HeaderRegion" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyRegion" runat="server">

<div class="ModalFormBlock">

    <asp:Label ID="lblError" runat="server" style="color: Red;" />
    
    <h2>Theme XML file:</h2>
    <asp:FileUpload ID="ThemeUploader" runat="server" />
   
    <h2>Theme Name: <span style="color: #777; font-size: 80%;">(optional)</span></h2>
    <asp:TextBox runat="Server" id="txtThemeName" TabIndex="1" Columns="32" MaxLength="256" />
    <h2></h2>
    <asp:CheckBox ID="OverrideFiles" runat="Server" Text="Override existing files" />
    <h2 style="font-size: 90%;"><i>Only enter the Theme Name if you want to override the value from the uploaded file.</i></h2>
   
    <div class="submit">
        <asp:Button ID="UploadTheme" Text="Upload" runat="server" OnClick="Upload_Click" />
        <a href="javascript:window.parent.Telligent_Modal.Close();">(Cancel)</a>
    </div>
</div>

<script runat="Server">
    
    void Page_Load(object sender, EventArgs e)
    {
        txtThemeName.Focus();
    }

    protected void Upload_Click(object sender, EventArgs e)
    {
        string themeName = String.Empty;
        
        if (!ThemeUploader.HasFile)
        {
            lblError.Text = "Select a theme XML file to upload.";
            return;
        }
        else
        {
            try
            {
                int fileLength = ThemeUploader.PostedFile.ContentLength;

                Byte[] input = new Byte[fileLength];
                Stream stream = ThemeUploader.FileContent;
                stream.Read(input, 0, fileLength);

                UTF8Encoding utf = new UTF8Encoding();
                string encodedFile = utf.GetString(input);

                if (String.IsNullOrEmpty(txtThemeName.Text))
                    themeName = ThemeConverter.ToDisk(encodedFile, Server.MapPath("~/files/themes/"), OverrideFiles.Checked, null);
                else
                    themeName = ThemeConverter.ToDisk(encodedFile, Server.MapPath("~/files/themes/"), OverrideFiles.Checked, txtThemeName.Text);

                ClientScript.RegisterStartupScript(this.GetType(), "close-and-postback",
                            "window.parent.location.href = \"Default.aspx?theme=" + themeName + "\";", true);
            }
            catch (Exception exc)
            {
                lblError.Text = "Error uploading theme. Make sure you have checked override existing files if the theme already exists and that you have selected a valid theme XML file.";
            }
        }
    }
   
</script>

</asp:Content>
