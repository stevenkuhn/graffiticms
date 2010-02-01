<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" Title="Graffiti Reports" Inherits="Graffiti.Core.ControlPanelPage" %>
<%@ Register TagPrefix="reports" TagName="daterangefilter" Src="~/graffiti-admin/reporting/daterangefilter_id.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
	<script type="text/javascript" src="../../swfobject.js"></script>
	<link href="../../reporting.css" runat="Server" type="text/css" media="all" rel="Stylesheet" />
</asp:Content>

<script runat="server">

    DateRange dateRange;

	void Page_Load(object sender, EventArgs e)
	{
        int postId;
		int.TryParse(Request.QueryString["id"], out postId);

		Post post = new Post(postId);
		PostLink.Text = post.Title;
		PostLink.NavigateUrl = post.Url;

		if (!Page.IsPostBack)
		{
			dateRange = DateRange.GetFromQueryString();
			Range.Text = dateRange.Text;

			minDate.Text = dateRange.Begin.Ticks.ToString();
			maxDate.Text = dateRange.End.Ticks.ToString();
		}
	}

	void RefreshButton_Click(object sender, EventArgs e)
	{
		if ((!BeginDate.IsDateTimeBlank) && (!EndDate.IsDateTimeBlank))
		{
			dateRange = new DateRange();
			dateRange.Type = "custom";
			dateRange.Begin = BeginDate.DateTime;
			dateRange.End = EndDate.DateTime;

			minDate.Text = dateRange.Begin.Ticks.ToString();
			maxDate.Text = dateRange.End.Ticks.ToString();
		}
	}

</script>

<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">
    <script type="text/javascript" language="javascript">
        function Expand()
        {
            document.getElementById("custom").style.display = "block";
        }
    </script>

<h1>Reporting <asp:Literal ID="Range" runat="server" /></h1>

<div id="emptyForm">

            <reports:daterangefilter ID="Daterangefilter1" runat="server" />
            <div id="custom" style="border: dashed 1px #ccc; display: none; margin-top: 5px; padding: 5px">
                between
                &nbsp;&nbsp;<Glow:DateTimeSelector runat="server" id="BeginDate" DateTimeFormat="MMMM d, yyyy" ShowCalendarPopup="true" />
                &nbsp;&nbsp;and
                &nbsp;&nbsp;<Glow:DateTimeSelector runat="server" id="EndDate" DateTimeFormat="MMMM d, yyyy" ShowCalendarPopup="true" />
                &nbsp;&nbsp;<asp:Button ID="RefreshButton" runat="server" OnClick="RefreshButton_Click" Text="Run Report" />
            </div>

                <h3>Comments by Date for <asp:HyperLink ID="PostLink" runat="server" /></h3>
	            <div id="linechart">
		            <strong>Unable to display the chart</strong>

	            </div>

	            <script type="text/javascript">
		            // <![CDATA[		
		            var so = new SWFObject("../../amline.swf", "amline", "100%", "200", "8", "#FFFFFF");
		            so.addVariable("path", "../../");
		            so.addVariable("settings_file", escape("../../linegraph.xml"));
		            so.addVariable("data_file", escape("../../charts.ashx?report=CommentsByPost_Single&id=<%=Request.QueryString["id"] %>&minDate=<asp:Literal id="minDate" runat="server"/>&maxDate=<asp:Literal id="maxDate" runat="server" />"));
		            so.addVariable("preloader_color", "#999999");
					so.addParam('wmode', 'transparent');
		            so.write("linechart");
		            // ]]>
	            </script>

</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
</asp:Content>

