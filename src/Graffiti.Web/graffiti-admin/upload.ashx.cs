using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using Telligent.Glow;
using Graffiti.Core;

namespace Graffiti.Web
{
    public class upload : MultipleUploadFileHandler
    {
        public override void ProcessRequest(HttpContext context)
        {
            if (context.Request.QueryString["Username"] != null && context.Request.QueryString["Ticket"] != null)
            {
                IGraffitiUser user = GraffitiUsers.GetUser(context.Request.QueryString["Username"], true);
                if (user == null || user.UniqueId.ToString() != context.Request.QueryString["Ticket"] || user.UniqueId == Guid.Empty)
                    throw new InvalidOperationException("The upload form can only be used by users who are logged in");
            }
            else
            {
                IGraffitiUser user = GraffitiUsers.Current;
                if (user == null)
                    throw new InvalidOperationException("The upload form can only be used by users who are logged in");
            }            

            base.ProcessRequest(context);
        }
    }
}
