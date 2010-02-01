using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Graffiti.Core
{
    [Serializable]
    public abstract class EditableForm
    {
        public virtual string EditUrl
        {
            get { return "edit.aspx"; }
        }

        /// <summary>
        /// This is the name displayed on the list page
        /// </summary>
        public abstract string Name { get;}

        /// <summary>
        /// An optional setting which can be overriden to set the title on the edit page
        /// </summary>
        public virtual string FormName { get { return Name; } }

        protected virtual NameValueCollection DataAsNameValueCollection()
        {
            return new NameValueCollection();
        }

        /// <summary>
        /// Called to build the form on postback. This way, we can maintain state.
        /// </summary>
        /// <param name="nvc"></param>
        /// <returns></returns>
        public virtual string BuildForm(NameValueCollection nvc)
        {
            FormElementCollection fec = AddFormElements();
            StringBuilder sb = new StringBuilder();

            if (fec != null)
            {
                foreach (FormElement fe in fec)
                {
                    fe.Write(sb, nvc);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Called on Page.IsPostBack is false.
        /// </summary>
        /// <returns></returns>
        public virtual string BuildForm()
        {
            return BuildForm(DataAsNameValueCollection());
        }

        protected abstract FormElementCollection AddFormElements();

        public abstract StatusType SetValues(HttpContext context, NameValueCollection nvc);

        /// <summary>
        /// Helper method to set a message to be displayed after a user attempts to save the edit form.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        protected static void SetMessage(HttpContext context, string message)
        {
            context.Items["PostType-Status-Message"] = message;
        }

        /// <summary>
        /// Get the message set via SetMessage
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetMessage(HttpContext context)
        {
            return context.Items["PostType-Status-Message"] as string;
        }

    }
}