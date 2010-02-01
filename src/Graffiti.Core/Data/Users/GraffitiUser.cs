//using System.Security.Principal;
//using System.Threading;
//using System.Web;
//using System.Web.Profile;
//using System.Web.Security;

//namespace Graffiti.Core
//{
//    /// <summary>
//    /// A login user of Graffit
//    /// </summary>
//    public class GraffitiUser : IGraffitiUser, IUser
//    {
//        private MembershipUser _mu = null;
//        private ProfileBase _pb = null;

//        /// <summary>
//        /// Users can only be invoked internally.
//        /// </summary>
//        /// <param name="mu"></param>
//        internal GraffitiUser(MembershipUser mu)
//        {
//            _mu = mu;
//            _pb = ProfileBase.Create(_mu.UserName);
            
//        }

//        /// <summary>
//        /// The User's Name
//        /// </summary>
//        public string Name
//        {
//            get { return _mu.UserName; }
//        }

//        /// <summary>
//        /// The full name of a user. If null or empty this will be the UserName (Name)
//        /// </summary>
//        public string  ProperName
//        {
//            get { string pn = _pb.GetPropertyValue("properName") as string; 
//                if(!string.IsNullOrEmpty(pn))
//                    return pn;
//                else
//                    return Name;
//            }
//            set { _pb.SetPropertyValue("properName", value); }
//        }

//        public string Email
//        {
//            get { return _mu.Email; }
//            set { _mu.Email = value; }
//        }

//        public string  PublicEmail
//        {
//            get { return _pb.GetPropertyValue("publicEmail") as string; }
//            set { _pb.SetPropertyValue("publicEmail", value); }
//        }


//        public double  TimeZoneOffSet
//        {
//            get { return (double)_pb.GetPropertyValue("timezone"); }
//            set { _pb.SetPropertyValue("timezone", value); }
//        }

//        public string WebSite
//        {
//            get { return _pb.GetPropertyValue("webSite") as string; }
//            set { _pb.SetPropertyValue("webSite", value); }
//        }

//        public string  Bio
//        {
//            get { return _pb.GetPropertyValue("bio") as string; }
//            set { _pb.SetPropertyValue("bio", value); }

//        }

//        public string  Avatar
//        {
//            get { return _pb.GetPropertyValue("avatar") as string; }
//            set { _pb.SetPropertyValue("avatar", value); }
//        }

//        private string[] _roles = null;
//        public string[] Roles
//        {
//            get
//            {
//                if (_roles == null)
//                    _roles = System.Web.Security.Roles.GetRolesForUser(Name);

//                return _roles;
//            }
//        }

//        public MembershipUser MembershipUser
//        {
//            get{ return _mu;}
//        }

//        public void Save()
//        {
//            Membership.UpdateUser(_mu);
//            _pb.Save();

//            ZCache.RemoveCache("user-" + Name.ToLower());
//        }

//        public string GetGraffitiRole()
//        {
//            if (IsInRole(GraffitiUsers.AdminRole))
//                return GraffitiUsers.EditorRole;
//            else if (IsInRole(GraffitiUsers.ManagerRole))
//                return GraffitiUsers.ManagerRole;
//            else if (IsInRole(GraffitiUsers.ContributorRole))
//                return GraffitiUsers.ContributorRole;

//            return null;
//        }

//        public bool IsInRole(string roleName)
//        {
//            foreach (string r in Roles)
//                if (Util.AreEqualIgnoreCase(r, roleName))
//                    return true;

//            return false;
//        }
//    }
//}
