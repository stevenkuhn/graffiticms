<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" Title="Graffiti Reports" Inherits="Graffiti.Core.ControlPanelPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
	<script type="text/javascript" src="../../swfobject.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">
<h1>Reporting</h1>

<div id="emptyForm">

<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

        int year;
		int month;
		int day;
		int.TryParse(Request.QueryString["year"], out year);
		int.TryParse(Request.QueryString["month"], out month);
		int.TryParse(Request.QueryString["day"], out day);
		
		DateTime date = new DateTime(year, month, day);
		Date.Text = date.ToString("d");
	}
</script>

                <h3>Comments by Post for <asp:Literal ID="Date" runat="server" /> (Top 10)</h3>
	            <div id="barchart">
		            <strong>Unable to display the chart</strong>

	            </div>

	            <script type="text/javascript">
		            // <![CDATA[
                        var so = new SWFObject("../../amcolumn.swf", "amcolumn", "100%", "500", "8", "#FFFFFF");
                        so.addVariable("path", "../../");
                        so.addVariable("settings_file", escape("../../columngraph_comments.xml"));
                        so.addVariable("data_file", escape("../../charts.ashx?report=CommentsByDate_Single&year=<%=Request.QueryString["year"] %>&month=<%=Request.QueryString["month"] %>&day=<%=Request.QueryString["day"] %>"));
                        so.addVariable("preloader_color", "#999999");
						so.addParam('wmode', 'transparent');
                        so.write("barchart");
		            // ]]>
	            </script>



</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
</asp:Content>

