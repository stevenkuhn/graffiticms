using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using Graffiti.Core;
using DataBuddy;
using Telligent.Glow;

public partial class graffiti_admin_posts_write_Default : ControlPanelPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		NameValueCollection nvcCustomFields = null;
		IGraffitiUser user = GraffitiUsers.Current;
		bool isAdmin = GraffitiUsers.IsAdmin(user);
		CategoryController cc = new CategoryController();
		Category uncategorized = cc.GetCachedCategory(CategoryController.UncategorizedName, false);
		Post post = null;

		if (Request.QueryString["id"] != null)
			post = new Post(Request.QueryString["id"]);

		ProcessCategoryDropdownList(cc, isAdmin, uncategorized);

		if (!IsPostBack)
		{
			Telligent.Glow.ClientScripts.RegisterScriptsForDateTimeSelector(this);
			Util.CanWriteRedirect(Context);

			SetDefaultFormValues(isAdmin);

			if (Request.QueryString["nid"] != null)
			{
				post = new Post(Request.QueryString["nid"]);
				if (post.IsLoaded)
				{
					if (isAdmin)
					{
						SetMessage("Your post was saved. View: <a href=\"" + post.Url + "\">" + post.Title + "</a>.", StatusType.Success);
					}
					else
					{
						SetMessage("Your post was saved. However, since you do not have permission to publish new content, it will need to be approved before it is viewable.", StatusType.Success);
					}
					FormWrapper.Visible = false;
				}
			}


			if (post != null)
			{
				bool isOriginalPublished = post.IsPublished;
				int currentVersionNumber = post.Version;

				VersionStoreCollection vsc = VersionStore.GetVersionHistory(post.Id);

				if (vsc.Count > 0)
				{
					List<Post> the_Posts = new List<Post>();
					foreach (VersionStore vs in vsc)
					{
						the_Posts.Add(ObjectManager.ConvertToObject<Post>(vs.Data));
					}

					the_Posts.Add(post);

					the_Posts.Sort(delegate(Post p1, Post p2) { return Comparer<int>.Default.Compare(p2.Version, p1.Version); });


					string versionHtml =
						 "<div style=\"width: 280px; overflow: hidden; padding: 6px 0; border-bottom: 1px solid #ccc;\"><b>Revision {0}</b> ({1})<div>by {2}</div><div style=\"font-style: italic;\">{3}</div></div>";
					string versionText = "Revision {0}";
					foreach (Post px in the_Posts)
					{
						VersionHistory.Items.Add(
							 new DropDownListItem(
								  string.Format(versionHtml, px.Version, px.ModifiedOn.ToString("dd-MMM-yyyy"), GraffitiUsers.GetUser(px.ModifiedBy).ProperName, px.Notes),
								  string.Format(versionText, px.Version), px.Version.ToString()));
					}



					int versionToEdit = Int32.Parse(Request.QueryString["v"] ?? "-1");
					if (versionToEdit > -1)
					{
						foreach (Post px in the_Posts)
						{
							if (px.Version == versionToEdit)
							{
								post = px;

								// add logic to change category if it was deleted here
								CategoryCollection cats = new CategoryController().GetCachedCategories();
								Category temp = cats.Find(
																delegate(Category c)
																{
																	return c.Id == post.CategoryId;
																});

								if (temp == null && post.CategoryId != 1)
								{
									post.CategoryId = uncategorized.Id;
									SetMessage("The category ID on this post revision could not be located. It has been marked as Uncategorized. ", StatusType.Warning);
								}

								break;
							}
						}
					}
					else
					{
						post = the_Posts[0];
					}

					VersionHistoryArea.Visible = true;
					VersionHistory.SelectedValue = post.Version.ToString();
					VersionHistory.Attributes["onchange"] = "window.location = '" +
																		 VirtualPathUtility.ToAbsolute("~/graffiti-admin/posts/write/") +
																		 "?id=" + Request.QueryString["id"] +
																		 "&v=' + this.options[this.selectedIndex].value;";
				}


				if (post.Id > 0)
				{
					nvcCustomFields = post.CustomFields();

					txtTitle.Text = Server.HtmlDecode(post.Title);
					txtContent.Text = post.PostBody;
					txtContent_extend.Text = post.ExtendedBody;
					txtTags.Text = post.TagList;
					txtName.Text = Util.UnCleanForUrl(post.Name);
					EnableComments.Checked = post.EnableComments;
					PublishDate.DateTime = post.Published;
					txtNotes.Text = post.Notes;
					postImage.Text = post.ImageUrl;
					FeaturedSite.Checked = (post.Id == SiteSettings.Get().FeaturedId);
					FeaturedCategory.Checked = (post.Id == post.Category.FeaturedId);
					txtKeywords.Text = Server.HtmlDecode(post.MetaKeywords ?? string.Empty);
					txtMetaScription.Text = Server.HtmlDecode(post.MetaDescription ?? string.Empty);
					HomeSortOverride.Checked = post.IsHome;

					ListItem li = CategoryList.Items.FindByValue(post.CategoryId.ToString());
					if (li != null)
						CategoryList.SelectedIndex = CategoryList.Items.IndexOf(li);
					else
						CategoryList.SelectedIndex = CategoryList.Items.IndexOf(CategoryList.Items.FindByValue(uncategorized.Id.ToString()));

					li = PublishStatus.Items.FindByValue(post.Status.ToString());
					if (li != null && post.Status != (int)PostStatus.PendingApproval && post.Status != (int)PostStatus.RequiresChanges)
						PublishStatus.SelectedIndex = PublishStatus.Items.IndexOf(li);
					else if (post.Status == (int)PostStatus.PendingApproval || post.Status == (int)PostStatus.RequiresChanges)
					{
						// turn published on if it is in req changes
						ListItem li2 = PublishStatus.Items.FindByValue(Convert.ToString((int)PostStatus.Publish));
						if (li2 != null)
							PublishStatus.SelectedIndex = PublishStatus.Items.IndexOf(li2);
					}

					if (post.Version != currentVersionNumber && !isOriginalPublished)
					{
						SetMessage("You are editing an unpublished revision of this post.", StatusType.Warning);
					}
					else if (post.Version != currentVersionNumber && isOriginalPublished)
					{
						SetMessage("The post your are editing has been published. However, the revision you are editing has not been published.", StatusType.Warning);
					}
					else if (!isOriginalPublished)
					{
						SetMessage("You are editing an unpublished revision of this post.", StatusType.Warning);
					}

				}
				else
				{
					FormWrapper.Visible = false;
					SetMessage("The post with the id " + Request.QueryString["id"] + " could not be found.", StatusType.Warning);
				}
			}
			else
			{
				ListItem liUncat = CategoryList.Items.FindByText(CategoryController.UncategorizedName);
				if (liUncat != null)
					CategoryList.SelectedIndex = CategoryList.Items.IndexOf(liUncat);
			}
		}

		if (FormWrapper.Visible)
		{
			NavigationConfirmation.RegisterPage(this);
			NavigationConfirmation.RegisterControlForCancel(Publish_Button);

			Page.ClientScript.RegisterStartupScript(GetType(),
				 "Writer-Page-StartUp",
				 "$(document).ready(function() { var eBody = $('#extended_body')[0]; " + (!string.IsNullOrEmpty(txtContent_extend.Text) ? "eBody.style.position = 'static'; eBody.style.visibility = 'visible';" : "eBody.style.position = 'absolute'; eBody.style.visibility = 'hidden';") + "categoryChanged($('#" + CategoryList.ClientID + "')[0]); Publish_Status_Change();});", true);

			Page.ClientScript.RegisterHiddenField("dateChangeFlag", "false");
		}

		CustomFormSettings cfs = CustomFormSettings.Get(int.Parse(CategoryList.SelectedItem.Value));
		if (cfs.HasFields)
		{
			if (nvcCustomFields == null)
			{
				nvcCustomFields = new NameValueCollection();
				foreach (CustomField cf in cfs.Fields)
				{
					if (Request.Form[cf.Id.ToString()] != null)
						nvcCustomFields[cf.Name] = Request.Form[cf.Id.ToString()];
				}
			}

			bool isNewPost = (post != null) && (post.Id < 1);
			the_CustomFields.Text = cfs.GetHtmlForm(nvcCustomFields, isNewPost);
		}
		else
		{
			CustomFieldsTab.Tab.Enabled = false;
			the_CustomFields.Text = "";
		}

		PublishStatus.Attributes.Add("onchange", "Publish_Status_Change();");
	}

	private void SetDefaultFormValues(bool isAdmin)
	{
		EnableComments.Checked = CommentSettings.Get().EnableCommentsDefault;

		featuresiteRegion.Visible = isAdmin;
		featuredCategoryRegion.Visible = isAdmin;

		if (isAdmin || RolePermissionManager.GetPermissions(Int32.Parse(CategoryList.SelectedValue), GraffitiUsers.Current).Publish)
			PublishStatus.Items.Add(new ListItem("Published", "1"));
		else
			P_Status.Style.Add("display", "none");

		PublishStatus.Items.Add(new ListItem("Draft", "2"));


		PublishStatus.Items.Add(new ListItem("Request Approval", "3"));

		if (isAdmin)
			PublishStatus.Items.Add(new ListItem("Requires Changes", "4"));

		PublishDate.DateTime = DateTime.Now.AddHours(SiteSettings.Get().TimeZoneOffSet);
	}

	private void ProcessCategoryDropdownList(CategoryController cc, bool isAdmin, Category uncategorized)
	{
		if (!IsPostBack || Request.Form[CategoryList.UniqueID] != CategoryList.SelectedValue)
		{

			CategoryCollection categories = cc.GetTopLevelCachedCategories();

			CategoryList.Items.Clear();
			foreach (Category parent in categories)
			{
				if (RolePermissionManager.GetPermissions(parent.Id, GraffitiUsers.Current).Edit)
					CategoryList.Items.Add(new ListItem(Server.HtmlDecode(parent.Name), parent.Id.ToString()));

				foreach (Category child in parent.Children)
				{
					if (RolePermissionManager.GetPermissions(child.Id, GraffitiUsers.Current).Edit)
						CategoryList.Items.Add(new ListItem("--" + Server.HtmlDecode(child.Name), child.Id.ToString()));
				}
			}

			if (RolePermissionManager.GetPermissions(uncategorized.Id, GraffitiUsers.Current).Edit)
				CategoryList.Items.Add(new ListItem(uncategorized.Name, uncategorized.Id.ToString()));

			if (isAdmin)
				CategoryList.Items.Add(new ListItem("[Add New Category]", "-1"));

			if (IsPostBack)
				CategoryList.SelectedValue = Request.Form[CategoryList.UniqueID];
		}
	}


	protected void publish_return_click(object sender, EventArgs e)
	{
		try
		{
			if (!IsValid)
				return;

			IGraffitiUser user = GraffitiUsers.Current;

			ListItem catItem = CategoryList.SelectedItem;
			if (catItem.Value == "-1" && String.IsNullOrEmpty(newCategory.Text))
			{
				SetMessage("Please enter a name for the new Category.", StatusType.Error);
				return;
			}

			string extenedBody = txtContent_extend.Text;
			string postBody = txtContent.Text;

			if (string.IsNullOrEmpty(postBody))
			{
				SetMessage("Please enter a post body.", StatusType.Warning);
				return;
			}

			Category c = new Category();

			if (catItem.Value == "-1")
			{
				try
				{
					Category temp = new Category();
					temp.Name = newCategory.Text;
					temp.Save();

					c = temp;

					CategoryController.Reset();
				}
				catch (Exception ex)
				{
					SetMessage("The category could not be created. Reason: " + ex.Message, StatusType.Error);
				}
			}
			else
			{
				c = new CategoryController().GetCachedCategory(Int32.Parse(catItem.Value), false);
			}

			string pid = Request.QueryString["id"];
			Post p = pid == null ? new Post() : new Post(pid);

			if (p.IsNew)
			{
				p["where"] = "web";

				p.UserName = user.Name;

				if (Request.Form["dateChangeFlag"] == "true")
					p.Published = PublishDate.DateTime;
				else
					p.Published = DateTime.Now.AddHours(SiteSettings.Get().TimeZoneOffSet);
			}
			else
			{
				p.Published = PublishDate.DateTime;
			}

			p.ModifiedOn = DateTime.Now.AddHours(SiteSettings.Get().TimeZoneOffSet);

			p.PostBody = postBody;
			if (string.IsNullOrEmpty(extenedBody) || extenedBody == "<p></p>" || extenedBody == "<p>&nbsp;</p>")
			{
				p.ExtendedBody = null;
			}
			else
			{
				p.ExtendedBody = extenedBody;
			}

			p.Title = Server.HtmlEncode(txtTitle.Text);
			p.EnableComments = EnableComments.Checked;
			p.Name = txtName.Text;
			p.TagList = txtTags.Text.Trim();
			p.ContentType = "text/html";
			p.CategoryId = c.Id;
			p.Notes = txtNotes.Text;
			p.ImageUrl = postImage.Text;
			p.MetaKeywords = Server.HtmlEncode(txtKeywords.Text.Trim());
			p.MetaDescription = Server.HtmlEncode(txtMetaScription.Text.Trim());
			p.IsHome = HomeSortOverride.Checked;
			p.PostStatus = (PostStatus)Enum.Parse(typeof(PostStatus), Request.Form[PublishStatus.UniqueID]);

			CustomFormSettings cfs = CustomFormSettings.Get(c);
			if (cfs.HasFields)
			{
				foreach (CustomField cf in cfs.Fields)
				{
					if (cf.FieldType == FieldType.CheckBox && Request.Form[cf.Id.ToString()] == null)
						p[cf.Name] = null; // false.ToString();
                    else if (cf.FieldType == FieldType.DateTime && Request.Form[cf.Id.ToString()].IndexOf("_") > -1)
                        p[cf.Name] = null;
					else
						p[cf.Name] = Request.Form[cf.Id.ToString()];
				}
			}

			if (HasDuplicateName(p))
			{
				SetMessage("A post in the selected category already exists with the same name.", StatusType.Error);
				return;
			}

			PostRevisionManager.CommitPost(p, user, FeaturedSite.Checked, FeaturedCategory.Checked);

            string CatQuery = (Request.QueryString["category"] == null) ? null : (p.Status == 1) ? "&category=" + p.CategoryId : "&category=" + Request.QueryString["category"];
            string AuthQuery = (Request.QueryString["author"] == null) ? null : "&author=" + Request.QueryString["author"];
            Response.Redirect("~/graffiti-admin/posts/" + "?id=" + p.Id + "&status=" + p.Status + CatQuery + AuthQuery);
		}
		catch (Exception ex)
		{
			SetMessage("Your post could not be saved. Reason: " + ex.Message, StatusType.Error);
		}
	}

	private bool HasDuplicateName(Post p)
	{
		Query q = Post.CreateQuery();
		q.PageSize = 1;
		q.AndWhere(Post.Columns.CategoryId, p.CategoryId);
		q.AndWhere(Post.Columns.Name, Util.CleanForUrl(!string.IsNullOrEmpty(p.Name) ? p.Name : p.Title));
		if (!p.IsNew)
			q.AndWhere(Post.Columns.Id, p.Id, Comparison.NotEquals);

		return (PostCollection.FetchByQuery(q).Count > 0);
	}

	private void SetMessage(string text, StatusType type)
	{
		Message1.Text += text;
		Message2.Text += text;
		Message3.Text += text;

		Message1.Type = type;
		Message2.Type = type;
		Message3.Type = type;
	}
}
