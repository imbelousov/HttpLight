using System.IO;
using HttpLight.Attributes;

#if FEATURE_CACHING
using System.Runtime.Caching;
#else
using System.Collections.Generic;
#endif

#if !FEATURE_STREAMCOPYTO
using HttpLight.Utils;
#endif

namespace HttpLight
{
    internal class DefaultController : Controller
    {
#if FEATURE_CACHING
        private MemoryCache _cache = new MemoryCache("StatusCode");
#else
        private Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();
#endif

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
            var result = LoadFromCache(resourceName);
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
            SaveToCache(resourceName, result);
            return result;
        }

        private byte[] LoadFromCache(string resourceName)
        {
#if FEATURE_CACHING
            return _cache.Get(resourceName) as byte[];
#else
            lock (_cache)
            {
                byte[] result;
                _cache.TryGetValue(resourceName, out result);
                return result;
            }
#endif
        }

        private void SaveToCache(string resourceName, byte[] result)
        {
#if FEATURE_CACHING
            _cache.Set(resourceName, result, new CacheItemPolicy());
#else
            lock (_cache)
            {
                _cache[resourceName] = result;
            }
#endif
        }
    }
}
