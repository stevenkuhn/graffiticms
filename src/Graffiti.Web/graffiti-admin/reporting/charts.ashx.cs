
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Xml;

namespace Graffiti.Core
{
    public class Charts : IHttpHandler
    {
        HttpContext context;

        public void ProcessRequest(HttpContext context)
        {
            if (!RolePermissionManager.CanViewControlPanel(GraffitiUsers.Current))
                context.Response.End();

            string report;
            report = context.Request.QueryString["report"];

            this.context = context;

            switch (report)
            {
                case "ViewsByDate":
                    ViewsByDateReport();
                    break;
                case "ViewsByDate_Single":
                    ViewsByDateReport_Single();
                    break;
                case "ViewsByPost":
                    ViewsByPostReport();
                    break;
                case "MostPopularPosts":
                    MostPopularPostReport();
                    break;
                case "ViewsByPost_Single":
                    ViewsByPostReport_Single();
                    break;

                case "CommentsByDate":
                    CommentsByDateReport();
                    break;
                case "CommentsByDate_Single":
                    CommentsByDateReport_Single();
                    break;
                case "CommentsByPost":
                    CommentsByPostReport();
                    break;
                case "CommentsByPost_Single":
                    CommentsByPostReport_Single();
                    break;
            }

        }

        #region Views

        private void ViewsByDateReport()
        {
            context.Response.ContentType = "text/xml";
            XmlTextWriter xml = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);
            DateTime minDate = new DateTime(long.Parse(context.Request.QueryString["minDate"]));
            DateTime maxDate = new DateTime(long.Parse(context.Request.QueryString["maxDate"]));
            string fromDashboard = context.Request.QueryString["fromDashboard"];

            xml.WriteStartElement("chart");

            // Series labels
            xml.WriteStartElement("xaxis");

            for (int i = 0; i <= maxDate.Subtract(minDate).Days; i++)
            {
                DateTime date = minDate.Date.AddDays(i);
                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", date.ToString("d"));
                xml.WriteValue(date.ToString("d MMM"));
                xml.WriteEndElement();
            }

            xml.WriteEndElement();


            xml.WriteStartElement("graphs");
            xml.WriteStartElement("graph");
            xml.WriteAttributeString("gid", "1");

            IDictionary<DateTime, int> dateCounts = Reports.ViewsByDateReport(minDate, maxDate);
            for (int i = 0; i <= maxDate.Subtract(minDate).Days; i++)
            {
                DateTime date = minDate.Date.AddDays(i);

                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", date.ToString("d"));

                if (!String.IsNullOrEmpty(fromDashboard))
                    xml.WriteAttributeString("url", "reporting/views/date/?year=" + date.Year + "&month=" + date.Month + "&day=" + date.Day);
                else
                    xml.WriteAttributeString("url", "views/date/?year=" + date.Year + "&month=" + date.Month + "&day=" + date.Day);

                if (dateCounts.ContainsKey(date))
                    xml.WriteValue(dateCounts[date]);
                else
                    xml.WriteValue(0);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.Close();
        }

        private void ViewsByDateReport_Single()
        {
            int year;
            int month;
            int day;
            int.TryParse(context.Request.QueryString["year"], out year);
            int.TryParse(context.Request.QueryString["month"], out month);
            int.TryParse(context.Request.QueryString["day"], out day);
            DateTime date = new DateTime(year, month, day);
            context.Response.ContentType = "text/xml";
            XmlTextWriter xml = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);


            ReportData viewsBySingle_ReportData = Reports.ViewsByDateSingle(date);

            xml.WriteStartElement("chart");

            // Series labels
            xml.WriteStartElement("series");

            foreach (int key in viewsBySingle_ReportData.Titles.Keys)
            {
                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", key.ToString());
                xml.WriteAttributeString("url", Post.GetCachedPost(key).Url);
                xml.WriteValue(viewsBySingle_ReportData.Titles[key]);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.WriteStartElement("graphs");
            xml.WriteStartElement("graph");
            xml.WriteAttributeString("gid", "1");

            foreach (int key in viewsBySingle_ReportData.Counts.Keys)
            {
                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", key.ToString());
                xml.WriteAttributeString("url", Post.GetCachedPost(key).Url);
                xml.WriteValue(viewsBySingle_ReportData.Counts[key]);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.Close();
        }

        private void ViewsByPostReport()
        {
            context.Response.ContentType = "text/xml";
            XmlTextWriter xml = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);
            DateTime minDate = new DateTime(long.Parse(context.Request.QueryString["minDate"]));
            DateTime maxDate = new DateTime(long.Parse(context.Request.QueryString["maxDate"]));

            ReportData data = Reports.GetViewsByPost(minDate, maxDate);


            xml.WriteStartElement("chart");

            // Series labels
            xml.WriteStartElement("series");

            foreach (int key in data.Titles.Keys)
            {
                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", key.ToString());
                xml.WriteAttributeString("url", Post.GetCachedPost(key).Url);
                xml.WriteValue(data.Titles[key]);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.WriteStartElement("graphs");
            xml.WriteStartElement("graph");
            xml.WriteAttributeString("gid", "1");

            foreach (int key in data.Counts.Keys)
            {
                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", key.ToString());
                xml.WriteAttributeString("url", "views/post/?id=" + key);
                xml.WriteValue(data.Counts[key]);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.Close();
        }

        private void MostPopularPostReport()
        {
            context.Response.ContentType = "text/xml";
            XmlTextWriter xml = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);

            ReportData data = Reports.MostPopularPosts();

            xml.WriteStartElement("pie");

            int counter = 0;

            foreach (int key in data.Titles.Keys)
            {
                counter++;

                xml.WriteStartElement("slice");
                xml.WriteAttributeString("title", data.Titles[key]);
                xml.WriteAttributeString("url", Post.GetCachedPost(key).Url);

                if (counter == 1)
                    xml.WriteAttributeString("pull_out", "true");

                xml.WriteValue(data.Counts[key]);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.Close();
        }

        private void ViewsByPostReport_Single()
        {
            int postId;
            int.TryParse(context.Request.QueryString["id"], out postId);
            DateTime minDate = new DateTime(long.Parse(context.Request.QueryString["minDate"]));
            DateTime maxDate = new DateTime(long.Parse(context.Request.QueryString["maxDate"]));
            context.Response.ContentType = "text/xml";
            XmlTextWriter xml = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);

            xml.WriteStartElement("chart");

            // Series labels
            xml.WriteStartElement("xaxis");

            for (int i = 0; i <= maxDate.Subtract(minDate).Days; i++)
            {
                DateTime date = minDate.Date.AddDays(i);
                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", date.ToString("d"));
                xml.WriteValue(date.ToString("d MMM"));
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.WriteStartElement("graphs");
            xml.WriteStartElement("graph");
            xml.WriteAttributeString("gid", "1");

            IDictionary<DateTime, int> dateCounts = Reports.ViewsByPostSingle(postId, minDate, maxDate);
            for (int i = 0; i <= maxDate.Subtract(minDate).Days; i++)
            {
                DateTime date = minDate.Date.AddDays(i);

                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", date.ToString("d"));
                xml.WriteAttributeString("url", "../date/?year=" + date.Year + "&month=" + date.Month + "&day=" + date.Day);
                if (dateCounts.ContainsKey(date))
                    xml.WriteValue(dateCounts[date]);
                else
                    xml.WriteValue(0);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.Close();
        }

        #endregion

        #region Comments

        private void CommentsByDateReport()
        {
            context.Response.ContentType = "text/xml";
            XmlTextWriter xml = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);
            DateTime minDate = new DateTime(long.Parse(context.Request.QueryString["minDate"]));
            DateTime maxDate = new DateTime(long.Parse(context.Request.QueryString["maxDate"]));

            xml.WriteStartElement("chart");

            // Series labels
            xml.WriteStartElement("xaxis");

            for (int i = 0; i <= maxDate.Subtract(minDate).Days; i++)
            {
                DateTime date = minDate.Date.AddDays(i);
                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", date.ToString("d"));
                xml.WriteValue(date.ToString("d MMM"));
                xml.WriteEndElement();
            }

            xml.WriteEndElement();


            xml.WriteStartElement("graphs");
            xml.WriteStartElement("graph");
            xml.WriteAttributeString("gid", "1");

            IDictionary<DateTime, int> dateCounts = Reports.CommentsByDate(minDate, maxDate);
            for (int i = 0; i <= maxDate.Subtract(minDate).Days; i++)
            {
                DateTime date = minDate.Date.AddDays(i);

                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", date.ToString("d"));
                xml.WriteAttributeString("url", "comments/date/?year=" + date.Year + "&month=" + date.Month + "&day=" + date.Day);
                if (dateCounts.ContainsKey(date))
                    xml.WriteValue(dateCounts[date]);
                else
                    xml.WriteValue(0);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.Close();
        }

        private void CommentsByDateReport_Single()
        {
            int year;
            int month;
            int day;
            int.TryParse(context.Request.QueryString["year"], out year);
            int.TryParse(context.Request.QueryString["month"], out month);
            int.TryParse(context.Request.QueryString["day"], out day);
            DateTime date = new DateTime(year, month, day);
            context.Response.ContentType = "text/xml";
            XmlTextWriter xml = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);

            ReportData data = Reports.CommentsByDateSingle(date);

            xml.WriteStartElement("chart");

            // Series labels
            xml.WriteStartElement("series");

            foreach (int key in data.Titles.Keys)
            {
                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", key.ToString());
                xml.WriteValue(data.Titles[key]);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.WriteStartElement("graphs");
            xml.WriteStartElement("graph");
            xml.WriteAttributeString("gid", "1");

            foreach (int key in data.Counts.Keys)
            {
                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", key.ToString());
                xml.WriteAttributeString("url", Post.GetCachedPost(key).Url);
                xml.WriteValue(data.Counts[key]);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.Close();
        }

        private void CommentsByPostReport()
        {
            context.Response.ContentType = "text/xml";
            XmlTextWriter xml = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);
            DateTime minDate = new DateTime(long.Parse(context.Request.QueryString["minDate"]));
            DateTime maxDate = new DateTime(long.Parse(context.Request.QueryString["maxDate"]));

            ReportData data = Reports.CommentsByPost(minDate, maxDate);

            xml.WriteStartElement("chart");

            // Series labels
            xml.WriteStartElement("series");

            foreach (int key in data.Titles.Keys)
            {
                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", key.ToString());
                xml.WriteValue(data.Titles[key]);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.WriteStartElement("graphs");
            xml.WriteStartElement("graph");
            xml.WriteAttributeString("gid", "1");

            foreach (int key in data.Counts.Keys)
            {
                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", key.ToString());
                xml.WriteAttributeString("url", "comments/post/?id=" + key);
                xml.WriteValue(data.Counts[key]);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.Close();
        }

        private void CommentsByPostReport_Single()
        {
            int postId;
            int.TryParse(context.Request.QueryString["id"], out postId);
            DateTime minDate = new DateTime(long.Parse(context.Request.QueryString["minDate"]));
            DateTime maxDate = new DateTime(long.Parse(context.Request.QueryString["maxDate"]));
            context.Response.ContentType = "text/xml";
            XmlTextWriter xml = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);

            xml.WriteStartElement("chart");

            // Series labels
            xml.WriteStartElement("xaxis");

            for (int i = 0; i <= maxDate.Subtract(minDate).Days; i++)
            {
                DateTime date = minDate.Date.AddDays(i);
                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", date.ToString("d"));
                xml.WriteValue(date.ToString("d MMM"));
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.WriteStartElement("graphs");
            xml.WriteStartElement("graph");
            xml.WriteAttributeString("gid", "1");

            IDictionary<DateTime, int> dateCounts = Reports.CommentsByPostSingle(postId, minDate, maxDate);
            for (int i = 0; i <= maxDate.Subtract(minDate).Days; i++)
            {
                DateTime date = minDate.Date.AddDays(i);

                xml.WriteStartElement("value");
                xml.WriteAttributeString("xid", date.ToString("d"));
                xml.WriteAttributeString("url", "../date/?year=" + date.Year + "&month=" + date.Month + "&day=" + date.Day);
                if (dateCounts.ContainsKey(date))
                    xml.WriteValue(dateCounts[date]);
                else
                    xml.WriteValue(0);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            xml.Close();
        }

        #endregion

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}