<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminModal.master" Title="Create New Theme" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Text" %>
<%@ Import Namespace="Graffiti.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="BodyRegion" runat="server">

<div class="ModalFormBlock">

    <asp:Label ID="lblError" runat="server" style="color: Red;" />
   
    <h2>Theme Name: <asp:RequiredFieldValidator ControlToValidate="txtThemeName" Text="Please enter a theme name" runat ="Server" /></h2>
    <asp:TextBox runat="Server" id="txtThemeName" TabIndex="1" Columns="32" MaxLength="256" />
    <h2 style="font-size: 90%;"><i>This theme will be created as defined in Graffiti/Web/__utility/themetemplate.xml</i></h2>

    <div class="submit">
        <asp:Button ID="CreateTheme" Text="Create" runat="server" OnClick="CreateTheme_Click" />
        <a href="javascript:window.parent.Telligent_Modal.Close();">(Cancel)</a>
    </div>
</div>

<script runat="Server">
    
    void Page_Load(object sender, EventArgs e)
    {
        txtThemeName.Focus();
    }

    protected void CreateTheme_Click(object sender, EventArgs e)
    {
        string themeName = String.Empty;

        try
        {
            string pathAndFile = Path.Combine(Server.MapPath("~/__utility/"), "themetemplate.xml");
            string encodedFile = File.ReadAllText(pathAndFile, Encoding.UTF8);

            themeName = ThemeConverter.ToDisk(encodedFile, Server.MapPath("~/files/themes/"), false, txtThemeName.Text);
            
            ClientScript.RegisterStartupScript(this.GetType(), "close-and-postback",
            "window.parent.location.href = \"EditTheme.aspx?theme=" + themeName + "\";", true);
        }
        catch (Exception exc)
        {
            lblError.Text = "Error creating theme. Is this directory in use?";
        }
    }
   
</script>

</asp:Content>
