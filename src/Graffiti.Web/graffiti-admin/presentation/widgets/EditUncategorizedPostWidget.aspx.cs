using System;
using System.Web;
using System.Web.UI;
using Graffiti.Core;
using DataBuddy;
using System.Text;
using System.Collections.Generic;

public partial class graffiti_admin_presentation_uncateogrized_post_widget : AdminControlPanelPage
{
    public string ItemFormat = "<div style=\"border: solid 1px #999; padding: 4px;\"><strong>{0}</strong><div style=\"text-align: right;\"><a title=\"Delete Link\" href=\"javascript:void();\" onclick=\"remove_Link('{0}','{1}'); return false;\">Delete</a></div></div>";

    protected void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context, "presentation");
        if (!IsPostBack)
        {
            PageWidget widget = Widgets.Fetch(new Guid(Request.QueryString["id"])) as PageWidget;
            if (widget == null)
                throw new Exception("The widget does not exist");

            BindData(widget);
        }
    }

    private void BindData(PageWidget widget)
    {
        txtTitle.Text = widget.Title;

        Query q = Post.CreateQuery();
        q.AndWhere(Post.Columns.CategoryId, CategoryController.UnCategorizedId);
        q.AndWhere(Post.Columns.IsDeleted, false);
        q.AndWhere(Post.Columns.Status, 1);

        PostCollection pc = new PostCollection();
        pc.LoadAndCloseReader(q.ExecuteReader());

        PostCollection pcInUse = new PostCollection();
        PostCollection pcNotInUse = new PostCollection();

        foreach(int i in widget.PostIds)
        {
            bool found = false;
            foreach(Post p in pc)
            {
                if(i == p.Id)
                {
                    pcInUse.Add(p);
                    found = true;
                }
            }

            if(found)
            {
                pc.Remove(pcInUse[pcInUse.Count - 1]);
            }
        }

        the_Posts.DataSource = pc;
        the_Posts.DataBind();

        existing_items.Items.Clear();
        foreach (Post p in pcInUse)
        {
            existing_items.Items.Add(new Telligent.Glow.OrderedListItem(string.Format(this.ItemFormat, p.Name, p.Id), p.Name, p.Id.ToString()));
        }
    }

    protected void PageWidget_Save_Click(object sender, EventArgs e)
    {
        PageWidget widget = Widgets.Fetch(new Guid(Request.QueryString["id"])) as PageWidget;
        if (widget == null)
            throw new Exception("The widget does not exist");

        try
        {
            List<int> postIds = new List<int>();
            foreach (Telligent.Glow.OrderedListItem item in existing_items.Items)
            {
                postIds.Add(int.Parse(item.Value));
            }

            widget.PostIds = postIds.ToArray();
            widget.Title = txtTitle.Text;
            Widgets.Save(widget);
            ZCache.RemoveCache(widget.DataCacheKey);

            Response.Redirect("?ws=" + widget.Id);
        }
        catch(Exception ex)
        {
            Message.Text = "Your widget called not be saved. Reason: " + ex.Message;
            Message.Type = StatusType.Error;
            return;
        }
    }
}
