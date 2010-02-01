<%@ WebHandler Language="C#" Class="Graffiti.Core.RSD" %>

using System.Text;
using System.Web;
using System.Xml;
using Graffiti.Core;

namespace Graffiti.Core
{
	public class RSD : IHttpHandler 
	{
	    
		public void ProcessRequest (HttpContext context) 
		{

			Macros macros = new Macros();
	        
			context.Response.ContentType = "text/xml";
			XmlTextWriter xml = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);
            xml.WriteStartDocument();
			xml.WriteStartElement("rsd");
			xml.WriteAttributeString("version", "1.0");
	        
			//service
			xml.WriteStartElement("service");
            xml.WriteElementString("engineName", SiteSettings.Version);
            xml.WriteElementString("engineLink", "http://graffiticms.com");
			xml.WriteElementString("homePageLink", macros.FullUrl(new Urls().Home));

			xml.WriteStartElement("apis");
			xml.WriteStartElement("api");
				xml.WriteAttributeString("name", "MetaWeblog");
				xml.WriteAttributeString("blogID", "root_blog");
				xml.WriteAttributeString("preferred", "true");
                xml.WriteAttributeString("apiLink", macros.FullUrl(VirtualPathUtility.ToAbsolute("~/api/metablog.ashx")));
			xml.WriteEndElement();
	    
			xml.WriteEndElement();
	        
			//End service
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