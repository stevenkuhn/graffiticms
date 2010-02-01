using System;
using System.Collections.Generic;
using System.Web;
using DataBuddy;

namespace Graffiti.Core
{
    public static  class PostRevisionManager
    {
        public static void SendRequestedChangesMessage(Post p, IGraffitiUser user)
        {
            List<IGraffitiUser> users = new List<IGraffitiUser>();
            foreach (IGraffitiUser u in GraffitiUsers.GetUsers("*"))
            {
                if (GraffitiUsers.IsAdmin(u) || RolePermissionManager.GetPermissions(p.CategoryId, u).Publish)
                    users.Add(u);
            }

            Macros m = new Macros();

            EmailTemplateToolboxContext pttc = new EmailTemplateToolboxContext();
            pttc.Put("sitesettings", SiteSettings.Get());
            pttc.Put("post", p);
            pttc.Put("user", user);
            pttc.Put("macros", m);
            pttc.Put("home", m.FullUrl(new Urls().Home));
            pttc.Put("adminUrl",
                     m.FullUrl(VirtualPathUtility.ToAbsolute("~/graffiti-admin/posts/write/")) + "?id=" + p.Id + "&v=" +
                     p.Version);

            EmailTemplate template = new EmailTemplate();
            template.Context = pttc;
            template.To = p.User.Email;
            template.Subject = "Changes Requested: " + p.Title;
            template.TemplateName = "RequestChanges.view";

            try
            {
                Emailer.Send(template); 
                //Emailer.Send("RequestChanges.view", p.User.Email, "Changes Requested: " + p.Title, pttc);
                Log.Info("Post Changes Email", p.User.Email + " was sent an email requesting changes");
            }
            catch (Exception ex)
            {
                Log.Error("Email Requested Changes Error", ex.Message);
            }
            
        }

        public static void SendPReqiresApprovalMessage(Post p, IGraffitiUser user)
        {
            List<IGraffitiUser> users = new List<IGraffitiUser>();
            foreach(IGraffitiUser u in GraffitiUsers.GetUsers("*"))
            {
                if (GraffitiUsers.IsAdmin(u) || RolePermissionManager.GetPermissions(p.CategoryId, u).Publish)
                    users.Add(u);
            }
            
            Macros m = new Macros();
            EmailTemplateToolboxContext pttc = new EmailTemplateToolboxContext();
            pttc.Put("sitesettings", SiteSettings.Get());
            pttc.Put("post", p);
            pttc.Put("user", user);
            pttc.Put("macros", m);
            pttc.Put("home", m.FullUrl(new Urls().Home));
            pttc.Put("adminUrl",
                     m.FullUrl(VirtualPathUtility.ToAbsolute("~/graffiti-admin/posts/write/")) + "?id=" + p.Id + "&v=" +
                     p.Version);

            string adminApprovalUrl = m.FullUrl(VirtualPathUtility.ToAbsolute("~/api/approve.ashx")) + "?key={0}&u={1}&id={2}&v={3}";

            EmailTemplate template = new EmailTemplate();
            template.Context = pttc;
            template.Subject = "You have content to approve: " + p.Title;
            template.TemplateName = "QueuedPost.view";

            foreach (IGraffitiUser admin in users)
            {
                template.Context.Put("adminApprovalUrl", string.Format(adminApprovalUrl, admin.UniqueId, admin.Name, p.Id, p.Version));

                try
                {
                    template.To = admin.Email;
                    Emailer.Send(template);
                    
                    //Emailer.Send("QueuedPost.view", admin.Email, "You have content to approve: " + p.Title, pttc);
                }
                catch(Exception ex)
                {
                    Log.Error("Email Error", ex.Message);
                }
            }

            Log.Info("Post approval email", "{0} user(s) were sent an email to approve the post \"{1}\" (id: {2}).", users.Count,p.Title,p.Id);
        }

        public static int CommitPost(Post p, IGraffitiUser user, bool isFeaturedPost, bool isFeaturedCategory)
        {
            Permission perm = RolePermissionManager.GetPermissions(p.CategoryId, user);
            bool isMan = perm.Publish;
            bool isEdit = GraffitiUsers.IsAdmin(user);

            if (isMan || isEdit)
            {
                p.IsPublished = (p.PostStatus == PostStatus.Publish);
            }
            else
            {
                p.IsPublished = false;
            
                if(p.PostStatus != PostStatus.Draft && p.PostStatus != PostStatus.PendingApproval)
                {
                    p.PostStatus = PostStatus.Draft;
                }
            }

            p.ModifiedBy = user.Name;

            if(p.IsNew) //No VERSION WORK, just save it.
            {
                p.Version = 1;
                p.Save(user.Name,SiteSettings.CurrentUserTime);
            }
            else if(p.IsPublished) //Make a copy of the current post, then save this one.
            {
                Post old_Post = new Post(p.Id);

                //if(old_Post.PostStatus == PostStatus.Publish)
                VersionPost(old_Post);

                p.Version = GetNextVersionId(p.Id, p.Version);
                p.Save(user.Name);
            }
            else
            {
                p.Version = GetNextVersionId(p.Id, p.Version);
                VersionPost(p);
                Post.UpdatePostStatus(p.Id,p.PostStatus);
            }

            ProcessFeaturedPosts(p, user, isFeaturedPost, isFeaturedCategory);

            if(p.PostStatus == PostStatus.PendingApproval)
                SendPReqiresApprovalMessage(p,user);
            else if(p.PostStatus == PostStatus.RequiresChanges)
                SendRequestedChangesMessage(p,user);

            return p.Id;
        }

        public static int GetNextVersionId(int postid, int currentVersion)
        {
            if (postid <= 0)
                return 1;
            else
            {
                QueryCommand command = new QueryCommand("Select Max(v.Version) FROM graffiti_VersionStore as v where v.Name = " + DataService.Provider.SqlVariable("Name"));
				command.Parameters.Add(VersionStore.FindParameter("Name")).Value = "Post:" + postid.ToString();
                object obj = DataService.ExecuteScalar(command);
                if (obj == null || obj is System.DBNull)
                    return 2;
                else
                    return Math.Max(((int)obj), currentVersion) + 1;
            }
        }

        private static void ProcessFeaturedPosts(Post p, IGraffitiUser user, bool isFeaturedPost, bool isFeaturedCategory)
        {
            SiteSettings settings = SiteSettings.Get();
            if (p.IsPublished && isFeaturedPost)
            {
                settings.FeaturedId = p.Id;
                settings.Save();
            }
            else if (settings.FeaturedId == p.Id)
            {
                settings.FeaturedId = -1;
                settings.Save();
            }

            Category c = p.Category;
            if (p.IsPublished && isFeaturedCategory)
            {
                c.FeaturedId = p.Id;
                c.Save(user.Name);
            }
            else if (c.FeaturedId == p.Id)
            {
                c.FeaturedId = -1;
                c.Save(user.Name);
            }
        }

        public static void VersionPost(Post old_Post)
        {
            VersionStore vs = new VersionStore();
            vs.Type = "post/xml";
            vs.ItemId = old_Post.Id;
            vs.Name = "Post:" + old_Post.Id;
            vs.UniqueId = Guid.NewGuid();
            vs.Data = old_Post.ToXML();
            vs.Version = old_Post.Version;
            vs.Save(GraffitiUsers.Current.Name);
        }
    }
}