using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif

namespace HttpLight.Test.Utils
{
    internal class FakeHttpRequest : IHttpRequest
    {
        private const string DefaultBaseUrl = "http://localhost:8080/";

        private Uri _url;
        private NameValueCollection _urlParameters;

        public string[] AcceptTypes { get; set; }
        public IDictionary<string, object> Bag { get; set; }
        public int ClientCertificateError { get; set; }
        public IHttpRequestContent Content { get; set; }
        public Encoding ContentEncoding { get; set; }
        public long? ContentLength { get; set; }
        public string ContentType { get; set; }
        public CookieCollection Cookies { get; set; }
        public bool HasContent { get; set; }
        public NameValueCollection Headers { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool IsLocal { get; set; }
        public bool IsSecureConnection { get; set; }
        public bool IsWebSocketRequest { get; set; }
        public bool KeepAlive { get; set; }
        public IPEndPoint LocalEndPoint { get; set; }
        public HttpMethod Method { get; set; }
        public Version ProtocolVersion { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
        public Guid RequestTraceIdentifier { get; set; }
        public string ServiceName { get; set; }
        public TransportContext TransportContext { get; set; }

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

        public Uri UrlReferrer { get; set; }
        public string UserAgent { get; set; }
        public string[] UserLanguages { get; set; }

        public FakeHttpRequest()
        {
        }

        public FakeHttpRequest(string path)
            : this(path, HttpMethod.Get)
        {
        }

        public FakeHttpRequest(string path, HttpMethod method)
            : this(DefaultBaseUrl, path, method)
        {
        }

        public FakeHttpRequest(string baseUrl, string path, HttpMethod method)
        {
            baseUrl = GetBaseUrl(baseUrl);
            var url = GetUrl(baseUrl, path);
            Content = new FakeHttpRequestContent();
            Method = method;
            Url = new Uri(url);
            Bag = new Dictionary<string, object>();
        }

        public X509Certificate2 GetClientCertificate()
        {
            throw new NotImplementedException();
        }

#if FEATURE_ASYNC
        public Task<X509Certificate2> GetClientCertificateAsync()
        {
            throw new NotImplementedException();
        }
#endif

        private static string GetBaseUrl(string baseUrl)
        {
            baseUrl = baseUrl ?? DefaultBaseUrl;
            if (!baseUrl.EndsWith("/"))
                baseUrl = baseUrl + "/";
            return baseUrl;
        }

        private static string GetUrl(string baseUrl, string path)
        {
            path = path ?? string.Empty;
            if (path.StartsWith("/"))
                path = path.Substring(1);
            return baseUrl + path;
        }
    }

    internal class FakeHttpRequestContent : IHttpRequestContent
    {
        public Stream Stream { get; set; }
        public NameValueCollection ContentParameters { get; set; }
        public string RawContent { get; set; }

        public FakeHttpRequestContent()
            : this(string.Empty)
        {
        }

        public FakeHttpRequestContent(Stream stream)
        {
            Stream = stream;
        }

        public FakeHttpRequestContent(string text)
            : this(new MemoryStream(Encoding.UTF8.GetBytes(text)))
        {
            RawContent = text;
            ContentParameters = HttpUtility.ParseQueryString(text);
        }
    }
}
