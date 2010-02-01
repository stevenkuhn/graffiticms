<%@ Page Language="C#" AutoEventWireup="true"  Inherits="Graffiti.Core.TemplatedThemePage" ClassName="GraffitiHomePage" %>
<%@ Import namespace="DataBuddy"%>
<script runat="Server">

    protected override string ViewLookUp(string baseName, string defaultViewName)
    {
        if (ViewExists("home" + baseName))
            return "home" + baseName;
        else
            return base.ViewLookUp(baseName, defaultViewName);

    }

    public override bool IsIndexable
    {
        get
        {
            return PageIndex <= 1;
        }
        set
        {
            base.IsIndexable = value;
        }
    }

    protected override void LoadContent(GraffitiContext graffitiContext)
    {

        graffitiContext["where"] = "home";
        graffitiContext["title"] = SiteSettings.Get().Title;

        if (SiteSettings.Get().UseCustomHomeList)
        {
            graffitiContext.RegisterOnRequestDelegate("posts", GetCustomHomeSortPosts);
            
            // GetCustomHomeSortPosts needs to be called so the pager works
            GetCustomHomeSortPosts("posts", graffitiContext);
        }
        else
        {
            graffitiContext.RegisterOnRequestDelegate("posts", GetDefaultPosts);
            
            // GetDefaultPosts needs to be called so the pager works
            GetDefaultPosts("posts", graffitiContext);
        }
    }
    
    protected object GetCustomHomeSortPosts(string key, GraffitiContext graffitiContext)
    {
        PostCollection pc = ZCache.Get<PostCollection>(CacheKey + ":customhome:" + PageIndex);
        if (pc == null)
        {
            pc = new PostCollection();

            Query q = PostCollection.HomeQueryOverride(PageIndex, SiteSettings.Get().PageSize);
            
            pc.LoadAndCloseReader(q.ExecuteReader());
            ZCache.InsertCache(CacheKey + ":customhome:" + PageIndex, pc, 30);

        }

        object postCount = ZCache.Get<object>(CacheKey + ":customhome:" +"Count");
        if (postCount == null)
        {
            postCount = PostCollection.HomeQueryOverride(-1, -1).GetRecordCount();
            ZCache.InsertCache(CacheKey + ":customhome:" + "Count", postCount, 60);
        }

        graffitiContext.TotalRecords = (int)postCount;
        graffitiContext.PageIndex = PageIndex;
        graffitiContext.PageSize = SiteSettings.Get().PageSize;

        PostCollection permissionsFiltered = new PostCollection();
        permissionsFiltered.AddRange(pc);

        foreach (Post p in pc)
        {
            if (!RolePermissionManager.GetPermissions(p.CategoryId, GraffitiUsers.Current).Read)
                permissionsFiltered.Remove(p);
        }
        
        return permissionsFiltered;
    }

    protected object GetDefaultPosts(string key, GraffitiContext graffitiContext)
    {
        PostCollection pc = ZCache.Get<PostCollection>(CacheKey + PageIndex);
        if (pc == null)
        {
            pc = new PostCollection();
            Query q = PostCollection.DefaultQuery();
            q.PageSize = SiteSettings.Get().PageSize;
            q.PageIndex = PageIndex;
            pc.LoadAndCloseReader(q.ExecuteReader());
            ZCache.InsertCache(CacheKey + PageIndex, pc, 30);

        }

        object postCount = ZCache.Get<object>(CacheKey + "Count");
        if (postCount == null)
        {
            postCount = PostCollection.DefaultQuery().GetRecordCount();
            ZCache.InsertCache(CacheKey + "Count", postCount, 60);
        }

        graffitiContext.TotalRecords = (int)postCount;
        graffitiContext.PageIndex = PageIndex;
        graffitiContext.PageSize = SiteSettings.Get().PageSize;


        PostCollection permissionsFiltered = new PostCollection();
        permissionsFiltered.AddRange(pc);

        foreach (Post p in pc)
        {
            if (!RolePermissionManager.GetPermissions(p.CategoryId, GraffitiUsers.Current).Read)
                permissionsFiltered.Remove(p);
        }

        return permissionsFiltered;
    }

    private const string CacheKey = "Posts-Index-";
    
</script>
