using System.Collections.Specialized;

namespace Graffiti.Core
{
    [WidgetInfo("f7aca00e-5b77-4bac-b0d7-b626cf1051cb","Chalk Widget", "Parsers a block of Chalk and renders it as a widget")]
    public class ChalkWidget : Widget
    {

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                    return "Chalk Widget";
                else
                    return Title + " (Chalk widget)";
            }
        }

        public string Chalk;

        protected override FormElementCollection AddFormElements()
        {
            FormElementCollection fec = new FormElementCollection();
            fec.Add(new TextFormElement("Title", "Title", "The title of the section"));
            fec.Add(new TextAreaFormElement("Chalk", "Your Chalk Script", null, 15));
            return fec;
        }

        public override StatusType SetValues(System.Web.HttpContext context, NameValueCollection nvc)
        {
            base.SetValues(context, nvc);
            Chalk = nvc["Chalk"];
            return StatusType.Success;
        }

        protected override NameValueCollection DataAsNameValueCollection()
        {
            NameValueCollection nvc = base.DataAsNameValueCollection();
            nvc["Chalk"] = Chalk;
            return nvc;
        }

        public override string RenderData()
        {
            if(!string.IsNullOrEmpty(Chalk))
            {
                return TemplateEngine.Evaluate(Chalk, GraffitiContext.Current);
            }

            return string.Empty;
        }
    }
}