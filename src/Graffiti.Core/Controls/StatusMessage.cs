using System.Web;
using System.Web.UI;

namespace Graffiti.Core
{

    /// <summary>
    /// Summary description for StatusMessage
    /// </summary>
    public class StatusMessage : Control
    {
        private const string JavaScriptRollup = @"
        <script language='javascript'>

        function rollUp(nodeId)
        {
	        var node = document.getElementById(nodeId);
	        if (node)
	        {
		        node._rollUpLastHeight = node.offsetHeight;
		        node._rollUpStepY = node._rollUpLastHeight / 15;  
                node._rollUpTimeoutHandle = window.setTimeout(new Function('rollUpStep(\'' + nodeId + '\');'), 1);
                node._marginLast = 1;
                node._paddingLast = 9;
	        }
        }

        function rollUpStep(nodeId)
        {        
	        var node = document.getElementById(nodeId);

	        if (node)
	        {
		        window.clearTimeout(node._rollUpTimeoutHandle);
		        node._rollUpLastHeight -= node._rollUpStepY;

                if(node._marginLast > -10)
                {
                    node._marginLast -= 1;
                    node.style.marginBottom = node._marginLast + 'px';
                }

                if(node._paddingLast > 0)
                {
                    node._paddingLast -= 1;
                    node.style.paddingBottom = node._paddingLast + 'px';
                    node.style.paddingTop = node._paddingLast + 'px';
                }

		        if (node._rollUpLastHeight > 0)
		        {
                    node.style.overflow = 'hidden';
                    node.style.height = parseInt(node._rollUpLastHeight - node._paddingLast) + 'px';

                    node._rollUpTimeoutHandle = window.setTimeout(new Function('rollUpStep(\'' + nodeId + '\');'), .1);
		        }
		        else
                {   
                    node.style.display = 'none';
                }
	        }
        }

        </script>";

        private StatusType _messageType = StatusType.Unknown;
        public StatusType Type
        {
            get { return _messageType; }
            set { _messageType = value; }
        }

        private string _message;

        public string Text
        {
            get { return _message; }
            set { _message = value; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.WriteLine(JavaScriptRollup);
            if (Text != null && Type != StatusType.Unknown)
            {
                string img = null;
                string cls = null;

                switch (Type)
                {
                    case StatusType.Success:
                        img = VirtualPathUtility.ToAbsolute("~/graffiti-admin/common/img/success.png");
                        cls = "success";
                        writer.WriteLine("<script language=\"javascript\">setTimeout(\"rollUp('statusmessage')\", 5000);</script>");
                        break;
                    case StatusType.Error:
                        img = VirtualPathUtility.ToAbsolute("~/graffiti-admin/common/img/error.png");
                        cls = "error";
                        break;
                    case StatusType.Warning:
                        img = VirtualPathUtility.ToAbsolute("~/graffiti-admin/common/img/warning.png");
                        cls = "warning";
                        break;
                    case StatusType.Information:
                        img = VirtualPathUtility.ToAbsolute("~/graffiti-admin/common/img/information.png");
                        cls = "information";
                        break;
                    default:
                        break;
                }

                writer.WriteLine("<div id=\"statusmessage\" class=\"{0}\">", cls);
                writer.WriteLine("<table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tr>");
                writer.WriteLine(
                    "<td style=\"padding-right: 8px;\"><img src=\"{0}\" /></td><td width=\"100%\">{1}</td>", img,
                    Text);
                writer.WriteLine("</tr></table></div>");
            }
        }
    }

    public enum StatusType
    {
        Success,
        Error,
        Warning,
        Information,
        Unknown
    }

}