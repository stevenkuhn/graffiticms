using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using Graffiti.Core;
using Glow = Telligent.Glow;

public partial class graffiti_admin_presentation_widgets_Default : AdminControlPanelPage
{
    protected static string TrashImage
    {
        get { return VirtualPathUtility.ToAbsolute("~/graffiti-admin/common/img/trash.gif"); }
    }

    protected static string EditImage
    {
        get { return VirtualPathUtility.ToAbsolute("~/graffiti-admin/common/img/config.gif"); }
    }

    protected static string GetSideBarLayout()
    {
        return "both";
    }

    protected static string GetDefaultSideBar
    {
        get
        {
            return (ConfigurationManager.AppSettings["Graffiti:Widgets:DefaultSidebar"] ?? "right").ToLower();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context,"presentation");
        if(!IsPostBack)
        {
            AvailableWidgets.Items.Clear();
            AvailableWidgets.Items.Add(new Glow.DropDownListItem("(Configure Widgets)", ""));
            foreach (WidgetDescription w in Widgets.GetAvailableWidgets())
            {
                AvailableWidgets.Items.Add(new Glow.DropDownListItem(w.Name, w.Name, w.UniqueId));
            }

            string widgetFormat = "<div style=\"border: solid 1px #999; padding: 4px;\"><b>{0}</b><div style=\"text-align:right;\"><a title=\"Edit Widget\" href=\"javascript:void();\" onclick=\"window.location=\'Edit.aspx?id={1}\'\">Edit</a> | <a title=\"Delete Widget\" href=\"javascript:void();\" onclick=\"javascript:deleteWidget(&#39;{1}&#39;); return false;\">Delete</a></div></div>";

            foreach (Widget w in Widgets.FetchByLocation(WidgetLocation.Queue))
            {
                qbar.Items.Add(new Glow.OrderedListItem(string.Format(widgetFormat, w.Name, w.Id), w.Name, w.Id.ToString()));
            }

            foreach (Widget w in Widgets.FetchByLocation(WidgetLocation.Left))
            {
                lbar.Items.Add(new Glow.OrderedListItem(string.Format(widgetFormat, w.Name, w.Id), w.Name, w.Id.ToString()));
            }

            foreach (Widget w in Widgets.FetchByLocation(WidgetLocation.Right))
            {
                rbar.Items.Add(new Glow.OrderedListItem(string.Format(widgetFormat, w.Name, w.Id), w.Name, w.Id.ToString()));
            }

            switch(GetSideBarLayout())
            {
                case "left":

                    ClientScript.RegisterStartupScript(GetType(), "hidebar", "$('right-sidebar').hide();", true);
                    break;

                case "right":

                    ClientScript.RegisterStartupScript(GetType(), "hidebar", "$('left-sidebar').hide();", true);
                    break;
            }

            string widgetSaved = Request.QueryString["ws"];

            if (!String.IsNullOrEmpty(widgetSaved))
            {
                Widget widget = Widgets.Fetch(new Guid(Request.QueryString["ws"]));
                
                if (widget == null)
                    throw new Exception("The widget does not exist");

                Message.Text = "The widget, <b>" + widget.Name + "</b> was updated.";
                Message.Type = StatusType.Success;
            }

        }
    }
}
