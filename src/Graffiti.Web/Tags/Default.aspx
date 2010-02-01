<%@ Page Language="C#" AutoEventWireup="true"  %>
<script runat="Server">

    
    void Page_Load(object sender, EventArgs e)
    {
        Response.RedirectLocation = new Uri(Context.Request.Url, VirtualPathUtility.ToAbsolute("~/")).ToString();
        Response.StatusCode = 301;
        Response.End();
    }

</script>

