<%@ Page Language="C#" %>
<%@ Import namespace="System.Collections.Generic"%>
<%@ Import namespace="DataBuddy"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    
    void Page_Load(object sender, EventArgs e)
    {
        Post p = new Post(Request.QueryString["id"]);
        this.Title = "Versions: " + p.Title;
        
        Query versionQuery = VersionStore.CreateQuery();
        versionQuery.AndWhere(VersionStore.Columns.Type, "post/xml");
        versionQuery.AndWhere(VersionStore.Columns.ItemId, Request.QueryString["id"]);
        versionQuery.OrderByDesc(VersionStore.Columns.Version);
        VersionStoreCollection vsc = new VersionStoreCollection();
        vsc.LoadAndCloseReader(versionQuery.ExecuteReader());
        
        List<Post> posts = new List<Post>();
        foreach(VersionStore vs in vsc)
        {
            Post post = ObjectManager.ConvertToObject<Post>(vs.Data);
            posts.Add(post);
        }

        VersionedPostList.DataSource = posts;
        VersionedPostList.DataBind();
    }

    private string BuildLink(object obj)
    {
        Post p = obj as Post;
        string baseLink = VirtualPathUtility.ToAbsolute("~/graffiti-admin/posts/write/");

        return baseLink + "?id=" + p.Id + "&amp;v=" + p.Version;
        
    }

</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function SendLinkToPage(linkToSend)
        {
            window.parent.Telligent_Modal.Close(linkToSend);
        }
    
    </script>
</head>
<body>
    <div>
    
        <Z:Repeater runat="Server" ID = "VersionedPostList" >
            <NoneTemplate>
                <h3>Sorry, there are no versions to edit</h3>
            </NoneTemplate>
            <HeaderTemplate>
                <table>
                    <tr>    
                        <td>Version</td>
                        <td>Date</td>
                        <td>Author</td>
                        <td>Notes</td>
                    </tr>
                
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                    
                    <a href="javascript:void(0);" onclick="SendLinkToPage('<%# BuildLink(Container.DataItem) %>');return false;"><%# Eval("Version") %></a>
                    
                    
                    </td>
                    <td>
                        <%# Eval("ModifiedOn") %>
                    </td>
                    <td>
                        <%# Eval("ModifiedBy") %>
                    </td>
                    <td>
                        <%# Eval("Notes") %>
                    </td>
                </tr>
                
                
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        
        </Z:Repeater>
    
    </div>
</body>
</html>
