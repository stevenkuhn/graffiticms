using System;
using System.Collections.Specialized;
using System.Text;

namespace Graffiti.Core
{
    [WidgetInfo("4b6d8a0a-61d2-4744-8bd8-d9d577bf82a8", "Tag Cloud", "Displays a list of tags sized by weight")]
    [Serializable]
    public class TagCloudWidget : Widget
    {
        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                    return "Tags";
                else
                    return Title;
            }
        }

        public override string Title
        {
            get
            {
                if (string.IsNullOrEmpty(base.Title))
                    base.Title = "Tags";

                return base.Title;
            }
            set
            {

                base.Title = value;
            }
        }

        protected override FormElementCollection AddFormElements()
        {
            FormElementCollection fec = new FormElementCollection();
            fec.Add(AddTitleElement());
            ListFormElement lfe = new ListFormElement("minNumberOfPosts", "Minimum Number of Posts", "The minimum number of posts a tag must be found in.");
            lfe.Add(new ListItemFormElement("No Minimum", "-1"));
            lfe.Add(new ListItemFormElement("More than One","2"));
            lfe.Add(new ListItemFormElement("3", "3"));
            lfe.Add(new ListItemFormElement("5","5",true));
            lfe.Add(new ListItemFormElement("10","10"));
            fec.Add(lfe);

            ListFormElement lfeMax = new ListFormElement("maxNumberOfTags", "Maximum number of Tags", "The maximum number of tags to display. If selected only the most popular tags will be returned.");
            lfeMax.Add(new ListItemFormElement("No Maximum","-1"));
            lfeMax.Add(new ListItemFormElement("25", "25",true));
            lfeMax.Add(new ListItemFormElement("50", "50"));
            lfeMax.Add(new ListItemFormElement("100", "100"));
            fec.Add(lfeMax);

            return fec;
        }

        public override string RenderData()
        {
            StringBuilder sb = new StringBuilder("<div class=\"tagCloud\">");

            sb.Append(new Macros().TagCloud(MinimiumNumberOfPosts, MaxNumberOfTags));

            sb.Append("</div>\n");

            return sb.ToString();
        }

        protected override NameValueCollection DataAsNameValueCollection()
        {
            NameValueCollection nvc = base.DataAsNameValueCollection();
            nvc["minNumberOfPosts"] = MinimiumNumberOfPosts.ToString();
            nvc["maxNumberOfTags"] = MaxNumberOfTags.ToString();

            return nvc;
        }

        public override StatusType SetValues(System.Web.HttpContext context, NameValueCollection nvc)
        {
            base.SetValues(context, nvc);
            MinimiumNumberOfPosts = Int32.Parse(nvc["minNumberOfPosts"]);
            MaxNumberOfTags = Int32.Parse(nvc["maxNumberOfTags"]);
            return StatusType.Success;
        }

        public override string FormName
        {
            get
            {
                return "Popular Post Configuration";
            }
        }

        public int MaxNumberOfTags = 25;

        public int MinimiumNumberOfPosts = 5;
    }
}