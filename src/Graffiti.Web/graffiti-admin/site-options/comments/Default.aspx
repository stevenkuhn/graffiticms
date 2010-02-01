<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_site_options_comments_Default" Title="Graffiti - Your Comment &amp; Spam Settings" Codebehind="Default.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

<h1>Your Comment &amp; Spam Settings</h1>

<Z:Breadcrumbs runat="server" SectionName="Comments" />
<z:StatusMessage runat="Server" ID = "Message" />

<div id = "messages_form">
    
    <div id="post_form_container" class="FormBlock">
        <h3>Comment Settings</h3>
        <h2><asp:CheckBox ID="EnableComments" Text="Enable Comments" runat="Server" /><br /><span class="form_tip">Controls if comments are enabled by default on new posts (and via <a href="http://en.wikipedia.org/wiki/MetaWeblog" target="_blank">MetaBlog API</a>).</span></h2> 
        
        <h2>Comment Days: <span class="form_label">Number of days new posts will accept comments</span></h2>
        <asp:DropDownList runat="Server" ID = "CommentDays" Style="width:200px;">
            <asp:ListItem Text="No days, Comments are Disabled" Value = "0" />
            <asp:ListItem Text="1 day" Value="1" />
            <asp:ListItem Text="2 days" Value="2" />
            <asp:ListItem Text="3 days" Value="3" />
            <asp:ListItem Text="1 week" Value="7" />
            <asp:ListItem Text="2 weeks" Value="14" />
            <asp:ListItem Text="1 month" Value="30" />
            <asp:ListItem Text="2 months" Value="60" />
            <asp:ListItem Text="3 months" Value="90" />
            <asp:ListItem Text="6 months" Value="180" />
            <asp:ListItem Text="1 year" Value="365" />
            <asp:ListItem Text="Forever" Value="-1" />
        </asp:DropDownList>
        
        <h2>Email Comments To: <span class="form_label">Seperate multiple email addresses with a ';' semi-colon</span></h2>
        <asp:TextBox CssClass = "large" runat="Server" id="txtEmail" />  
        
        
        <h3>Spam Settings</h3>
        
        <h2>Spam Score: <span class="form_tip">The threshold for flagging a comment as spam (relies on badwords.txt under the _utility/spam folder)</span></h2>
        <asp:TextBox CssClass = "large" runat="Server" id="txtSpamScore" style="width: 75px;" />  
        
        <h2><asp:CheckBox ID="chkUseAkismet" Text="Use Akismet For Spam" runat="Server" /> <span class="form_tip"><a href="http://akismet.com/" target="_blank">Akismet</a> is a free (and commerical) service you can use to prevent spam.</span></h2>
         
        <div id="akismetSettings" runat="server">
            <h2>Akismet Id: <span class="form_tip"></h2>
            <asp:TextBox CssClass = "large" runat="Server" id="txtAkismetId" />  
        
            <h2>Akismet Score: <span class="form_tip">The threshold for flagging a comment as spam via akismet</span></h2>
            <asp:TextBox CssClass = "large" runat="Server" id="txtAkismetScore" style="width: 75px;"   />  
        </div>
    </div>
    
    <div class="submit">
        <div id="buttons">
            <asp:Button runat="Server" ID="SettingsSave" Text = "Update Settings" OnClick="SettingsSave_Click" />
            <asp:HyperLink ID ="Cancel_Edit" Text = "(Cancel)" NavigateUrl="~/graffiti-admin/site-options/" runat="Server" />
        </div>
    </div> 
</div>

<script language="javascript">
    $('#<%= chkUseAkismet.ClientID %>').click(function() {
    $('#<%= akismetSettings.ClientID %>').slideToggle();
    });
</script>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
</asp:Content>

