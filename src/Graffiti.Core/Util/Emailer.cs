using System.Net.Mail;
using System.Web;

namespace Graffiti.Core
{
    public static class Emailer
    {
        

        public static bool Send(EmailTemplate template)
        {
            Events.Instance().ExecuteBeforeEmailSent(template);

            string fileText = Util.GetFileText(HttpContext.Current.Server.MapPath("~/__utility/emails/" + template.TemplateName));
            fileText = TemplateEngine.Evaluate(fileText, template.Context);

            using (MailMessage mm = new MailMessage(template.From ?? SiteSettings.Get().EmailFrom, template.To))
            {
                mm.Subject = template.Subject;
                mm.IsBodyHtml = template.IsHTML;
                mm.Body = fileText;

                SendMailMessage(mm);
            }

            Events.Instance().ExecuteAfterEmailSent(template);

            return true;

        }

        public static bool SendMailMessage(MailMessage mm)
        {
            using (mm)
            {
                SiteSettings settings = SiteSettings.Get();

                SmtpClient client = new SmtpClient();
                client.Host = settings.EmailServer;

                if (settings.EmailServerRequiresAuthentication)
                    client.Credentials = new System.Net.NetworkCredential(settings.EmailUser, settings.EmailPassword);

                if(settings.EmailRequiresSSL)
                    client.EnableSsl = true;

                if (settings.EmailPort > 0)
                    client.Port = settings.EmailPort;

                client.Send(mm);

                
            }
            return true;
        }

        public static bool Send(string templateFile, string emailTo, string subject, PageTemplateToolboxContext cntxt)
        {
            string fileText =
                Util.GetFileText(HttpContext.Current.Server.MapPath("~/__utility/emails/" + templateFile));

            fileText = TemplateEngine.Evaluate(fileText, cntxt);

            SiteSettings settings = SiteSettings.Get();

            using (MailMessage mm = new MailMessage(settings.EmailFrom, emailTo))
            {
                mm.Subject = subject;
                mm.IsBodyHtml = true;
                mm.Body = fileText;

                SmtpClient client = new SmtpClient();
                client.Host = settings.EmailServer;

                if(settings.EmailServerRequiresAuthentication)
                    client.Credentials = new System.Net.NetworkCredential(settings.EmailUser, settings.EmailPassword);

                if (settings.EmailRequiresSSL)
                    client.EnableSsl = true;

                if (settings.EmailPort > 0)
                    client.Port = settings.EmailPort;

                client.Send(mm);
            }

            return true;
        }
    }
}