using System;
using System.IO;
using System.Text;

namespace HttpLight.Test.Utils
{
    internal class FakeHttpResponse : IHttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public Exception Exception { get; set; }
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public long? ContentLength { get; set; }
        public Stream OutputStream { get; set; }

        public FakeHttpResponse()
        {
            ContentEncoding = Encoding.UTF8;
        }
    }
}
