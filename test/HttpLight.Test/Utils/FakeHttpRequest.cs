using System;
using System.Collections.Specialized;
using System.Web;

namespace HttpLight.Test.Utils
{
    internal class FakeHttpRequest : IHttpRequest
    {
        private Uri _url;
        private NameValueCollection _urlParameters;

        public HttpMethod HttpMethod { get; set; }

        public Uri Url
        {
            get { return _url; }
            set
            {
                _url = value;
                _urlParameters = HttpUtility.ParseQueryString(_url.Query);
            }
        }

        public NameValueCollection UrlParameters
        {
            get { return _urlParameters; }
            set { _urlParameters = value; }
        }

        public FakeHttpRequest(string url)
        {
            HttpMethod = HttpMethod.Get;
            Url = new Uri(url);
        }

        public FakeHttpRequest()
            : this("http://localhost:8080/path?param=value")
        {
        }
    }
}
