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
using Telligent.Glow.Editor;
using Graffiti.Core;

public partial class graffiti_admin_presentation_plugins_Edit : AdminControlPanelPage
{


    protected void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context,"settings");

        EventDetails ed = Graffiti.Core.Events.GetEvent(Request.QueryString["t"]);


        PageTitle.Text = ed.Event.Name;
        this.Page.Title = "Plug-in: " + ed.Event.Name;

            if (!IsPostBack)
            {
                FormRegion.Text = ed.Event.BuildForm();
            }
            else
                FormRegion.Text = ed.Event.BuildForm(Request.Form);
    }

    protected void EditWidget_Click(object sender, EventArgs e)
    {
        EventDetails ed = Graffiti.Core.Events.GetEvent(Request.QueryString["t"]);
    
        StatusType st = ed.Event.SetValues(Context, Request.Form);
        if(st == StatusType.Success)
        {
            Graffiti.Core.Events.Save(ed);
            Response.Redirect(ResolveUrl("~/graffiti-admin/site-options/plug-ins/?ps=") + Server.UrlEncode(ed.Event.Name));
        }
        else
        {
            Message.Text = GraffitiEvent.GetMessage(Context) ?? "Your plugin could not be updated";
            Message.Type = st;
        }
    }
}