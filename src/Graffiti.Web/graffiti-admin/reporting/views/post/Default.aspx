<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" Title="Graffiti Reports" Inherits="Graffiti.Core.ControlPanelPage" %>
<%@ Register TagPrefix="reports" TagName="daterangefilter" Src="~/graffiti-admin/reporting/daterangefilter_id.ascx" %>
<%@ Import Namespace="DataBuddy" %>
<%@ Import Namespace="System.Collections.Generic" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
	<script type="text/javascript" src="../../swfobject.js"></script>
	<link href="../../reporting.css" runat="Server" type="text/css" media="all" rel="Stylesheet" />
</asp:Content>

<script runat="server">

    DateRange dateRange;

	void Page_Load(object sender, EventArgs e)
	{
        LiHyperLink.SetNameToCompare(Context, "reporting");
        
		int postId;
		int.TryParse(Request.QueryString["id"], out postId);

		Post post = new Post(postId);
		PostLink.Text = post.Title;
		PostLink.NavigateUrl = post.Url;

        if (!RolePermissionManager.GetPermissions(post.CategoryId, GraffitiUsers.Current).Read)
        {
            Response.Redirect("~/graffiti-admin/");
            return;
        }
        
		if (!Page.IsPostBack)
		{
			dateRange = DateRange.GetFromQueryString();

            if (dateRange.Begin < post.Published)
            {
                dateRange.Type = "custom";
                dateRange.Begin = post.Published;
                dateRange.End = DateTime.Today;
            }
            
			Range.Text = dateRange.Text;

			minDate.Text = dateRange.Begin.Ticks.ToString();
			maxDate.Text = dateRange.End.Ticks.ToString();

            int count = Reports.ViewsByPostSingleCount(postId, dateRange.Begin, dateRange.End);

            if (count == 1)
                lblTotalViewed.Text = "<span class=\"largeNumberText\">1</span> person has ";
            else
                lblTotalViewed.Text = "<span class=\"largeNumberText\">" + count + "</span> people have ";

            TimeSpan ts = dateRange.End.Subtract(dateRange.Begin);

            try
            {
                lblAverageViewsDay.Text = Math.Round(((decimal)count / ts.Days), 2).ToString();
            }
            catch (Exception)
            {
                lblAverageViewsDay.Text = "n/a";
            }

            Query q = Comment.CreateQuery();
            q.AndWhere(Comment.Columns.IsPublished, false);
            q.AndWhere(Comment.Columns.IsDeleted, false);
            q.AndWhere(Comment.Columns.PostId, postId);
            q.AndWhere(Comment.Columns.Published, dateRange.Begin, Comparison.GreaterThan);
            q.AndWhere(Comment.Columns.Published, dateRange.End, Comparison.LessOrEquals);

            CommentCollection cc = CommentCollection.FetchByQuery(q);

            PendingCommentList.DataSource = cc;
            PendingCommentList.DataBind();

            if (cc != null)
            {
                if (cc.Count == 1)
                    lblTotalCommentsPending.Text = " <span class=\"largeNumberText\" id=\"commentsPending\">1</span> comment is ";
                else
                    lblTotalCommentsPending.Text = " <span class=\"largeNumberText\" id=\"commentsPending\">" + cc.Count.ToString() + "</span> comments are ";
            }
            else
            {
                lblTotalCommentsPending.Text = " are <span class=\"largeNumberText\">0</span> comments ";
            }

            int publishedCount = Reports.CommentsByPostSingleCount(postId, dateRange.Begin, dateRange.End);

            if (publishedCount == 1)
                lblTotalCommentsPublished.Text = "<span class=\"largeNumberText\">1</span> comment has ";
            else
                lblTotalCommentsPublished.Text = "<span class=\"largeNumberText\">" + publishedCount + "</span> comments have ";

            ReportData top5 = Reports.MostPopularPosts();

            if (top5.Titles.Contains(new KeyValuePair<int, string>(postId, post.Title)))
                top5post.Visible = true;
            else
                top5post.Visible = false;
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
	
	protected string IsAltRow(int index)
    {
        if (index % 2 == 0)
            return string.Empty;
        else
            return " class=\"alt\"";
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
            <br />
                <h3 style="float: left;">Views by Date for <asp:HyperLink ID="PostLink" runat="server" /></h3>
                <img src="~/graffiti-admin/common/img/top5.gif" runat="server" id="top5post" style="float: right;" />
	            <div style="clear: both;"></div>
	            <div id="linechart">
		            <strong>Unable to display the chart</strong>

	            </div>

	            <script type="text/javascript">
		            // <![CDATA[		
		            var so = new SWFObject("../../amline.swf", "amline", "100%", "200", "8", "#FFFFFF");
		            so.addVariable("path", "../../");
		            so.addVariable("settings_file", escape("../../linegraph.xml"));
		            so.addVariable("data_file", escape("../../charts.ashx?report=ViewsByPost_Single&id=<%=Request.QueryString["id"] %>&minDate=<asp:Literal id="minDate" runat="server"/>&maxDate=<asp:Literal id="maxDate" runat="server" />"));
		            so.addVariable("preloader_color", "#999999");
					so.addParam('wmode', 'transparent');
		            so.write("linechart");
		            // ]]>
	            </script>
	            
	        <div style="padding-top: 30px; padding-bottom: 20px; padding-left: 20px; float: left; width: 25%;">
                <asp:label ID="lblTotalViewed" runat="server" /> viewed this content.
                <br /><br />
                <span class="largeNumberText"><asp:label ID="lblAverageViewsDay" runat="server" /></span> Average Views/Day.
            </div>
            
            <div style="padding-top: 30px; padding-bottom: 20px; float: right; width: 70%;">
                <p style="margin-bottom: 5px; margin-top: 0px; margin-left: 2px;"><asp:label ID="lblTotalCommentsPublished" runat="server" /> been published and <asp:Label ID="lblTotalCommentsPending" runat="server" /> waiting for approval.</p>
        
                <Z:Repeater runat="Server" ShowHeaderFooterOnNone = "false" ID = "PendingCommentList">
                    <HeaderTemplate>
                        <table id="postList" style="margin-top: 15px;">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr id='comment-<%# Eval("Id") %>' <%# IsAltRow(Container.ItemIndex) %>>
                            <td style="padding: 4px; padding-top: 7px; padding-bottom: 7px;">
                                <span style="font-size: 135%;">Comment from <strong><%# new Macros().CommentLink(Container.DataItem as Comment) %></strong> - <span style="font-size: 80%;"><a target="_blank" href="http://ws.arin.net/cgi-bin/whois.pl?queryinput=<%# Eval("IPAddress") %>"><%# Eval("IPAddress") %></a></span></span><br />
                                <span style="line-height: 18px;">Post: <a href="<%# Eval("Post.Url") %>"><%# Eval("Post.Name") %></a></span><br />
                                <span style="line-height: 13px;"><%# Eval("Body") %></span>
                            </td>
                            <td width="230px" style="text-align: center; vertical-align: middle;">
                                Received <%# Eval("Published", "{0:dd-MMM-yyyy}")%> <%# Eval("Published", "{0:hh:mm tt}") %>
                            </td>
                            <td width="150px" style="text-align: center; vertical-align: middle;">
                                <asp:PlaceHolder id="approve" runat="server">
                                    <a style="font-size: 120%; <asp:literal id="ApproveBold" runat="server" Text="font-weight: bold;" Visible="false" />" href="javascript:void(0);" onclick="Comments.approve('<%= new Urls().AdminAjax %>', <%# Eval("Id") %>,'comment-<%# Eval("Id") %>'); return false;">Approve</a>
                                </asp:PlaceHolder>
                                <asp:Label ID="pipe2" runat="server" Text=" | " />
                                <asp:PlaceHolder id="delete" runat="server">
                                    <a style="font-size: 120%;" href="javascript:void(0);" onclick="Comments.deleteCommentWithStatus('<%= new Urls().AdminAjax %>', <%# Eval("Id") %>,'comment-<%# Eval("Id") %>'); return false;">Delete</a>
                                </asp:PlaceHolder>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </Z:Repeater>
            </div>

</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
</asp:Content>

