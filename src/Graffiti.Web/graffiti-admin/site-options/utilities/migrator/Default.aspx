<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" %>
<%@ Import Namespace="System.Collections.Generic" %>

<asp:Content ContentPlaceHolderID="MainRegion" runat="server">
<h1>Graffiti Migration Tool</h1>
<Z:Breadcrumbs runat="server" SectionName="Migrator" />

<script language="javascript" type="text/javascript">
function togglePanel(pnl)
{
    // hide all panels
    var cs2007button = document.getElementById('<%= CS2007Database.ClientID %>');
    var cs2007 = document.getElementById('<%= pnlControlsCS2007.ClientID %>');
    var cs21button = document.getElementById('<%= CS21Database.ClientID %>');
    var cs21 = document.getElementById('<%= pnlControlsCS21.ClientID %>');
    var wordpressbutton = document.getElementById('<%= Wordpress.ClientID %>');
    var wordpress = document.getElementById('<%= pnlControlsWordpress.ClientID %>');
    var blogmlbutton = document.getElementById('<%= BlogML.ClientID %>');
    var blogml = document.getElementById('<%= pnlControlsBlogML.ClientID %>');
    var dasblogbutton = document.getElementById('<%= dasBlog.ClientID %>');
    var dasblog = document.getElementById('<%= pnlControlsdasBlog.ClientID %>');   
    
    cs2007button.checked = false;
    cs21button.checked = false;
    wordpressbutton.checked = false;
    blogmlbutton.checked = false;   
    dasblogbutton.checked = false;
    
    cs2007.style.display = "none";
    cs21.style.display = "none";
    wordpress.style.display = "none";
    blogml.style.display = "none";
    dasblog.style.display = "none";
     
    switch(pnl)
    {
        case 1:
            cs2007.style.display = "block";
            cs2007button.checked = true;
        break;
        case 2:
            wordpress.style.display = "block";
            wordpressbutton.checked = true;
        break;
        case 3:
            blogml.style.display = "block";
            blogmlbutton.checked = true;
        break;
        case 4:
            cs21.style.display = "block";
            cs21button.checked = true;
        break;
        case 5:
            dasblog.style.display = "block";
            dasblogbutton.checked = true;
        break;
    }
}

function selectAllToggle()
{
    var rows = document.getElementById('<%= totalposts.ClientID %>').value;
    var checked = document.getElementById("headercheckbox").checked;
  
    var postListGrid = document.getElementById("postList").getElementsByTagName("input");
   
    for(var i=0; i < postListGrid.length; i++)
    {
        if(postListGrid[i].type == 'checkbox')
            postListGrid[i].checked = checked;
    }
}

var rows;
var importMethod;
var successfulImports;
var warningImports;
var unsuccessfulImports;
var totalRows;
var currentRunning;
var totalToProcess = 0;

function InitImport()
{
    rows = document.getElementById('<%= totalposts.ClientID %>').value;
    importMethod = document.getElementById('<%= importMethod.ClientID %>').value;
    successfulImports = 0;
    warningImports = 0;
    unsuccessfulImports = 0;
    totalRows=0;
    
    currentRunning = 0;
    
    $('statussuccess').innerHTML = '';
    $('statuswarning').innerHTML = '';
    $('statuserror').innerHTML = '';
    
    $('statusimport').innerHTML = 'Initializing, please wait...';

    var postListGrid = document.getElementById("postList").getElementsByTagName("input");
    
    for(var i=0; i < postListGrid.length; i++)
    {
        if(postListGrid[i].name == 'iaccept')
            postListGrid[i].style.display = 'none';
            
        if(postListGrid[i].name == 'ispinner')
            postListGrid[i].style.display = 'none';
            
        if(postListGrid[i].name == 'ierror')
            postListGrid[i].style.display = 'none';
            
        if(postListGrid[i].name == 'errormsg')
            postListGrid[i].style.display = 'none';
            
        if(postListGrid[i].name == 'warningmsg')
            postListGrid[i].style.display = 'none';
            
        if(postListGrid[i].type == 'checkbox' && postListGrid[i].checked)
            totalToProcess += parseInt(1);       
    }
    
    if(document.getElementById("headercheckbox").checked)
        totalToProcess -= parseInt(1);
    
    if(document.getElementById("<%= ddlCategory.ClientID %>").value == "")
    {
        alert('Please select a category to import into');
        return;
    }
    
    setTimeout("ImportPosts(0)", 100);
}

function ImportPosts(start)
{
    for (var x=start; x<=rows; x++)
    {
        if(currentRunning >= 12)
        {
            setTimeout('ImportPosts(' + start + ')', 700);
            return;
        }
    
        var body = document.getElementById("body" + x);
        var prevauthor = document.getElementById("prevauthor" + x);
        var createdon = document.getElementById("createdon" + x);
        var name = document.getElementById("name" + x);
        var subject = document.getElementById("subject" + x);
        var tags = document.getElementById("tags" + x);
        var selected = document.getElementById("selected" + x);
        var ispublished = document.getElementById("ispublished" + x);
        var postid = document.getElementById("postid" + x);
        
        var author = document.getElementById("<%= ddlAuthor.ClientID %>");
        
        var v = author.selectedIndex
        var authorname = author.options[v].value;
        
        var category = document.getElementById("<%= ddlCategory.ClientID %>");
        
        var impPost = selected.checked;

        if(impPost)
        {
            totalRows += parseInt(1);
        
            document.getElementById("ImgSpinner"+x).style.display = "block";
            
            currentRunning ++;
            $.ajax(
            {
                type: 'POST',
                dataType: 'text',
                url:'<%= new Urls().AdminAjax %>?command=importPosts',
                data:  {body:body.innerHTML,
                                prevauthor:prevauthor.innerHTML,
                                createdon:createdon.innerHTML,
                                name:name.innerHTML,
                                subject:subject.innerHTML,
                                author:authorname,
                                category:category.value,
                                tags:tags.innerHTML,
                                published:ispublished.innerHTML,
                                postid:postid.innerHTML,
                                method:importMethod,
                                panel:x},
                success: function(transport)
                {
                    currentRunning--;
                    var response = transport || 'no response text';
                    if(response.substring(0, 7) == 'Success')
                    {
                        response = response.replace("Success", "");
                        var panelID = response;

                        successfulImports += parseInt(1);
                        $('statussuccess').innerHTML = 'Imported ' + successfulImports + ' post(s)';
                        
                        document.getElementById("ImgSpinner"+panelID).style.display = "none";
                        document.getElementById("ImgAccept"+panelID).style.display = "block";
                        document.getElementById("selected" + panelID).checked = false;
                        document.getElementById("selected" + panelID).disabled = true;
                    }
                    else if(response.substring(0, 7) == 'Warning')
                    {
                        response = response.replace("Warning", "");
                        var pnlID = response;

                        successfulImports += parseInt(1);
                        warningImports += parseInt(1);
                        $('statussuccess').innerHTML = 'Imported ' + successfulImports + ' post(s)';
                        $('statuswarning').innerHTML = '( ' + warningImports + ' warning(s) - see above)';
                        
                        document.getElementById("ImgSpinner"+pnlID).style.display = "none";
                        document.getElementById("ImgAccept"+pnlID).style.display = "block";
                        
                        document.getElementById("WarningMessage"+pnlID).style.display = "block";
                        document.getElementById("WarningMessage"+pnlID).innerHTML = "This data was imported, but has been marked as requiring changes before being published. This probably occured because the post does not have a name. A temporary name has been given to the post.";
                        document.getElementById("selected" + pnlID).checked = false;
                        document.getElementById("selected" + pnlID).disabled = true;
                    }
                    else
                    {
                        var panelid = response.substring(0, response.indexOf(":"));
                        
                        unsuccessfulImports += parseInt(1);
                        $('statuserror').innerHTML = 'Failed to import ' + unsuccessfulImports + ' post(s) - see above';
                        
                        document.getElementById("ImgSpinner"+panelid).style.display = "none";
                        document.getElementById("ImgError"+panelid).style.display = "block";
                           
                        response = response.replace(panelid + ":", "");
                        document.getElementById("ErrorMessage"+panelid).style.display = "block";
                        document.getElementById("ErrorMessage"+panelid).innerHTML = response;
                        document.getElementById("selected" + panelid).checked = false;
                        document.getElementById("selected" + panelid).disabled = true;
                    }

                    setTotals();
                },
                error: function(){ 
                    currentRunning--;
                    
                    setTotals();
                    
                    unsuccessfulImports += parseInt(1);
                }
            });
        }
        start++;
    }
}

function setTotals()
{
    var totalImported = parseInt(successfulImports) + parseInt(unsuccessfulImports); 
    $('statusimport').innerHTML = 'Importing posts (' + totalImported + ' of ' + totalToProcess + ') ';
    
    if(totalImported == totalToProcess)
        $('statusimport').innerHTML = 'Finished!';
}
</script>

<script language="javascript">
setTimeout('<asp:Literal id="js" runat="server"></asp:Literal>', 1); 
</script>

<div style="margin-top: 15px;">
    <asp:radiobutton ID="CS2007Database" runat="server" Checked="false" Text="CS 2007 Database" onclick='togglePanel(1);' />
    <asp:radiobutton ID="CS21Database" runat="server" Checked="false" Text="CS 2.1 Database" onclick='togglePanel(4);' />
    <asp:radiobutton ID="Wordpress" runat="server" Checked="false" Text="Wordpress" onclick='togglePanel(2);' />
    <asp:radiobutton ID="BlogML" runat="server" Checked="false" Text="BlogML" onclick='togglePanel(3);' />
    <asp:radiobutton ID="dasBlog" runat="server" Checked="true" Text="dasBlog" onclick='togglePanel(5);' />
</div>

<div style="margin: 10px;">

    <asp:panel ID="pnlControlsCS2007" style="display: none;" runat="server">
        <p style="margin: 0;">Database Connection String:</p>
        <p style="margin: 0;"><asp:TextBox ID="txtDBCS" runat="server" CssClass="short" style="width: 500px;" /></p>

        <p style="margin: 0;">Application Key:</p>
        <p style="margin: 0;"><asp:TextBox ID="txtApplicationKey" runat="server" CssClass="short" /></p>

        <p style="margin: 0;">Username:</p>
        <p style="margin: 0;"><asp:TextBox ID="txtUserName" runat="server" CssClass="short" /></p>
    </asp:panel>
    
    <asp:panel ID="pnlControlsCS21" style="display: none;" runat="server">
        <p style="margin: 0;">Database Connection String:</p>
        <p style="margin: 0;"><asp:TextBox ID="txt21DBCS" runat="server" CssClass="short" style="width: 500px;" /></p>

        <p style="margin: 0;">Application Key:</p>
        <p style="margin: 0;"><asp:TextBox ID="txt21ApplicationKey" runat="server" CssClass="short" /></p>

        <p style="margin: 0;">Username:</p>
        <p style="margin: 0;"><asp:TextBox ID="txt21UserName" runat="server" CssClass="short" /></p>
    </asp:panel>

    <asp:panel ID="pnlControlsWordpress" style="display: none;" runat="server">
        <p style="margin: 0;">Wordpress Export:</p>
        <p style="margin: 0;"><asp:FileUpload ID="WordpressUpload" runat="server" /></p>
    </asp:panel>
    
    <asp:panel ID="pnlControlsBlogML" style="display: none;" runat="server">
        <p style="margin: 0;">BlogML Export:</p>
        <p style="margin: 0;"><asp:FileUpload ID="BlogMLUpload" runat="server" /></p>
    </asp:panel>
    
    <asp:panel ID="pnlControlsdasBlog" runat="server">
        <br />
        <p style="margin: 0;">Upload the content folder from dasBlog on your server to the <strong>/files/temp/</strong> folder in your Graffiti installation and then click 'Get data'. <br />
        Note: after the migration, you can copy your content/binary directory to Graffiti's root directory. This will prevent broken images from your site.</p>
        <br /><br />
    </asp:panel>

</div>

<div class="submit">
    <asp:Button ID="btnSubmit" runat="Server" OnClick="btnSubmit_Click" text="Get data" />
    <span class="form_tip">(this step will <b>not</b> import your data into Graffiti)</span>
</div>

<asp:Repeater ID="rptPosts" runat="server">
    <HeaderTemplate>
        <h3>Post to be imported</h3>
        <table id="postList" style="width: 100%;">
        <thead style="font-size: 115%;">
            <tr>
                <td style="width: 50px;"><input type="checkbox" id="headercheckbox" onclick="selectAllToggle();" /></td>
                <td><b>Post Body</b></td>
                <td style="width: 100px;"><b>Author</b></td>
                <td style="width: 150px;"><b>Created On</b></td>
                <td style="width: 135px;"><b>Post Name</b></td>
                <td style="width: 150px;"><b>Post Subject</b></td>
                <td style="width: 135px;"><b>Tags and Categories</b></td>
            </tr>
        </thead>
    </HeaderTemplate>
    <ItemTemplate>
        <tr <%# IsAltRow(Container.ItemIndex) %>>
            <td>
                <span style="float:left;"><input id="selected<%# Container.ItemIndex %>" name="importCheckbox" type="checkbox"  /></span>
                <span style="float:left; padding-top: 2px; padding-left: 2px;"><img id="ImgSpinner<%# Container.ItemIndex %>" name="ispinner" style="display:none;" src="../../../../graffiti-admin/common/img/spinner.gif" /></span>
                <span style="float:left; padding-top: 2px;"><img id="ImgAccept<%# Container.ItemIndex %>" name="iaccept" style="display:none;" src="../../../../graffiti-admin/common/img/accept.png" /></span>
                <span style="float:left; padding-top: 2px;"><img id="ImgError<%# Container.ItemIndex %>" name="ierror" style="display:none;" src="../../../../graffiti-admin/common/img/stop.png" /></span>
                <span id="ispublished<%# Container.ItemIndex %>" style="display: none;"><%# Eval("IsPublished")%></span>
                <span id="postid<%# Container.ItemIndex %>" style="display: none;"><%# Eval("PostID")%></span>
            </td>
            <td><div style="overflow: hidden; width: 300px;"><%# TrimBody(Server.HtmlEncode(Eval("Body").ToString())) %></div><span ID="body<%# Container.ItemIndex %>" style="display: none;"><%# Server.HtmlEncode(Eval("Body").ToString()) %></span></td>
            <td><span id="prevauthor<%# Container.ItemIndex %>"><%# Eval("Author") %></span></td>
            <td><span id="createdon<%# Container.ItemIndex %>"><%# Eval("CreatedOn")%></span></td>
            <td><div style="overflow: hidden; width: 135px;"><span id="name<%# Container.ItemIndex %>"><%# Eval("Name") != null ? Server.HtmlEncode(Eval("Name").ToString()) : Server.HtmlEncode(Eval("Subject").ToString()) %></span></div></td>
            <td><div style="overflow: hidden; width: 150px;"><span id="subject<%# Container.ItemIndex %>"><%# Server.HtmlEncode(Eval("Subject").ToString()) %></span></div></td>
            <td>
                <asp:Repeater ID="rptPostsTagsAndCategories" runat="server" DataSource=<%# Eval("TagsAndCategories") %>>
                    <ItemTemplate>
                        <%# Container.DataItem.ToString() %>
                    </ItemTemplate>
                </asp:Repeater>
                <span id="tags<%# Container.ItemIndex %>" style="display: none;"><%# GetTagsAndCategories(Container.DataItem) %></span>
            </td>
        </tr>
        <tr <%# IsAltRow(Container.ItemIndex) %>>
            <td colspan="7">
                <span id="ErrorMessage<%# Container.ItemIndex %>" name="errormsg" style="font-weight: bold; margin: 5px; display: none; background: #ff5d40; padding: 5px; color: White;"></span>
                <span id="WarningMessage<%# Container.ItemIndex %>" name="warningmsg" style="font-weight: bold; margin: 5px; display: none; background: #f8f0ad; padding: 5px;"></span>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
        
        <asp:PlaceHolder ID="showFooter" runat="Server" Visible="false">
        <tr>
            <td colspan="7" style="padding: 5px;"></td>
        </tr>
        <tr style="padding-top: 10px; background: #ccc;">
            <td></td>
            <td style="font-size: 115%; font-weight: bold; padding: 25px 0px 25px 0px;">
                Posts will be imported as
            </td>
            <td style="vertical-align: middle;">
                Author:<br />
                <asp:DropDownList ID="ddlAuthor" runat="server" />
            </td>
            <td></td>
            <td></td>
            <td></td>
            <td style="vertical-align: middle;">
                Category:<br />
                <asp:DropDownList ID="ddlCategory" runat="server" />
            </td>
        </tr>
        </table>
        <div class="submit">
            <input type="button" onclick="setTimeout('InitImport();', 10);" value ="Import posts" />
            <span id="statusimport" style="font-weight: bold; font-size: 105%; padding-right: 20px;"></span>
            <span id="statussuccess" style="font-weight: bold; color: Green; font-size: 105%;"></span>
            <span id="statuswarning" style="font-weight: bold; color: Green; font-size: 105%; padding-right: 20px;"></span>
            <span id="statuserror" style="font-weight: bold; color: red; font-size: 105%;"></span>
        </div>
        </asp:PlaceHolder>

<asp:HiddenField ID="totalposts" runat="Server" />
<asp:HiddenField ID="importMethod" runat="Server" />
</asp:Content>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context, "settings"); 
    }
    
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        List<MigratorPost> posts = null;

        if (CS2007Database.Checked)
        {
            importMethod.Value = "CS2007Database";
            
            CS2007Database db = new CS2007Database();
            db.DatabaseConnectionString = txtDBCS.Text;
            db.ApplicationKey = txtApplicationKey.Text;
            db.UserName = txtUserName.Text;
            posts = db.GetPosts();

            js.Text = "togglePanel(1);";
        }

        if (Wordpress.Checked)
        {
            importMethod.Value = "Wordpress";
            
            Wordpress wp = new Wordpress();
            wp.EncodedFile = MigratorUtilities.GetEncodedFileFromFileUpload(WordpressUpload);
            posts = wp.GetPosts();

            js.Text = "togglePanel(2);";
        }

        if (BlogML.Checked)
        {
            importMethod.Value = "BlogML";

            BlogML wp = new BlogML();
            wp.FileBytes = MigratorUtilities.GetFileBytes(BlogMLUpload);
            wp.EncodedFile = MigratorUtilities.GetEncodedFileFromFileUpload(BlogMLUpload);
            posts = wp.GetPosts();

            js.Text = "togglePanel(3);";
        }

        if (CS21Database.Checked)
        {
            importMethod.Value = "CS21Database";

            CS21Database db = new CS21Database();
            db.DatabaseConnectionString = txt21DBCS.Text;
            db.ApplicationKey = txt21ApplicationKey.Text;
            db.UserName = txt21UserName.Text;
            posts = db.GetPosts();

            js.Text = "togglePanel(4);";
        }

        if (dasBlog.Checked)
        {
            importMethod.Value = "dasBlog";

            dasBlog db = new dasBlog();
            posts = db.GetPosts();

            js.Text = "togglePanel(5);";
        }
        
        rptPosts.DataSource = posts;
        rptPosts.DataBind();

        if (posts != null && posts.Count > 0)
        {
            showFooter.Visible = true;
            
            ddlAuthor.DataSource = GraffitiUsers.GetUsers("*");
            ddlAuthor.DataTextField = "ProperName";
            ddlAuthor.DataValueField = "Name";
            ddlAuthor.DataBind();

            for (int x = 0; x < ddlAuthor.Items.Count; x++)
                if (ddlAuthor.Items[x].Text == GraffitiUsers.Current.ProperName)
                    ddlAuthor.SelectedIndex = x;


            ddlCategory.DataSource = new CategoryController().GetCachedCategories();
            ddlCategory.DataTextField = "Name";
            ddlCategory.DataValueField = "Id";
            ddlCategory.DataBind();

            ddlCategory.Items.Insert(0, new ListItem("Uncategorized", "1"));
            
            totalposts.Value = Convert.ToString(posts.Count - 1);
        }
    }

    protected string TrimBody(string body)
    {
        if (body.Length > 175)
            return body.Substring(0, 175) + " ...";
        else
            return body;
    }

    protected string IsAltRow(int index)
    {
        if (index % 2 == 0)
            return string.Empty;
        else
            return " class=\"alt\"";
    }

    protected string GetTagsAndCategories(object temp)
    {
        MigratorPost post = temp as MigratorPost;
        
        if(post.TagsAndCategories != null)
            return String.Join(",", post.TagsAndCategories.ToArray());

        return String.Empty;
    }
    
</script>