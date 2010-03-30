<%@ Page Language="C#" Inherits="Graffiti.Core.TemplatedThemePage" ClassName="GraffitiSearchPage" %>
<script runat="Server">
    
        protected override string ViewLookUp(string baseName, string defaultViewName)
        {
            if (ViewExists("search" + baseName))
                return "search" + baseName;
            else
                return base.ViewLookUp(baseName, defaultViewName);

        }

        protected override void LoadContent(GraffitiContext graffitiContext)
        {
            base.LoadContent(graffitiContext);

            IsIndexable = false;

            if (Request.QueryString["q"] == null)
            {
                Response.Redirect("~/");
                Response.End();
            }
            else
            {
                graffitiContext["where"] = "search";
                graffitiContext["title"] = "Search Results for '" + HttpUtility.HtmlEncode(Request.QueryString["q"]) + "'";

                graffitiContext.RegisterOnRequestDelegate("posts", GetSearchResults);
                
                // GetSearchResults needs to be called so the pager works
                GetSearchResults("posts", graffitiContext);
            }
        }
    
        protected object GetSearchResults(string key, GraffitiContext graffitiContext)
        {
            try
            {
                graffitiContext["where"] = "search";
               
                SearchIndex si = new SearchIndex();

                SearchQuery sq = new SearchQuery();
                sq.PageSize = SiteSettings.Get().PageSize;
                sq.PageIndex = PageIndex - 1;
                sq.QueryText = new Macros().SearchQuery;

                SearchResultSet<Post> posts = si.Search(sq);
                SearchResultSet<Post> filteredPosts = new SearchResultSet<Post>();

                foreach (Post p in posts)
                {
                    if (RolePermissionManager.GetPermissions(p.CategoryId, GraffitiUsers.Current).Read)
                        filteredPosts.Add(p);
                }

                graffitiContext.TotalRecords = posts.TotalRecords;
                graffitiContext.PageIndex = PageIndex;
                graffitiContext.PageSize = SiteSettings.Get().PageSize;

                return filteredPosts;
            }
            catch (Exception)
            {
                return new PostCollection();
            }
        }
</script>