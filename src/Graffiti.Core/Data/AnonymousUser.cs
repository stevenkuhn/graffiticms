using System;

namespace Graffiti.Core
{
    /// <summary>
    /// Each Graffit request will have a valid user. If the user is not logged in, the AnonymousUser object 
    /// will be used. This user can NEVER be saved. 
    /// 
    /// Since it derives from Graffiti.User, it also implements both IPrinciple and IIdentitity
    /// </summary>
    [Serializable]
    public class AnonymousUser : IUser
    {
        public static readonly string AnonymousUserName = "Anonymous";
        private string _name;
        private string _webSite;

        public AnonymousUser(string name, string webSite)
        {
            _name = name;
            _webSite = webSite;
        }

        public string Name
        {
            get { return _name; }
        }

        public string ProperName
        {
            get { return Name; }
        }

        public string WebSite
        {
            get { return _webSite; }
        }
    }
}