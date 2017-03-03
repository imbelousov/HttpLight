using System;
using System.Collections.Specialized;
using System.Net;
using HttpLight.Utils;

namespace HttpLight
{
    /// <summary>
    /// An <see cref="HttpListenerRequest"/> wrapper
    /// </summary>
    public class HttpRequest : IHttpRequest
    {
        private HttpListenerRequest _innerRequest;
        private HttpMethod _httpMethod;

        public HttpMethod HttpMethod
        {
            get { return _httpMethod; }
        }

        public Uri Url
        {
            get { return _innerRequest.Url; }
        }

        public NameValueCollection UrlParameters
        {
            get { return _innerRequest.QueryString; }
        }

        public HttpRequest(HttpListenerRequest innerRequest)
        {
            _innerRequest = innerRequest;
            _httpMethod = HttpMethodHelper.Convert(innerRequest.HttpMethod);
        }
    }

    public interface IHttpRequest
    {
        HttpMethod HttpMethod { get; }
        Uri Url { get; }
        NameValueCollection UrlParameters { get; }
    }
}
