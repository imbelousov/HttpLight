namespace HttpLight.Utils
{
    internal static class HttpMethodHelper
    {
        public static HttpMethod Convert(string httpMethod)
        {
            switch (httpMethod.ToUpper())
            {
                case "GET":
                    return HttpMethod.Get;
                case "HEAD":
                    return HttpMethod.Head;
                case "POST":
                    return HttpMethod.Post;
                case "PUT":
                    return HttpMethod.Put;
                case "DELETE":
                    return HttpMethod.Delete;
                default:
                    return HttpMethod.Unknown;
            }
        }
    }
}
