using HttpLight.Utils;
using NUnit.Framework;

namespace HttpLight.Test.UnitTests
{
    [TestFixture]
    public class HttpMethodHelperTest
    {
        [TestCase("GET", ExpectedResult = HttpMethod.Get, TestName = "Get")]
        [TestCase("POST", ExpectedResult = HttpMethod.Post, TestName = "Post")]
        [TestCase("Not HTTP method", ExpectedResult = HttpMethod.Unknown, TestName = "Unknown")]
        [TestCase("gEt", ExpectedResult = HttpMethod.Get, TestName = "Case insensitive")]
        public HttpMethod Convert(string httpMethod)
        {
            return HttpMethodHelper.Convert(httpMethod);
        }
    }
}
