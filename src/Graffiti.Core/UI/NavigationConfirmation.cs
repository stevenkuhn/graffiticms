using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Graffiti.Core
{
    public static class NavigationConfirmation
    {
        private const string script =
            @"  g_blnCheckUnload = true;
                window.onbeforeunload=RunOnBeforeUnload;
                function RunOnBeforeUnload()
                {
                    if (g_blnCheckUnload)
                    {
                        return 'You will lose any non-saved text';
                    }
                }
                function bypassCheck()
                {
                    g_blnCheckUnload  = false;
                }";

        public static void RegisterPage(Page page)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "nav-reg", script, true);
        }

        public static void RegisterControlForCancel(WebControl control)
        {
            control.Attributes.Add("onclick", "bypassCheck();");
        }
    }
}
