using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Graffiti.Core
{
    /// <summary>
    /// Summary description for Tag
    /// </summary>
    public partial  class Tag
    {
        protected override void AfterCommit()
        {
            base.AfterCommit();
            
            WritePage(Name);
        }

        public static void WritePage(string name)
        {
            PageTemplateToolboxContext templateContext = new PageTemplateToolboxContext();
            templateContext.Put("tag", name);
            templateContext.Put("MetaDescription", "Posts and articles tagged as " + name);
            templateContext.Put("MetaKeywords", name);


			PageWriter.Write("tag.view", "~/tags/" + name + "/" + Util.DEFAULT_PAGE, templateContext);
            PageWriter.Write("tagrss.view", "~/tags/" + name + "/feed/" + Util.DEFAULT_PAGE, templateContext);
            
        }
    }
}