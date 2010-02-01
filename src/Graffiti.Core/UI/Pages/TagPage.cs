namespace Graffiti.Core
{
    /// <summary>
    /// Summary description for TagPage
    /// </summary>
    public class TagPage : TemplatedThemePage
    {

        protected override string ViewLookUp(string baseName, string defaultViewName)
        {
            if (ViewExists("tag" + baseName))
                return "tag" + baseName;
            else
                return base.ViewLookUp(baseName, defaultViewName);

        }

        protected override void LoadContent(GraffitiContext graffitiContext)
        {
            IsIndexable = false;

            graffitiContext["where"] = "tag";
            graffitiContext["title"] = TagName + " : " + SiteSettings.Get().Title;
            graffitiContext["tag"] = TagName;


            graffitiContext.RegisterOnRequestDelegate("posts",GetPostsByTag);

            // GetPostsByTag needs to be called so the pager works
            GetPostsByTag("posts", graffitiContext);
        }

        protected object GetPostsByTag(string key, GraffitiContext graffitiContext)
        {
            PostCollection pc = ZCache.Get<PostCollection>("Tags-" + TagName);
            if (pc == null)
            {
                pc = Post.FetchPostsByTag(TagName);
                ZCache.InsertCache("Tags-" + TagName, pc, 60);
            }

            return pc;
        }
    }
}