using System;

namespace GraffitiClient.API
{
    public class GraffitiServiceException : ApplicationException
    {
        public int StatusCode = 200; 

        public GraffitiServiceException(string message): base(message)
        {
            
        }

        public GraffitiServiceException(string message, int statusCode): base(message)
        {
            StatusCode = statusCode;
        }
    }
}