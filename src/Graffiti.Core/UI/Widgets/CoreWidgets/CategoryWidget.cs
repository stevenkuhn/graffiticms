using System.Text;

namespace Graffiti.Core
{
    /// <summary>
    /// A widget which displays a title and the a block of text
    /// </summary>
    [WidgetInfo("34a544d7-3839-4e14-af48-ab233231d0e1", "Category Widget", "Represents a box")]
    public class CategoryWidget : Widget
    {

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                    return "Categories";
                else
                    return Title + " (Categories)";
            }
        }

        public override string Title
        {
            get
            {
                if (string.IsNullOrEmpty(base.Title))
                    base.Title = "Categories";

                return base.Title;
            }
            set
            {
                base.Title = value;
            }
        }

        public override string EditUrl
        {
            get
            {
                return "EditCategoryWidget.aspx";
            }
        }

        protected override FormElementCollection AddFormElements()
        {
            return null;
        }

        public override string RenderData()
        {
            CategoryCollection cc = null;

            if (CategoryIds == null || CategoryIds.Length == 0)
                cc = new CategoryController().GetTopLevelCachedCategories();
            else
            {
                CategoryController controller = new CategoryController();
                cc = new CategoryCollection();
                foreach (int i in CategoryIds)
                {
                    Category c = controller.GetCachedCategory(i, true);
                    if(c != null)
                        cc.Add(c);
                }
            }
           

            StringBuilder sb = new StringBuilder("<ul>");
            foreach(Category c in cc)
            {
                sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", c.Url, c.Name);
            }

            sb.Append("</ul>");

            return sb.ToString();
        }

        public int[] CategoryIds = new int[0];

    }
}