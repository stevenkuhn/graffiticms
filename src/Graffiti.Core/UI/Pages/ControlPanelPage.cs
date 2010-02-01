using System;
using System.Web.UI;

namespace Graffiti.Core
{
    public class GraffitiPage : Page
    {
        protected virtual void Authenticate()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            Authenticate();
            base.OnInit(e);
        }
    }

    public class ControlPanelPage : GraffitiPage
    {

        protected override void Authenticate()
        {
            if (GraffitiUsers.Current == null)
                Response.Redirect("~/login/");

            if (!RolePermissionManager.CanViewControlPanel(GraffitiUsers.Current) && !GraffitiUsers.IsAdmin(GraffitiUsers.Current))
                Response.Redirect("~/");
        }

    }

    public class ManagerControlPanelPage : ControlPanelPage
    {
        protected override void Authenticate()
        {
            if (GraffitiUsers.Current == null)
                Response.Redirect("~/login/");

            if (!RolePermissionManager.CanViewControlPanel(GraffitiUsers.Current) && !GraffitiUsers.IsAdmin(GraffitiUsers.Current))
                Response.Redirect("~/");   
        }
    }

    public class AdminControlPanelPage : ControlPanelPage
    {
        protected override void Authenticate()
        {
            if (GraffitiUsers.Current == null)
                Response.Redirect("~/login/");

            if (!GraffitiUsers.IsAdmin(GraffitiUsers.Current))
                Response.Redirect("~/");
        }
    }
}
