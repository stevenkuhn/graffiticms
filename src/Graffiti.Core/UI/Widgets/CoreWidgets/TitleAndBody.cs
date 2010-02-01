using System.Collections.Specialized;

namespace Graffiti.Core
{
    /// <summary>
    /// A widget which displays a title and the a block of text
    /// </summary>
    [WidgetInfo("e7abe913-0de7-45a5-9cc5-6c6c5809e808", "Title and Body", "Represents a title and body")]
    public class TitleAndBody : Widget
    {

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                    return "An empty box";
                else
                    return Title + " (Title and Body widget)";
            }
        }

        public string HTML;

        protected override System.Collections.Specialized.NameValueCollection DataAsNameValueCollection()
        {
            NameValueCollection nvc = base.DataAsNameValueCollection();
            nvc["HTML"] = HTML;

            return nvc;
        }

        public override StatusType SetValues(System.Web.HttpContext context, NameValueCollection nvc)
        {
            base.SetValues(context, nvc);
            HTML = nvc["HTML"];
            return StatusType.Success;
        }

        protected override FormElementCollection AddFormElements()
        {
            FormElementCollection fec = new FormElementCollection();

            fec.Add(new TextFormElement("Title", "Title", "The title of the section"));
            fec.Add(new WYWIWYGFormElement("HTML", "Your Content", null));
            return fec;
        }

        public override string RenderData()
        {
            return HTML;
        }
    }
}