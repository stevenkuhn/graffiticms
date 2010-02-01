using System;
using System.Web;

namespace Graffiti.Core
{
    public class DateRange
    {
        private string _type = "lastthirtydays";
        private DateTime _begin = _defaultBegin;
        private DateTime _end = _defaultEnd;

		private static DateTime _defaultBegin = new DateTime(2000, 1, 1); // should this be DateTime.MinValue?
		private static DateTime _defaultEnd = new DateTime(2100, 12, 31); // should this be DateTime.MaxValue?

        public static DateRange GetFromQueryString()
        {
            DateRange range = new DateRange();
            HttpRequest request = HttpContext.Current.Request;

            if (!string.IsNullOrEmpty(request.QueryString["range"]))
                range.Type = request.QueryString["range"].ToLower();

            if ((range.Type.ToLower() == "custom") && (!string.IsNullOrEmpty(request.QueryString["begindate"])))
                range.Begin = DateTime.ParseExact(request.QueryString["begindate"], "yyyyMMdd", null);

            if ((range.Type.ToLower() == "custom") && (!string.IsNullOrEmpty(request.QueryString["enddate"])))
				range.End = DateTime.ParseExact(request.QueryString["enddate"], "yyyyMMdd", null);

            return range;
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public DateTime Begin
        {
            get
            {
				DateTime today = DateTime.Now.Date;
                if (Type.ToLower() == "today")
					return today;
                else if (Type.ToLower() == "yesterday")
					return today.AddDays(-1);
                else if (Type.ToLower() == "lastsevendays")
					return today.AddDays(-6);
                else if (Type.ToLower() == "lastthirtydays")
					return today.AddDays(-29);
                else if (Type.ToLower() == "currentmonth")
					return today.AddDays(-today.Day + 1);
                else if (Type.ToLower() == "lastmonth")
                    return today.AddDays(-today.Day + 1).AddMonths(-1);
                else if (Type.ToLower() == "forever")
					return new DateTime(2007, 1, 1); // should this be _defaultBegin?
                else
                    return _begin;
            }
            set { _begin = value; }
        }

        public DateTime End
        {
            get
            {
				DateTime today = DateTime.Now.Date;
				if (Type.ToLower() == "today")
					return today;
                else if (Type.ToLower() == "yesterday")
                    return today.AddDays(-1);
                else if (Type.ToLower() == "lastsevendays")
					return today;
                else if (Type.ToLower() == "lastthirtydays")
					return today;
                else if (Type.ToLower() == "currentmonth")
					return today;
                else if (Type.ToLower() == "lastmonth")
					return today.AddDays(-today.Day);
                else if (Type.ToLower() == "forever")
					return today; // should this be _defaultEnd?
                else
                    return _end;
            }
            set { _end = value; }
        }

        public string Text
        {
            get
            {
                string rangeText = string.Empty;

                if (Type.ToLower() == "today")
                    rangeText = string.Format("for today ({0})", Begin.ToShortDateString());
                else if (Type.ToLower() == "yesterday")
                    rangeText = string.Format("for yesterday ({0})", Begin.ToShortDateString());
                else if (Type.ToLower() == "lastsevendays")
                    rangeText = string.Format("for the last seven days ({0} - {1})", Begin.ToShortDateString(), End.ToShortDateString());
                else if (Type.ToLower() == "lastthirtydays")
                    rangeText = string.Format("for the last thirty days ({0} - {1})", Begin.ToShortDateString(), End.ToShortDateString());
                else if (Type.ToLower() == "currentmonth")
                    rangeText = string.Format("for {0} ({1} - {2})", Begin.ToString("MMMM"), Begin.ToShortDateString(), End.ToShortDateString());
                else if (Type.ToLower() == "lastmonth")
                    rangeText = string.Format("for {0} ({1} - {2})", Begin.ToString("MMMM"), Begin.ToShortDateString(), End.ToShortDateString());
                else if (Type.ToLower() == "forever")
                    rangeText = string.Format("({0} - {1})", Begin.ToShortDateString(), End.ToShortDateString());
                else if (Begin == End)
                    rangeText = string.Format("for {0}", Begin.ToShortDateString());
				else if ((Begin != _defaultBegin) && (End != _defaultEnd))
                    rangeText = string.Format("({0} - {1})", Begin.ToShortDateString(), End.ToShortDateString());

                return rangeText;
            }
        }

        public string ToQueryString()
        {
            string queryString = string.Format("range={0}", Type.ToLower());

            if (Type.ToLower() == "custom")
				queryString += string.Format("&begindate={0}&enddate={1}", Begin.ToString("yyyyMMdd"), End.ToString("yyyyMMdd"));

            return queryString;
        }
    }
}
