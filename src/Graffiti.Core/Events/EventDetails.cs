using System;
using System.Xml.Serialization;

namespace Graffiti.Core
{
    /// <summary>
    /// Represents an Event, manages status, etc.
    /// </summary>
    [Serializable]
    public class EventDetails
    {
        private string  _eventType;

        public string  EventType
        {
            get { return _eventType; }
            set { _eventType = value; }
        }

        private bool _enabled = false;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        private GraffitiEvent _event;
        [XmlIgnore]
        public GraffitiEvent Event
        {
            get { return _event; }
            set { _event = value; }
        }

        private string  _xml;

        public string  Xml
        {
            get { return _xml; }
            set { _xml = value; }
        }
	
	
	
    }
}