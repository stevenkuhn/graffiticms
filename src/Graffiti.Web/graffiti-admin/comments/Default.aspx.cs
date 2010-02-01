using System;
using System.Web.UI;
using DataBuddy;
using Graffiti.Core;
using System.Web.UI.WebControls;
using System.Text;

public partial class graffiti_admin_comments_Default : ControlPanelPage
{
    protected void CommentSave_Click(object sender, EventArgs e)
    {
        Comment comment = new Comment(Request.QueryString["id"]);
        if (comment.IsNew)
            throw new Exception("Invalid Comment Id");

        comment.Body = CommentEditor.Value;
        comment.Name = Server.HtmlEncode(txtName.Text);
        comment.WebSite = txtSite.Text;
        comment.Email = txtEmail.Text;
        comment.Save();

        Response.Redirect("~/graffiti-admin/comments/");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            BuildPage();   
        }
    }

    private void BuildPage()
    {
        if (Request.QueryString["id"] == null)
        {
            //CommentCollection cc = new CommentCollection();
            Query q = Comment.CreateQuery();

            if (!(Request.QueryString["a"] == "d"))
                q.AndWhere(Comment.Columns.IsPublished, !(Request.QueryString["a"] == "f"));

            q.AndWhere(Comment.Columns.IsDeleted, (Request.QueryString["a"] == "d"));

            if (!String.IsNullOrEmpty(Request.QueryString["pid"]))
            {
                q.AndWhere(Comment.Columns.PostId, Request.QueryString["pid"]);
            }

            q.OrderByDesc(Comment.Columns.Id);

            CommentCollection tempCC = CommentCollection.FetchByQuery(q);

            CommentCollection permissionsFilteredCount = new CommentCollection();
            permissionsFilteredCount.AddRange(tempCC);

            foreach (Comment c in tempCC)
            {
                if (!RolePermissionManager.GetPermissions(c.Post.CategoryId, GraffitiUsers.Current).Read)
                    permissionsFilteredCount.Remove(c);
            }

            q.PageIndex = Int32.Parse(Request.QueryString["p"] ?? "1");
            q.PageSize = 25;

            CommentCollection cc = CommentCollection.FetchByQuery(q);

            CommentCollection permissionsFiltered = new CommentCollection();
            permissionsFiltered.AddRange(cc);

            foreach (Comment c in cc)
            {
                if (!RolePermissionManager.GetPermissions(c.Post.CategoryId, GraffitiUsers.Current).Read)
                    permissionsFiltered.Remove(c);
            }

            CommentList.DataSource = permissionsFiltered;
            CommentList.DataBind();

            string qs = Request.QueryString["a"] != null ? "?a=" + Request.QueryString["a"] : "?a=t";

            Pager.Text = Util.Pager(q.PageIndex, q.PageSize, permissionsFilteredCount.Count, "navigation", qs);

            if (Request.QueryString["a"] == "f")
                CommentLinks.SetActiveView(PendingComments);
            else if (Request.QueryString["a"] == "d")
                CommentLinks.SetActiveView(DeletedComments);
        }
        else
        {
            the_Views.SetActiveView(Comment_Form);
            Comment comment = new Comment(Request.QueryString["id"]);
            if (comment.IsNew)
                throw new Exception("Invalid Comment Id");

            txtName.Text = Server.HtmlDecode(comment.Name);
            txtSite.Text = comment.WebSite;
            txtEmail.Text = comment.Email;
            CommentEditor.Value = comment.Body;
        }

        #region build the page title

        StringBuilder sb = new StringBuilder();

        string page = Request.QueryString["a"] ?? "t";

        switch (page)
        {
            case "t":
                sb.Append("Published ");
                break;
            case "f":
                sb.Append("Pending ");
                break;
            case "d":
                sb.Append("Deleted ");
                break;
        }

        sb.Append(" Comments");

        lblPageTitle.Text = sb.ToString();

        string post = Request.QueryString["pid"];

        if (!String.IsNullOrEmpty(post))
        {
            Post p = new Post(Convert.ToInt32(post));

            lblPageTitle.Text += " for \"" + p.Name + "\"";
        }

        #endregion
    }

    protected void CommentList_OnItemCommand(object sender, RepeaterCommandEventArgs e)
    {
        int count = 0;

        switch (e.CommandName)
        {
            case "Delete":

                foreach (RepeaterItem item in CommentList.Items)
                {
                    int id = Convert.ToInt32(((HiddenField)item.FindControl("CommentID")).Value);

                    if (((CheckBox)item.FindControl("CommentCheckbox")).Checked)
                    {
                        Comment.Delete(id);
                        count++;
                    }
                }

                CommentStatus.Text = "You have deleted " + count.ToString() + " comment(s).";

                break;

            case "Approve":

                foreach (RepeaterItem item in CommentList.Items)
                {
                    int id = Convert.ToInt32(((HiddenField)item.FindControl("CommentID")).Value);

                    if (((CheckBox)item.FindControl("CommentCheckbox")).Checked)
                    {
                        Comment cmt = new Comment(id);
                        cmt.IsDeleted = false;
                        cmt.IsPublished = true;
                        cmt.Save();

                        count++;
                    }
                }

                CommentStatus.Text = "You have approved " + count.ToString() + " comment(s).";

                break;

            case "Undelete":

                foreach (RepeaterItem item in CommentList.Items)
                {
                    int id = Convert.ToInt32(((HiddenField)item.FindControl("CommentID")).Value);

                    if (((CheckBox)item.FindControl("CommentCheckbox")).Checked)
                    {
                        Comment comment = new Comment(id);
                        comment.IsDeleted = false;
                        comment.Save();

                        count++;
                    }
                }

                CommentStatus.Text = "You have undeleted " + count.ToString() + " comment(s).";

                break;
        }

        if (count == 0)
        {
            CommentStatus.Type = StatusType.Warning;
            CommentStatus.Text = "No comments were selected.";
        }
        else
        {
            CommentStatus.Type = StatusType.Success;
        }

        BuildPage();
    }

    protected void CommentList_OnItemCreated(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Header)
        {
            PlaceHolder bulkApprove = (PlaceHolder)e.Item.FindControl("bulkApprove");
            PlaceHolder bulkDelete = (PlaceHolder)e.Item.FindControl("bulkDelete");
            PlaceHolder bulkUndelete = (PlaceHolder)e.Item.FindControl("bulkUndelete");

            // hide everything for now
            bulkApprove.Visible = false;
            bulkDelete.Visible = false;
            bulkUndelete.Visible = false;

            string page = Request.QueryString["a"] ?? "t";

            if (!String.IsNullOrEmpty(page))
            {
                switch (page)
                {
                    case "t": // published
                        bulkDelete.Visible = true;
                        break;
                    case "f": // pending
                        bulkApprove.Visible = true;
                        bulkDelete.Visible = true;
                        break;
                    case "d": // bulkDeleted
                        bulkUndelete.Visible = true;
                        break;
                }
            }
            else
            {
                bulkDelete.Visible = true;
            }
        }

        if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
        {
            if (e.Item.DataItem == null)
                return;

            Literal control = (Literal)e.Item.FindControl("cssclass");

            if (control != null)
                control.Text = IsAltRow(e.Item.ItemIndex);

            HiddenField hf = (HiddenField)e.Item.FindControl("CommentID");

            if (hf != null)
                hf.Value = (((Comment)e.Item.DataItem).Id.ToString());

			PlaceHolder edit = (PlaceHolder)e.Item.FindControl("edit");
			PlaceHolder approve = (PlaceHolder)e.Item.FindControl("approve");
			PlaceHolder delete = (PlaceHolder)e.Item.FindControl("delete");
			PlaceHolder undelete = (PlaceHolder)e.Item.FindControl("undelete");

            // just to set them bold or not...
			//Literal EditBold = (Literal)edit.FindControl("EditBold");
			//Literal ApproveBold = (Literal)approve.FindControl("ApproveBold");

			Label pipe1 = (Label)e.Item.FindControl("pipe1");
			Label pipe2 = (Label)e.Item.FindControl("pipe2");

            // hide everything for now
            edit.Visible = false;
            approve.Visible = false;
            delete.Visible = false;
            undelete.Visible = false;
            pipe1.Visible = false;
            pipe2.Visible = false;

            string page = Request.QueryString["a"] ?? "t";

            Comment c = (Comment)e.Item.DataItem;

            if (!String.IsNullOrEmpty(page))
            {
                switch (page)
                {
                    case "t": // published
                        if(RolePermissionManager.GetPermissions(c.Post.CategoryId, GraffitiUsers.Current).Edit)
                            edit.Visible = true;
                        
                        if (RolePermissionManager.GetPermissions(c.Post.CategoryId, GraffitiUsers.Current).Publish)
                        {
                            //EditBold.Visible = true;
                            pipe1.Visible = true;
                            delete.Visible = true;
                        }
                        break;
                    case "f": // pending
                        if(RolePermissionManager.GetPermissions(c.Post.CategoryId, GraffitiUsers.Current).Edit)
                        edit.Visible = true;

                        if (RolePermissionManager.GetPermissions(c.Post.CategoryId, GraffitiUsers.Current).Publish)
                        {
                            pipe1.Visible = true;
                            approve.Visible = true;
                            //ApproveBold.Visible = true;
                            pipe2.Visible = true;
                            delete.Visible = true;
                        }
                        break;
                    case "d": // deleted
                        if(RolePermissionManager.GetPermissions(c.Post.CategoryId, GraffitiUsers.Current).Publish)
                            undelete.Visible = true;
                        break;
                }
            }
            else
            {
                // published is default tab
                if(RolePermissionManager.GetPermissions(c.Post.CategoryId, GraffitiUsers.Current).Edit)
                    edit.Visible = true;

                if (RolePermissionManager.GetPermissions(c.Post.CategoryId, GraffitiUsers.Current).Publish)
                {
                    pipe1.Visible = true;
                    delete.Visible = true;
                }
            }
            
            
            if(!RolePermissionManager.GetPermissions(c.Post.CategoryId, GraffitiUsers.Current).Publish)
            {
                CheckBox commentCheckbox = (CheckBox)e.Item.FindControl("CommentCheckbox");
                commentCheckbox.Visible = false;
            }
            
        }
    }

	protected string IsBold(string id)
	{
		string page = Request.QueryString["a"] ?? "t";

        if (!String.IsNullOrEmpty(page))
            switch (page)
            {
                case "t": // published
                    return (id == "EditBold") ? "font-weight: bold;" : string.Empty;//EditBold.Visible = true;
                case "f": // pending
					return (id == "ApproveBold") ? "font-weight: bold;" : string.Empty;//ApproveBold.Visible = true;
            }
        
        return string.Empty;
	}

	protected string IsAltRow(int index)
    {
        if (index % 2 == 0)
            return string.Empty;
        else
            return " class=\"alt\"";
    }

    protected string IsPostBound()
    {
        string post = Request.QueryString["pid"];

        if(!String.IsNullOrEmpty(post))
            return "&pid=" + post;

        return string.Empty;
    }
}
