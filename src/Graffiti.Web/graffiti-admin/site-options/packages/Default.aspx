<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" Title="Graffiti Packages" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<%@ Import namespace="System.Collections.Generic"%>
<script runat="Server">

protected void Page_Load(object sender, EventArgs e)
{
    LiHyperLink.SetNameToCompare(Context, "settings");

    if (!Page.IsPostBack)
    {
        string packageSaved = Request.QueryString["pu"];

        if (!String.IsNullOrEmpty(packageSaved))
        {
            Message.Text = "The package <strong>" + Server.UrlDecode(packageSaved) + "</strong> was uploaded.";
            Message.Type = StatusType.Success;
        }

        PackageList.DataSource = PackageSettings.Get().Packages;
        PackageList.DataBind();
    }
}

protected void DeletePackage(object sender, CommandEventArgs e)
{
    string package = e.CommandArgument.ToString();

    if (!String.IsNullOrEmpty(package))
    {
        if (PackageSettings.RemovePackage(package))
        {
            Message.Text = "The package <strong>" + package + "</strong> was deleted.";
            Message.Type = StatusType.Success;
        }
        else
        {
            Message.Text = "The package <strong>" + package + "</strong> could not be found.";
            Message.Type = StatusType.Error;
        }
    }

    PackageList.DataSource = PackageSettings.Get().Packages;
    PackageList.DataBind();
}

</script>
<asp:Content ContentPlaceHolderID="MainRegion" Runat="Server">

<script language="javascript" type="text/javascript">

function OpenUploadPackageFileModal()
{
    Telligent_Modal.Open('UploadPackage.aspx', 435, 330, null);
}

</script>

<h1>Packages</h1>
<Z:Breadcrumbs runat="server" SectionName="Packages" />
<Z:StatusMessage runat="server" ID="Message" />

<h3>Installed Packages</h3>
<asp:HyperLink runat="server" Text="Upload Package" NavigateUrl="javascript:OpenUploadPackageFileModal();" />
<!--
<span style="padding: 0 4px 0 4px;">|</span>
<asp:HyperLink runat="server" NavigateUrl="javascript:Telligent_Modal.Open('Catalog.aspx', 600, 475, refresh);" Text="Search online packages..." />
-->
<Z:Repeater runat="Server" ShowHeaderFooterOnNone="false" ID="PackageList">
    <NoneTemplate>
        <z:StatusMessage runat="Server" Text="Sorry, there are no packages to manage." Type="Warning" />
    </NoneTemplate>
    <HeaderTemplate>
        <br /><br />
        <ul class="listboxes">
    </HeaderTemplate>
    <ItemTemplate>
        <li>
            <div class="nonnested">
                <div class="title"><%# Eval("Name") %> <%# Eval("Version") %></div>
                <div class="commands">
                    <asp:linkbutton ID="DeletePackage" CommandArgument='<%# Eval("Name").ToString() %>' CommandName="Delete" runat="server" OnCommand="DeletePackage" Text="Delete" />    
                </div>
                <div class="body">
                    <asp:Repeater id="fileList" DataSource='<%# Eval("Files") %>' runat="server">
                    <ItemTemplate>
                        <%# Container.DataItem %>
                        <br />
                    </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div class="title" style="font-weight: normal; font-size: 90%;">Installed <%# Convert.ToDateTime(Eval("DateInstalled")).ToShortDateString() %></div>
            </div>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</Z:Repeater>

</asp:Content>
<asp:Content ContentPlaceHolderID="SideBarRegion" Runat="Server">
    <div id="sidebar"><div class="gutter">
    <div class="box">
        <h3>Deleting a Package</h3>
        <p>When you delete a package, any original Graffiti files the package replaced will be restored.
        </p>
    </div>
    <div class="box">
        <h3>Create a Package</h3>
        <p>To create your own package, setup the directory structure for how you want your files installed in Graffiti. <br /><br />
        For Example, create a directory called <i>C:\MyGraffitiPackage\Bin</i>. In that directory, add <i>MyChalkWidget.dll</i> and <i>MyPlugIn.dll</i>.
        Run the package utility that ships with Graffiti at the command line:
        <i>Package MyPackage "C:\MyGraffitiPackage" "/"</i><br /><br />
        After your files are packaged, simply upload your package to this page to install your plugin and chalk widget!
        </p>
    </div>
    <div style="clear: both;"></div>
    </div></div>
</asp:Content>

