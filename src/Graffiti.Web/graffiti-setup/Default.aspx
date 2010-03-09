<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="Server">
	void Page_Load(object sender, EventArgs e)
	{
		SiteSettings settings = SiteSettings.Get();

		if (settings.InitialSetupCompleted)
			Context.Response.Redirect(new Urls().Home);
				
		// As a security measure, do not allow use of the setup form if an admin user already exists
		List<IGraffitiUser> adminUsers = GraffitiUsers.GetUsers(GraffitiUsers.AdminRole);
		if (adminUsers != null && adminUsers.Count > 0)
		{
			settings.InitialSetupCompleted = true;
			settings.Save();
			Context.Response.Redirect(new Urls().Home);
		}

		StatusMessagePanel.Visible = false;

		DateTime dt = DateTime.Now;
		// there are timezones which differ by 15 minutes
		for (double i = -24; i < 24; i = i + .25)
		{
			TimeOffSet.Items.Add(new ListItem(dt.AddHours(i).ToString("ddd, dd MMMM yyyy HH:mm"), i.ToString()));
		}

		ListItem liSelected = TimeOffSet.Items.FindByValue(settings.TimeZoneOffSet.ToString());
		if (liSelected != null)
			liSelected.Selected = true;
		else
			TimeOffSet.Items.FindByValue("0").Selected = true;

	}

	protected void SetupSave_Click(object sender, EventArgs e)
	{
		try
		{
			string adminUserName = txtUserName.Text.Trim();
			string adminPassword = txtPassword.Text.Trim();
			string adminEmail = txtEmail.Text.Trim();
			string adminProperName = txtProperName.Text.Trim();

			if (string.IsNullOrEmpty(adminUserName) || string.IsNullOrEmpty(adminPassword)
				|| string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminProperName))
			{
				StatusMessage.Text = "Please enter all of the information for the admin user account";
				StatusMessagePanel.Visible = true;
				return;
			}
			
			IGraffitiUser user = GraffitiUsers.CreateUser(Server.HtmlEncode(adminUserName), adminPassword, adminEmail, GraffitiUsers.EveryoneRole);
			user.ProperName = Server.HtmlEncode(adminProperName);
			GraffitiUsers.Save(user, user.Name);
			GraffitiUsers.AddUserToRole(user.Name, GraffitiUsers.AdminRole);

			SiteSettings settings = SiteSettings.Get();
			settings.Title = Server.HtmlEncode(txtTitle.Text.Trim());
			settings.TimeZoneOffSet = double.Parse(TimeOffSet.SelectedValue);
			settings.Save();
			
			Context.Response.Redirect(new Urls().Admin);
		}
		catch (Exception ex)
		{
			StatusMessage.Text = "Your settings could not be updated. Reason: " + ex.Message;
			StatusMessagePanel.Visible = true;
		}
	}

</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title>Graffiti CMS</title>
	<style>
		body {
			margin: 0;
			padding: 0;
			font-family: Verdana, sans-serif;
			background: #bbb;
		}
		a:link, a:visited {
			color: #2070ae;
		}
		p {
			margin: 4px 0 10px 0;
			font-size: 14px;
		}
		#SetupForm {
			font-size: 14px;
			margin: 0px;
			padding: 0px;
			color: #333;
			line-height: 1.5em;
			background: #bbb;
			height: 1%;
			overflow: hidden;
		}
		#SetupForm .Content  {
			background: #fff;
			border: solid 1px #E76E19;
			min-height: 375px;
			padding: 15px;   
			margin: 20px;
		}
		#SetupForm h1 {
			padding-left: 10px;
			font-size: 125%;
			font-weight: bold;
			color: #fff;
			background: #E76E19;
			line-height: 28px;
			margin: 0px;
			border-bottom: solid 1px #E76E19;
			margin: -15px -15px 0 -15px;
		}
		#SetupForm h2 {
			font-size: 16px;
			margin: 12px 0 0px 0;
			font-weight: bold;
		}
		#SetupForm h2 span {
			font-size: 12px;
			font-weight: normal;
			color: #666;
			vertical-align: 1px;
		}
		#SetupForm input,#SetupForm select {
			width: 450px;
			font-size: 14px;
			margin: 4px 0 0 0;
		}
		#SetupForm input[type=text]:focus {
			background-color: #ffc;
			color: #000;
		}
		#UserForm  {
			margin: 0 0 10px 15px;
		}
		#UserForm span {
			font-size: 14px;
			margin: 10px 5px 0 0;
			font-weight: normal;
		}
		#UserForm input {
			width: 250px;
			margin: 10px 5px 0 0;
			font-size: 14px;
		}
		.submit input {
			font-size: 150%;
			font-weight: bold;
			padding: 6px;
		}
		.statusmessage {
			font-family: Tahoma, Arial, Helvetica;
			font-size: 110%;
			padding: 8px 8px;
			font-weight: normal;
			letter-spacing: .5px;
			margin: 10px 0 10px 0;
			color: #333;
			border: solid 1px #E25D3D;
			background-color: #e58b76;
			display: none;
		}
	</style>
</head>
<body>
	<form id="form1" runat="server">
	<div id="SetupForm">
	<div class="Content">
	<div>
		<h1>Graffiti CMS Setup</h1>
		<p>Welcome to Graffiti - Content Made Simple! Lets fill in a few key pieces of info and then you can get started.</p>
		<asp:Panel ID="StatusMessagePanel" runat="server" CssClass="statusmessage"><asp:label ID="StatusMessage" runat="server" /></asp:Panel>
	</div>
	<div id="post_form_container" class="FormBlock">
		<h2>
			What is the title of this site?
			<asp:RequiredFieldValidator ControlToValidate="txtTitle" Text="Please enter a title" runat="Server" />
		</h2>
		<asp:TextBox runat="Server" ID="txtTitle" Text="My Graffiti CMS Site" TabIndex="1" />
		
		<h2>
			TimeZone OffSet: <span class="form_label">(What time is it for you locally?)</span>
		</h2>
		<asp:DropDownList runat="Server" ID = "TimeOffSet" Style="width:auto;" TabIndex="2" />

		<h2>Create a site administrator</h2>
		<p>Use the fields below to create the first site administrator account. You can then login and create any other accounts you need in the Graffiti admin panel.</p>
		
		<div id="UserForm">
			<div>
				<span>Username:</span>
				<asp:TextBox runat="Server" ID="txtUserName" TabIndex="3" Text="admin" MaxLength="256" />
				<asp:RequiredFieldValidator ControlToValidate="txtUserName" Text="Please enter a User Name" CssClass="UserValidator" runat="Server" />
			</div>

			<div>
				<span>Proper Name:</span>
				<asp:TextBox runat="Server" id="txtProperName" TabIndex="4" Text="The Admin" MaxLength="256" />  
				<asp:RequiredFieldValidator ControlToValidate="txtProperName" Text="Please enter a Proper Name" CssClass="UserValidator" runat="Server" />
			</div>

			<div>
				<span>Email:</span>
				<asp:TextBox runat="Server" ID="txtEmail" TabIndex="5" MaxLength="256" />
				<asp:RequiredFieldValidator Display="Dynamic" ControlToValidate="txtEmail" Text="Please enter an Email Address" CssClass="UserValidator" runat="Server" />
				<asp:RegularExpressionValidator Display="Dynamic" ValidationExpression="^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$" runat="Server" ControlToValidate="txtEmail" Text="This email address is invalid" />
			</div>
		
			<div>
				<span>Password:</span>
				<asp:TextBox runat="Server" ID="txtPassword" TextMode="Password" TabIndex="6" MaxLength="256" />
				<asp:RequiredFieldValidator ControlToValidate="txtPassword" Text="Please enter a Password" CssClass="UserValidator" runat="Server" />
			</div>
		</div>
		
		<div class="submit">
			<asp:Button runat="Server" ID="SetupSave" Text="Save Settings" OnClick="SetupSave_Click" TabIndex="9" />
		</div> 
	</div>
	</div>
	</div>
	</form>
</body>
</html>
