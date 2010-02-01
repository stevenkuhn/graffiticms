using System;

namespace Graffiti.Core.API
{
    public class RESTConflict : ApplicationException
    {
        public RESTConflict(string message):base(message)
        {
            
        }
    }
}