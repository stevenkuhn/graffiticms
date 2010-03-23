using System;
using System.Collections.Specialized;
using System.Text;

namespace Graffiti.Core
{
	[WidgetInfo("adbc8808-47c1-485b-9b12-d9e93e56a7f6", "Recent Posts", "Controls the display of a list of recent posts")]
	[Serializable]
	public class RecentPostWidget : Widget
	{
		public override string Name
		{
			get
			{
				if (string.IsNullOrEmpty(Title))
					return "Recent Posts";
				else
					return Title;
			}
		}

		public override string Title
		{
			get
			{
				if (string.IsNullOrEmpty(base.Title))
					base.Title = "Recent Posts";

				return base.Title;
			}
			set
			{

				base.Title = value;
			}
		}

		private int _categoryId = -1;
		public int CategoryId
		{
			get { return _categoryId; }
			set { _categoryId = value; }
		}

		public bool _showExcerpt = false;
		public bool ShowExcerpt
		{
			get { return _showExcerpt; }
			set { _showExcerpt = value; }
		}

		public int _numberOfPosts = 5;
		public int NumberOfPosts
		{
			get { return _numberOfPosts; }
			set { _numberOfPosts = value; }
		}

		protected override FormElementCollection AddFormElements()
		{
			FormElementCollection fec = new FormElementCollection();
			fec.Add(AddTitleElement());

			ListFormElement lfe = new ListFormElement("numberOFposts", "Number of Posts", "The number of most recent posts to list");
			lfe.Add(new ListItemFormElement("3", "3"));
			lfe.Add(new ListItemFormElement("5", "5", true));
			lfe.Add(new ListItemFormElement("10", "10"));
			fec.Add(lfe);

			ListFormElement lfeCats = new ListFormElement("categoryId", "Filter by Category", "Do you want to filter by a category?");
			lfeCats.Add(new ListItemFormElement("All Categories", "-1", CategoryId == -1));
			foreach (Category c in new CategoryController().GetTopLevelCachedCategories())
			{
				lfeCats.Add(new ListItemFormElement(c.Name, c.Id.ToString(), c.Id == CategoryId));
				if (c.HasChildren)
				{
					foreach (Category child in c.Children)
					{
						lfeCats.Add(new ListItemFormElement("--" + child.Name, child.Id.ToString(), child.Id == CategoryId));
					}
				}
			}

			fec.Add(lfeCats);

			fec.Add(new CheckFormElement("showExcerpt", "Show Excerpt", "Do you want to display a short excerpt", false));

			return fec;
		}

		public override string RenderData()
		{
			StringBuilder sb = new StringBuilder("<ul>");
			Data data = new Data();
			PostCollection pc = CategoryId > 0
											? data.PostsByCategory(
													new CategoryController().GetCachedCategory(CategoryId, true), NumberOfPosts)
											: data.RecentPosts(NumberOfPosts);

			foreach (Post p in pc)
			{
				if (RolePermissionManager.GetPermissions(p.CategoryId, GraffitiUsers.Current).Read)
					sb.AppendFormat("<li><a href=\"{0}\">{1}</a>{2}</li>\n", p.Url, p.Title, ShowExcerpt ? "<br />" + p.CustomExcerpt(100) : null);
			}

			sb.Append("</ul>\n");

			return sb.ToString();
		}

		protected override NameValueCollection DataAsNameValueCollection()
		{
			NameValueCollection nvc = base.DataAsNameValueCollection();
			nvc["numberOFposts"] = NumberOfPosts.ToString();
			nvc["showExcerpt"] = ShowExcerpt.ToString();
			nvc["categoryid"] = CategoryId.ToString();
			return nvc;
		}

		public override StatusType SetValues(System.Web.HttpContext context, NameValueCollection nvc)
		{
			base.SetValues(context, nvc);

			if (!string.IsNullOrEmpty(nvc["numberOFposts"]))
				NumberOfPosts = Int32.Parse(nvc["numberOFposts"]);

			if (!string.IsNullOrEmpty(nvc["categoryid"]))
				CategoryId = Int32.Parse(nvc["categoryid"]);

			ShowExcerpt = (nvc["showExcerpt"] == "checked" || nvc["showExcerpt"] == "on");
			return StatusType.Success;
		}

		public override string FormName
		{
			get
			{
				return "Recent Post Configuration";
			}
		}

	}
}