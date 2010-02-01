using System;
using System.Collections.Generic;
using System.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Graffiti.Core;

public partial class graffiti_admin_people_Default : ControlPanelPage
{
    IGraffitiUser user = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context, "UserManagement");

        IGraffitiUser currentUser = GraffitiUsers.Current;
        

        if(Request.QueryString["user"] != null)
        {

            if (!IsPostBack)
            {
                user = GraffitiUsers.GetUser(Request.QueryString["user"]);


                if (user == null)
                    throw new Exception("This user does not exist or cannot be edited.");

                if(!GraffitiUsers.IsAdmin(currentUser) && user.Name != currentUser.Name)
                    throw new SecurityException("You do not have permission to edit this user");


                if (Request.QueryString["new"] != null && !IsPostBack)
                {
                    Message.Text = "The user <strong>" + user.Name + "</strong> was created.";
                    Message.Type = StatusType.Success;
                }
                PageText.Text = "Update " + user.ProperName + "'s profile.";
                AdminUserLinks.Visible = true;
                PasswordLink.NavigateUrl = string.Format("~/graffiti-admin/user-management/users/changepassword.aspx?user={0}", Request.QueryString["user"]);
                if (GraffitiUsers.CanRenameUsers && GraffitiUsers.IsAdmin(GraffitiUsers.Current))
                {
                    AdminUserLinksDelim.Visible = true;
                    RenameLink.Visible = true;
                    RenameLink.NavigateUrl = string.Format("javascript:Telligent_Modal.Open('RenameUser.aspx?user={0}', 400, 200, null);", Request.QueryString["user"]);
                }
                txtExistingUserName.Text = Server.HtmlDecode(user.Name);
                txtProperName.Text = Server.HtmlDecode(user.ProperName);
                txtExistingEmail.Text = user.Email;
                txtAvatar.Text = user.Avatar;
                Editor.Value = user.Bio;
                txtWebsite.Text = string.IsNullOrEmpty(user.WebSite)
                                      ? new Macros().FullUrl(new Urls().Home)
                                      : Server.HtmlEncode(user.WebSite);

                bool isAdmin = GraffitiUsers.IsUserInRole(GraffitiUsers.Current.Name, GraffitiUsers.AdminRole);

                role_section.Visible = isAdmin;
                AllRoles.Visible = isAdmin;

                if (!isAdmin)
                    Cancel_Edit.NavigateUrl = "~/graffiti-admin/";

                if(isAdmin)
                {
                    RolePermissionsCollection rp = RolePermissionManager.GetRolePermissions();

                    RolePermissionsCollection newrp = new RolePermissionsCollection();
                    newrp.AddRange(rp);

                    RolePermissions temp = newrp.Find(delegate(RolePermissions r)
                                                        {
                                                            return r.RoleName == GraffitiUsers.EveryoneRole;
                                                        });

                    if (temp != null)
                        newrp.Remove(temp);

                    newrp.Sort(delegate(RolePermissions rp1, RolePermissions rp2)
                    {
                        return Comparer<string>.Default.Compare(rp1.RoleName, rp2.RoleName);
                    });

                    Roles.DataSource = newrp;
                    Roles.DataBind();

                    foreach (string role in user.Roles)
                    {
                        if (role == GraffitiUsers.AdminRole)
                        {
                            chkAdmin.Checked = true;

                            if(GraffitiUsers.Current.Name == user.Name)
                                chkAdmin.Enabled = false;
                        }
                    }
                }
            }

            new_user_container.Visible = false;
            User_List.Visible = false;
        	user_edit_form.Visible = true;
        }
        else
        {
            
            if (!GraffitiUsers.IsUserInRole(currentUser.Name, GraffitiUsers.AdminRole))
                Response.Redirect("?user=" + currentUser.Name);

            new_user_container.Visible = true;
            user_edit_form.Visible = false;
            User_List.Visible = true;

        	List<IGraffitiUser> users = GraffitiUsers.GetUsers("*");

            User_List.DataSource = users;
            User_List.DataBind();

            // filter out everyone if they are not a content publisher for licensing
            List<IGraffitiUser> filteredUsers = new List<IGraffitiUser>();
            filteredUsers.AddRange(users);

            bool isEveryonePublisher = RolePermissionManager.IsEveryoneAContentPublisher();

            if (!isEveryonePublisher)
            {
                foreach (IGraffitiUser user in users)
                {
                    if (user.Roles != null && user.Roles[0] == GraffitiUsers.EveryoneRole)
                        filteredUsers.Remove(user);
                }
            }
		}
    }

    protected void Roles_OnItemDataBound(object sender, DataListItemEventArgs e)
    {
        RolePermissions p = e.Item.DataItem as RolePermissions;

        if (p != null)
        {
            CheckBox role = e.Item.FindControl("role") as CheckBox;
            role.Text = p.RoleName;

            foreach (string rl in user.Roles)
            {
                if (rl.ToLower() == role.Text.ToLower())
                    role.Checked = true;
            } 
        }
    }

    protected void EditPerson_Click(object sender, EventArgs e)
    {
        try
        {
            IGraffitiUser currentUser = GraffitiUsers.Current;
            IGraffitiUser user = GraffitiUsers.GetUser(Request.QueryString["user"]);
            bool isAdmin = GraffitiUsers.IsAdmin(currentUser);

            if (!isAdmin && user.Name != currentUser.Name)
                throw new SecurityException("You do not have permission to edit this user");

            user.ProperName = Server.HtmlEncode(txtProperName.Text.Trim());
            user.Bio = Editor.Value.Trim();
            user.Email = txtExistingEmail.Text.Trim();

            if (!string.IsNullOrEmpty(txtWebsite.Text.Trim()))
                user.WebSite = Server.HtmlEncode(txtWebsite.Text.Trim());
            else
                user.WebSite = null;

            if (!string.IsNullOrEmpty(txtAvatar.Text.Trim()))
                user.Avatar = Server.HtmlEncode(txtAvatar.Text.Trim());
            else
                user.Avatar = null;

            if (isAdmin)
            {
                foreach (string role in user.Roles)
                    GraffitiUsers.RemoveUserFromRole(user.Name, role);

                GraffitiUsers.AddUserToRole(user.Name, GraffitiUsers.EveryoneRole);

                if (chkAdmin.Checked == true)
                    GraffitiUsers.AddUserToRole(user.Name, GraffitiUsers.AdminRole);

                foreach (DataListItem dli in Roles.Items)
                {
                    CheckBox role = dli.FindControl("role") as CheckBox;

                    if (role.Checked)
                        GraffitiUsers.AddUserToRole(user.Name, role.Text);
                }
            }

            GraffitiUsers.Save(user, GraffitiUsers.Current.Name);

            Message.Text = "The user <strong>" + user.ProperName + "</strong> was updated.";
            Message.Type = StatusType.Success;  


        }
        catch (Exception ex)
        {
            string exMessage = ex.Message;
            if (!string.IsNullOrEmpty(exMessage) && exMessage.IndexOf("UNIQUE") > -1)
                exMessage = "This username (or email) already exists.";

            Message.Text = "A user with the name of " + txtExistingUserName.Text + " could not be updated.<br />" +
                           exMessage;
            Message.Type = StatusType.Error;
        }
    }

    protected void CreateUser_Click(object sender, EventArgs e)
    {
        try
        {
            GraffitiUsers.CreateUser(Server.HtmlEncode(txtUserName.Text.Trim()), txtPassword.Text.Trim(), txtEmail.Text.Trim(), GraffitiUsers.EveryoneRole);

            Response.Redirect("~/graffiti-admin/user-management/users/?user=" + txtUserName.Text + "&new=true");
            
        }
        catch(Exception ex)
        {
            string exMessage = ex.Message;
            if (!string.IsNullOrEmpty(exMessage) && exMessage.IndexOf("UNIQUE") > -1)
                exMessage = "This user (or email) already exists.";

            Message.Text = "A name or email with the name of " + txtUserName.Text + " could not be created.<br />" +
                           exMessage;
            Message.Type = StatusType.Error;
        }
    }

    protected bool CanDelete()
    {
        if (GraffitiUsers.CanDeleteUsers && GraffitiUsers.IsAdmin(GraffitiUsers.Current))
            return true;

        return false;
    }

}
