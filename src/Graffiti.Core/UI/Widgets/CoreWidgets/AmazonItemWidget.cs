using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace Graffiti.Core
{
	[WidgetInfo("3af19ee1-fef2-4b38-a2c1-c1ea0b112261", "Amazon Items", "A list of Amazon Items")]
	public class AmazonItemWidget : Widget
	{
		public string[] Links = new string[0];
		public string AmazonId;
		public int HowMany = -1;
		public string AmazonLinkItems = null;

		public override string RenderData()
		{
			string[] itemsToRender = null;
			if (HowMany == -1)
			{
				itemsToRender = Links;
			}
			else
			{
				itemsToRender = Util.Randomize(Links, HowMany > 0 ? HowMany : Links.Length);
			}

			StringBuilder sb = new StringBuilder("<ul>");
			foreach (string link in itemsToRender)
			{
				sb.Append("<li>" + link + "</li>\n");
			}

			sb.Append("</ul>");
			return sb.ToString();
		}

		public override string Name
		{
			get { return "Amazon Items" + (!string.IsNullOrEmpty(Title) ? ": " + Title : string.Empty); }
		}

		protected override FormElementCollection AddFormElements()
		{
			FormElementCollection fec = new FormElementCollection();
			fec.Add(new TextFormElement("title", "Name", "Give your list of items a name"));
			fec.Add(new TextFormElement("amazonid", "Amazon Id", "Please enter your Amazon Seller Id"));
			fec.Add(new TextAreaFormElement("amazonLinkItems", "Items", "Copy a link from amazon on each line", 5));
			ListFormElement lfe = new ListFormElement("show", "Items to Show", "How many items do you want to show at a time?");
			lfe.Add(new ListItemFormElement("All", "-1", true));
			lfe.Add(new ListItemFormElement("All (random)", "0"));
			lfe.Add(new ListItemFormElement("1 random item", "1"));
			lfe.Add(new ListItemFormElement("3 random items", "3"));
			lfe.Add(new ListItemFormElement("5 random items", "5"));
			fec.Add(lfe);

			return fec;
		}

		protected override NameValueCollection DataAsNameValueCollection()
		{
			NameValueCollection nvc = base.DataAsNameValueCollection();
			nvc["amazonid"] = AmazonId;
			nvc["show"] = HowMany.ToString();
			nvc["amazonLinkItems"] = AmazonLinkItems;

			return nvc;
		}

		public override StatusType SetValues(System.Web.HttpContext context, NameValueCollection nvc)
		{
			StatusType st = base.SetValues(context, nvc);

			if (!string.IsNullOrEmpty(nvc["show"]))
				HowMany = Int32.Parse(nvc["show"]);

			AmazonLinkItems = nvc["AmazonLinkItems"];

			if (string.IsNullOrEmpty(nvc["amazonid"]))
			{
				SetMessage(context, "Your must include an AmazonId");
				return StatusType.Error;
			}

			AmazonId = nvc["amazonid"];

			List<string> newLinks = new List<string>();
			if (!string.IsNullOrEmpty(AmazonLinkItems))
			{
				string[] lines = AmazonLinkItems.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string line in lines)
				{
					string asin = ParseLinkForASIN(line);
					if (asin != null)
						newLinks.Add(string.Format(amazonFormat, asin, AmazonId));
				}
			}

			Links = newLinks.ToArray();


			return st;
		}

		private static string ParseLinkForASIN(string link)
		{
			const string pattern = "\\/(ASIN|product|dp)\\/(?<ASIN>[0-9a-zA-Z\\-]{10,14})";
			Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

			if (link != null && link.ToLower().StartsWith("http://"))
			{
				Match m = regex.Match(link);
				if (m.Success)
				{
					string asin = m.Groups["ASIN"].Value;
					if (!string.IsNullOrEmpty(asin))
					{
						Log.Info("Amazon Widget", "Successful Link Conversion",
								  "Converted  " + link + " to " + asin);

						return asin;
					}
				}

			}
			Log.Warn("Amazon Widget", "The link " + link + " could not be processed");
			return null;
		}

		private const string amazonFormat =
			 "<a href=\"http://www.amazon.com/exec/obidos/redirect?path=ASIN/{0}&amp;link_code=as2&amp;camp=1789&amp;tag={1}&amp;creative=9325\"><img src=\"http://images.amazon.com/images/P/{0}.01._AA_SCTZZZZZZZ_.jpg\" /></a>";

	}
}
