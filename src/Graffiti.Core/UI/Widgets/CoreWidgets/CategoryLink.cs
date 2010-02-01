using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Graffiti.Core
{
    //BETTER NAME!

    /// <summary>
    /// Responsible for rendering/managing a set of links with a title
    /// </summary>
    [WidgetInfo("4ddd7cb7-a867-4812-b60a-605e12dc695d", "List of Links", "Represents a list of links for your site's sidebar")]
    public class CategoryLink : Widget
    {
        [Serializable]
        public class Link
        {
            public string Text;
            public string Href;
        }

        public Link[] Links = new Link[0];

        public string LinkData = null;

        public override string RenderData()
        {
            StringBuilder sb = new StringBuilder("<ul>");

            foreach(Link link in Links)
            {
                sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>\n", link.Href,link.Text);
            }

            sb.Append("</ul>\n");

            return sb.ToString();
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                    return "List of Links";
                else
                    return Title + " (" + Links.Length + " links)";
            }
        }

        public override string FormName
        {
            get
            {
                return "List of Links Editor";
            }
        }

        protected override  FormElementCollection AddFormElements()
        {
            FormElementCollection fec = new FormElementCollection();
            fec.Add(AddTitleElement());
            fec.Add(new TextAreaFormElement("LinkData", "Links", "Add one link per line using the format Text | Link", 10));
            return fec;
        }

        protected override NameValueCollection DataAsNameValueCollection()
        {
            NameValueCollection nvc = base.DataAsNameValueCollection();
            nvc["LinkData"] = LinkData;
            return nvc;
        }

        public override StatusType SetValues(System.Web.HttpContext context, NameValueCollection nvc)
        {
            base.SetValues(context, nvc);

            LinkData = nvc["LinkData"];

            string[] lines = string.IsNullOrEmpty(LinkData)
                                 ? new string[0]
                                 : LinkData.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            List<Link> links = new List<Link>();
            foreach (string line in lines)
            {
                string[] linkInfo = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                
                if (linkInfo != null && linkInfo.Length > 1)
                {
                    Link link = new Link();
                    link.Text = linkInfo[0].Trim();
                    link.Href = linkInfo[1].Trim();
                    links.Add(link);
                }
            }

            Links = links.ToArray();

            return StatusType.Success;

        }

    }
}