<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" Title="Graffiti Plug-Ins" Inherits="Graffiti.Core.AdminControlPanelPage" %>
<%@ Import namespace="System.Collections.Generic"%>
<script runat="Server">

    void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context, "settings");
        List<EventDetails> the_Events = Graffiti.Core.Events.GetEvents();
        PluginList.DataSource = the_Events;
        PluginList.DataBind();

        string pluginSaved = Request.QueryString["ps"];

        if (!String.IsNullOrEmpty(pluginSaved))
        {
            Message.Text = "The plug-in <b>" + Server.UrlDecode(pluginSaved) + "</b> was updated.";
            Message.Type = StatusType.Success;
        }
    }
    
    string EditLink(object obj)
    {
        EventDetails detail = (EventDetails)obj;

        if (detail.Event.IsEditable)
            return string.Format("<a href=\"{0}?t={1}\">{2}</a>", detail.Event.EditUrl, Server.UrlEncode(detail.EventType), "Edit");
        else
            return "n.a.";
    }
    
    static string StatusText(object obj)
    {
        bool b = (bool)obj;

        if (b)
            return "Enabled";
        else
            return "Disabled";
    }

    static string SwapStatusText(object obj)
    {
        bool b = (bool)obj;

        if (!b)
            return "Enable";
        else
            return "Disable";
    }

    static string GetCssClassText(object obj)
    {
        bool b = (bool)obj;

        if (b)
            return "listbox_status_enabled";
        else
            return "listbox_status_disabled";
    }

    static string GetID(object obj)
    {
        string the_ID = obj as string;
        return Util.CleanForUrl(the_ID);
       
    }

</script>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">
<script type="text/javascript">

function refresh()
{
    window.location.href = window.location.href;
}

function ToggleStatus(typeName, btnID)
{

    $.ajax(
    {
        type: 'POST',
        dataType: 'text',
        url: '<%= new Urls().AdminAjax %>?command=toggleEventStatus&t=' + typeName,
        data: "{}",
        success: function(transport) {
            var response = transport;

            $('#' + btnID).html(response);
            $('#span-' + btnID).html(response);
            
            if (response == 'Enabled') {
                $('#' + btnID).html('Disable');
            } else {
                $('#' + btnID).html('Enable');
            }

            statusbox = $$('#status-' + btnID);

            if (statusbox.className == 'listbox_status_enabled')
                statusbox.className = 'listbox_status_disabled';
            else if (statusbox.className == 'listbox_status_disabled')
                statusbox.className = 'listbox_status_enabled';
        },
        error: function() { alert('Something went wrong...') }
    });
}

</script>


<h1>Plug-Ins</h1>
<Z:Breadcrumbs runat="server" SectionName="PlugIns" />
<Z:StatusMessage runat="server" ID="Message" />

<h3>Installed Plug-Ins</h3>
<a href="javascript:Telligent_Modal.Open('<%= new Urls().AdminMarketplace("Plugins") %>', 600, 475, refresh);">Search online plugins...</a>
<Z:Repeater runat="Server" ShowHeaderFooterOnNone = "false" ID = "PluginList">
    <NoneTemplate>
        <z:StatusMessage runat="Server" Text="Sorry, there are no plugins to manage." Type="Warning" />
    </NoneTemplate>
    <HeaderTemplate>
<br /><br />
        <ul class="listboxes">
    </HeaderTemplate>
    <ItemTemplate>
        <li>
            <div id="status-<%# GetID(Eval("EventType"))%>" class="<%# GetCssClassText(Eval("Enabled")) %>"><span id="span-<%# GetID(Eval("EventType"))%>"><%# StatusText(Eval("Enabled")) %></span></div>
            <div class="nonnested">
                <div class="title"><%# ((GraffitiEvent)Eval("Event")).Name %></div>
                <div class="commands">
                    <%# EditLink(Container.DataItem) %> | <a href="#" id="<%# GetID(Eval("EventType")) %>" onclick="ToggleStatus('<%# Eval("EventType")%>', '<%# GetID(Eval("EventType"))%>');"><%# SwapStatusText(Eval("Enabled")) %></a>    
                </div>
                <div class="body"><%# String.IsNullOrEmpty(((GraffitiEvent)Eval("Event")).Description) ? "n/a" : ((GraffitiEvent)Eval("Event")).Description%></div>
            </div>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</Z:Repeater>
        
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
</asp:Content>

