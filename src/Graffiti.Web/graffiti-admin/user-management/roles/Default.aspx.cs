using System;
using System.Collections.Generic;
using System.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Graffiti.Core;
using System.Web.UI.HtmlControls;
using System.Web;

public partial class graffiti_admin_roles_Default : AdminControlPanelPage
{
	protected string DecodeFromQS(string key)
	{
		return Request.QueryString[key] != null ? HttpUtility.HtmlDecode(HttpUtility.UrlDecode(Request.QueryString[key])) : null;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		LiHyperLink.SetNameToCompare(Context, "UserManagement");

		string role = DecodeFromQS("role");

		if (!Page.IsPostBack)
		{
			SetupTogglePermissionsScript(read, edit, publish, read, "read");
			SetupTogglePermissionsScript(read, edit, publish, edit, "edit");
			SetupTogglePermissionsScript(read, edit, publish, publish, "publish");

			SetupTogglePermissionsScript(readRolePermission, editRolePermission, publishRolePermission, readRolePermission, "read");
			SetupTogglePermissionsScript(readRolePermission, editRolePermission, publishRolePermission, editRolePermission, "edit");
			SetupTogglePermissionsScript(readRolePermission, editRolePermission, publishRolePermission, publishRolePermission, "publish");

			if (!String.IsNullOrEmpty(role))
			{
				RolePermissionsCollection rpc = RolePermissionManager.GetRolePermissions();

				RolePermissions rp = rpc.Find(
													 delegate(RolePermissions rper)
													 {
														 return rper.RoleName.ToLower() == role.ToLower();
													 });

				if (rp != null)
				{
					readRolePermission.Checked = rp.HasRead;
					editRolePermission.Checked = rp.HasEdit;
					publishRolePermission.Checked = rp.HasPublish;
				}
			}
		}

		if (role != null)
		{
			string encodedRoleName = HttpUtility.HtmlEncode(role);

			if (!IsPostBack)
			{
				if (Request.QueryString["new"] != null)
				{
					Message.Text = string.Format("The role <strong>{0}</strong> was created.", encodedRoleName);
					Message.Type = StatusType.Success;
				}

				litExistingRoleName.Text = encodedRoleName;
				PageText.Text = "Update " + encodedRoleName;

				CategoryList.DataSource = new CategoryController().GetAllCachedCategories();
				CategoryList.DataBind();
			}

			new_role_container.Visible = false;
			Role_List.Visible = false;
			role_edit_form.Visible = true;
		}
		else
		{
			if (!Page.IsPostBack)
			{
				RolePermissionsCollection rps = RolePermissionManager.GetRolePermissions();

				rps.Sort(delegate(RolePermissions rp1, RolePermissions rp2)
				{
					return Comparer<string>.Default.Compare(rp1.RoleName, rp2.RoleName);
				});

				// move everyone to the top
				RolePermissionsCollection rpss = new RolePermissionsCollection();

				foreach (RolePermissions rp in rps)
				{
					if (rp.RoleName == GraffitiUsers.EveryoneRole)
						rpss.Insert(0, rp);
				}

				foreach (RolePermissions rp in rps)
				{
					if (rp.RoleName != GraffitiUsers.EveryoneRole)
						rpss.Add(rp);
				}

				Role_List.DataSource = rpss;
				Role_List.DataBind();

				if (Request.QueryString["roleSaved"] != null)
				{
					string roleSaved = HttpUtility.UrlDecode(Request.QueryString["roleSaved"]);
					Message.Text = string.Format("The role <strong>{0}</strong> was updated.", roleSaved);
					Message.Type = StatusType.Success;
				}
			}

			new_role_container.Visible = true;
			role_edit_form.Visible = false;
			Role_List.Visible = true;
		}
	}

	protected void CategoryList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		string roleName = DecodeFromQS("role");

		Category category = e.Item.DataItem as Category;
		HiddenField cat = e.Item.FindControl("categoryId") as HiddenField;
		Label categoryName = e.Item.FindControl("categoryName") as Label;

		if (cat != null && category != null)
			cat.Value = category.Id.ToString();

		if (categoryName != null && category != null)
			categoryName.Text = category.ParentId > 0 ? category.Parent.Name + " - " + category.Name : category.Name;

		CheckBox read = e.Item.FindControl("readRoleCatPermission") as CheckBox;
		CheckBox edit = e.Item.FindControl("editRoleCatPermission") as CheckBox;
		CheckBox publish = e.Item.FindControl("publishRoleCatPermission") as CheckBox;

		if (read != null && edit != null && publish != null)
		{
			SetupTogglePermissionsScript(read, edit, publish, read, "read");
			SetupTogglePermissionsScript(read, edit, publish, edit, "edit");
			SetupTogglePermissionsScript(read, edit, publish, publish, "publish");

			RoleCategoryPermissionsCollection rpc = RolePermissionManager.GetRoleCategoryPermissions();

			RoleCategoryPermissions rp = rpc.Find(
												 delegate(RoleCategoryPermissions rcper)
												 {
													 return rcper.RoleName.ToLower() == roleName.ToLower() &&
																rcper.CategoryId == category.Id;
												 });

			if (rp != null)
			{
				read.Checked = rp.HasRead;
				edit.Checked = rp.HasEdit;
				publish.Checked = rp.HasPublish;
			}
		}
	}

	protected void CreateRole_Click(object sender, EventArgs e)
	{
		string encodedRoleName = HttpUtility.HtmlEncode(txtRoleName.Text);

		if (RolePermissionManager.IsDuplicate(txtRoleName.Text))
		{
			Message.Text = string.Format("The role <strong>{0}</strong> already exists.", encodedRoleName);
			Message.Type = StatusType.Error;
			return;
		}

		if (txtRoleName.Text == "gAdmin")
		{
			Message.Text = string.Format("The role <strong>{0}</strong> is a reserved Graffiti Role and cannot be used.", encodedRoleName);
			Message.Type = StatusType.Error;
			return;
		}

		GraffitiUsers.AddUpdateRole(txtRoleName.Text, read.Checked, edit.Checked, publish.Checked);

		Response.Redirect(string.Format("~/graffiti-admin/user-management/roles/?role={0}&new=true", HttpUtility.UrlEncode(encodedRoleName)));
	}

	protected void EditRoles_Save(object sender, EventArgs e)
	{
		string roleName = DecodeFromQS("role");
		bool isCategoryPermissions = false;

		RolePermissionManager.ClearPermissionsForRole(roleName);

		foreach (RepeaterItem ri in CategoryList.Items)
		{
			HiddenField cat = ri.FindControl("categoryId") as HiddenField;
			CheckBox read = ri.FindControl("readRoleCatPermission") as CheckBox;
			CheckBox edit = ri.FindControl("editRoleCatPermission") as CheckBox;
			CheckBox publish = ri.FindControl("publishRoleCatPermission") as CheckBox;

			if (read != null && edit != null && publish != null)
			{
				if (read.Checked || edit.Checked || publish.Checked)
				{
					isCategoryPermissions = true;
					GraffitiUsers.AddUpdateRole(roleName, Convert.ToInt32(cat.Value), read.Checked, edit.Checked, publish.Checked);
				}
			}
		}

		if (!isCategoryPermissions)
		{
			GraffitiUsers.AddUpdateRole(roleName, readRolePermission.Checked, editRolePermission.Checked, publishRolePermission.Checked);
		}
		else
		{
			GraffitiUsers.AddUpdateRole(roleName, false, false, false);
		}

		Response.Redirect(string.Format("~/graffiti-admin/user-management/roles/?roleSaved={0}", HttpUtility.UrlEncode(HttpUtility.HtmlEncode(roleName))));
	}

	protected bool CanDelete()
	{
		return GraffitiUsers.IsAdmin(GraffitiUsers.Current);
	}

	protected string IsAltRow(int index)
	{
		if (index % 2 == 0)
			return string.Empty;
		else
			return " class=\"alt\"";
	}

	protected string IsNotSystemRole(string role)
	{
		if (role != GraffitiUsers.EveryoneRole && role != GraffitiUsers.ManagerRole && role != GraffitiUsers.ContributorRole)
			return "";
		else
			return "style=\"display: none;\"";
	}

	private void SetupTogglePermissionsScript(CheckBox read, CheckBox edit, CheckBox publish, CheckBox applyTo, string command)
	{
		string onclick = "togglePermissions('{0}','{1}','{2}','{3}');";

		applyTo.Attributes.Add("onclick", String.Format(onclick, read.ClientID, edit.ClientID, publish.ClientID, command));
	}
}
