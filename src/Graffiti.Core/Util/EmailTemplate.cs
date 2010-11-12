
namespace Graffiti.Core
{

    public class EmailTemplate
    {
 
        private string  _templateName;

        public string  TemplateName
        {
            get { return _templateName; }
            set { _templateName = value; }
        }

        private string _subject;

        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        private string _to;

        public string To
        {
            get { return _to; }
            set { _to = value; }
        }

        private string _from;

        public string From
        {
            get { return _from; }
            set { _from = value; }
        }

        private string _replyto;

        public string ReplyTo
        {
            get { return _replyto; }
            set { _replyto = value; }
        }

        private EmailTemplateToolboxContext _context;

        public EmailTemplateToolboxContext Context
        {
            get { return _context; }
            set{ _context = value;}
        }

        private bool _isHtml = true;

        public bool IsHTML
        {
            get { return _isHtml; }
            set { _isHtml = value; }
        }
	
	
    }
}
