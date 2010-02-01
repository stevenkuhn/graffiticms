using System;
using System.Web;
using System.Web.UI;
using Graffiti.Core;
using DataBuddy;
using System.Collections.Generic;

public partial class graffiti_admin_presentation_category_widget : AdminControlPanelPage
{
    public string ItemFormat = "<div style=\"border: solid 1px #999; padding: 4px;\"><strong>{0}</strong><div style=\"text-align: right;\"><a title=\"Delete Link\" href=\"javascript:void();\" onclick=\"remove_Link('{0}','{1}'); return false;\">Delete</a></div></div>";

    protected void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context, "presentation");
        if (!IsPostBack)
        {
            CategoryWidget widget = Widgets.Fetch(new Guid(Request.QueryString["id"])) as CategoryWidget;
            if (widget == null)
                throw new Exception("The widget does not exist");

            BindData(widget);
        }
    }

    private void BindData(CategoryWidget widget)
    {
        txtTitle.Text = widget.Title;

        CategoryCollection cc = new CategoryController().GetTopLevelCachedCategories();

        CategoryCollection InUse = new CategoryCollection();
        CategoryCollection NotInUse = new CategoryCollection();

        NotInUse.AddRange(cc);

        foreach(int i in widget.CategoryIds)
        {
            bool found = false;
            foreach(Category c in NotInUse)
            {
                if(i == c.Id)
                {
                    InUse.Add(c);
                    found = true;
                }
            }

            if(found)
            {
                NotInUse.Remove(InUse[InUse.Count - 1]);
            }
        }

        the_Categories.DataSource = NotInUse;
        the_Categories.DataBind();

        existing_items.Items.Clear();
        foreach (Category c in InUse)
        {
            existing_items.Items.Add(new Telligent.Glow.OrderedListItem(string.Format(this.ItemFormat, c.Name, c.Id), c.Name, c.Id.ToString()));
        }
    }

    protected void PageWidget_Save_Click(object sender, EventArgs e)
    {
        CategoryWidget widget = Widgets.Fetch(new Guid(Request.QueryString["id"])) as CategoryWidget;
        if (widget == null)
            throw new Exception("The widget does not exist");

        try
        {
            List<int> catIds = new List<int>();
            foreach (Telligent.Glow.OrderedListItem item in existing_items.Items)
            {
                catIds.Add(int.Parse(item.Value));
            }

            widget.CategoryIds = catIds.ToArray();
            widget.Title = txtTitle.Text;
            Widgets.Save(widget);

            Response.Redirect("?ws=" + widget.Id);
        }
        catch(Exception ex)
        {
            Message.Text = "Your widget could not be saved. Reason: " + ex.Message;
            Message.Type = StatusType.Error;
            return;
        }
    }
}
