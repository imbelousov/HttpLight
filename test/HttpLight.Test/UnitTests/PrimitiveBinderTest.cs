using System;
using HttpLight.Test.Utils;
using NUnit.Framework;

namespace HttpLight.Test.UnitTests
{
    [TestFixture]
    public class PrimitiveBinderTest
    {
        [TestCase("a", "1", "a", typeof(int), ExpectedResult = 1, TestName = "Value type")]
        [TestCase("a", "asd", "a", typeof(string), ExpectedResult = "asd", TestName = "Reference type")]
        [TestCase("b", "1", "a", typeof(int?), ExpectedResult = null, TestName = "Default value")]
        public object Bind(string urlName, string urlValue, string parameterName, Type type)
        {
            var binder = new PrimitiveBinder();
            var context = new ActionParameterBinderContext
            {
                Source = new UrlActionParameterSource(new FakeHttpRequest("/?" + urlName + "=" + urlValue)),
                ParameterAttributes = new Attribute[0],
                ParameterName = parameterName,
                ParameterType = type
            };
            var result = binder.Bind(context);
            return result;
        }

        [Test]
        public void BindArray()
        {
            var binder = new PrimitiveBinder();
            var context = new ActionParameterBinderContext
            {
                Source = new UrlActionParameterSource(new FakeHttpRequest("/?a=1&a=2")),
                ParameterAttributes = new Attribute[0],
                ParameterName = "a",
                ParameterType = typeof(int[])
            };
            var result = (int[]) binder.Bind(context);
            CollectionAssert.AreEqual(new[] {1, 2}, result);
        }
    }
}
