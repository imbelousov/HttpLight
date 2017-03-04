using System;
using HttpLight.Test.Utils;
using NUnit.Framework;

namespace HttpLight.Test.UnitTests
{
    [TestFixture]
    public class PrimitivesBinderTest
    {
        [TestCase("a", "1", "a", typeof(int), ExpectedResult = 1, TestName = "Value type")]
        [TestCase("a", "asd", "a", typeof(string), ExpectedResult = "asd", TestName = "Reference type")]
        [TestCase("b", "1", "a", typeof(int?), ExpectedResult = null, TestName = "Default value")]
        public object Bind(string urlName, string urlValue, string parameterName, Type type)
        {
            var binder = new PrimitivesBinder();
            var context = new ActionParameterBinderContext
            {
                HttpRequest = new FakeHttpRequest(),
                ParameterAttributes = new Attribute[0],
                ParameterName = parameterName,
                ParameterType = type
            };
            context.HttpRequest.UrlParameters.Add(urlName, urlValue);
            var result = binder.Bind(context);
            return result;
        }

        [Test]
        public void BindArray()
        {
            const string name = "a";
            var binder = new PrimitivesBinder();
            var context = new ActionParameterBinderContext
            {
                HttpRequest = new FakeHttpRequest(),
                ParameterAttributes = new Attribute[0],
                ParameterName = "a",
                ParameterType = typeof(int[])
            };
            context.HttpRequest.UrlParameters.Add(name, "1");
            context.HttpRequest.UrlParameters.Add(name, "2");
            var result = (int[]) binder.Bind(context);
            CollectionAssert.AreEqual(new[] {1, 2}, result);
        }
    }
}
