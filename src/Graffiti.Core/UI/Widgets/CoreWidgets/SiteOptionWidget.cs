using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Graffiti.Core
{
    [WidgetInfo("007c05fa-a3c0-43a0-9f7a-ba505e99aa11", "Admin Options", "Options which are only displayed to site admins")]
    public class SiteOptionWidget : Widget
    {
        public override string RenderData()
        {
            StringBuilder sb = new StringBuilder("<ul>");

            Urls urls = new Urls();

            HttpContext context = HttpContext.Current;
            if(context != null)
            {
                TemplatedThemePage ttp = context.Handler as TemplatedThemePage;
                if(ttp != null && ttp.PostId > 0)
                {
                    Post p = new Post(ttp.PostId);
                    if (RolePermissionManager.GetPermissions(p.CategoryId, GraffitiUsers.Current).Edit)
                    {
                        sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>\n", urls.Edit(ttp.PostId), "Edit this Post");
                    }
                }
            }

            if (RolePermissionManager.CanViewControlPanel(GraffitiUsers.Current))
            {
                sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>\n", urls.Write, "Write a new Post");
                sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>\n", urls.Admin, "Control Panel");
            }
            
            sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>\n", urls.Logout, "Logout");
            sb.Append("</ul>\n");

            return sb.ToString();
        }

        public override string Title
        {
            get
            {
                if (string.IsNullOrEmpty(base.Title))
                    base.Title = "Site Options";
                
                return base.Title;
            }
            set
            {
                base.Title = value;
            }
        }

        public override bool IsUserValid()
        {
            return GraffitiUsers.Current != null;
        }

        public override string Name
        {
            get
            {
                return "Admin Options";
            }
        }

        protected override FormElementCollection AddFormElements()
        {
            FormElementCollection fec = new FormElementCollection();
            fec.Add(AddTitleElement());
            return fec;
        }
        
    }
}