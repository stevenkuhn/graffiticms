using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Graffiti.Core;
using Telligent.Glow;

namespace Graffiti.Web.graffiti_admin.site_options.utilities
{
    public class FileBrowser1 : System.Web.UI.UserControl
    {
		#region private variables

		private string platformFilesMediaPath = Path.Combine("files", "media");

		private string basicFilesMediaPath = @"files\media";

		#endregion

        #region Child Controls

        protected Literal theBreadCrumbs;
        protected Graffiti.Core.Repeater FolderList;
        protected MultiView FileViews;
        protected View FileLists;
        protected System.Web.UI.WebControls.Repeater LeftFiles;
        protected System.Web.UI.WebControls.Repeater RightFiles;
        protected View FileDetails;
        protected HyperLink FileDetailsName;
        protected Literal FileDetailsText;
        protected StatusMessage ActionMessage;
        protected HtmlGenericControl revsionLI;
        protected Literal FileDetailsRevision;
        protected HtmlGenericControl assemblyLI;
        protected Literal FileDetailsAssemblyVersion;
        protected Literal FileDetailsSize;
        protected Literal FileDetailsLastModified;
        protected Button DownloadButton;
        protected HtmlInputButton EditButton;
        protected Button DeleteButton;
        protected View FileEditor;
        protected HyperLink EditFileName;
        protected StatusMessage EditMessage;
        protected TextBox EditBox;
        protected Button Button1;
        protected Telligent.Glow.DropDownList VersionHistory;
        protected HtmlControl VersionHistoryArea;

        #endregion

        #region Public Properties

        public string OnClientFileClickedFunction
        {
            get { return (string)ViewState["OnClientFileClickedFunction"] ?? "null"; }
            set { ViewState["OnClientFileClickedFunction"] = value; }
        }

        public bool IncludeUtilityBreadCrumbs
        {
            get { return (bool)(ViewState["IncludeUtilityBreadCrumbs"] ?? true); }
            set { ViewState["IncludeUtilityBreadCrumbs"] = value; }
        }

        public bool EnableResizeToHeight
        {
            get { return (bool)(ViewState["EnableResizeToHeight"] ?? false); }
            set { ViewState["EnableResizeToHeight"] = value; }
        }

        public int ContentHeightOffset
        {
            get { return (int)(ViewState["ContentHeightOffset"] ?? 0); }
            set { ViewState["ContentHeightOffset"] = value; }
        }

        #endregion

        private string version;

        protected void Page_Load(object sender, EventArgs e)
        {
            LiHyperLink.SetNameToCompare(Context, "settings");

            DeleteButton.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this file? This action cannot be undone!');");

            string rootPath = Server.MapPath("~/");
            string path = Request.QueryString["path"] ?? "";

			if (!path.ToLower().StartsWith(basicFilesMediaPath) && !GraffitiUsers.IsAdmin(GraffitiUsers.Current))
				Response.Redirect(Request.Url.AbsolutePath + "?path=" + basicFilesMediaPath, true);

            path = Path.Combine( rootPath, Util.NormalizePath( path ) );
            string fileName = Request.QueryString["f"];

            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.FullName.ToLower().StartsWith(rootPath.ToLower()))
            {
                Log.Error("FileBrowser", "A request was made to an invalid directory {0}. If this persists, you should contact your ISP", di.FullName);
                throw new Exception("Bad Path");
            }

            SetBreadCrumbs(fileName);
            SetFolders(di, rootPath);

            if (string.IsNullOrEmpty(fileName))
            {
                FileViews.SetActiveView(FileLists);
                SetFileList(di);
            }
            else
            {
                FileInfo fi = new FileInfo(Path.Combine(path, fileName));
                if (!fi.Exists)
                {
                    Log.Warn("FileBrowser", "A requested file {0} does not exist", fi.FullName);
                    throw new Exception("File does not exist");
                }

                if (!FileFilters.IsValidFile(fi.Name))
                {
                    Log.Error("FileBrowser",
                              "A forbidden file {0} was requested by the FileBrowser. Access to view/edit this file has been denied.",
                              fi.FullName);
                    throw new Exception("File does not exist");
                }


                if (Request.QueryString["edit"] != "true")
                {
                    SetFileDetails(fi);
                }
                else
                {
                    SetFileEdit(fi);
                }
            }
        }

        private FileInfo GetFile()
        {
            string path = Request.QueryString["path"] ?? "";
            path = Path.Combine(Server.MapPath("~/"), path);
            string fileName = Request.QueryString["f"];

            return new FileInfo(Path.Combine(path, fileName));
        }

        protected void DeleteFile_Click(object sender, EventArgs e)
        {
            FileInfo file = GetFile();
            if (FileFilters.IsDeletable(file.Name))
            {
                try
                {
                    file.Delete();
                    ActionMessage.Text = "The file " + file.Name + " was deleted";
                    ActionMessage.Type = StatusType.Success;

                    DownloadButton.Visible = false;
                    EditButton.Visible = false;
                    DeleteButton.Visible = false;
                }
                catch (Exception ex)
                {
                    Log.Error("FileBrowser", "The file {0} could not be deleted\n\nReason: {1}", file.FullName, ex.Message);

                    ActionMessage.Text = "The file " + file.Name + " could not be deleted";
                    ActionMessage.Type = StatusType.Error;
                }
            }
        }


        protected void SaveFile_Click(object sender, EventArgs e)
        {
            FileInfo fi = GetFile();
            if (FileFilters.IsEditable(fi.Name) && FileFilters.IsValidFile(fi.Name))
            {
                try
                {
                    bool isversioned = FileFilters.IsVersionable(fi.Name);

                    if (isversioned && VersionStore.CurrentVersion(fi) == 0)
                    {
                        VersionStore.VersionFile(fi);
                    }

                    using (StreamWriter sw = new StreamWriter(fi.FullName, false))
                    {
                        sw.Write(EditBox.Text);
                        sw.Close();
                    }

                    if (isversioned)
                    {
                        fi = GetFile();
                        VersionStore.VersionFile(fi);

                        version = VersionStore.CurrentVersion(fi).ToString();

                        SetVersioning(fi.FullName);
                    }

                    EditMessage.Text = "<strong>" + fi.Name + "</strong> was successfully updated.";
                    EditMessage.Type = StatusType.Success;
                }
                catch (Exception ex)
                {
                    EditMessage.Text = "Your file could not be updated. \n\n Reason: " + ex.Message;
                    EditMessage.Type = StatusType.Error;
                }
            }
        }


        protected void DownloadFile_Click(object sender, EventArgs e)
        {
            FileInfo fi = GetFile();
            if (FileFilters.IsDownloadable(fi.Name))
            {
                Context.Response.AppendHeader("content-disposition", "attachment; filename=" + fi.Name);

                Context.Response.ContentType = Util.GetMapping(fi.Name);
                Context.Response.WriteFile(fi.FullName);
                Context.Response.End();
            }
        }

        private void SetFileEdit(FileInfo fi)
        {
            FileViews.SetActiveView(FileEditor);

            if (!FileFilters.IsEditable(fi.Name))
            {
                Log.Error("FileBrowser", "Invalid attempt to edit the file {0} which is not editable", fi.FullName);
                throw new Exception("File does not exist");
            }

            EditFileName.Text = fi.Name;
            EditFileName.NavigateUrl = "~/" + (Request.QueryString["path"] ?? "") + "/" + fi.Name;

            if (!IsPostBack)
            {
                using (StreamReader sr = new StreamReader(fi.FullName))
                {
                    EditBox.Text = sr.ReadToEnd();
                    sr.Close();
                }

                SetVersioning(fi.FullName);
            }
        }

        private void SetVersioning(string fileName)
        {
            // set up versioning
            VersionStoreCollection vsc = VersionStore.GetVersionHistory(fileName);

            if (vsc.Count > 1)
            {
                VersionHistoryArea.Visible = true;

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
                                                        ResolveUrl(
                                                            "~/graffiti-admin/site-options/utilities/FileBrowser.aspx") +
                                                            "?path=" + Server.UrlEncode(Request.QueryString["path"]) +
                                                            "&f=" + Server.UrlEncode(Request.QueryString["f"]) +
                                                            "&edit=true" +
                                                            "&v=' + this.options[this.selectedIndex].value;";

                VersionStore selected;

                if (!String.IsNullOrEmpty(Request.QueryString["v"]))
                {
                    if (!String.IsNullOrEmpty(version))
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
                    EditBox.Text = selected.Data;

                    if (selected.Version < vsc[vsc.Count - 1].Version)
                    {
                        EditMessage.Text =
                            "You are editing a previous version of this file. If you click <b>Save Changes</b>, a new version of this file (revision " +
                            (vsc.Count + 1) + ") will be created.";
                        EditMessage.Type = StatusType.Warning;
                    }
                }
            }
            else
            {
                VersionHistoryArea.Visible = false;
            }
        }

        private void SetFileDetails(FileInfo fi)
        {
            FileViews.SetActiveView(FileDetails);

            FileDetailsLastModified.Text = fi.LastWriteTime.ToLongDateString() + " " +
                                           fi.LastWriteTime.ToShortTimeString();
            if (FileFilters.IsLinkable(fi.Name))
            {
                FileDetailsName.Text = fi.Name;
                FileDetailsName.NavigateUrl = "~/" + (Request.QueryString["path"] ?? "") + "/" + fi.Name;
            }
            else
            {
                FileDetailsName.Visible = false;
                FileDetailsText.Text = fi.Name;
            }

            DownloadButton.Visible = FileFilters.IsDownloadable(fi.Name);
            EditButton.Visible = FileFilters.IsEditable(fi.Name);
            DeleteButton.Visible = FileFilters.IsDeletable(fi.Name);

            if (fi.Extension == ".dll")
            {
                Assembly assembly = Assembly.LoadFile(fi.FullName);
                FileDetailsAssemblyVersion.Text = assembly.GetName().Version.ToString();
            }
            else
            {
                assemblyLI.Visible = false;
            }

            if (FileFilters.IsVersionable(fi.Name))
            {
                FileDetailsRevision.Text = (VersionStore.CurrentVersion(fi) == 0 ? 1 : VersionStore.CurrentVersion(fi)).ToString();
            }
            else
            {
                FileDetailsRevision.Text = "n.a.";
                revsionLI.Visible = false;
            }

            FileDetailsSize.Text = fi.Length.ToString("0,0") + " kB";
        }

        private void SetFolders(DirectoryInfo di, string rootPath)
        {
            DirectoryInfo[] directories = di.GetDirectories();
            List<Folder> folders = new List<Folder>();
            foreach (DirectoryInfo d in directories)
            {
                if (d.Name != ".svn")
                {
                    Folder folder = new Folder();
                    folder.Name = d.Name;
                    folder.Path = d.FullName.Substring(rootPath.Length);

                    folders.Add(folder);
                }
            }

            FolderList.DataSource = folders;
            FolderList.DataBind();
        }

        private void SetFileList(DirectoryInfo di)
        {
            FileInfo[] files = di.GetFiles();
            List<FileInfo> the_Files = new List<FileInfo>();

            foreach (FileInfo fi in files)
            {
                if (FileFilters.IsValidFile(fi.Name))
                    the_Files.Add(fi);
            }

            if (the_Files.Count > 0)
            {
                the_Files.Sort(delegate(FileInfo f1, FileInfo f2) { return Comparer<string>.Default.Compare(f1.Name, f2.Name); });
                int half = the_Files.Count / 2 + the_Files.Count % 2;

                List<AFile> left = new List<AFile>();
                List<AFile> right = new List<AFile>();

                for (int i = 0; i < the_Files.Count; i++)
                {
                    AFile af = new AFile();
                    af.Name = the_Files[i].Name;
                    af.Path = Request.QueryString["path"] ?? "";
                    af.OnClick = string.IsNullOrEmpty(this.OnClientFileClickedFunction) ? "" : "return select('" + GetJavaScriptUrl(the_Files[i].FullName) + "');";

                    if (i + 1 <= half)
                        left.Add(af);
                    else
                        right.Add(af);
                }

                LeftFiles.DataSource = left;
                LeftFiles.DataBind();

                RightFiles.DataSource = right;
                RightFiles.DataBind();
            }
        }

        private string GetJavaScriptUrl(string fullPath)
        {
            Uri url = (new Uri(Context.Request.Url, Context.Response.ApplyAppPathModifier("~" + fullPath.Substring(Context.Request.PhysicalApplicationPath.Length - 1).Replace("\\", "/"))));

            return url.PathAndQuery.ToString().Replace("'", "\\'");
        }

        private void SetBreadCrumbs(string fileName)
        {
            string thePath = Request.QueryString["path"] ?? "";

            if (thePath.EndsWith("\\"))
                thePath = thePath.Substring(0, thePath.Length - 1);

            //            if(thePath.Trim().Length == 0 && string.IsNullOrEmpty(fileName))
            //                return;

            StringBuilder sb = new StringBuilder("<div class=\"breadcrumbs\">");

            if (this.IncludeUtilityBreadCrumbs)
            {
                sb.Append("<a href=\"../\">Site Options</a>");
                sb.Append("<span class=\"seperator\">></span>");
                sb.Append("<a href=\"../utilities/\">Utilities</a>");
                sb.Append("<span class=\"seperator\">></span>");
            }

            bool isAdmin = GraffitiUsers.IsAdmin(GraffitiUsers.Current);

            if (isAdmin)
                sb.Append("<a href=\"?path=\">Home</a>");
            else
                sb.Append("Home");


            string previous = "?path=";

            if (thePath.IndexOf("\\") > -1)
            {
                while (thePath.IndexOf("\\") != -1)
                {
                    sb.Append("<span class=\"seperator\">></span>");
                    string text = thePath.Substring(0, thePath.IndexOf("\\"));
                    previous += text + "\\";
                    thePath = thePath.Substring(text.Length + 1);

					if (previous.ToLower().StartsWith("?path=" + basicFilesMediaPath) || isAdmin)
                        sb.AppendFormat("<a href=\"{0}\">{1}</a>", previous.Substring(0, previous.Length - 1), text);
                    else
                        sb.Append(text);
                }
            }

            if (thePath.Trim().Length > 0)
            {
                sb.Append("<span class=\"seperator\">></span>");
                if (!string.IsNullOrEmpty(fileName))
                {
                    sb.AppendFormat("<a href=\"?path={0}\">{1}</a>", Request.QueryString["path"] ?? "", thePath.Trim());
                }
                else
                {
                    sb.Append(thePath.Trim());
                }
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                sb.Append("<span class=\"seperator\">></span>");
                sb.Append(fileName);
            }

            sb.Append("</div>");

            theBreadCrumbs.Text = sb.ToString();
        }
    }

    public class AFile
    {
        private string _name;
        private string _onClick;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public string Url
        {
            get { return "?path=" + Path + "&f=" + Name; }
        }

        public string OnClick
        {
            get { return _onClick; }
            set { _onClick = value; }
        }

        public string CssClass
        {
            get
            {
                string ext = System.IO.Path.GetExtension(Name);
                if (ext.Length > 0)
                    ext = ext.Substring(1).ToLower();

                switch (ext)
                {
                    case "ascx": return "ascx";
                    case "aspx": return "aspx";
                    case "bmp": return "bmp";

                    case "config": return "config";
                    case "cs": return "cs";
                    case "css": return "css";
                    case "dll": return "dll";
                    case "doc": return "doc";
                    case "exe": return "exe";
                    case "gif": return "gif";
                    case "png": return "image";
                    case "tff": return "image";
                    case "mp3":
                    case "wmv":

                        return "ipod";

                    case "mdb": return "mdb";

                    case "js":
                        return "js";

                    case "pdf":
                        return "pdf";

                    case "ppt":
                        return "ppt";

                    case "vb":
                        return "vb";

                    case "vm":
                        return "vm";

                    case "xml":
                        return "xml";

                    default:
                        return "file";

                }

            }
        }

    }

    public class Folder
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public string Url
        {
            get { return "?path=" + Path; }
        }

    }
}