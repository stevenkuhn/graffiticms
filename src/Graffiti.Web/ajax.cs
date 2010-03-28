using System;
using System.Web;
using Graffiti.Core;
using System.Text.RegularExpressions;

namespace Graffiti.Web
{

public class ajax : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {

        if (context.Request.RequestType != "POST")
            return;

        if (context.Items["UserId"] == null)
            return;


        context.Response.ContentType = "text/plain";


        switch (context.Request.QueryString["command"])
        {
            case "newComment":


                Comment comment = new Comment();

                comment.Name = context.Request.Form["author"];
                comment.WebSite = context.Request.Form["url"];
                comment.Email = context.Request.Form["email"];

                comment.Body = context.Request.Form["comment"];

                if (!context.Request.IsAuthenticated && String.IsNullOrEmpty(comment.Name))
                {
                    context.Response.Write("Please enter your name");
                    return;
                }

                if (String.IsNullOrEmpty(comment.Body))
                {
                    context.Response.Write("Please enter a comment");
                    return;
                }

                comment.IPAddress = context.Request.UserHostAddress;
                comment.PostId = Int32.Parse(context.Request.Form["comment_post_ID"]);

                comment.Published = DateTime.Now.AddHours(SiteSettings.Get().TimeZoneOffSet);

                comment.Save();
                context.Response.Write("Your comment has been received and will be published shortly. Thanks!");

                break;

            case "newContactMessage":

                string subject = context.Request.Form["subject"];
                string email = context.Request.Form["email"];
                string name = context.Request.Form["name"];
                string message = context.Request.Form["message"];

                if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(message))
                {
                    context.Response.Write("All of the fields are required, your message has not been sent");
                    context.Response.End();
                    return;
                }

                if (!Regex.IsMatch(email, @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b", RegexOptions.IgnoreCase))
                {
                    context.Response.Write("The email address you entered is not valid");
                    context.Response.End();
                    return;
                }

                EmailTemplateToolboxContext templateContext = new EmailTemplateToolboxContext();
                templateContext.Put("subject", context.Server.HtmlEncode(subject));
                templateContext.Put("email", context.Server.HtmlEncode(email));
                templateContext.Put("name", context.Server.HtmlEncode(name));
                templateContext.Put("message", Util.ConvertTextToHTML(message));
                templateContext.Put("ip", context.Request.UserHostAddress);

                EmailTemplate et = new EmailTemplate();
                et.Subject = "Contact Request: " + subject;
                et.Context = templateContext;
                et.From = email;
                et.TemplateName = "contact.view";

                Log.Info("Contact Received", "Subject: {0}\nFrom:{1} ({2})\nIP:{3}\n\n{4}", subject, name, email, context.Request.UserHostAddress, message);

                foreach (IGraffitiUser user in GraffitiUsers.GetUsers(GraffitiUsers.AdminRole))
                {
                    et.To = user.Email;
                    Emailer.Send(et);
                }

                context.Response.Write("Your message was received. Thanks!");

                break;
        }

    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}

// end namespace
}