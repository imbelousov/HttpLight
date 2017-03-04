using System;
using HttpLight.Attributes;
using NUnit.Framework;

namespace HttpLight.Test.UnitTests
{
    [TestFixture]
    public class ControllerCollectionTest
    {
        [Test]
        public void Add_NotController()
        {
            var controllers = new ControllerCollection(new ActionCollection());
            Assert.Throws<Exception>(() => controllers.Add(typeof(ControllerCollectionTestClass)));
        }

        [Test]
        public void Add_Success()
        {
            var controllers = new ControllerCollection(new ActionCollection());
            controllers.Add<ControllerCollectionTestController>();
            CollectionAssert.AreEqual(new[] {typeof(ControllerCollectionTestController)}, controllers);
        }

        [Test]
        public void Clear()
        {
            var actions = new ActionCollection();
            var controllers = new ControllerCollection(actions);
            controllers.Add<ControllerCollectionTestController>();
            controllers.Clear();
            CollectionAssert.AreEqual(new Type[0], controllers);
            var action = actions.Get(HttpStatusCode.NotFound);
            Assert.IsNull(action);
        }

        [Test]
        public void GetUsualAction_SpecifiedPath()
        {
            var actions = new ActionCollection();
            var controllers = new ControllerCollection(actions);
            controllers.Add<ControllerCollectionTestController>();
            bool methodNotAllowed;
            var action = actions.Get(HttpMethod.Post, "/test", out methodNotAllowed);
            Assert.IsNotNull(action);
        }

        [Test]
        public void GetUsualAction_DefaultPath()
        {
            var actions = new ActionCollection();
            var controllers = new ControllerCollection(actions);
            controllers.Add<ControllerCollectionTestController>();
            bool methodNotAllowed;
            var action = actions.Get(HttpMethod.Get, "/testget", out methodNotAllowed);
            Assert.IsNotNull(action);
        }

        [Test]
        public void GetStatusCodeAction()
        {
            var actions = new ActionCollection();
            var controllers = new ControllerCollection(actions);
            controllers.Add<ControllerCollectionTestController>();
            var action = actions.Get(HttpStatusCode.NotFound);
            Assert.IsNotNull(action);
        }
    }

    internal class ControllerCollectionTestController : Controller
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

    internal class ControllerCollectionTestClass
    {
    }
}
