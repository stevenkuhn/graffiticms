using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using DataBuddy;

namespace Graffiti.Core
{
    public partial class User : IGraffitiUser, IMenuItem
    {
        private string[] _roles = null;
        public string[] Roles
        {
            get
            {
                if(_roles == null)
                {
                    UserRoleCollection urc = UserRoleCollection.FetchByColumn(UserRole.Columns.UserId, Id);
                    List<string> roles = new List<string>(urc.Count);
                    foreach(UserRole ur in urc)
                        roles.Add(ur.RoleName);

                    _roles = roles.ToArray();
                }

                return _roles;
            }
        }



        private string _loadedPassword = null;

        protected override void BeforeValidate()
        {
            if (UniqueId == Guid.Empty)
                UniqueId = Guid.NewGuid();
            
            if (string.IsNullOrEmpty(Password))
                throw new Exception("Invalid Password");

            if (_loadedPassword != Password)
            {
                PasswordFormat = 1;
                PasswordSalt = GenerateSalt();
                Password = EncodePassword(Password, PasswordSalt);
            }


            if (string.IsNullOrEmpty(ProperName))
                ProperName = Name;

        }



//        protected override void PostUpdate()
//        {
//            base.PostUpdate();

//            ZCache.RemoveCache("Users");
//        }

        protected override void Loaded()
        {
            //use this to check for password updates
            _loadedPassword = Password;

            base.Loaded();
        }


        public override void Save()
        {
            Events.Instance().ExecuteUserBeforeUserUpdate(this);

            base.Save();

            Events.Instance().ExecuteAfterUserUpdated(this);
        }

        private static string GenerateSalt()
        {
            byte[] buf = new byte[16];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        internal static string EncodePassword(string pass, string salt)
        {
            byte[] bIn = Encoding.Unicode.GetBytes(pass);
            byte[] bSalt = Convert.FromBase64String(salt);
            byte[] bAll = new byte[bSalt.Length + bIn.Length];

            Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
            Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);

            return Convert.ToBase64String(HashAlgorithm.Create("MD5").ComputeHash(bAll));
        }

        #region IMenuItem Members

        public List<MenuItem> GetMenuItems()
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            menuItems.Add(new MenuItem("Name", "$user.Name", "The username", "User"));
            menuItems.Add(new MenuItem("ProperName", "$user.ProperName", "A user\'s formal name. If null, it will be the Name", "User"));
            menuItems.Add(new MenuItem("Email", "$user.Email", "", "User"));
            menuItems.Add(new MenuItem("TimeZoneOffSet", "$user.TimeZoneOffSet", "", "User"));
            menuItems.Add(new MenuItem("WebSite", "$user.WebSite", "", "User"));
            menuItems.Add(new MenuItem("Bio", "$user.Bio", "", "User"));

            return menuItems;
        }

        #endregion
    }
}