using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBuddy;
using Graffiti.Core;
using System.Collections.Generic;
using System.Text;
using Repeater = Graffiti.Core.Repeater;

public partial class graffiti_admin_posts_Default : ControlPanelPage
{
    private List<CategoryCount> cats = null;
    private int currentChildIndex = 0; // this will hold what child we are on so we know which img to display in the hierarchical view

    //This was previous leveraged by licensing which has been removed. 
    protected static string ReportStyle
    {
        get
        {
            return "text-align: center; vertical-align: middle;";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if(Request.QueryString["id"] != null)
        {
            Post the_Post = new Post(Request.QueryString["id"]);
            switch(the_Post.PostStatus)
            {
                case PostStatus.Publish:

                    if (the_Post.Published > SiteSettings.CurrentUserTime)
                    {
                        PostUpdateStatus.Text = "The post, <em>&quot;<a title=\"click to view the post\" href=\"" + the_Post.Url +
                                                "\">" + the_Post.Title +
                                                "</a>&quot;</em>, was published. However, since this is a forward dated post, only site administrators can view it until " + the_Post.Published.ToLongDateString() + " " + the_Post.Published.ToShortTimeString();
                    }
                    else
                    {
                        PostUpdateStatus.Text = "The post, <em>&quot;<a title=\"click to view the post\" href=\"" + the_Post.Url +
                                                "\">" + the_Post.Title +
                                                "</a>&quot;</em>, was published. If this was a revision, the new version is now live.";
                    }
                    PostUpdateStatus.Type = StatusType.Success;
                    break;

                case PostStatus.Draft:

                    PostUpdateStatus.Text = "The post, <em>&quot;" + the_Post.Title +
                                            "&quot;</em>, was saved as a draft.";
                    PostUpdateStatus.Type = StatusType.Success;

                    break;

                case PostStatus.PendingApproval:
                    PostUpdateStatus.Text = "The post, <em>&quot;" + the_Post.Title +
                                            "&quot;</em>, was saved, but requires approval before it can be published. The site editors and publishers have been notified by email about this post. Once they publish it, you will be able to view it live.";

                    PostUpdateStatus.Type = StatusType.Warning;
                    break;

                case PostStatus.RequiresChanges:
                    PostUpdateStatus.Text = "The post, <em>&quot;" + the_Post.Title +
                                            "&quot;</em>, was saved. An email has been sent to the original author with the change notification. You will receive an email once the author sets the post status to <em>Request Approval</em>.";

                    PostUpdateStatus.Type = StatusType.Warning;
                    break;
            }
        }


        if(!IsPostBack)
        {
            string page = Request.QueryString["status"] ?? "1";
            string category = Request.QueryString["category"];
            string author = Request.QueryString["author"];

            Master.FindControl("SideBarRegion").Visible = true;

            List<AuthorCount> auts = null;

 

            switch (page)
            {
                case "1": // published
                    PostsLinks.SetActiveView(Published);
                    cats = Post.GetCategoryCountForStatus(PostStatus.Publish, author);
                    auts = Post.GetAuthorCountForStatus(PostStatus.Publish, category);
                    break;
                case "2": // draft
                    PostsLinks.SetActiveView(Draft);
                    cats = Post.GetCategoryCountForStatus(PostStatus.Draft, author);
                    auts = Post.GetAuthorCountForStatus(PostStatus.Draft, category);
                    break;
                case "3": // pending review
                    PostsLinks.SetActiveView(PendingReview);
                    cats = Post.GetCategoryCountForStatus(PostStatus.PendingApproval, author);
                    auts = Post.GetAuthorCountForStatus(PostStatus.PendingApproval, category);
                    break;
                case "4": // requires changes
                    PostsLinks.SetActiveView(RequiresChanges);
                    cats = Post.GetCategoryCountForStatus(PostStatus.RequiresChanges, author);
                    auts = Post.GetAuthorCountForStatus(PostStatus.RequiresChanges, category);
                    break;
                case "-1": // deleted
                    PostsLinks.SetActiveView(Deleted);
                    Master.FindControl("SideBarRegion").Visible = false;
                    break;
            }

            if (auts != null)
            {
                rptAuthors.DataSource = auts;
                rptAuthors.DataBind();
            }

            if (cats != null)
            {
                List<CategoryCount> temp = new List<CategoryCount>();
                temp.AddRange((List<CategoryCount>)cats.FindAll(
                                                    delegate(CategoryCount ca)
                                                    {
                                                        return ca.ParentId <= 0; // only want the parents
                                                                                    // the repeater below will handle the children
                                                    }));

                temp.Sort(
                    delegate(CategoryCount c, CategoryCount c1)
                    {
                        return c.Name.CompareTo(c1.Name);
                    });

                rptCategories.DataSource = temp;

                List<CategoryCount> toRemove = new List<CategoryCount>();

                foreach(CategoryCount cc in temp)
                {
                    if (!RolePermissionManager.GetPermissions(cc.ID, GraffitiUsers.Current).Read)
                        toRemove.Add(cc);
                }

                foreach (CategoryCount cc in toRemove)
                {
                    temp.Remove(cc);
                }

                rptCategories.DataBind();
            }

            User user = null;

            if (!String.IsNullOrEmpty(author))
            {
                user = new User(Convert.ToInt32(author));
                author = user.Name;
            }

            Query q = Post.CreateQuery();


            if (Request.QueryString["category"] != null && Request.QueryString["category"] != "-1")
                q.AndWhere(Post.Columns.CategoryId, Request.QueryString["category"]);

            if (!String.IsNullOrEmpty(author))
                q.AndWhere(Post.Columns.CreatedBy, author);

            if (Request.QueryString["status"] == "-1")
                q.AndWhere(Post.Columns.IsDeleted, true);
            else
            {
                q.AndWhere(Post.Columns.IsDeleted, false);
                q.AndWhere(Post.Columns.Status, Request.QueryString["status"] ?? "1");
            }

            q.OrderByDesc(Post.Columns.Published);

            PostCollection tempPC = new PostCollection();
            tempPC.LoadAndCloseReader(q.ExecuteReader());

            PostCollection permissionsFilteredCount = new PostCollection();
            permissionsFilteredCount.AddRange(tempPC);

            foreach (Post p in tempPC)
            {
                if (!RolePermissionManager.GetPermissions(p.CategoryId, GraffitiUsers.Current).Read)
                    permissionsFilteredCount.Remove(p);
            }
           
            q.PageSize = 15;
            q.PageIndex = Int32.Parse(Request.QueryString["p"] ?? "1");

            PostCollection pc = new PostCollection();
            pc.LoadAndCloseReader(q.ExecuteReader());

            PostList.NoneItemsDataBound += new RepeaterItemEventHandler(PostList_NoneItemsDataBound);

            PostCollection permissionsFiltered = new PostCollection();
            permissionsFiltered.AddRange(pc);

            foreach (Post p in pc)
            {
                if (!RolePermissionManager.GetPermissions(p.CategoryId, GraffitiUsers.Current).Read)
                    permissionsFiltered.Remove(p);
            }

            PostList.DataSource = permissionsFiltered;
            PostList.DataBind();

            string catID = Request.QueryString["category"] ?? "0";
            string autID = Request.QueryString["author"] ?? "0";

            if(pc.Count > 0)
            {
                string qs = "?status=" + page;
                if(catID != "0")
                    qs += "&category=" + catID;
                if (autID != "0")
                    qs += "&author=" + autID;

                Pager.Text = Util.Pager(q.PageIndex, q.PageSize, permissionsFilteredCount.Count, null, qs);
            }

            SetCounts(Int32.Parse(catID));

            #region build the page title

            StringBuilder sb = new StringBuilder();

            sb.Append("Posts ");

            switch (page)
            {
                case "1":
                    sb.Append("Published ");
                    break;
                case "2":
                    sb.Append("Drafted ");
                    break;
                case "3":
                    sb.Append("Pending Review ");
                    break;
                case "4":
                    sb.Append("Requiring Changes ");
                    break;
                case "5":
                    sb.Append("Deleted ");
                    break;
            }

            sb.Append("in ");

            if (Request.QueryString["category"] != null)
            {
                CategoryCollection categories = new CategoryController().GetAllCachedCategories();

                Category temp = categories.Find(
                        delegate(Category c)
                        {
                            return c.Id == Int32.Parse(Request.QueryString["category"]);
                        });

                if (temp != null)
                    sb.Append(temp.Name);
            }
            else
            {
                sb.Append("All Categories");
            }

            if (!String.IsNullOrEmpty(author))
            {
                sb.Append(" for " + user.ProperName);
            }

            lblPageTitle.Text = sb.ToString();

            #endregion

            if(Request.QueryString["dstry"] != null)
            {
                PostUpdateStatus.Text = Server.UrlDecode(Request.QueryString["dstry"]) +
                                        " has been permanently deleted.";
                PostUpdateStatus.Type = StatusType.Success;

            } 
        }
    }

    void PostList_NoneItemsDataBound(object sender, RepeaterItemEventArgs e)
    {
        
        StatusMessage sm = e.Item.FindControl("NoneMessage") as StatusMessage;
        if(sm != null)
        {
            string page = Request.QueryString["status"] ?? "1";

            switch (page)
            {
                case "1": // published
                    sm.Text = "Sorry, there are no published posts. Why not add a <a href=\"write/\">new one</a>?"; PostsLinks.SetActiveView(Published);
                    break;
                case "2": // draft
                    sm.Text = "Sorry, there are no drafts for you to edit.";
                    break;
                case "3": // pending review
                    sm.Text = "Sorry, there are no posts pending review.";
                    break;
                case "4": // requires changes
                    sm.Text = "Sorry, there are no posts with required changes.";
                    break;
                case "-1": // deleted
                    sm.Text = "Sorry, there are no deleted posts.";
                    break;
            }
        }
        
       
    }


    protected void PostList_OnItemCreated(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
        {
            Literal control = (Literal)e.Item.FindControl("cssclass");
            Literal counts = (Literal)e.Item.FindControl("CommentCounts");
            PlaceHolder pnlDelete = (PlaceHolder)e.Item.FindControl("delete");

            Post p = (Post) e.Item.DataItem;

            if (p != null)
            {
                if (RolePermissionManager.GetPermissions(p.CategoryId, GraffitiUsers.Current).Publish)
                    pnlDelete.Visible = true;
                else
                    pnlDelete.Visible = false;

                if (control != null)
                    control.Text = IsAltRow(e.Item.ItemIndex, p.Id);

                if (counts != null)
                {
                    counts.Text = "";

                    if (p.CommentCount > 0)
                    {
                        counts.Text += "<a href=\"" + ResolveUrl("~/graffiti-admin/comments/") + "?pid=" + p.Id + "\">";
                        if (p.CommentCount == 1)
                            counts.Text += "1 comment published";
                        else
                            counts.Text += p.CommentCount + " comments published";
                        counts.Text += "</a><br />";
                    }

                    if (p.PendingCommentCount > 0)
                    {
                        counts.Text += "<a style=\"color: #9d0a0e;\" href=\"" + ResolveUrl("~/graffiti-admin/comments/") + "?a=f&pid=" + p.Id + "\">";

                        if (p.PendingCommentCount == 1)
                            counts.Text += "1 comment not published";
                        else
                            counts.Text += p.PendingCommentCount + " comments not published";

                        counts.Text += "</a>";
                    }

                    if (p.CommentCount == 0 && p.PendingCommentCount == 0)
                        counts.Text += "No comments";
                }
            }
        }
    }

    protected string IsAltRow(int index, int postId)
    {
        if(postId.ToString() == Request.QueryString["id"])
            return " class=\"selected\"";

        if (index % 2 == 0)
            return string.Empty;
        else
            return " class=\"alt\"";
    }

    protected string IsSelectedCategory(string id)
    {
        if (id == (Request.QueryString["category"] ?? "0"))
            return " class=\"selected\"";
        else
            return " class=\"notselected\"";
    }

    protected string IsSelectedAuthor(string id)
    {
        if (id == (Request.QueryString["author"] ?? "0"))
            return " class=\"selected\"";
        else
            return " class=\"notselected\"";
    }

    protected string GetCategoryLink(string name, string url)
    {
        if (name.ToLower() == "uncategorized")
            return name;
        else
            return "<a href=\"" + url + "\">" + name + "</a>";
    }

    private void SetCounts(int catID)
    {
        List<PostCount> postCounts = Post.GetPostCounts(catID, null);

        foreach (PostCount pc in postCounts)
        {
            switch (pc.PostStatus)
            {
                case PostStatus.Draft:

                    string draftText = " (" + pc.Count + ")";

                    Draft1.Text = draftText;
                    Draft2.Text = draftText;
                    Draft3.Text = draftText;
                    Draft4.Text = draftText;
                    Draft5.Text = draftText;

                    break;
                case PostStatus.PendingApproval:

                    string pendingReviewText = " (" + pc.Count + ")";

                    PendingReview1.Text = pendingReviewText;
                    PendingReview2.Text = pendingReviewText;
                    PendingReview3.Text = pendingReviewText;
                    PendingReview4.Text = pendingReviewText;
                    PendingReview5.Text = pendingReviewText;

                    break;
                case PostStatus.RequiresChanges:

                    string requiresChangesText = " (" + pc.Count + ")";

                    RequiresChanges1.Text = requiresChangesText;
                    RequiresChanges2.Text = requiresChangesText;
                    RequiresChanges3.Text = requiresChangesText;
                    RequiresChanges4.Text = requiresChangesText;
                    RequiresChanges5.Text = requiresChangesText;

                    break;
            }
        }
    }

    protected void rptCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            CategoryCount cc = (CategoryCount)e.Item.DataItem;

            HyperLink a = (HyperLink)e.Item.FindControl("parentCategory");

            if (cc.Count != 0)
            {
                a.NavigateUrl = "javascript:categoryselect('" + cc.ID + "')";
                a.Text = cc.Name + " (" + cc.Count + ")";
            }
            else
            {
                a.Attributes.Add("style", "text-decoration: none;");
                a.Text = cc.Name;
            }

            List<CategoryCount> children = (List<CategoryCount>)cats.FindAll(
                                                delegate(CategoryCount c)
                                                {
                                                    return c.ParentId == cc.ID;
                                                });

            children.Sort(
                delegate(CategoryCount c, CategoryCount c1)
                {
                    return c.Name.CompareTo(c1.Name);
                });

            if (children != null && children.Count > 0)
            {
                Repeater c = (Repeater)e.Item.FindControl("rptCategoriesNested");
                c.ItemDataBound += new RepeaterItemEventHandler(rptCategoriesNested_ItemDataBound);
                c.DataSource = children;
                c.DataBind();

                currentChildIndex = 0;
            }
        }
    }

    protected void rptCategoriesNested_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            // increment the child index counter
            ++currentChildIndex;

            CategoryCount cat = (CategoryCount)e.Item.DataItem;

            // how many children are there?
            List<CategoryCount> children = (List<CategoryCount>)cats.FindAll(
                                                delegate(CategoryCount c)
                                                {
                                                    return c.ParentId == cat.ParentId;
                                                });

            if (children != null && children.Count > 0)
            {
                Image img = (Image)e.Item.FindControl("TreeImage");

                if (currentChildIndex < children.Count)
                    img.ImageUrl = ResolveUrl("~/graffiti-admin/common/img/m.gif");
                else
                    img.ImageUrl = ResolveUrl("~/graffiti-admin/common/img/b.gif");
            }
        }
    }
            
}
