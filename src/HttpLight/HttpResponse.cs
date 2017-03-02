using System;
using System.Net;
using System.Text;

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

        public Encoding ContentEncoding
        {
            get { return _innerResponse.ContentEncoding; }
            set { _innerResponse.ContentEncoding = ContentEncoding; }
        }

        public string ContentType
        {
            get { return _innerResponse.ContentType; }
            set { _innerResponse.ContentType = value; }
        }

        public long ContentLength
        {
            get { return _innerResponse.ContentLength64; }
            set { _innerResponse.ContentLength64 = value; }
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
