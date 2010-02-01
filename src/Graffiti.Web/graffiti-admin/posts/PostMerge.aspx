<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PostMerge.aspx.cs" Inherits="Graffiti.Web.graffiti_admin.posts.PostMerge" MasterPageFile="../common/AdminModal.master" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="DataBuddy" %>

<script runat="server">

    List<Post> posts = null;
    int versionLeft = -2;
    int versionRight = -1;
    
    void Page_Init(object sneder, EventArgs e)
    {
        Query versionQuery = VersionStore.CreateQuery();
        versionQuery.AndWhere(VersionStore.Columns.Type, "post/xml");
        versionQuery.AndWhere(VersionStore.Columns.ItemId, Request.QueryString["id"]);
        versionQuery.OrderByDesc(VersionStore.Columns.Version);
        VersionStoreCollection vsc = new VersionStoreCollection();
        vsc.LoadAndCloseReader(versionQuery.ExecuteReader());

        posts = new List<Post>();
        foreach (VersionStore vs in vsc)
        {
            Post post = ObjectManager.ConvertToObject<Post>(vs.Data);
            posts.Add(post);
        }

        posts.Add(new Post(Request.QueryString["id"]));
        posts.Sort(delegate(Post p1, Post p2) { return Comparer<int>.Default.Compare(p2.Version, p1.Version); });
        
        if (string.IsNullOrEmpty(Request.Form[mergeVersion1.UniqueID]))
            versionLeft = int.Parse(Request.QueryString["versionLeft"] ?? "-2");
        else
            versionLeft = int.Parse(Request.Form[mergeVersion1.UniqueID]);
        
        if (string.IsNullOrEmpty(Request.Form[mergeVersion2.UniqueID]))
            versionRight = int.Parse(Request.QueryString["versionRight"] ?? "-1");
        else
            versionRight = int.Parse(Request.Form[mergeVersion2.UniqueID]);
        
        if (versionLeft == -2)
        {
            if (posts.Count > 1)
                versionLeft = posts[1].Version;
            else if (posts.Count > 0)
                versionLeft = posts[posts.Count - 1].Version;
        }
    }
        
    void Page_Load(object sender, EventArgs e)
    {
        mergeVersion1.Items.Clear();
        mergeVersion2.Items.Clear();

        string versionHtml = "<div style=\"width: 280px; overflow: hidden; padding: 6px 0; border-bottom: 1px solid #ccc;\"><b>{0}</b> ({1})<div>by {2}</div><div style=\"font-style: italic;\">{3}</div></div>";
        string versionText = "Revision {0}";

        mergeVersion1.Items.Add(new Telligent.Glow.DropDownListItem(
                                string.Format(versionHtml, "Editor Content", "Now", GraffitiUsers.Current.ProperName, ""),
                                "(Editor Content)", 
                                "-1"));
        mergeVersion2.Items.Add(new Telligent.Glow.DropDownListItem(
                                string.Format(versionHtml, "Editor Content", "Now", GraffitiUsers.Current.ProperName, ""),
                                "(Editor Content)",
                                "-1"));
        
        string contentLeft = string.Empty, contentRight = string.Empty;
        for (int i = 0; i < posts.Count; i++)
        {
            mergeVersion1.Items.Add(new Telligent.Glow.DropDownListItem(
                                string.Format(versionHtml, "Revision " + posts[i].Version, posts[i].ModifiedOn.ToString("dd-MMM-yyyy"), posts[i].UserProperName, posts[i].Notes),
                                string.Format(versionText, posts[i].Version), 
                                posts[i].Version.ToString()));
            mergeVersion2.Items.Add(new Telligent.Glow.DropDownListItem(
								string.Format(versionHtml, "Revision " + posts[i].Version, posts[i].ModifiedOn.ToString("dd-MMM-yyyy"), posts[i].UserProperName, posts[i].Notes),
                                string.Format(versionText, posts[i].Version),
                                posts[i].Version.ToString()));
        }

        mergeVersion1.SelectedValue = versionLeft.ToString();
        mergeVersion2.SelectedValue = versionRight.ToString();

        if (posts.Count > 0)
            this.Title = "Changes Made to " + posts[0].Title;

        if (this.IsPostBack)
            Merge();
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!this.IsPostBack)
            ClientScript.RegisterStartupScript(this.GetType(), "refresh", "window.setTimeout(new Function('updateMerge(); " + ClientScript.GetPostBackEventReference(new PostBackOptions(this)).Replace("\"", "\\\"").Replace("\'", "\\\'") + "'), 499);", true);
    }

    public void Merge()
    {
        string contentLeft = currentPostText.Value, contentRight = currentPostText.Value;
        for (int i = 0; i < posts.Count; i++)
        {
            if (posts[i].Version == versionLeft)
                contentLeft = posts[i].Body;

            if (posts[i].Version == versionRight)
                contentRight = posts[i].Body;
        }
        
        Merger merger = new Merger(contentLeft, contentRight);
        changes.InnerHtml = merger.Merge();
    }

</script>

<asp:Content ContentPlaceHolderID="BodyRegion" runat="server">

    <script type="text/javascript">
        
        function updateMerge ()
        {
            if (<%= mergeVersion1.ClientID %>.GetValue() == "-1" || <%= mergeVersion2.ClientID %>.GetValue() == "-1")
            {
                if (window.opener && window.opener.GetMergeContent)
                    postText.value = window.opener.GetMergeContent();     
            }
        }
        
    </script>
    <div style="background-color: #0080C3; padding: 10px; border-bottom: solid 1px #000; margin: -15px; margin-bottom: 0; color: #fff;">
        <table cellpadding="0" cellspacing="0" border="0"><tr>
            <td style="font-weight: bold;">Compare&nbsp;</td>
            <td><Glow:DropDownList runat="server" ID="mergeVersion1" SelectListWidth="300" Width="150px" SelectListHeight="200" ShowHtmlWhenSelected="false" /></td>
            <td style="font-weight: bold;">&nbsp;to&nbsp;</td>
            <td><Glow:DropDownList runat="server" ID="mergeVersion2" SelectListWidth="300" Width="150px" SelectListHeight="200" ShowHtmlWhenSelected="false" /></td>
            <td>
                <input type="hidden" id="currentPostText" runat="server" value="" />
                &nbsp;<asp:Button runat="server" ID="update" Text="Update" OnClientClick="updateMerge();" />
            </td>
        </tr></table>
    </div>
    <div runat="server" id="changes">
    </div>

    <script type="text/javascript">
    
    var postText = document.getElementById('<%= currentPostText.ClientID %>');
    
    </script>

</asp:Content>