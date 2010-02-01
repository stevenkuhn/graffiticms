using System.Collections.Specialized;
using System.Text;

namespace Graffiti.Core
{
    [WidgetInfo("542463b6-8816-44e0-9e08-14b00dbdfa1d","Search Box","A simple widget for collection search queries")]
    public class SearchWidget : Widget
    {
        public override string RenderData()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<form method=\"get\" id=\"search_form\" action=\"{0}\">\n", new Urls().Search);
            sb.Append("<input type=\"text\" class=\"search_input\" value=\"Search this site\" name=\"q\" id=\"q\" onfocus=\"if (this.value == 'Search this site') {this.value = '';}\" onblur=\"if (this.value == '') {this.value = 'Search this site';}\" />\n");
            sb.Append("<input type=\"hidden\" id=\"searchsubmit\" value=\"Search\" />\n");
            sb.Append("</form>");

            return sb.ToString();
        }

        public override string Name
        {
            get { return "Search Box"; }
        }

        public override string Title
        {
            get
            {
                if (string.IsNullOrEmpty(base.Title))
                    base.Title = "Search";

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
            fec.Add(new TextFormElement("title", "Search Text", "Text for the search box title"));
            return fec;
        }
    }
}