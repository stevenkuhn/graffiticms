using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Graffiti.Core;

public partial class graffiti_admin_presentation_widgets_Edit : AdminControlPanelPage
{


    protected void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context,"presentation");

            Widget widget = Widgets.Fetch(new Guid(Request.QueryString["id"]));
            if (widget == null)
                throw new Exception("The widget does not exist");

        if(!Graffiti.Core.Util.AreEqualIgnoreCase("Edit.aspx", widget.EditUrl))
            Response.Redirect(widget.EditUrl + "?id=" + Request.QueryString["id"]);

        PageTitle.Text = widget.FormName;
        this.Page.Title = "Widget: " + widget.FormName;

            if (!IsPostBack)
            {
                FormRegion.Text = widget.BuildForm();
            }
            else
                FormRegion.Text = widget.BuildForm(Request.Form);
    }

    protected void EditWidget_Click(object sender, EventArgs e)
    {
        Widget widget = Widgets.Fetch(new Guid(Request.QueryString["id"]));
        StatusType st = widget.SetValues(Context, Request.Form);
        if(st == StatusType.Success)
        {
            Widgets.Save(widget);
            Response.Redirect(ResolveUrl("~/graffiti-admin/presentation/widgets/?ws=") + widget.Id);
        }
        else
        {
            Message.Text = Widget.GetMessage(Context) ?? "Your widget could not be updated";
            Message.Type = st;
        }
    }
}