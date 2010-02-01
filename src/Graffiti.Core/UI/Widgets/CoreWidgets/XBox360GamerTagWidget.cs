using System.Collections.Specialized;

namespace Graffiti.Core
{
    /// <summary>
    /// A widget which displays a 360 gamer tag.
    /// </summary>
    [WidgetInfo("464d3aaa-2745-4386-8b28-b6c3dc781064", "XBox 360 GamerTag", "Adds a gamer tag to your sidebar")]
    public class XBox360GamerTagWidget : Widget
    {
        public string TagName;

        //Since we are not using the Title, we need to override Render and remove it.
        public override string Render(string beforeTitle, string afterTitle, string beforeContent, string afterContent)
        {
            return beforeContent + RenderData() + afterContent;
        }

        //All we need to do is return an IFrame using out gamer tag.
        public override string RenderData()
        {
            return
                string.Format(
                    "<p><iframe src=\"http://gamercard.xbox.com/{0}.card\" scrolling=\"no\" frameborder=\"0\" height=\"140\" width=\"204\"></iframe></p>",
                    TagName);
        }

        public override string Name
        {
            get
            {
                return "XBox 360 GamerTag" + (!string.IsNullOrEmpty(TagName) ? " (" + TagName + ")" : string.Empty);
            }
        }

        protected override System.Collections.Specialized.NameValueCollection DataAsNameValueCollection()
        {
            NameValueCollection nvc = base.DataAsNameValueCollection();
            nvc["tagname"] = TagName;
            return nvc;
        }

        public override StatusType SetValues(System.Web.HttpContext context, NameValueCollection nvc)
        {
            base.SetValues(context, nvc);
            if(string.IsNullOrEmpty(nvc["tagname"]))
            {
                SetMessage(context, "You did not include a GamerTag name.");
                return StatusType.Error;
            }

            TagName = nvc["tagname"];
            return StatusType.Success;
        }

        //We need to override this to present the user with a useful form. By default
        //this method would have rendered the title and data element. The data would have been a textarea
        //which would have worked, but would have been overkill.
        protected override FormElementCollection AddFormElements()
        {
            FormElementCollection fec = new FormElementCollection();
            fec.Add(new TextFormElement("tagname", "Gamer Tag", "Please enter your XBox 360 Gamer Tag"));
            return fec;
        }
    }
}