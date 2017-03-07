using System.IO;

namespace HttpLight.Test.Utils
{
    internal class FakeRequestStateMachineContext : RequestStateMachineContext
    {
        private const string DefaultBaseUrl = "http://localhost:8080/";

        public FakeRequestStateMachineContext()
            : this("/")
        {
        }

        public FakeRequestStateMachineContext(string path)
            : this(null, path)
        {
        }

        public FakeRequestStateMachineContext(string path, HttpMethod method)
            : this(null, path, method)
        {
        }

        public FakeRequestStateMachineContext(string baseUrl, string path)
            : this(baseUrl, path, HttpMethod.Get)
        {
        }

        public FakeRequestStateMachineContext(string baseUrl, string path, HttpMethod method)
        {
            baseUrl = GetBaseUrl(baseUrl);
            var url = GetUrl(baseUrl, path);
            var request = new FakeHttpRequest(url);
            request.Method = method;
            var response = new FakeHttpResponse();
            Request = request;
            Response = response;
            OutputStream = response.OutputStream;
        }

        public void SetOutputStream(Stream stream)
        {
            OutputStream = stream;
        }

        private string GetBaseUrl(string baseUrl)
        {
            baseUrl = baseUrl ?? DefaultBaseUrl;
            if (!baseUrl.EndsWith("/"))
                baseUrl = baseUrl + "/";
            return baseUrl;
        }

        private string GetUrl(string baseUrl, string path)
        {
            path = path ?? string.Empty;
            if (path.StartsWith("/"))
                path = path.Substring(1);
            return baseUrl + path;
        }
    }
}
