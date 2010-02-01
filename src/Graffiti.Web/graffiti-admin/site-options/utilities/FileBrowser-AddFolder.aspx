<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminModal.master" Title="Create A Folder" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<%@ Import Namespace="System.IO" %>
<%@ Import namespace="DataBuddy"%>

<asp:Content ContentPlaceHolderID="HeaderRegion" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyRegion" runat="server">

<div class="ModalFormBlock">

    <asp:Label ID="lblError" runat="server" style="color: Red;" />

    <h2>Folder name: <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtTitle" Text="Please enter a folder anme" runat ="Server" /></h2>
    <asp:TextBox CssClass = "large" runat="Server" id="txtTitle" TabIndex="1" MaxLength="256" />
   
    <div class="submit">
        <asp:Button ID="Save" Text="Create Folder" runat="server" OnClick="Save_Click" />
        <a href="javascript:window.parent.Telligent_Modal.Close();">(Cancel)</a>
    </div>
</div>

<script runat="Server">
    
    void Page_Load(object sender, EventArgs e)
    {
        txtTitle.Focus();
    }

    protected void Save_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {

            string rootPath = Server.MapPath("~/");
            string path = Request.QueryString["path"] ?? "";
            path = Path.Combine(rootPath, path);

            DirectoryInfo di = new DirectoryInfo(path);

            if (!di.FullName.ToLower().StartsWith(rootPath.ToLower()))
            {
                Log.Error("FileBrowser", "A request was made to an invalid directory {0}. If this persists, you should contact your ISP", di.FullName);
                throw new Exception("Bad Path");
            }

            string newFolderName = txtTitle.Text.Trim();
            
            DirectoryInfo diNewFolder = new DirectoryInfo(Path.Combine(di.FullName,newFolderName));

            if (!diNewFolder.FullName.ToLower().StartsWith(rootPath.ToLower()))
            {
                Log.Error("FileBrowser", "A request was made to an invalid directory {0}. If this persists, you should contact your ISP", di.FullName);
                throw new Exception("Bad Path");
            }

            diNewFolder.Create();

            string newPath = Request.QueryString["path"] ?? "";
            if (string.IsNullOrEmpty(newPath))
                newPath = newFolderName;
            else
                newPath += "\\" + newFolderName;
   
            ClientScript.RegisterStartupScript(this.GetType(), "close-and-postback",
                    "window.parent.location = window.parent.location;", true);
        }
        
    }
   
</script>

</asp:Content>
