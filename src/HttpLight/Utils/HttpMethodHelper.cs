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
                case "POST":
                    return HttpMethod.Post;
                default:
                    return HttpMethod.Unknown;
            }
        }
    }
}
