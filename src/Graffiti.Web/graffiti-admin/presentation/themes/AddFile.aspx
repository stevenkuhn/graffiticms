<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminModal.master" Title="Add a new file" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<%@ Import Namespace="System.IO" %>
<%@ Import namespace="DataBuddy"%>

<asp:Content ContentPlaceHolderID="HeaderRegion" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyRegion" runat="server">

<div class="ModalFormBlock">

    <asp:Label ID="lblError" runat="server" style="color: Red;" />

    <h2>File name: <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtTitle" Text="Please enter a filename" runat ="Server" /></h2>
    <asp:TextBox CssClass = "large" runat="Server" id="txtTitle" TabIndex="1" MaxLength="256" />
   
    <div class="submit">
        <asp:Button ID="Save" Text="Save file" runat="server" OnClick="Save_Click" />
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
        string theme = Request.QueryString["theme"];
        
        string themePath = Server.MapPath("~/files/themes/" + theme + "/");
        string currentFile = Path.Combine(themePath, txtTitle.Text);

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
        
        Util.CreateFile(currentFile, "[feed me content!]");
     
        ClientScript.RegisterStartupScript(this.GetType(), "close-and-postback", 
                "window.parent.location.href = 'EditTheme.aspx?theme=" + Request.QueryString["theme"] + 
                "&file=" + txtTitle.Text + "'", true);
        
    }
   
</script>

</asp:Content>
