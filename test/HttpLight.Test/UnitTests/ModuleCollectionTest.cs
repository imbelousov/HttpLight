using System;
using HttpLight.Attributes;
using NUnit.Framework;

namespace HttpLight.Test.UnitTests
{
    [TestFixture]
    public class ModuleCollectionTest
    {
        [Test]
        public void Add_NotModule()
        {
            var moduleCollection = new ModuleCollection(new RouteCollection());
            Assert.Throws<Exception>(() => moduleCollection.Add(typeof(ModuleCollectionTestClass)));
        }

        [Test]
        public void Add_Success()
        {
            var moduleCollection = new ModuleCollection(new RouteCollection());
            moduleCollection.Add<ModuleCollectionTestModule>();
            CollectionAssert.AreEqual(new[] {typeof(ModuleCollectionTestModule)}, moduleCollection);
        }

        [Test]
        public void Clear()
        {
            var routeCollection = new RouteCollection();
            var moduleCollection = new ModuleCollection(routeCollection);
            moduleCollection.Add<ModuleCollectionTestModule>();
            moduleCollection.Clear();
            CollectionAssert.AreEqual(new Type[0], moduleCollection);
            var route = routeCollection.Get(HttpStatusCode.NotFound);
            Assert.IsNull(route);
        }

        [Test]
        public void GetActionRoute_SpecifiedPath()
        {
            var routeCollection = new RouteCollection();
            var moduleCollection = new ModuleCollection(routeCollection);
            moduleCollection.Add<ModuleCollectionTestModule>();
            bool methodNotAllowed;
            var route = routeCollection.Get(HttpMethod.Post, "/test", out methodNotAllowed);
            Assert.IsNotNull(route);
        }

        [Test]
        public void GetActionRoute_DefaultPath()
        {
            var routeCollection = new RouteCollection();
            var moduleCollection = new ModuleCollection(routeCollection);
            moduleCollection.Add<ModuleCollectionTestModule>();
            bool methodNotAllowed;
            var route = routeCollection.Get(HttpMethod.Get, "/testget", out methodNotAllowed);
            Assert.IsNotNull(route);
        }

        [Test]
        public void GetStatusCodeActionRoute()
        {
            var routeCollection = new RouteCollection();
            var moduleCollection = new ModuleCollection(routeCollection);
            moduleCollection.Add<ModuleCollectionTestModule>();
            var route = routeCollection.Get(HttpStatusCode.NotFound);
            Assert.IsNotNull(route);
        }
    }

    internal class ModuleCollectionTestModule : HttpModule
    {
        [Get]
        public void TestGet([Binder(typeof(PrimitivesBinder))] int a, int b)
        {
        }

        [Post]
        [Path("/test")]
        public void TestPost()
        {
        }

        [StatusCode(HttpStatusCode.NotFound)]
        public void TestStatusCode()
        {
        }
    }

    internal class ModuleCollectionTestClass
    {
    }
}
