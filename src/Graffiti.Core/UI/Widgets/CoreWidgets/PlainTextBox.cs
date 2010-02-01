using System;
using System.Collections.Specialized;

namespace Graffiti.Core
{
    /// <summary>
    /// A widget which displays a title and the a block of text
    /// </summary>
    [WidgetInfo("34e0b61b-8a66-408b-9fa2-71672132c583", "An Empty Box", "Represents a box")]
    public class PlainTextBox : Widget
    {

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                    return "An Empty Box";
                else
                    return Title + " (plain text widget)";
            }
        }

        public string Text;

        protected override NameValueCollection DataAsNameValueCollection()
        {
            NameValueCollection nvc = base.DataAsNameValueCollection();
            nvc["Text"] = Text;

            return nvc;
        }

        public override StatusType SetValues(System.Web.HttpContext context, NameValueCollection nvc)
        {
            base.SetValues(context, nvc);
            Text = nvc["Text"];
            return StatusType.Success;
        }

        protected override FormElementCollection AddFormElements()
        {
            FormElementCollection fec = new FormElementCollection();
            fec.Add(new TextFormElement("Title", "Description", "This field will not be displayed on your site."));
            fec.Add(
                new TextAreaFormElement("Text", "Content",
                                        "Any content you enter in this field will be displayed on your site's sidebar. (HTML is allowed)",
                                        10));

            return fec;
        }

        public override string Render(string beforeTitle, string afterTitle, string beforeContent, string afterContent)
        {
            return beforeContent + RenderData() + afterContent;
        }

        public override string RenderData()
        {
            return Text;
        }
    }
}