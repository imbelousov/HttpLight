using System.IO;
using System.Runtime.Caching;
using HttpLight.Attributes;

namespace HttpLight
{
    internal class DefaultModule : HttpModule
    {
        private MemoryCache _cache = new MemoryCache("StatusCode");

        [StatusCode(HttpStatusCode.NotFound)]
        public byte[] NotFound()
        {
            var content = GetContent("HttpLight.Views.404.html");
            Response.ContentLength = content.Length;
            Response.ContentType = HttpContentType.Html;
            return content;
        }

        [StatusCode(HttpStatusCode.MethodNotAllowed)]
        public byte[] MethodNotAllowed()
        {
            var content = GetContent("HttpLight.Views.405.html");
            Response.ContentLength = content.Length;
            Response.ContentType = HttpContentType.Html;
            return content;
        }

        [StatusCode(HttpStatusCode.InternalServerError)]
        public byte[] InternalServerError()
        {
            var content = GetContent("HttpLight.Views.500.html");
            Response.ContentLength = content.Length;
            Response.ContentType = HttpContentType.Html;
            return content;
        }

        private byte[] GetContent(string resourceName)
        {
            var result = _cache.Get(resourceName) as byte[];
            if (result != null)
                return result;
            var stream = GetType().Assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                return new byte[0];
            using (stream)
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                result = memoryStream.ToArray();
            }
            _cache.Set(resourceName, result, new CacheItemPolicy());
            return result;
        }
    }
}
