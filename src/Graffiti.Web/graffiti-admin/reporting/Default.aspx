<%@ Page Language="C#" Title="Graffiti Reports" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Graffiti.Web.graffiti_admin.reporting.Default" %>
<%@ Register TagPrefix="reports" TagName="daterangefilter" Src="~/graffiti-admin/reporting/daterangefilter.ascx" %>
<%@ Register TagPrefix="Glow" Assembly="Telligent.Glow" Namespace="Telligent.Glow" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
	<script type="text/javascript" src="swfobject.js"></script>
	<link id="Link3" href="reporting.css" runat="Server" type="text/css" media="all" rel="Stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">
    <script type="text/javascript" language="javascript">
        function Expand()
        {
            document.getElementById("custom").style.display = "block";
        }
    </script>

<h1>Reporting <asp:Literal ID="Range" runat="server" /></h1>

<div id="emptyForm">

            <reports:daterangefilter runat="server" />
            <div id="custom" style="border: dashed 1px #ccc; display: none; margin-top: 5px; padding: 5px">
                between
                &nbsp;&nbsp;<Glow:DateTimeSelector runat="server" id="BeginDate" DateTimeFormat="MMMM d, yyyy" ShowCalendarPopup="true" />
                &nbsp;&nbsp;and
                &nbsp;&nbsp;<Glow:DateTimeSelector runat="server" id="EndDate" DateTimeFormat="MMMM d, yyyy" ShowCalendarPopup="true" />
                &nbsp;&nbsp;<asp:Button ID="RefreshButton" runat="server" OnClick="RefreshButton_Click" Text="Run Report" />
            </div>

			<br />
                <h3>Views by Date</h3>
	            <div id="linechart">
		            <strong>Unable to display the chart</strong>

	            </div>

	            <script type="text/javascript">
		            // <![CDATA[		
		            var so = new SWFObject("amline.swf", "amline", "100%", "200", "8", "#FFFFFF");
		            so.addVariable("path", "");
		            so.addVariable("settings_file", escape("linegraph.xml"));
		            so.addVariable("data_file", escape("charts.ashx?report=ViewsByDate&minDate=<asp:Literal id="minDate1" runat="server"/>&maxDate=<asp:Literal id="maxDate1" runat="server" />"));
		            so.addVariable("preloader_color", "#999999");
					so.addParam('wmode', 'transparent');
		            so.write("linechart");
		            // ]]>
	            </script>


                <h3>Views by Post (Top 10)</h3>
	            <div id="barchart">
		            <strong>Unable to display the chart</strong>

	            </div>

	            <script type="text/javascript">
		            // <![CDATA[
                        var so = new SWFObject("amcolumn.swf", "amcolumn", "100%", "500", "8", "#FFFFFF");
                        so.addVariable("path", "");
                        so.addVariable("settings_file", escape("columngraph_views.xml"));
                        so.addVariable("data_file", escape("charts.ashx?report=ViewsByPost&minDate=<asp:Literal id="minDate2" runat="server"/>&maxDate=<asp:Literal id="maxDate2" runat="server" />"));
                        so.addVariable("preloader_color", "#999999");
						so.addParam('wmode', 'transparent');
                        so.write("barchart");
		            // ]]>
	            </script>

		<br />
                <h3>Comments by Date</h3>
	            <div id="linechart2">
		            <strong>Unable to display the chart</strong>

	            </div>

	            <script type="text/javascript">
		            // <![CDATA[		
		            var so = new SWFObject("amline.swf", "amline", "100%", "200", "8", "#FFFFFF");
		            so.addVariable("path", "");
		            so.addVariable("settings_file", escape("linegraph.xml"));
		            so.addVariable("data_file", escape("charts.ashx?report=CommentsByDate&minDate=<asp:Literal id="minDate3" runat="server"/>&maxDate=<asp:Literal id="maxDate3" runat="server" />"));
		            so.addVariable("preloader_color", "#999999");
					so.addParam('wmode', 'transparent');
		            so.write("linechart2");
		            // ]]>
	            </script>


                <h3>Comments by Post (Top 10)</h3>
	            <div id="barchart2">
		            <strong>Unable to display the chart</strong>

	            </div>

	            <script type="text/javascript">
		            // <![CDATA[
                        var so = new SWFObject("amcolumn.swf", "amcolumn", "100%", "500", "8", "#FFFFFF");
                        so.addVariable("path", "");
                        so.addVariable("settings_file", escape("columngraph_comments.xml"));
                        so.addVariable("data_file", escape("charts.ashx?report=CommentsByPost&minDate=<asp:Literal id="minDate4" runat="server"/>&maxDate=<asp:Literal id="maxDate4" runat="server" />"));
                        so.addVariable("preloader_color", "#999999");
						so.addParam('wmode', 'transparent');
                        so.write("barchart2");
		            // ]]>
	            </script>



</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
</asp:Content>


