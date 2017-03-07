using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using HttpLight.Utils;

#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif

namespace HttpLight
{
    /// <summary>
    /// An <see cref="HttpListenerRequest"/> wrapper
    /// </summary>
    internal class HttpRequest : IHttpRequest
    {
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        private IDictionary<string, object> _bag;
        private IHttpRequestContent _content;
        private HttpListenerRequest _innerRequest;
        private HttpMethod _method;

        public string[] AcceptTypes
        {
            get { return _innerRequest.AcceptTypes; }
        }

        public IDictionary<string, object> Bag
        {
            get { return _bag; }
        }

        public int ClientCertificateError
        {
            get { return _innerRequest.ClientCertificateError; }
        }

        public IHttpRequestContent Content
        {
            get { return _content; }
        }

        public Encoding ContentEncoding
        {
            get { return _innerRequest.ContentEncoding; }
        }

        public long? ContentLength
        {
            get { return _innerRequest.ContentLength64 >= 0 ? (long?) _innerRequest.ContentLength64 : null; }
        }

        public string ContentType
        {
            get { return _innerRequest.ContentType; }
        }

        public CookieCollection Cookies
        {
            get { return _innerRequest.Cookies; }
        }

        public bool HasContent
        {
            get { return _innerRequest.HasEntityBody; }
        }

        public NameValueCollection Headers
        {
            get { return _innerRequest.Headers; }
        }

        public bool IsAuthenticated
        {
            get { return _innerRequest.IsAuthenticated; }
        }

        public bool IsLocal
        {
            get { return _innerRequest.IsLocal; }
        }

        public bool IsSecureConnection
        {
            get { return _innerRequest.IsSecureConnection; }
        }

        public bool IsWebSocketRequest
        {
            get
            {
#if FEATURE_WEBSOCKET
                return _innerRequest.IsWebSocketRequest;
#else
                return false;
#endif
            }
        }

        public bool KeepAlive
        {
            get { return _innerRequest.KeepAlive; }
        }

        public IPEndPoint LocalEndPoint
        {
            get { return _innerRequest.LocalEndPoint; }
        }

        public HttpMethod Method
        {
            get { return _method; }
        }

        public Version ProtocolVersion
        {
            get { return _innerRequest.ProtocolVersion; }
        }

        public IPEndPoint RemoteEndPoint
        {
            get { return _innerRequest.RemoteEndPoint; }
        }

        public Guid RequestTraceIdentifier
        {
            get { return _innerRequest.RequestTraceIdentifier; }
        }

        public string ServiceName
        {
            get { return _innerRequest.ServiceName; }
        }

        public TransportContext TransportContext
        {
            get { return _innerRequest.TransportContext; }
        }

        public Uri Url
        {
            get { return _innerRequest.Url; }
        }

        public NameValueCollection UrlParameters
        {
            get { return _innerRequest.QueryString; }
        }

        public Uri UrlReferrer
        {
            get { return _innerRequest.UrlReferrer; }
        }

        public string UserAgent
        {
            get { return _innerRequest.UserAgent; }
        }

        public string[] UserLanguages
        {
            get { return _innerRequest.UserLanguages; }
        }

        public HttpRequest(HttpListenerRequest innerRequest)
        {
            _innerRequest = innerRequest;
            _content = new HttpRequestContent(innerRequest.InputStream, ContentEncoding ?? DefaultEncoding);
            _method = HttpMethodHelper.Convert(innerRequest.HttpMethod);
            _bag = new Dictionary<string, object>();
        }

        public X509Certificate2 GetClientCertificate()
        {
            return _innerRequest.GetClientCertificate();
        }

#if FEATURE_ASYNC
        public Task<X509Certificate2> GetClientCertificateAsync()
        {
            return _innerRequest.GetClientCertificateAsync();
        }
#endif
    }

    public class HttpRequestContent : IHttpRequestContent
    {
        private Encoding _encoding;
        private Stream _stream;

        public Stream Stream
        {
            get { return _stream; }
        }

        internal HttpRequestContent(Stream stream, Encoding encoding)
        {
            _stream = stream;
            _encoding = encoding;
        }

        public byte[] ReadArray()
        {
            using (var ms = new MemoryStream())
            {
                _stream.CopyTo(ms);
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        public NameValueCollection ReadParameters()
        {
            var text = ReadText();
            return HttpUtility.ParseQueryString(text, _encoding);
        }

        public string ReadText()
        {
            var buf = ReadArray();
            return _encoding.GetString(buf);
        }

#if FEATURE_ASYNC
        public async Task<byte[]> ReadArrayAsync()
        {
            using (var ms = new MemoryStream())
            {
                await _stream.CopyToAsync(ms);
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        public async Task<NameValueCollection> ReadParametersAsync()
        {
            var text = await ReadTextAsync();
            return HttpUtility.ParseQueryString(text, _encoding);
        }

        public async Task<string> ReadTextAsync()
        {
            var buf = await ReadArrayAsync();
            return _encoding.GetString(buf);
        }
#endif
    }
}
