using System;

namespace Graffiti.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ChalkAttribute : Attribute
    {
        //private bool _RequestOnly;

        //public bool RequestOnly
        //{
        //    get { return _RequestOnly; }
        //    set { _RequestOnly = value; }
        //}

        private string _key = null;

        public string Key
        {
            get { return _key; }
        }

        public ChalkAttribute(string key)
        {
            _key = key;
        }
        
    }
}