using System;
using System.Net;
using System.Text;

namespace HttpLight
{
    /// <summary>
    /// An <see cref="HttpListenerResponse"/> wrapper
    /// </summary>
    internal class HttpResponse : IHttpResponse
    {
        private Exception _exception;
        private HttpListenerResponse _innerResponse;

        public Encoding ContentEncoding
        {
            get { return _innerResponse.ContentEncoding; }
            set { _innerResponse.ContentEncoding = value; }
        }

        public long? ContentLength
        {
            get { return _innerResponse.ContentLength64 >= 0 ? (long?) _innerResponse.ContentLength64 : null; }
            set { _innerResponse.ContentLength64 = value.HasValue ? value.Value : -1; }
        }

        public string ContentType
        {
            get { return _innerResponse.ContentType; }
            set { _innerResponse.ContentType = value; }
        }

        public CookieCollection Cookies
        {
            get { return _innerResponse.Cookies; }
            set { _innerResponse.Cookies = value; }
        }

        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }

        public WebHeaderCollection Headers
        {
            get { return _innerResponse.Headers; }
            set { _innerResponse.Headers = value; }
        }

        public bool KeepAlive
        {
            get { return _innerResponse.KeepAlive; }
            set { _innerResponse.KeepAlive = value; }
        }

        public Version ProtocolVersion
        {
            get { return _innerResponse.ProtocolVersion; }
            set { _innerResponse.ProtocolVersion = value; }
        }

        public bool SendChunked
        {
            get { return _innerResponse.SendChunked; }
            set { _innerResponse.SendChunked = value; }
        }

        public HttpStatusCode StatusCode
        {
            get { return (HttpStatusCode) _innerResponse.StatusCode; }
            set { _innerResponse.StatusCode = (int) value; }
        }

        public string StatusDescription
        {
            get { return _innerResponse.StatusDescription; }
            set { _innerResponse.StatusDescription = value; }
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
