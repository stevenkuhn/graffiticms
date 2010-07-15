using System.Collections.Specialized;

namespace GraffitiClient.API
{
    public abstract class ResourceProxy<T> : ServiceProxy where T : class, new()
    {
        internal ResourceProxy(string username, string password, string baseUrl)
            : base(username, password, baseUrl)
        {
        }

  
        public abstract T Get(int id);

        public abstract PagedList<T> Get(NameValueCollection nvc);

        public abstract int Create(T t);

        public abstract bool Update(T t);

        public abstract bool Delete(int id);

    }
}
