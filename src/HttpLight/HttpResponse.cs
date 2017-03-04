using System;
using System.Net;
using System.Text;

namespace HttpLight
{
    /// <summary>
    /// An <see cref="HttpListenerResponse"/> wrapper
    /// </summary>
    public class HttpResponse : IHttpResponse
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
            set { _exception = value; }
        }

        public Encoding ContentEncoding
        {
            get { return _innerResponse.ContentEncoding; }
            set { _innerResponse.ContentEncoding = value; }
        }

        public string ContentType
        {
            get { return _innerResponse.ContentType; }
            set { _innerResponse.ContentType = value; }
        }

        public long? ContentLength
        {
            get
            {
                return _innerResponse.ContentLength64 >= 0
                    ? (long?) _innerResponse.ContentLength64
                    : null;
            }
            set
            {
                _innerResponse.ContentLength64 = value.HasValue
                    ? value.Value
                    : -1;
            }
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

    public interface IHttpResponse
    {
        /// <summary>
        /// HTTP status code
        /// </summary>
        HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Instance of occured exception during action invocation
        /// </summary>
        Exception Exception { get; set; }

        /// <summary>
        /// Content encoding
        /// </summary>
        Encoding ContentEncoding { get; set; }

        /// <summary>
        /// Content type
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Content length
        /// </summary>
        long? ContentLength { get; set; }
    }
}
