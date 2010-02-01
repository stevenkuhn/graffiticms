using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DataBuddy;

namespace Graffiti.Core
{
    public partial class VersionStore
    {
        public static int VersionFile(FileInfo fi)
        {
            VersionStore vs = new VersionStore();
            vs.Name = fi.FullName;
            vs.Version =  CurrentVersion(fi) + 1;
            vs.Type = Util.GetMapping(fi.FullName);
            vs.UniqueId = Guid.NewGuid();

            using (StreamReader sr = new StreamReader(fi.FullName))
            {
                vs.Data = sr.ReadToEnd();
                sr.Close();
            }

            vs.Save(GraffitiUsers.Current.Name, SiteSettings.CurrentUserTime);

            return vs.Id;
        }

        public static int CurrentVersion(FileInfo fi)
        {
            Query q = CreateQuery();
            q.Top = "1";
            q.AndWhere(Columns.Name, fi.FullName);
            q.AndWhere(Columns.Type, Util.GetMapping(fi.FullName));
            q.OrderByDesc(Columns.Version);

            VersionStore vs = FetchByQuery(q);

            return vs.Version;
        }

        public static VersionStoreCollection GetVersionHistory(string filePath)
        {
            return GetVersionHistory(filePath, true);
        }

		internal static VersionStoreCollection GetVersionHistory(string filePath, bool checkLicensed)
		{
			Query versionQuery = CreateQuery();
			versionQuery.AndWhere(Columns.Name, filePath);
			versionQuery.AndWhere(Columns.Type, Util.GetMapping(filePath));
			versionQuery.OrderByAsc(Columns.Version);

			return VersionStoreCollection.FetchByQuery(versionQuery);
		}

		public static VersionStoreCollection GetVersionHistory(int postId)
		{
			Query versionQuery = CreateQuery();
			versionQuery.AndWhere(Columns.Type, "post/xml");
			versionQuery.AndWhere(Columns.ItemId, postId);
			versionQuery.OrderByDesc(Columns.Version);

			return VersionStoreCollection.FetchByQuery(versionQuery);
		}
	}
}
