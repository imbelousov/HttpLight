using System.Reflection;
using HttpLight.Utils;
using NUnit.Framework;

namespace HttpLight.Test.UnitTests
{
    [TestFixture]
    public class RouteCollectionTest
    {
        [Test]
        public void Action_Success()
        {
            var routeCollection = new RouteCollection();
            var route = CreateRoute();
            routeCollection.Add(HttpMethod.Get, "/path", route);
            bool methodNotAllowed;
            var result = routeCollection.Get(HttpMethod.Get, "/path", out methodNotAllowed);
            Assert.AreEqual(route, result);
        }

        [Test]
        public void StatusCodeAction_Success()
        {
            var routeCollection = new RouteCollection();
            var route = CreateRoute();
            routeCollection.Add(HttpStatusCode.NotFound, route);
            var result = routeCollection.Get(HttpStatusCode.NotFound);
            Assert.AreEqual(route, result);
        }

        [Test]
        public void Action_TwoEntries()
        {
            var routeCollection = new RouteCollection();
            var route1 = CreateRoute();
            var route2 = CreateRoute();
            routeCollection.Add(HttpMethod.Get, "/path1", route1);
            routeCollection.Add(HttpMethod.Get, "/path2", route2);
            bool methodNotAllowed;
            var result1 = routeCollection.Get(HttpMethod.Get, "/path1", out methodNotAllowed);
            var result2 = routeCollection.Get(HttpMethod.Get, "/path2", out methodNotAllowed);
            Assert.AreEqual(route1, result1);
            Assert.AreEqual(route2, result2);
            Assert.AreNotEqual(result1, result2);
        }

        [Test]
        public void StatusCodeAction_TwoEntries()
        {
            var routeCollection = new RouteCollection();
            var route1 = CreateRoute();
            var route2 = CreateRoute();
            routeCollection.Add(HttpStatusCode.NotFound, route1);
            routeCollection.Add(HttpStatusCode.InternalServerError, route2);
            var result1 = routeCollection.Get(HttpStatusCode.NotFound);
            var result2 = routeCollection.Get(HttpStatusCode.InternalServerError);
            Assert.AreEqual(route1, result1);
            Assert.AreEqual(route2, result2);
            Assert.AreNotEqual(result1, result2);
        }

        [Test]
        public void Action_Override()
        {
            var routeCollection = new RouteCollection();
            var route1 = CreateRoute();
            var route2 = CreateRoute();
            routeCollection.Add(HttpMethod.Get, "/path", route1);
            routeCollection.Add(HttpMethod.Get, "/path", route2);
            bool methodNotAllowed;
            var result = routeCollection.Get(HttpMethod.Get, "/path", out methodNotAllowed);
            Assert.AreEqual(route2, result);
        }

        [Test]
        public void StatusCodeAction_Override()
        {
            var routeCollection = new RouteCollection();
            var route1 = CreateRoute();
            var route2 = CreateRoute();
            routeCollection.Add(HttpStatusCode.NotFound, route1);
            routeCollection.Add(HttpStatusCode.NotFound, route2);
            var result = routeCollection.Get(HttpStatusCode.NotFound);
            Assert.AreEqual(route2, result);
        }

        [Test]
        public void Action_PathUndefined()
        {
            var routeCollection = new RouteCollection();
            bool methodNotAllowed;
            var result = routeCollection.Get(HttpMethod.Get, "/path", out methodNotAllowed);
            Assert.IsNull(result);
            Assert.IsFalse(methodNotAllowed);
        }

        [Test]
        public void Action_MethodNotAllowed()
        {
            var routeCollection = new RouteCollection();
            var route = CreateRoute();
            routeCollection.Add(HttpMethod.Get, "/path", route);
            bool methodNotAllowed;
            var result = routeCollection.Get(HttpMethod.Post, "/path", out methodNotAllowed);
            Assert.IsNull(result);
            Assert.IsTrue(methodNotAllowed);
        }

        [Test]
        public void StatusCodeAction_Undefined()
        {
            var routeCollection = new RouteCollection();
            var result = routeCollection.Get(HttpStatusCode.NotFound);
            Assert.IsNull(result);
        }

        [TestCase("/path", "/PaTh", TestName = "Case insensitive")]
        [TestCase("path", "/path", TestName = "Leading slash missed 1")]
        [TestCase("/path", "path", TestName = "Leading slash missed 2")]
        [TestCase("path", "path", TestName = "Leading slash missed 3")]
        public void Action_Path(string addPath, string getPath)
        {
            var routeCollection = new RouteCollection();
            var route = CreateRoute();
            routeCollection.Add(HttpMethod.Get, addPath, route);
            bool methodNotAllowed;
            var result = routeCollection.Get(HttpMethod.Get, getPath, out methodNotAllowed);
            Assert.AreEqual(route, result);
        }

        [Test]
        public void Action_Clear()
        {
            var routeCollection = new RouteCollection();
            var route = CreateRoute();
            routeCollection.Add(HttpMethod.Get, "/path", route);
            routeCollection.Clear();
            bool methodNotAllowed;
            var result = routeCollection.Get(HttpMethod.Get, "/path", out methodNotAllowed);
            Assert.IsNull(result);
        }

        [Test]
        public void StatusCodeAction_Clear()
        {
            var routeCollection = new RouteCollection();
            var route = CreateRoute();
            routeCollection.Add(HttpStatusCode.NotFound, route);
            routeCollection.Clear();
            var result = routeCollection.Get(HttpStatusCode.NotFound);
            Assert.IsNull(result);
        }

        private Route CreateRoute()
        {
            var methodInfo = GetType().GetMethod(nameof(TestMethod), BindingFlags.Instance | BindingFlags.NonPublic);
            var invoker = new MethodInvoker(methodInfo, GetType());
            var actionInfo = new ActionInfo(methodInfo);
            return new Route(invoker, actionInfo);
        }

        private void TestMethod()
        {
        }
    }
}
