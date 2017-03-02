using System;
using System.Net;

namespace HttpLight
{
    /// <summary>
    /// An <see cref="HttpListenerResponse"/> wrapper
    /// </summary>
    public class HttpResponse
    {
        private HttpListenerResponse _innerResponse;
        private Exception _exception;

        public HttpStatusCode StatusCode
        {
            get { return (HttpStatusCode) _innerResponse.StatusCode; }
            set { _innerResponse.StatusCode = (int) value; }
        }

        public Exception Exception
        {
            get { return _exception; }
            internal set { _exception = value; }
        }

        internal HttpListenerResponse InnerResponse
        {
            get { return _innerResponse; }
        }

        public HttpResponse(HttpListenerResponse innerResponse)
        {
            _innerResponse = innerResponse;
        }
    }
}
