using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Graffiti.Core;
using Repeater=Graffiti.Core.Repeater;
using System.Collections;

public partial class graffiti_admin_categories_Default : ManagerControlPanelPage
    {
        private CategoryCollection cats;
        private int currentChildIndex = 0; // this will hold what child we are on so we know which img to display in the hierarchical view

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

                    if (Request.QueryString["new"] != null && !IsPostBack)
                    {
                        Message.Text = "The category <strong>" + c.Name + "</strong> was created";
                        Message.Type = StatusType.Success;
                    }

                    txtExistingCategory.Text = Server.HtmlDecode(c.Name);
                    txtFeedBurner.Text = c.FeedUrlOverride;
                    Editor.Value = c.Body;
                    txtKeywords.Text = Server.HtmlDecode(c.MetaKeywords ?? string.Empty);
                    txtMetaScription.Text = Server.HtmlDecode(c.MetaDescription ?? string.Empty);
                    SortOrder.SelectedValue = c.SortOrder.ToString();

                    txtExistingLinkName.Text =  Util.CleanForUrl(c.FormattedName).Replace("-", " ");
                    if (c.ParentId > 0)
                    {
                        Category parent = new CategoryController().GetCachedCategory(c.ParentId, false);
                        existingParentLinkName.Text = parent.LinkName + "/ ";
                    }
                }

                new_category_container.Visible = false;
                Category_List.Visible = false;
                category_edit_form.Visible = true;

            }
            else
            {
                if (!IsPostBack)
                {
                    CategoryCollection parents = new CategoryController().GetTopLevelCachedCategories();

                    foreach(Category parent in parents)
                    {
                        Parent_Categories.Items.Add(new ListItem(Server.HtmlDecode(parent.Name),parent.Id.ToString()));
                    }



                    Parent_Categories.Items.Insert(0, new ListItem("[No parent category]", "-1"));

                    if (Request.QueryString["upd"] != null)
                    {
                        Message.Text = "The category <strong>" + Request.QueryString["upd"] + "</strong> was updated";
                        Message.Type = StatusType.Success;
                    }
                }
                new_category_container.Visible = true;
                category_edit_form.Visible = false;
                Category_List.Visible = true;

                GetCategories();
            }
        }

        protected void EditCategory_Click(object sender, EventArgs e)
        {
            try
            {
                Category c = new Category(Request.QueryString["id"]);
                c.Name = Server.HtmlEncode(txtExistingCategory.Text.Trim());
                c.FormattedName = txtExistingLinkName.Text.Trim();
                c.Body = Editor.Value;
                c.FeedUrlOverride = txtFeedBurner.Text;
                c.MetaDescription = Server.HtmlEncode(txtMetaScription.Text.Trim());
                c.MetaKeywords = Server.HtmlEncode(txtKeywords.Text.Trim());
                c.SortOrder = (SortOrderType)Enum.Parse(typeof(SortOrderType), SortOrder.SelectedValue, true);
                c.Save();

                Response.Redirect("~/graffiti-admin/categories/?upd=" + c.Name);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (!string.IsNullOrEmpty(exMessage) && exMessage.IndexOf("UNIQUE") > -1)
                    exMessage = "This category already exists";

                Message.Text = "A category with the name of " + txtExistingCategory.Text + " could not be created <br />" +
                               exMessage;
                Message.Type = StatusType.Error;
            }
        }

        protected void CreateCategory_Click(object sender, EventArgs e)
        {
            try
            {
                Category c = new Category();
                c.Name = Server.HtmlEncode(txtCategory.Text.Trim());
                c.ParentId = Int32.Parse(Parent_Categories.SelectedValue);

                c.Save();
                Response.Redirect("~/graffiti-admin/categories/?id=" + c.Id + "&new=true");

            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (!string.IsNullOrEmpty(exMessage) && exMessage.IndexOf("UNIQUE") > -1)
                    exMessage = "This category already exists";

                Message.Text = "A category with the name of " + txtCategory.Text + " could not be created <br />" +
                               exMessage;
                Message.Type = StatusType.Error;
            }
        }

        private void GetCategories()
        {
            cats = new CategoryController().GetCachedCategories();

            List<Category> parents = (List<Category>)cats.FindAll(
                                                        delegate(Category c)
                                                        {
                                                            return c.ParentId >= 0;
                                                        });

            if (cats != null && cats.Count > 0)
            {
                Category_List.Visible = true;

                
                CategoryCollection source = new CategoryController().GetTopLevelCachedCategories();
                source.Sort(
                    delegate(Category c, Category c1)
                    {
                        return c.Name.CompareTo(c1.Name);
                    });

                Categories_List.DataSource = source;
                Categories_List.ItemDataBound += new RepeaterItemEventHandler(Categories_List_ItemDataBound);
                Categories_List.DataBind();
            }
            else
            {
                Category_List.Visible = false;
            }
        }

        protected void Categories_List_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Category cat = (Category)e.Item.DataItem;

                List<Category> children = (List<Category>)cats.FindAll(
                                                    delegate(Category c)
                                                    {
                                                        return c.ParentId == cat.Id;
                                                    });
                if (children != null && children.Count > 0)
                {
                    // reset the child index counter
                    currentChildIndex = 0;

                    Repeater c = (Repeater)e.Item.FindControl("NestedCategoriesList");
                    c.ItemDataBound += new RepeaterItemEventHandler(NestedCategoriesList_ItemDataBound);
                    c.DataSource = children;
                    c.DataBind();
                }
            }
        }

        protected void NestedCategoriesList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // increment the child index counter
                ++currentChildIndex;

                Category cat = (Category)e.Item.DataItem;

                // how many children are there?
                List<Category> children = (List<Category>)cats.FindAll(
                                                    delegate(Category c)
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

        protected void lbDelete_Command(object sender, CommandEventArgs args)
        {
            int id = Convert.ToInt32(args.CommandArgument);
            Category c = new Category(id);

            if (c.HasChildren)
            {
                Message.Text = "You cannot delete this category because it has child categories.";
                Message.Type = StatusType.Error;
                return;
            }

            List<PostCount> postCounts = Post.GetPostCounts(id, null);

            if (postCounts != null && postCounts.Count > 0)
            {
                int totalPosts = 0;
                foreach (PostCount p in postCounts)
                    totalPosts += p.Count;

                if (totalPosts == 1)
                {
                    Message.Text = "You cannot delete this category because it is in use by " + totalPosts + " post.";
                    Message.Text += " <a href=\"" + ResolveUrl("~/graffiti-admin/posts/?category=") + id + "\">Click here</a> to change this post to another category or delete it.";
                    Message.Type = StatusType.Error;
                }
                else
                {
                    Message.Text = "You cannot delete this category because it is in use by " + totalPosts + " posts.";
                    Message.Text += " <a href=\"" + ResolveUrl("~/graffiti-admin/posts/?category=") + id + "\">Click here</a> to change these posts to another category or delete them.";
                    Message.Type = StatusType.Error;
                }
                return;
            }

            try
            {
                // destroy any deleted posts in this category
                Post.DestroyDeletedPostCascadingForCategory(id);

                Category.Destroy(Category.Columns.Id, id);

                NavigationSettings navSettings = NavigationSettings.Get();
                DynamicNavigationItem item = navSettings.Items == null ? null : navSettings.Items.Find(
                                                                                        delegate(DynamicNavigationItem itm)
                                                                                        {
                                                                                            return itm.CategoryId == id;
                                                                                        });

                if (item != null)
                    NavigationSettings.Remove(item.Id);

                CategoryController.Reset();
                GetCategories();

                Message.Text = "The category " + c.Name + " was deleted.";
            }
            catch (Exception ex)
            {
                Message.Text = ex.Message;
                Message.Type = StatusType.Error;
            }
        }
    }