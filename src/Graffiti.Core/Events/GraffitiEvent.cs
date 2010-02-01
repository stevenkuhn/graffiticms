using System;
using System.Collections.Specialized;
using System.Web;

namespace Graffiti.Core
{
    /// <summary>
    /// Graffiti Events allow plugins to be executed a key points in the Graffiti lifecycle. 
    /// 
    /// Events can be configured using dynmaically built forms.
    /// </summary>
    [Serializable]
    public abstract class GraffitiEvent : EditableForm
    {
        public abstract void Init(GraffitiApplication ga);

        public virtual bool IsEditable
        {
            get{ return false;}
        }

        public override  string Name
        {
            get { return GetType().FullName; }
        }

        public virtual string Description
        {
            get { return string.Empty; }
        }

        protected override FormElementCollection AddFormElements()
        {
            return new FormElementCollection();
        }

        public override StatusType SetValues(HttpContext context, NameValueCollection nvc)
        {
            return StatusType.Success;
        }

        public virtual void EventEnabled()
        {

        }

        public virtual void EventDisabled()
        {

        }
    
    }
}