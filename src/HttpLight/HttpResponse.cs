using System.Net;

namespace HttpLight
{
    /// <summary>
    /// An <see cref="HttpListenerResponse"/> wrapper
    /// </summary>
    public class HttpResponse
    {
        private HttpListenerResponse _innerResponse;

        public HttpStatusCode StatusCode
        {
            get { return (HttpStatusCode) _innerResponse.StatusCode; }
            set { _innerResponse.StatusCode = (int) value; }
        }

        public HttpResponse(HttpListenerResponse innerResponse)
        {
            _innerResponse = innerResponse;
        }
    }
}
