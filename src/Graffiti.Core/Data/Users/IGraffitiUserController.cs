using System.Collections.Generic;

namespace Graffiti.Core
{
    public interface IGraffitiUserController
    {
        bool ChangePassword(string username, string old_password, string new_password);
        bool ChangePassword(string username, string password);
        IGraffitiUser Login(string username, string password);
        void CreateUser(string username, string password, string email, string role);
        IGraffitiUser GetUser(string username);
        //List<IGraffitiUser> GetUsers(string role);
        string[] GetUsersInRole(string roleName);
        void RemoveUserFromRole(string userName, string RoleName);
        void AddUserToRole(string userName, string RoleName);
        void Save(IGraffitiUser user, string modifed_by);

        void AddRole(string roleName);
        void DeleteRole(string roleName);

        bool CanDeleteUsers
        {
            get;
        }
        void DeleteUser(IGraffitiUser user);

        bool CanRenameUsers
        {
            get;
        }
        void RenameUser(string oldUserName, string newUserName);

    }
}