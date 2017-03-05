using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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

        public string[] AcceptTypes { get; set; }
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

        public FakeHttpRequest(string url)
        {
            Content = new FakeHttpRequestContent();
            Method = HttpMethod.Get;
            Url = new Uri(url);
        }

        public FakeHttpRequest()
            : this("http://localhost:8080/")
        {
        }

        public X509Certificate2 GetClientCertificate()
        {
            throw new NotImplementedException();
        }

        public Task<X509Certificate2> GetClientCertificateAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class FakeHttpRequestContent : IHttpRequestContent
    {
        public Stream Stream { get; set; }

        public byte[] ReadArray()
        {
            throw new NotImplementedException();
        }

        public NameValueCollection ReadParameters()
        {
            throw new NotImplementedException();
        }

        public string ReadText()
        {
            throw new NotImplementedException();
        }

#if FEATURE_ASYNC
        public Task<byte[]> ReadArrayAsync()
        {
            throw new NotImplementedException();
        }

        public Task<NameValueCollection> ReadParametersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> ReadTextAsync()
        {
            throw new NotImplementedException();
        }
#endif
    }
}
