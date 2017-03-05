using System;
using System.IO;
using System.Net;
using System.Text;

namespace HttpLight.Test.Utils
{
    internal class FakeHttpResponse : IHttpResponse
    {
        public Encoding ContentEncoding { get; set; }
        public long? ContentLength { get; set; }
        public string ContentType { get; set; }
        public CookieCollection Cookies { get; set; }
        public Exception Exception { get; set; }
        public WebHeaderCollection Headers { get; set; }
        public bool KeepAlive { get; set; }
        public Stream OutputStream { get; set; }
        public Version ProtocolVersion { get; set; }
        public bool SendChunked { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public FakeHttpResponse()
        {
            ContentEncoding = Encoding.UTF8;
        }
    }
}
