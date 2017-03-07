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
            : this(new FakeHttpRequest(path))
        {
        }

        public FakeRequestStateMachineContext(string path, HttpMethod method)
            : this(new FakeHttpRequest(path, method))
        {
        }

        public FakeRequestStateMachineContext(string baseUrl, string path, HttpMethod method)
            : this(new FakeHttpRequest(baseUrl, path, method))
        {
        }

        public FakeRequestStateMachineContext(FakeHttpRequest request)
            : this(request, null)
        {
        }

        public FakeRequestStateMachineContext(FakeHttpRequest request, Stream outputStream)
        {
            var response = new FakeHttpResponse();
            Request = request;
            Response = response;
            OutputStream = outputStream ?? response.OutputStream;
        }

        public void SetOutputStream(Stream stream)
        {
            OutputStream = stream;
        }
    }
}
