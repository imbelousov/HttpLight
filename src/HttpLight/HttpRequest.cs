﻿using System;
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

        public string Method
        {
            get { return _innerRequest.HttpMethod; }
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
        private NameValueCollection _contentParameters;
        private string _rawContent;

        public Stream Stream
        {
            get { return _stream; }
        }

        public NameValueCollection ContentParameters
        {
            get
            {
                if (_contentParameters != null)
                    return _contentParameters;
                _contentParameters = HttpUtility.ParseQueryString(RawContent, _encoding);
                return _contentParameters;
            }
        }

        public string RawContent
        {
            get
            {
                if (_rawContent != null)
                    return _rawContent;
                _rawContent = ReadAsText();
                return _rawContent;
            }
        }

        internal HttpRequestContent(Stream stream, Encoding encoding)
        {
            _stream = stream;
            _encoding = encoding;
        }

        private string ReadAsText()
        {
            using (var ms = new MemoryStream())
            {
                _stream.CopyTo(ms);
                ms.Position = 0;
                return _encoding.GetString(ms.ToArray());
            }
        }
    }
}
