using System.Collections.Generic;
using System.Data;
using DataBuddy;

namespace Graffiti.Core
{
    public class GraffitiCoreUserController : IGraffitiUserController
    {
        public bool ChangePassword(string username, string old_password, string new_password)
        {
			username = username.ToLower();
            User user = Login(username, old_password) as User;
            if(user != null)
            {
                user.Password = new_password;
                GraffitiUsers.Save(user, null);
                return true;
            }

            return false;
        }

        public bool ChangePassword(string username, string password)
        {
			username = username.ToLower();
            User user = User.FetchByColumn(User.Columns.Name, username);
            if(!user.IsNew)
            {
                user.Password = password;
                GraffitiUsers.Save(user,null);
                return true;
            }

            return false;
        }

        public IGraffitiUser Login(string username, string password)
        {
			username = username.ToLower();
            User user = GetUser(username) as User;
            if (user != null && user.IsLoaded && !user.IsNew)
            {
                if (user.PasswordFormat == 0 && ((user.Password == "change_me") ? password.Equals(SiteSettings.DefaultPassword) :  password.Equals(user.Password)))
                    return user;
                else if (user.PasswordFormat == 1 && User.EncodePassword(password, user.PasswordSalt) == user.Password)
                    return user;
            }
            return null;

        }

        public void CreateUser(string username, string password, string email, string role)
        {
			username = username.ToLower();

            User user = new User();
            user.Name = username;
            user.Password = password;
            user.Email = email;

            user.Save();

            UserRole userRole = new UserRole();
            userRole.UserId = user.Id;
            userRole.RoleName = role;
            userRole.Save();
        }

        public IGraffitiUser GetUser(string username)
        {
			username = username.ToLower();

            User user =  User.FetchByColumn(User.Columns.Name, username);
            if (user.IsLoaded && !user.IsNew)
                return user;

            return null;
        }

        //public List<IGraffitiUser> GetUsers(string role)
        //{
        //    throw new NotImplementedException();
        //}

        public string[] GetUsersInRole(string roleName)
        {
            QueryCommand command = new QueryCommand("SELECT u.Name FROM graffiti_Users AS u INNER JOIN graffiti_UserRoles AS ur on u.Id = ur.UserId WHERE ur.RoleName = " + DataService.Provider.SqlVariable("RoleName"));
            command.Parameters.Add(UserRole.FindParameter("RoleName")).Value = roleName;
            List<string> userNames = new List<string>();
            using(IDataReader reader = DataService.ExecuteReader(command))
            {
                while(reader.Read())
                {
                    userNames.Add(reader["Name"] as string);
                }

                reader.Close();
            }
           
            return userNames.ToArray();
        }

        public void RemoveUserFromRole(string userName, string roleName)
        {
			userName = userName.ToLower();
            Query q = UserRole.CreateQuery();
            q.AndWhere(UserRole.Columns.UserId, (GraffitiUsers.GetUser(userName) as User).Id);
            q.AndWhere(UserRole.Columns.RoleName, roleName);
            UserRoleCollection urCol = new UserRoleCollection();
            urCol.LoadAndCloseReader(q.ExecuteReader());
            if (urCol.Count > 0)
                UserRole.Destroy(urCol[0].Id);

            
        }

        public void AddUserToRole(string userName, string RoleName)
        {
			userName = userName.ToLower();
            User user = (User)GraffitiUsers.GetUser(userName);

            Query q = UserRole.CreateQuery();
            q.AndWhere(UserRole.Columns.UserId, user.Id);
            q.AndWhere(UserRole.Columns.RoleName, RoleName);
            UserRoleCollection urCol = new UserRoleCollection();
            urCol.LoadAndCloseReader(q.ExecuteReader());
            if (urCol.Count == 0)
            {
                UserRole ur = new UserRole();
                ur.UserId = user.Id;
                ur.RoleName = RoleName;
                ur.Save();
            }
        }

        public void Save(IGraffitiUser user, string modifed_by)
        {
            User internal_User = user as User;
            internal_User.Save(modifed_by);
        }

        public void AddRole(string roleName)
        {
        }

        public void DeleteRole(string roleName)
        {
        }

        public bool CanDeleteUsers
        {
            get { return true; }
        }

        public void DeleteUser(IGraffitiUser user)
        {
            User.Destroy(User.Columns.UniqueId, user.UniqueId);
        }

        public bool CanRenameUsers
        {
            get { return true; }
        }

        public void RenameUser(string oldUserName, string newUserName)
        {
            oldUserName = oldUserName.ToLower();
            User user = (User)GraffitiUsers.GetUser(oldUserName);
            user.Name = newUserName;
            GraffitiUsers.Save(user, null);
        }


    }
}