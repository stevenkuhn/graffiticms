<%@ Import namespace="DataBuddy"%>
<%@ Import namespace="System.Threading"%>
<%@ Import namespace="Graffiti.Core"%>
<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup

        applicationPath = HttpRuntime.AppDomainAppPath;
        _timer = new System.Threading.Timer(PeriodicTasks, null, 30000, 150000);
		
    	Application["Telligent_Glow_Editor:UserFilesPath"] = (HttpRuntime.AppDomainAppVirtualPath + "/files/media/").Replace("//", "/");
        
        // make sure the everyone roles etc are created
        RolePermissionManager.GetRolePermissions();

        if(!SiteSettings.Get().GenerateFolders)
            UrlRouting.Initialize();
    }

    static System.Threading.Timer _timer = null;
    static bool firstRunComplete = false;
    static string applicationPath = "";
    
    static void PeriodicTasks(object state)
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
        try
        {
            Comment.DeleteDeletedComments();
            Comment.DeleteUnpublishedComments();
            Post.DestroyDeletedPosts();
            FeedManager.UpdateFeedData();
            UploadFileManager.RemoveUploadContextsOlderThan(applicationPath, 6);
            
            
            if (firstRunComplete)
            {
                
                Log.RemoveLogsOlderThan(48);
                
            }
        }
        catch(Exception ex)
        {
            Log.Error("Period Tasks", "Something went wrong. Reason: {0} Stack: {1}",ex.Message,ex.StackTrace);
        }

        firstRunComplete = true;
        _timer.Change(150000, 150000);
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown
        if (_timer != null)
            _timer.Dispose();
    }
        
    private static Regex wwwStatusRegex = new Regex("https?://www\\.", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex noWwwStatusRegex = new Regex("https?://", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    void Application_BeginRequest(object sender, EventArgs e)
    {
        bool isWWW = wwwStatusRegex.IsMatch(Request.Url.ToString());
        string redirectLocation = null;
        if(isWWW && !SiteSettings.Get().RequireWWW)
        {
            redirectLocation = wwwStatusRegex.Replace(Request.Url.ToString(), string.Format("{0}://", Request.Url.Scheme));
        }
        else if (!isWWW && SiteSettings.Get().RequireWWW)
        {
            redirectLocation = noWwwStatusRegex.Replace(Request.Url.ToString(), string.Format("{0}://www.", Request.Url.Scheme));
          
        }
        
        if(redirectLocation != null)
        {
            if (redirectLocation.ToLower().EndsWith("default.aspx"))
                redirectLocation = redirectLocation.Substring(0, redirectLocation.Length - 12);
            
            Response.RedirectLocation = redirectLocation;
            Response.StatusCode = 301;
            Response.End();
        }
        
        GraffitiContext graffitiContext = GraffitiContext.Create(Context);
        graffitiContext.Theme = SiteSettings.Get().Theme;

       HttpCookie userCookie = Request.Cookies["graffitibot"];
       Guid userid = Guid.Empty;
       if (userCookie != null)
       {
           try
           {
               FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(userCookie.Value);
               userid = new Guid(ticket.UserData);
               HttpContext.Current.Items.Add("UserId", userid);
           }
           catch
           {
           }
       }

       if (userid == Guid.Empty) userid = Guid.NewGuid();

       FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(1, "Graffiti-Bot-User", DateTime.Now, DateTime.Now.AddHours(6), true, userid.ToString());

       HttpCookie newUserCookie = new HttpCookie("graffitibot");
       newUserCookie.Value = FormsAuthentication.Encrypt(newTicket);
       newUserCookie.Expires = DateTime.Now.AddHours(6);
       Response.Cookies.Add(newUserCookie);
        
        Graffiti.Core.Events.Instance().ExecuteBeginRequest(sender,e);

    }

    void Application_EndRequest(object sender, EventArgs e)
    {
        GraffitiContext.Unload();
        Graffiti.Core.Events.Instance().ExecuteEndRequest(sender,e);
    }
    
       
</script>
