<%@ Page Language="C#" Inherits="Graffiti.Core.GraffitiPage" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Xml" %>
<script runat="server">
	protected const string PluginInfoFormatString = "Enabled: {0}<br />Description: {1}<br />Type: {2}<br />";


	protected override void OnLoad(System.EventArgs e)
	{
		SiteSettings settings = SiteSettings.Get();
		CommentSettings commentSettings = CommentSettings.Get();

		StringWriter sw = new StringWriter();
		sw.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
		XmlTextWriter writer = new XmlTextWriter(sw);

		writer.WriteStartElement("html");
		writer.WriteAttributeString("xmlns", "http://www.w3.org/1999/xhtml");

		writer.WriteStartElement("head");
		writer.WriteElementString("title", "Graffiti CMS - System Information");
		writer.WriteEndElement();

		writer.WriteStartElement("body");
		writer.WriteElementString("h1", "Graffiti CMS");

		writer.WriteElementString("h2", "System Information");
		writer.WriteStartElement("dl");
		GenerateDLItem(writer, "Version", SiteSettings.VersionDescription);
		GenerateDLItem(writer, "Url Routing Supported", SiteSettings.UrlRoutingSupported.ToString());
		GenerateDLItem(writer, "Generate Folders", settings.GenerateFolders.ToString());
		GenerateDLItem(writer, "Cache Views", settings.CacheViews.ToString());
		GenerateDLItem(writer, "Require SSL", settings.RequireSSL.ToString());
		GenerateDLItem(writer, "Require WWW", settings.RequireWWW.ToString());
		GenerateDLItem(writer, "Use Proxy Server", settings.UseProxyServer.ToString());
		GenerateDLItem(writer, "Email Server", settings.EmailServer.ToString());
		GenerateDLItem(writer, "Current User Time", SiteSettings.CurrentUserTime.ToString());
		GenerateDLItem(writer, "TimeZone Offset", settings.TimeZoneOffSet.ToString());
		GenerateDLItem(writer, "Theme", settings.Theme.ToString());
		GenerateDLItem(writer, "Site Title", settings.Title.ToString());
		GenerateDLItem(writer, "Default Page Size", settings.PageSize.ToString());
		GenerateDLItem(writer, "Enable Comments", commentSettings.EnableCommentsDefault.ToString());
		writer.WriteEndElement();

		writer.WriteElementString("h2", "Plugins");
		writer.WriteStartElement("dl");
		writer.WriteEndElement();
		List<EventDetails> plugins = Graffiti.Core.Events.GetEvents();
		foreach (EventDetails plugin in plugins)
		{
			GenerateDLItem(writer, plugin.Event.Name, string.Format(PluginInfoFormatString, plugin.Enabled.ToString(), plugin.Event.Description, plugin.EventType));
		}
		writer.WriteEndElement();

		writer.WriteEndElement();
		writer.WriteEndElement();

		Context.Response.ContentEncoding = System.Text.Encoding.UTF8;
		Context.Response.ContentType = "application/rss+xml";
		Context.Response.Write(sw.ToString());
	}

	protected void GenerateDLItem(XmlTextWriter writer, string key, string value)
	{
		writer.WriteElementString("dt", key);
		writer.WriteElementString("dd", value);
	}
</script>