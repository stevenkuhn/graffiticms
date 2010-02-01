using System.Collections;
using System.Text;
using DataBuddy;

namespace Graffiti.Core
{
    /// <summary>
    /// A widget which displays a title and the a block of text
    /// </summary>
    [WidgetInfo("bf6171d0-e9d5-46ba-bce3-be104a5e6fbd", "Uncategorized Posts Widget", "Represents a box")]
    public class PageWidget : Widget
    {

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                    return "Uncategorized Posts";
                else
                    return Title + " (Pages)";
            }
        }

        public override string EditUrl
        {
            get
            {
                return "EditUncategorizedPostWidget.aspx";
            }
        }

        protected override FormElementCollection AddFormElements()
        {
            return null;
        }

        public override string RenderData()
        {
            if (PostIds == null || PostIds.Length == 0)
                return string.Empty;

            PostCollection pc = ZCache.Get<PostCollection>(DataCacheKey);
            if (pc == null)
            {
               
                pc = new PostCollection();
                foreach(int i in PostIds)
                {
                    Post p = new Post(i);
                    if(!p.IsNew && !p.IsDeleted && p.IsPublished )
                        pc.Add(p);
                }

                ZCache.InsertCache(DataCacheKey, pc, 180);
            }

            StringBuilder sb = new StringBuilder("<ul>");
            foreach(Post p in pc)
            {
                sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", p.Url, p.Title);
            }

            sb.Append("</ul>");

            return sb.ToString();
        }

        public int[] PostIds = new int[0];

        public string DataCacheKey
        {
            get { return "Posts-PageWidget-" + Id; }
        }
    }
}