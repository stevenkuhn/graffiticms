<%@ WebHandler Language="C#" Class="Graffiti.Core.WLWManifest" %>

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;
using Graffiti.Core;

namespace Graffiti.Core
{
	public class WLWManifest : IHttpHandler 
	{
    
		public void ProcessRequest (HttpContext context) 
		{

            Macros macros = new Macros();
	        
			context.Response.ContentType = "text/xml";
			XmlTextWriter xml = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);

			xml.WriteStartElement("manifest");
			xml.WriteAttributeString("xmlns", "http://schemas.microsoft.com/wlw/manifest/weblog");
	        
			//service
			xml.WriteStartElement("weblog");

			xml.WriteElementString("imageUrl", "favicon.gif");
			xml.WriteElementString("homepageLinkText", "View your weblog");
			xml.WriteElementString("adminLinkText", "Edit your weblog");
            xml.WriteElementString("adminUrl", macros.FullUrl(VirtualPathUtility.ToAbsolute("~/graffiti-admin/")));
            xml.WriteElementString("postEditingUrl", macros.FullUrl(VirtualPathUtility.ToAbsolute("~/graffiti-admin/posts/write/")));
	        
			xml.WriteEndElement();
	        
			xml.WriteStartElement("options");
	       
			xml.WriteElementString("supportsPostAsDraft", "Yes");
			xml.WriteElementString("supportsFileUpload", "Yes");
			xml.WriteElementString("supportsCustomDate", "Yes");
			xml.WriteElementString("supportsCategories", "Yes");
			xml.WriteElementString("supportsCategoriesInline", "Yes");
            xml.WriteElementString("supportsNewCategoriesInline", "Yes");
            xml.WriteElementString("supportsGetTags", "Yes");
            xml.WriteElementString("supportsKeywords", "Yes");
			xml.WriteElementString("supportsMultipleCategories", "No");
			xml.WriteElementString("supportsNewCategories", "Yes");
			xml.WriteElementString("supportsEmbeds", "Yes");
			xml.WriteElementString("supportsAutoUpdate", "Yes");
			xml.WriteElementString("supportsSlug", "Yes");
			xml.WriteElementString("supportsExcerpt", "No");
			xml.WriteElementString("supportsExtendedEntries", "Yes");
			xml.WriteElementString("supportsEmptyTitles", "No");
	       
			//End options
			xml.WriteEndElement();

			xml.Close();
        
	    }
 
		public bool IsReusable {
			get {
				return false;
			}
		}
	}
}