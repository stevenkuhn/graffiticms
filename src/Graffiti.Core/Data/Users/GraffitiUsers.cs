using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Security;
using DataBuddy;

namespace Graffiti.Core
{
    public static class GraffitiUsers
    {
        private static IGraffitiUserController controller = null;
        static GraffitiUsers()
        {
            string controller_Type = ConfigurationManager.AppSettings["Graffiti::Users::IGraffitiUserController"] ??
                                     "Graffiti.Core.GraffitiCoreUserController, Graffiti.Core";

            Type type = Type.GetType(controller_Type);

            if (type == null)
                throw new Exception("The Type: " + controller_Type + " does not exist");

            controller = Activator.CreateInstance(type) as IGraffitiUserController;

            if(controller == null)
                throw new Exception("The Type: " + controller_Type + " could not be instantiated as a IGraffitiUserController");
        }

        public static readonly string AdminRole = ConfigurationManager.AppSettings["Graffiti:Roles:Admin"];
        public static readonly string EveryoneRole = ConfigurationManager.AppSettings["Graffiti:Roles:Everyone"];
        public static readonly string ContributorRole = "gContributor";
        public static readonly string ManagerRole = "gManager";
        
        public static IGraffitiUser Current
        {
            get
            {
                HttpContext context = HttpContext.Current;
                IGraffitiUser user = null;
                
                if(context != null)
                    user = context.Items["Current.GraffitiUser"] as IGraffitiUser;

                if (user == null)
                {
                    IPrincipal ip = Thread.CurrentPrincipal;

                    if (ip != null && ip.Identity != null && ip.Identity.IsAuthenticated)
                        user = GraffitiUsers.GetUser(ip.Identity.Name);
                }

                return user;
            }
        }

        /// <summary>
        /// Is this User in a specific role
        /// </summary>
        public static bool IsUserInRole(string username, string role)
        {
			username = username.ToLower();
            foreach(string r in GetUser(username).Roles)
                if(Util.AreEqualIgnoreCase(r,role))
                    return true;

            return false;
        }

        public static bool IsUserInRole(IGraffitiUser user, string role)
        {
            if(user == null)
                return false;

            foreach (string r in user.Roles)
                if (Util.AreEqualIgnoreCase(r, role))
                    return true;

            return false;
        }

        public static bool IsAdmin(IGraffitiUser user)
        {
            return IsUserInRole(user, AdminRole);
        }

        /// <summary>
        /// End user change password option. You must know the existing password.
        /// </summary>
        public static bool ChangePassword(string username, string old_password, string new_password)
        {
			username = username.ToLower();
            return controller.ChangePassword(username, old_password, new_password);
        }

        /// <summary>
        /// Admin change password. 
        /// </summary>
        public static bool ChangePassword(string username, string password)
        {
			username = username.ToLower();
            return controller.ChangePassword(username, password);
        }

        /// <summary>
        /// Returns the User to be logged in. If the credentials are invalid, it returns null.
        /// </summary>
        public static IGraffitiUser Login(string username, string password, bool isOnline)
        {
			username = username.ToLower();
            IGraffitiUser user = controller.Login(username, password);

            if(user != null && isOnline)
            {
                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    context.Items["Current.GraffitiUser"] = user;
                }
            }

            return user;
        }

        public static IGraffitiUser Login(string username, string password)
        {
            return Login(username, password, false);
        }

        /// <summary>
        /// Creates a new user. UserName and Email must be unique. 
        /// </summary>
        public static IGraffitiUser CreateUser(string username, string password, string email, string role)
        {
			username = username.ToLower();
            controller.CreateUser(username,password,email, role);

            if(role != null)
            {
                ZCache.RemoveCache("usersByRole-" + role);
                ZCache.RemoveByPattern("usersByRole-");
            }

            IGraffitiUser user = GetUser(username);
            Events.Instance().ExecuteAfterNewUser(user);

            return user;
        }

        public static IGraffitiUser GetUser(string username)
        {
			username = username.ToLower();
            return GetUser(username, false);
        }

        /// <summary>
        /// Returns a Graffiti User Object, which is a wrapper around the ASP.Net membership system. 
        /// 
        /// Users are cached for 2 minutes using a lower case version of the username. 
        /// </summary>
        /// <returns></returns>
        public static IGraffitiUser GetUser(string username, bool isOnline)
        {
            username = username.ToLower();
            IGraffitiUser user = ZCache.Get<IGraffitiUser>("user-" + username);
            if(user == null)
            {
                user = controller.GetUser(username);
                if(user != null)
                    ZCache.InsertCache("user-" + username, user,120);
            }

            if(isOnline)
            {
                HttpContext context = HttpContext.Current;
                if(context != null)
                {
                    context.Items["Current.GraffitiUser"] = user;
                }
            }

            return user;
        }

        /// <summary>
        /// Returns all the users for a given role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static List<IGraffitiUser> GetUsers(string role)
        {
            string[] userList = ZCache.Get<string[]>("usersByRole-" + role);
            if(userList == null)
            {
                if(role != "*")
                {
                    userList = controller.GetUsersInRole(role);
                }
                else
                {
                    StringCollection sc = new StringCollection();

                    foreach (RolePermissions rp in RolePermissionManager.GetRolePermissions())
                    {
                        string[] users = controller.GetUsersInRole(rp.RoleName);
                        foreach (string u in users)
                            if(!sc.Contains(u))
                                sc.Add(u.ToLower());
                    }

                    string[] admimUsers = controller.GetUsersInRole(GraffitiUsers.AdminRole);
                    foreach (string u in admimUsers)
                        if(!sc.Contains(u))
                            sc.Add(u.ToLower());

                    userList = new string[sc.Count];
                    sc.CopyTo(userList,0);
                }

                ZCache.InsertCache("usersByRole-" + role, userList, 180);
            }

            List<IGraffitiUser> the_users = new List<IGraffitiUser>();
            foreach(string username in userList)
            {
                the_users.Add(GetUser(username));
            }

            the_users.Sort(delegate(IGraffitiUser u1, IGraffitiUser u2) {
                                                                          return
                                                                              Comparer<string>.Default.Compare(
                                                                                  u1.ProperName, u2.ProperName); });

            return the_users;
        }

        public static void AddUserToRole(string userName, string roleName)
        {
            controller.AddUserToRole(userName,roleName);
        }

        public static void RemoveUserFromRole(string userName, string roleName)
        {
            controller.RemoveUserFromRole(userName, roleName);
        }

        public static void Save(IGraffitiUser user, string modified_by)
        {
            if (user == null)
                throw new Exception("The supplied user object is null and cannot be edited");

            if (user.UniqueId == Guid.Empty)
                user.UniqueId = Guid.NewGuid();

            controller.Save(user, modified_by);
            ZCache.RemoveCache("user-" + user.Name.ToLower());
        }


        public static void AddUpdateRole(string roleName, bool hasRead, bool hasEdit, bool hasPublish)
        {
            RolePermissions permissions = new RolePermissions();
            permissions.RoleName = roleName;
            permissions.HasRead = hasRead;
            permissions.HasEdit = hasEdit;
            permissions.HasPublish = hasPublish;
            permissions.Save();

            RolePermissionManager.MarkDirty();

            // Add role to other membership databases
            controller.AddRole(roleName);
        }

        public static void AddUpdateRole(string roleName, int categoryID, bool hasRead, bool hasEdit, bool hasPublish)
        {
            RoleCategoryPermissions rcp = new RoleCategoryPermissions();
            rcp.RoleName = roleName;
            rcp.HasRead = hasRead;
            rcp.HasEdit = hasEdit;
            rcp.HasPublish = hasPublish;
            rcp.CategoryId = categoryID;
            rcp.Save();

            RolePermissionManager.MarkDirty();
            
            // Add role to other membership databases
            controller.AddRole(roleName);
        }

        public static void DeleteRole(string roleName)
        {

            // Remove users from role
            List<IGraffitiUser> roleUsers = GraffitiUsers.GetUsers(roleName);
            if (roleUsers != null && roleUsers.Count > 0)
            {
                foreach (IGraffitiUser user in roleUsers)
                {
                    GraffitiUsers.RemoveUserFromRole(user.Name, roleName);
                }
            }

            RolePermissionsCollection rp = RolePermissionManager.GetRolePermissions();
            RoleCategoryPermissionsCollection rpc = RolePermissionManager.GetRoleCategoryPermissions();

            foreach (RolePermissions rperm in rp)
            {
                if (String.Compare(rperm.RoleName, roleName, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    RolePermissions.Destroy(RolePermissions.Columns.RoleName, rperm.RoleName);
                    break;
                }
            }

            foreach (RoleCategoryPermissions rcatperm in rpc)
            {
                if (String.Compare(rcatperm.RoleName, roleName, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    RoleCategoryPermissions.Destroy(RolePermissions.Columns.RoleName, rcatperm.RoleName);
                    break;
                }
            }

            RolePermissionManager.MarkDirty();

            // Remove role from other membership databases
            controller.DeleteRole(roleName);
        }

        /// <summary>
        /// Indicates whether the membership system being used supports deleting users.
        /// </summary>
        public static bool CanDeleteUsers
        {
            get { return controller.CanDeleteUsers; }
        }

        /// <summary>
        /// Deletes a user, and reassigns any content created by that user to another existing user
        /// </summary>
        public static bool DeleteUser(IGraffitiUser user, IGraffitiUser userToAssumeContent, out string errorMessage)
        {

            if (!controller.CanDeleteUsers)
            {
                errorMessage = "The membership system in use does not support deleting users.";
                return false;
            }
            if (user == null)
            {
                throw new Exception("The supplied user object is null and cannot be deleted");
            }
            
            // Check if the user has created any content
            PostCollection pc = new PostCollection();
            Query q = Post.CreateQuery();
            q.AndWhere(Post.Columns.UserName, user.Name);
            pc.LoadAndCloseReader(q.ExecuteReader());

            if (pc != null && pc.Count > 0)
            {
                if (userToAssumeContent == null)
                {
                    errorMessage = "The user you are trying to delete has created posts. Another existing user must be selected to assign these posts to.";
                    return false;
                }
                foreach (Post p in pc)
                {
                    if (p.UserName == user.Name)
                        p.UserName = userToAssumeContent.Name;
                    if (p.ModifiedBy == user.Name)
                        p.ModifiedBy = userToAssumeContent.Name;
                    if (p.CreatedBy == user.Name)
                        p.CreatedBy = userToAssumeContent.Name;
                }
            }

            // Remove from roles
            if (user.Roles != null && user.Roles.Length > 0)
            {
                foreach (string roleName in user.Roles)
                {
                    controller.RemoveUserFromRole(user.Name, roleName);
                }
                ZCache.RemoveByPattern("usersByRole-");
            }

            controller.DeleteUser(user);

            ZCache.RemoveCache("user-" + user.Name.ToLower());

            errorMessage = string.Empty;
            return true;

        }

        /// <summary>
        /// Indicates whether the membership system being used supports renaming users.
        /// </summary>
        public static bool CanRenameUsers
        {
            get { return controller.CanRenameUsers; }
        }

        /// <summary>
        /// Renames a user account
        /// </summary>
        public static void RenameUser(string oldUserName, string newUserName)
        {
            if (!controller.CanDeleteUsers)
            {
                throw new Exception("The membership system in use does not support deleting users");
            }

            IGraffitiUser user = GetUser(oldUserName);
            if (user == null)
            {
                throw new Exception("The supplied username does not exist!");
            }

            oldUserName = oldUserName.ToLower();
            newUserName = newUserName.ToLower();
            controller.RenameUser(oldUserName, newUserName);

            // Check if the user has created/modified any content
            PostCollection pc = new PostCollection();
            Query q = Post.CreateQuery();
            q.OrWhere(Post.Columns.UserName, oldUserName);
            q.OrWhere(Post.Columns.CreatedBy, oldUserName);
            q.OrWhere(Post.Columns.ModifiedBy, oldUserName);
            pc.LoadAndCloseReader(q.ExecuteReader());

            if (pc != null && pc.Count > 0)
            {
                foreach (Post p in pc)
                {
                    if (p.UserName == oldUserName)
                        p.UserName = newUserName;
                    if (p.ModifiedBy == oldUserName)
                        p.ModifiedBy = newUserName;
                    if (p.CreatedBy == oldUserName)
                        p.CreatedBy = newUserName;

                    p.Save();
                }
            }

            // Check if user has created any comments
            CommentCollection cc = new CommentCollection();
            q = Comment.CreateQuery();
            q.OrWhere(Comment.Columns.UserName, oldUserName);
            q.OrWhere(Comment.Columns.CreatedBy, oldUserName);
            q.OrWhere(Comment.Columns.ModifiedBy, oldUserName);
            cc.LoadAndCloseReader(q.ExecuteReader());

            if (cc != null && cc.Count > 0)
            {
                foreach (Comment c in cc)
                {
                    if (c.UserName == oldUserName)
                        c.UserName = newUserName;
                    if (c.ModifiedBy == oldUserName)
                        c.ModifiedBy = newUserName;
                    if (c.CreatedBy == oldUserName)
                        c.CreatedBy = newUserName;

                    c.Save();
                }
            }

            //Check if the user has created any post versions
            VersionStoreCollection vsc = new VersionStoreCollection();
            vsc = VersionStoreCollection.FetchAll();
            
            if (vsc != null && vsc.Count > 0)
            {
                foreach (VersionStore v in vsc)
                {
                    Post vp = ObjectManager.ConvertToObject<Graffiti.Core.Post>(v.Data);

                    if (v.CreatedBy == oldUserName)
                        v.CreatedBy = newUserName;
                    if (v.Type == "post/xml")
                    {
                        if (vp.UserName == oldUserName)
                            vp.UserName = newUserName;
                        if (vp.ModifiedBy == oldUserName)
                            vp.ModifiedBy = newUserName;
                        if (vp.CreatedBy == oldUserName)
                            vp.CreatedBy = newUserName;
                        v.Data = vp.ToXML();
                    }
                    
                    v.Save();
                }
            }
                        
            ZCache.RemoveCache("user-" + oldUserName);
            // Clear roles cache
            if (user.Roles != null && user.Roles.Length > 0)
                ZCache.RemoveByPattern("usersByRole-");

        }

    }
}