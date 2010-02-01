<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" Title="Presentation" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Graffiti.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
<script language="javascript">
function refresh()
{
    window.location.href = window.location.href;
}

function OpenUploadThemeFileModal()
{
    Telligent_Modal.Open('UploadTheme.aspx', 415, 300, null);
}

function OpenCreateThemeFileModal()
{
    Telligent_Modal.Open('CreateNewTheme.aspx', 375, 200, null);
}
</script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

    <h1>Your Site's Presentation</h1>
    <Z:Breadcrumbs runat="server" SectionName="Themes" />
    <Z:StatusMessage runat="Server" ID="Message" />

    <asp:HyperLink runat="server" Text="Create New Theme" NavigateUrl="javascript:OpenCreateThemeFileModal();" runat="Server" />
    <span style="padding: 0 4px 0 4px;">|</span>
    <asp:LinkButton Text="Upload Theme" runat="Server" OnClientClick="OpenUploadThemeFileModal();return false;" />
    <span style="padding: 0 4px 0 4px;">|</span>
    <asp:HyperLink runat="server" NavigateUrl="javascript:Telligent_Modal.Open('Catalog.aspx', 600, 475, refresh);" Text="Search online themes..." />
    <div style="text-align: center;">
    <asp:DataList runat="Server" ID="Theme_List" RepeatColumns="2" RepeatDirection="Horizontal" style="margin: auto;">
        <ItemTemplate>
            <div class="listboxes">
            <div <%# IsSelectedTheme(Eval("Name").ToString()) %> style="width: 278px; margin: 25px;">
                <div class="title" style="font-size: 100%; padding-top: 3px; text-align: left; width: 100%;">
                   <%# Eval("Name") %><span style="font-size: 80%; vertical-align: 1px; text-align:right;"><%# IsCurrentTheme(Eval("Name").ToString()) %></span>
                </div>
                <div class="body" style="padding: 4px; height: 206px;">
                   <img src='<%# GetPreviewImage(Eval("Name").ToString()) %>' height="206" width="270" style="display:none;" />
                   <asp:ImageButton ToolTip="Click to set as your current theme" ID="ImageButton1" ImageUrl='<%# GetPreviewImage(Eval("Name").ToString())  %>' Height="206" Width="270" runat="Server" OnClick="SetCurrentTheme" CommandArgument = '<%# Eval("Name") %>' />
                </div>
                <div class="commands">
                     <%# IsConfigurable(Eval("Name").ToString()) %>
                     <a href="EditTheme.aspx?Theme=<%# Eval("Name") %>">Personalize</a> |
                   <span <%# IsDeleteVisible(Eval("Name").ToString()) %>><asp:LinkButton ID="DeleteTheme" runat="server" Text="Delete" OnCommand="DeleteTheme_Click" CommandArgument=<%# Eval("Name") %> /> |</span> <a href="download.ashx?Theme=<%# Eval("Name") %>">Share</a>
                </div>
            </div>
            </div>
        </ItemTemplate>
    </asp:DataList>
    </div>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
    <div id="sidebar"><div class="gutter">
    <div class="box">
        <asp:PlaceHolder ID="About_Panel" runat="server">
            <h3>About This Page</h3>
            <p>This page is used to allow you to modify the look and feel of the website, change the 
            theme your site uses or create an entirely new theme all together.</p>
        </asp:PlaceHolder>
    </div>
    <div style="clear: both;"></div>
    </div></div>
</asp:Content>

<script runat="Server">

    private SiteSettings settings;
    
    void Page_Load(object sender, EventArgs e)
    {
        settings = SiteSettings.Get();
        LiHyperLink.SetNameToCompare(Context,"presentation");
        
        if (!IsPostBack)
        {
            LoadThemes();
            CheckFilePermissions();

            if (!String.IsNullOrEmpty(Request.QueryString["theme"]))
            {
                Message.Text = Request.QueryString["theme"] + " has been uploaded.";
                Message.Type = StatusType.Success;
            }
        }
    }
    
    void SetCurrentTheme(object sender, EventArgs e)
    {
        LinkButton lb = sender as LinkButton;
        string theme_name = null;
        if(lb != null)
        {
           
            theme_name = lb.CommandArgument;
        }
        else
        {
            ImageButton ib = sender as ImageButton;
            if (ib != null)
                theme_name = ib.CommandArgument;
        }
        
        if(theme_name != null)
        {
            SiteSettings settings = SiteSettings.Get();
            settings.Theme = theme_name;
            settings.Save();
            Response.Redirect("~/graffiti-admin/presentation/themes/");      
        }
       
    }
    
    void CheckFilePermissions()
    {
        string testFile = Server.MapPath("~/files/themes/test.txt");
        
        
        try
        {
            File.AppendAllText(testFile, "Please feel free to delete this file.");
        }
        catch (UnauthorizedAccessException)
        {
            Message.Text = "Please make sure you have write permissions to the /files/themes/ directory on your web server.";
            Message.Type = StatusType.Error;

            return;
        }
        catch (Exception)
        {
            Message.Text = "Unable to write to the /files/themes/ directory on your web server. Please contact your administrator for more information.";
            Message.Type = StatusType.Error;

            return;
        }

        File.Delete(testFile);

    }

    
    void LoadThemes()
    {
        ArrayList themes = new ArrayList();
        
                
        DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/files/themes/"));
        foreach (DirectoryInfo diChild in di.GetDirectories())
        {
            if (!diChild.Name.StartsWith("_svn") && !diChild.Name.StartsWith(".svn"))
            {
                Theme theme = new Theme(Server.MapPath("~/files/themes/"), diChild.Name);

                themes.Add(theme);
            }
        }

        ArrayList sortedThemes = new ArrayList();
        
        // find the selected theme and add it
        foreach (Theme t in themes)
        {
            if (t.Name == settings.Theme)
                sortedThemes.Add(t);
        }
        
        // add the rest
        foreach (Theme t in themes)
        {
            if (t.Name != settings.Theme)
                sortedThemes.Add(t);
        }
        
        Theme_List.DataSource = sortedThemes;
        Theme_List.DataBind();
    }

    string IsSelectedTheme(string currentTheme)
    {
        if (currentTheme == settings.Theme)
            return "class=\"nonnestedselected\"";

        return "class=\"nonnested\"";
    }

    string IsCurrentTheme(string currentTheme)
    {
        if (currentTheme == settings.Theme)
            return "&nbsp;&nbsp;&nbsp;(Current Theme)";

        return "";
    }

    string IsDeleteVisible(string currentTheme)
    {
        if (currentTheme == settings.Theme)
            return "style=\"display: none;\"";

        return String.Empty;
    }
    
    string IsConfigurable(string currentTheme)
    {
        FileInfo file = new FileInfo(Server.MapPath("~/files/themes/" + currentTheme + "/theme.config"));

        if (file.Exists)
            return "<a href=\"ConfigureTheme.aspx?Theme=" + currentTheme + "\">Configure</a> | ";
        else
            return String.Empty;
    }

    string GetPreviewImage(string themeName)
    {
        FileInfo file = new FileInfo(Server.MapPath("~/files/themes/" + themeName + "/preview.png"));
        
        if(file.Exists)
            return "~/files/themes/" + themeName + "/preview.png";
        else
            return "~/graffiti-admin/common/img/npa.png";
    }

    void DeleteTheme_Click(object sender, CommandEventArgs e)
    {
        try
        {
            DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/files/themes/" + e.CommandArgument));

            if (di.Exists)
            {
                di.Delete(true);
            }

            LoadThemes();

            Message.Text = e.CommandArgument + " was deleted.";
            Message.Type = StatusType.Success;
        }
        catch (Exception)
        {
            Message.Text = "Could not delete " + e.CommandArgument + ". Is a file in use?";
            Message.Type = StatusType.Error;
        }
    }
    
</script>