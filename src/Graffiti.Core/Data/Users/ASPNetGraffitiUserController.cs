using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Profile;
using System.Web.Security;

namespace Graffiti.Core
{
    public class ASPNetGraffitiUserController : IGraffitiUserController
    {
        public bool ChangePassword(string username, string old_password, string new_password)
        {
            return Membership.Provider.ChangePassword(username, old_password, new_password);
        }

        public bool ChangePassword(string username, string password)
        {
            MembershipUser mu = Membership.GetUser(username);
            string temp = mu.ResetPassword();
            return ChangePassword(username, temp, password);
        }

        public IGraffitiUser Login(string username, string password)
        {
            if (Membership.ValidateUser(username, password))
                return GetUser(username);

            return null;
        }

        public void CreateUser(string username, string password, string email, string role)
        {
            Membership.CreateUser(username, password, email);

            if(role != null)
                Roles.AddUserToRole(username,role);

            ProfileBase pb = ProfileBase.Create(username);
            pb.SetPropertyValue("uniqueId", Guid.NewGuid());
            pb.Save();
        }

        public IGraffitiUser GetUser(string username)
        {
            MembershipUser mu = Membership.GetUser(username);
            if (mu == null)
                throw new Exception("Membership User Does not Exist for " + username);

            return new ASPNetMembershipGraffitiUser(mu);
        }

        public string[] GetUsersInRole(string roleName)
        {
            string[] userNames = null;
            try
            {
                userNames = Roles.GetUsersInRole(roleName);
            }
            catch
            {
                userNames = new string[0];
            }

            return userNames;
        }

        public void RemoveUserFromRole(string userName, string RoleName)
        {
            if(Roles.IsUserInRole(userName,RoleName))
                Roles.RemoveUserFromRole(userName,RoleName);
        }

        public void AddUserToRole(string userName, string RoleName)
        {
            if (!Roles.IsUserInRole(userName, RoleName))
                Roles.AddUserToRole(userName, RoleName);
        }

        public void Save(IGraffitiUser user, string modifed_by)
        {
            ASPNetMembershipGraffitiUser the_User = user as ASPNetMembershipGraffitiUser;
            the_User.Save();
        }

        public void AddRole(string roleName)
        {
            if (!Roles.RoleExists(roleName))
                Roles.CreateRole(roleName);
        }

        public void DeleteRole(string roleName)
        {
            Roles.DeleteRole(roleName);
        }

        public bool CanDeleteUsers
        {
            get { return false; }
        }

        public void DeleteUser(IGraffitiUser user)
        {
            throw new NotImplementedException("The ASPNetGraffitiUserController does not support deleting users.");
        }

        public bool CanRenameUsers
        {
            get { return false; }
        }

        public void RenameUser(string oldUserName, string newUserName)
        {
            throw new NotImplementedException("The ASPNetGraffitiUserController does not support renaming users.");
        }

    }
}