<%@ WebHandler Language="C#" Class="download" %>

using System;
using System.IO;
using System.Web;
using Graffiti.Core;

public class download : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {

        if (!GraffitiUsers.IsAdmin(GraffitiUsers.Current))
            context.Response.End();
        
        string themeName = context.Request.QueryString["theme"];
        if (themeName != null)
        {
            DirectoryInfo di = new DirectoryInfo(context.Server.MapPath("~/files/themes/"));
            foreach (DirectoryInfo diChild in di.GetDirectories())
            {
                if(Util.AreEqualIgnoreCase(diChild.Name,themeName))
                {
                    string xml = ThemeConverter.ToXML(diChild.FullName);

                    context.Response.AppendHeader("content-disposition",
        "attachment; filename=" + themeName.Replace(" ", "_") + ".xml");
                    
                    context.Response.ContentType = "application/xml";
                    context.Response.Write(xml);
                    context.Response.End();
                }
            }
        }
            
            
            
            
            context.Response.Redirect("~/graffiti-admin/presentation/");
        
        
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}