using System;
using System.Collections.Specialized;
using System.IO;
#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif
using System.Web;

namespace HttpLight.Test.Utils
{
    internal class FakeHttpRequest : IHttpRequest
    {
        private Uri _url;
        private NameValueCollection _urlParameters;

        public IHttpRequestBody Body { get; }

        public HttpMethod Method { get; set; }

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
            Body = new FakeHttpRequestBody();
            Method = HttpMethod.Get;
            Url = new Uri(url);
        }

        public FakeHttpRequest()
            : this("http://localhost:8080/")
        {
        }
    }

    internal class FakeHttpRequestBody : IHttpRequestBody
    {
        public bool HasBody { get; set; }
        public Stream Stream { get; set; }

        public string ReadText()
        {
            throw new NotImplementedException();
        }

        public byte[] ReadArray()
        {
            throw new NotImplementedException();
        }

#if FEATURE_ASYNC
        public Task<string> ReadTextAsync()
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadArrayAsync()
        {
            throw new NotImplementedException();
        }
#endif
    }
}
