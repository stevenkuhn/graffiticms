using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Graffiti.Core;

namespace Graffiti.Web.graffiti_admin.reporting
{
    public partial class Default : ControlPanelPage
    {
        DateRange dateRange;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                dateRange = DateRange.GetFromQueryString();
                Range.Text = dateRange.Text;

                minDate1.Text = minDate2.Text = minDate3.Text = minDate4.Text = dateRange.Begin.Ticks.ToString();
                maxDate1.Text = maxDate2.Text = maxDate3.Text = maxDate4.Text = dateRange.End.Ticks.ToString();
            }
        }

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            if ((!BeginDate.IsDateTimeBlank) && (!EndDate.IsDateTimeBlank))
            {
                dateRange = new DateRange();
                dateRange.Type = "custom";
                dateRange.Begin = BeginDate.DateTime;
                dateRange.End = EndDate.DateTime;

                minDate1.Text = minDate2.Text = minDate3.Text = minDate4.Text = dateRange.Begin.Ticks.ToString();
                maxDate1.Text = maxDate2.Text = maxDate3.Text = maxDate4.Text = dateRange.End.Ticks.ToString();
            }
        }
    }
}
