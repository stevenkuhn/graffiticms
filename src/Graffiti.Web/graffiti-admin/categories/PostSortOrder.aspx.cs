using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Graffiti.Core;
using Repeater=Graffiti.Core.Repeater;
using System.Collections;

public partial class graffiti_admin_categories_PostSortOrder : AdminControlPanelPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                Util.CanWriteRedirect(Context);
            }

            if (Request.QueryString["id"] != null)
            {
                if (!IsPostBack)
                {
                    Category c = new Category(Request.QueryString["id"]);

                    if (!c.IsLoaded || c.IsNew)
                        throw new Exception("This category id does not exist");

                    Posts.Items.Clear();

                    DataBuddy.Query query = Post.CreateQuery();
                    query.AndWhere(Post.Columns.CategoryId, c.Id);
                    query.AndWhere(Post.Columns.IsDeleted, false);
                    query.OrderByAsc(Post.Columns.SortOrder);

                    string itemFormat = "<div style=\"border: solid 1px #999; padding: 4px;\"><strong>{0}</strong></div>";
                    foreach (Post p in PostCollection.FetchByQuery(query))
                    {
                        Posts.Items.Add(new Telligent.Glow.OrderedListItem(string.Format(itemFormat, p.Title), p.Title, p.Id.ToString()));
                    }
                }
            }
        }
    }