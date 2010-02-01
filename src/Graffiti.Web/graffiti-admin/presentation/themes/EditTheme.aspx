<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<%@ Import namespace="DataBuddy"%>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Collections.Generic" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
<script language="javascript">
function OpenAddNewFileModal()
{
    Telligent_Modal.Open('AddFile.aspx?theme=<%= Request.QueryString["Theme"] %>', 320, 200, null);
}

function OpenUploadFileModal()
{
    Telligent_Modal.Open('UploadFile.aspx?theme=<%= Request.QueryString["Theme"] %>', 320, 200, null);
}

function scrollUp(id, scrollBy)
{
	var element = document.getElementById(id);
	if (element)
	{
		if (element.scrollTop > scrollBy)
			element.scrollTop -= scrollBy;
		else if (element.scrollTop > 0)
			element.scrollTop = '0';
	}
}

function scrollDown(id, scrollBy)
{
	var element = document.getElementById(id);
	if (element)
	{
		if (element.scrollHeight - (element.offsetHeight + element.scrollTop) > scrollBy)
			element.scrollTop += scrollBy;
		else if (element.scrollHeight - (element.offsetHeight + element.scrollTop) > 0)
			element.scrollTop = element.scrollHeight - element.offsetHeight;
	}
}

function ToggleScrollOn()
{
    document.getElementById('topscroll').style.display = 'block';
    document.getElementById('bottomscroll').style.display = 'block';
}

function ToggleScrollOff()
{
    document.getElementById('topscroll').style.display = 'none';
    document.getElementById('bottomscroll').style.display = 'none';
}

</script>
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

<asp:Content ContentPlaceHolderID="MainRegion" runat="Server">
    
    <h1>Edit Theme</h1>
    
    <Z:Breadcrumbs runat="server" SectionName="ThemeEdit" />
    <Z:StatusMessage runat="Server" ID="Message" />
    
    <div class="filelistbox" <%= IsSelectedFileVM() %>>
        <div id="spacer" runat="server" style="height: 23px;"></div>
        <div id="topscroll" style="display: none; background-color: #eee; border: solid 1px #ddd; text-align:center; font-size: 85%; font-weight: bold; cursor: pointer; text-decoration: underline;" onclick="scrollUp('scrollArea',20);">scroll up</div>
        <asp:Repeater ID="File_List" runat="Server">
            <HeaderTemplate>
                <div id="scrollArea" style="height: 300px; overflow: hidden;">
                <ul class="filelist">
            </HeaderTemplate>
            <ItemTemplate>
                <li<%# IsSelectedFile(Eval("FileName").ToString()) %>><asp:LinkButton ID="OpenFile" runat="server" Text=<%# Eval("FileName") %> OnCommand="OpenFile_Click" CommandArgument=<%# Eval("FileName") %> /></li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <div id="bottomscroll" style="display: none; background-color: #eee; font-weight: bold; cursor: pointer; border: solid 1px #ddd; text-align:center; font-size: 85%; text-decoration: underline;" onclick="scrollDown('scrollArea',20);">scroll down</div>

        <div style="margin-top: 20px; text-align: center;">
            <asp:Button ID="AddNewFile" Text="Create new file" runat="server" OnClientClick="OpenAddNewFileModal();return false;" />
            <div style="font-style: italic; font-weight: bold;">or</div>
            <asp:Button ID="Button1" Text="Upload file" runat="server" OnClientClick="OpenUploadFileModal();return false;" />
        </div>
    </div>
    
    <div id="FileEditorBox" runat="server" class="fileeditorbox">
        
        <div id="VersionHistoryArea" runat="server" style="text-align: right; padding-bottom: 5px; padding-right: 3px;">
            <Glow:DropDownList runat="server" ID="VersionHistory" SelectListWidth="300" Width="150px" ShowHtmlWhenSelected="false" />
        </div>
    
        <Z:Menu id="Menu" runat="server" Editor="File_Contents" />
        
        <asp:TextBox ID="File_Contents" Width="99%" Height="430" TextMode="MultiLine" runat="Server" /><br />
        
        <div class="submit">
            <div style="float: left;">
                <asp:Button ID="Save_Changes" Text="Save Changes" OnClick="SaveChanges_Click" runat="server" />
                <asp:LinkButton ID="DeleteFile" Text="(Delete this file)" runat="Server" OnClientClick="return confirm('Are you sure you want to delete this file?\nThis cannot be undone.');" OnClick="DeleteFile_Click" />
            </div>
        </div>
    </div>
    
    <div id="InstructionsBox" class="fileeditorbox" runat="server" style="padding-left: 20px; width: 75%;">
        <h3>Editing Files</h3>
        <p>On the left side of this screen you will find all files for for the theme <b><%= Request.QueryString["theme"] %></b>. Simply click the file to edit the contents. Graffiti will automatically version the file when you click save.</p>
        <h3>Creating files</h3>
        <p>Click "Create a new file" to create a new file for this theme. You can link to this file in any way you choose.</p>
        <h3>Uploading Files</h3>
        <p>Click "Upload file" to add an existing file to this theme.</p>
    </div>
 
    <asp:HiddenField ID="CurrentFile" runat="server" />
</asp:Content>

<script runat="Server">

    private string _theme = string.Empty;
    private string version;
    
    void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context, "presentation");
        
        _theme = Request.QueryString["theme"];
        
        if (_theme.Trim() == "") Response.Redirect("Default.aspx");
        

        if (!IsPostBack)
        {
            FileEditorBox.Visible = false;
            LoadFiles();
        }
    }

    void LoadFiles()
    {
        Theme theme = new Theme(Server.MapPath("~/files/themes/"), _theme);

        if (!IsPostBack)
        {
            Util.CanWriteRedirect(Context);
            
            if (theme.Files.Count > 0 && Request.QueryString["file"] != null)
            {
                ThemeFile addedFile = theme.Files.Find(
                                                delegate(ThemeFile file)
                                                {
                                                    return file.FileName == Request.QueryString["file"];
                                                });

                if (addedFile != null)
                {
                    CurrentFile.Value = addedFile.FileName;
                    LoadFileContents();
                }
            }
        }

        if (theme.Files.Count > 11)
            ClientScript.RegisterStartupScript(this.GetType(), "toggleScrollOn", "ToggleScrollOn()", true);
        else
            ClientScript.RegisterStartupScript(this.GetType(), "toggleScrollOn", "ToggleScrollOff()", true);

        File_List.DataSource = theme.Files;
        File_List.DataBind();
    }
    
    void LoadFileContents()
    {
        FileEditorBox.Visible = true;
        InstructionsBox.Visible = false;
        
        string file = CurrentFile.Value;
        
        string themePath = Server.MapPath("~/files/themes/" + _theme + "/");
        string filePath = Path.Combine(themePath, file);

        FileInfo fi = new FileInfo(filePath);

        if (!fi.FullName.ToLower().StartsWith(themePath.ToLower()))
        {
           Response.Redirect("~/graffiti-admin/presentation/");
        }

        if (!fi.Exists)
        {
            Response.Redirect("~/graffiti-admin/presentation/");
        }
        
        if (filePath.ToLower().EndsWith(".view"))
        {
            Menu.Visible = true;
        }
        else
        {
            Menu.Visible = false;
        }

        string ext = Path.GetExtension(filePath).Substring(1);
        VersionStoreCollection vsc = VersionStore.GetVersionHistory(filePath);

        if (vsc.Count > 1)
        {
            VersionHistoryArea.Visible = true;
            spacer.Visible = true;

            string versionHtml =
                "<div style=\"width: 280px; overflow: hidden; padding: 6px 0; border-bottom: 1px solid #ccc;\"><b>Revision {0}</b> ({1})<div>by {2}</div><div style=\"font-style: italic;\"></div></div>";
            string versionText = "Revision {0}";
            foreach (VersionStore vs in vsc)
            {
                VersionHistory.Items.Add(
                    new DropDownListItem(
                        string.Format(versionHtml, vs.Version, vs.CreatedOn, vs.CreatedBy),
                        string.Format(versionText, vs.Version), vs.Version.ToString()));
            }
            
            VersionHistory.Attributes["onchange"] = "window.location = '" +
                                                    ResolveUrl("~/graffiti-admin/presentation/themes/EditTheme.aspx") +
                                                    "?theme=" + Request.QueryString["theme"] +
                                                    "&file=" + file +
                                                    "&v=' + this.options[this.selectedIndex].value;";

            VersionStore selected;
            
            if (!String.IsNullOrEmpty(Request.QueryString["v"]))
            {
                if(!String.IsNullOrEmpty(version))
                {
                    selected = vsc.Find(
                        delegate(VersionStore vs)
                        {
                            return vs.Version.ToString() == version;
                        });
                }
                else
                {
                    selected = vsc.Find(
                        delegate(VersionStore vs)
                        {
                            return vs.Version.ToString() == Request.QueryString["v"];
                        });
                }
            }
            else
            {
                selected = vsc[vsc.Count - 1];
            }

            if (selected != null)
            {
                VersionHistory.SelectedValue = selected.Version.ToString();
                File_Contents.Text = selected.Data;

                if (selected.Version < vsc[vsc.Count - 1].Version)
                {
                    Message.Text = "You are editing a previous version of this file. If you click <b>Save Changes</b>, a new version of this file (revision " + (vsc.Count + 1) + ") will be created.";
                    Message.Type = StatusType.Warning;
                }
            }
            
            
        }
        else
        {
            VersionHistoryArea.Visible = false;
            spacer.Visible = false;
            
            string[] fileContents = File.ReadAllLines(filePath);

            File_Contents.Text = "";
            foreach (string line in fileContents)
            {
                File_Contents.Text = File_Contents.Text + line + "\n";
            }
        }
    }

    void SaveChanges_Click(object sender, EventArgs e)
    {
        string file = CurrentFile.Value;
        
        string themePath = Server.MapPath("~/files/themes/" + _theme + "/");
        string currentFile = Path.Combine(themePath, file);
        
        FileInfo fi = new FileInfo(currentFile);
        
        if(!fi.FullName.ToLower().StartsWith(themePath.ToLower()))
        {
            Message.Text = "Unable to save changes. You cannot edit files outside of the current theme directory";
            Message.Type = StatusType.Error;
            return;
        }

        if(!fi.Exists)
        {
            Message.Text = "Unable to save changes. Your file does not exist";
            Message.Type = StatusType.Error;
            return;
        }

        try
        {

            if (VersionStore.CurrentVersion(fi) == 0 && File.ReadAllText(currentFile) != "[feed me content!]")
                VersionStore.VersionFile(fi);
                
            using (StreamWriter sw = new StreamWriter(currentFile, false))
            {
                sw.Write(File_Contents.Text);
                sw.Close();
            }

            VersionStore.VersionFile(fi);

            version = VersionStore.CurrentVersion(fi).ToString();
            
            LoadFiles();
            LoadFileContents();

            Message.Text = "Your changes have been saved!";
            Message.Type = StatusType.Success;
        }
        catch (Exception ex)
        {
            Message.Text = "Unable to save changes. Please contact your administrator. Reason: " + ex.Message;
            Message.Type = StatusType.Error;
        }
    }

    void DeleteFile_Click(object sender, EventArgs e)
    {
        string file = CurrentFile.Value;

        string themePath = Server.MapPath("~/files/themes/" + _theme + "/");
        string filePath = Path.Combine(themePath, file);

        FileInfo fi = new FileInfo(filePath);
        fi.Delete();

        VersionStore.Destroy(VersionStore.Columns.Name, filePath);

        Response.Redirect(Request.RawUrl);
    }

    void OpenFile_Click(object sender, CommandEventArgs e)
    {
        string url = ResolveUrl("~/graffiti-admin/presentation/themes/EditTheme.aspx") +
        "?theme=" + Request.QueryString["theme"] +
        "&file=" + e.CommandArgument.ToString();

        Response.Redirect(url);
    }

    string IsSelectedFile(string fileName)
    {
        if (fileName == CurrentFile.Value)
            return " class=\"selected\"";
        else
            return string.Empty;
    }

    string IsSelectedFileVM()
    {
        if (CurrentFile.Value.EndsWith(".view"))
            return " style=\"padding-top: 23px\"";
        else
            return " style=\"padding-top: 1px\"";
    }
    
</script>
