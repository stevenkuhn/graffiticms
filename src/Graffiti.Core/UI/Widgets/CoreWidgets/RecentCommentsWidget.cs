using System;
using System.Collections.Specialized;
using System.Text;

namespace Graffiti.Core
{
    [WidgetInfo("4eaf767d-c787-4e9c-aafc-e37d0ef3f70c", "Recent Comments", "Controls the display of a list of recent comments")]
    [Serializable]
    public class RecentCommentsWidget : Widget
    {
        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                    return "Recent Comments";
                else
                    return Title;
            }
        }

        public override string Title
        {
            get
            {
                if (string.IsNullOrEmpty(base.Title))
                    base.Title = "Recent Comments";

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

        protected override FormElementCollection AddFormElements()
        {
            FormElementCollection fec = new FormElementCollection();
            fec.Add(AddTitleElement());
            ListFormElement lfe = new ListFormElement("numberOFcomments", "Number of Comments", "The number of most recent comments to list");
            lfe.Add(new ListItemFormElement("3", "3"));
            lfe.Add(new ListItemFormElement("5","5",true));
            lfe.Add(new ListItemFormElement("10","10"));
            fec.Add(lfe);

            return fec;
        }

        public override string RenderData()
        {
            StringBuilder sb = new StringBuilder("<ul>");

            sb.Append(new Macros().ULRecentComments(NumberOfComments));

            sb.Append("</ul>\n");

            return sb.ToString();
        }

        protected override NameValueCollection DataAsNameValueCollection()
        {
            NameValueCollection nvc = base.DataAsNameValueCollection();
            nvc["numberOFcomments"] = NumberOfComments.ToString();
            //nvc["categoryid"] = CategoryId.ToString();
            return nvc;
        }

        public override StatusType SetValues(System.Web.HttpContext context, NameValueCollection nvc)
        {
            base.SetValues(context, nvc);
            NumberOfComments = Int32.Parse(nvc["numberOFcomments"]);
            //CategoryId = Int32.Parse(nvc["categoryid"]);
            return StatusType.Success;
        }

        public override string FormName
        {
            get
            {
                return "Recent Comment Configuration";
            }
        }


        public int NumberOfComments = 5;
    }
}