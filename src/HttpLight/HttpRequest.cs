using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif
using HttpLight.Utils;

namespace HttpLight
{
    /// <summary>
    /// An <see cref="HttpListenerRequest"/> wrapper
    /// </summary>
    public class HttpRequest : IHttpRequest
    {
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        private HttpListenerRequest _innerRequest;
        private HttpMethod _method;
        private IHttpRequestBody _body;

        public IHttpRequestBody Body
        {
            get { return _body; }
        }

        public HttpMethod Method
        {
            get { return _method; }
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
            _body = new HttpRequestBody(innerRequest.InputStream, innerRequest.HasEntityBody, innerRequest.ContentEncoding ?? DefaultEncoding);
            _method = HttpMethodHelper.Convert(innerRequest.HttpMethod);
        }
    }

    public class HttpRequestBody : IHttpRequestBody
    {
        private Stream _stream;
        private bool _hasBody;
        private Encoding _encoding;

        public bool HasBody
        {
            get { return _hasBody; }
        }

        public Stream Stream
        {
            get { return _stream; }
        }

        internal HttpRequestBody(Stream stream, bool hasBody, Encoding encoding)
        {
            _stream = stream;
            _hasBody = hasBody;
            _encoding = encoding;
        }

        public string ReadText()
        {
            var buf = ReadArray();
            return _encoding.GetString(buf);
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

#if FEATURE_ASYNC
        public async Task<string> ReadTextAsync()
        {
            var buf = await ReadArrayAsync();
            return _encoding.GetString(buf);
        }

        public async Task<byte[]> ReadArrayAsync()
        {
            using (var ms = new MemoryStream())
            {
                await _stream.CopyToAsync(ms);
                ms.Position = 0;
                return ms.ToArray();
            }
        }
#endif
    }

    public interface IHttpRequest
    {
        /// <summary>
        /// Payload
        /// </summary>
        IHttpRequestBody Body { get; }

        /// <summary>
        /// HTTP method
        /// </summary>
        HttpMethod Method { get; }

        /// <summary>
        /// Requested URL
        /// </summary>
        Uri Url { get; }

        /// <summary>
        /// Parameters inside URL
        /// </summary>
        NameValueCollection UrlParameters { get; }
    }

    public interface IHttpRequestBody
    {
        bool HasBody { get; }
        Stream Stream { get; }
        string ReadText();
        byte[] ReadArray();
#if FEATURE_ASYNC
        Task<string> ReadTextAsync();
        Task<byte[]> ReadArrayAsync();
#endif
    }
}
