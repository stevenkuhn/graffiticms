<%@ Page Language="C#" %>

<% SiteSettings settings = SiteSettings.Get(); %>

<html>
	<head>
		<title>Graffiti CMS - System Information</title>
		<style type="text/css">
			html, body { background-color: #ffc;}
			h2 { border-bottom: solid 2px #666; }
			table td { vertical-align: top; padding-bottom: 0.25em;}
			table { padding-bottom: 1em;}
			.name { font-weight: bold; padding-right: 2em;}
		</style>
	</head>
	<body>
		<h1>Graffiti CMS</h1>
		<h2>System Information</h2>
		<table>
			<tr>
				<td class="name">Version</td>
				<td><%= SiteSettings.VersionDescription %></td>
			</tr>
			<tr>
				<td class="name">Url Routing Supported</td>
				<td><%= SiteSettings.UrlRoutingSupported %></td>
			</tr>
			<tr>
				<td class="name">Generate Folders</td>
				<td><%= settings.GenerateFolders %></td>
			</tr>
			<tr>
				<td class="name">Cache Views</td>
				<td><%= settings.CacheViews %></td>
			</tr>
			<tr>
				<td class="name">Require SSL</td>
				<td><%= settings.RequireSSL %></td>
			</tr>
			<tr>
				<td class="name">Require WWW</td>
				<td><%= settings.RequireWWW %></td>
			</tr>
			<tr>
				<td class="name">Use Proxy Server</td>
				<td><%= settings.UseProxyServer %></td>
			</tr>
			<tr>
				<td class="name">Email Server</td>
				<td><%= settings.EmailServer %></td>
			</tr>
			<tr>
				<td class="name">Current User Time</td>
				<td><%= SiteSettings.CurrentUserTime.ToString("dddd dd MMMM yyyy hh:mm:ss") %></td>
			</tr>
			<tr>
				<td class="name">TimeZone Offset</td>
				<td><%= settings.TimeZoneOffSet %></td>
			</tr>
			<tr>
				<td class="name">Theme</td>
				<td><%= settings.Theme %></td>
			</tr>
			<tr>
				<td class="name">Site Title</td>
				<td><%= settings.Title %></td>
			</tr>
			<tr>
				<td class="name">Default Page Size</td>
				<td><%= settings.PageSize %></td>
			</tr>
			<tr>
				<td class="name">Enable Comments</td>
				<td><%= CommentSettings.Get().EnableCommentsDefault %></td>
			</tr>
		</table>
		<h2>Plugins</h2>
		<dl>
		<% foreach(EventDetails plugin in Graffiti.Core.Events.GetEvents())
		   { %>
			<dt class="name"><%= plugin.Event.Name %></dt>
			<dd>
				<table>
					<tr>
						<td class="name">Enabled</td>
						<td><%= plugin.Enabled %></td>
					</tr>
					<tr>
						<td class="name">Description </td>
						<td><%= plugin.Event.Description%></td>
					</tr>
					<tr>
						<td class="name">Plugin Type</td>
						<td><%= plugin.EventType %></td>
					</tr>
				</table>
			</dd>
		 <%} %>
		 </dl>
		 
		<h2>Recent Events</h2>
		<% 
			DataBuddy.Query q = Log.CreateQuery();
			q.PageSize = 10;
			q.PageIndex = 1;	
			q.OrderByDesc(Log.Columns.CreatedOn);

			foreach (Log log in LogCollection.FetchByQuery(q))
			{%>
				<dt class="name"><%= log.Title %></dt>
				<dd><%= log.Type == 3 ? "Error" : log.Type== 2 ? "Warning"  : "Information" %>
				<dd><%= log.CreatedOn.ToString("dddd dd MMMM yyyy hh:mm:ss")%></dd>
				<dd><%= log.Message %></dd>
			<%} %>
	</body>
</html>