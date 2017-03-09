using System;
using System.Linq;
using HttpLight.Attributes;
using HttpLight.Test.Utils;
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
            Assert.Throws<Exception>(() => controllers.Add(typeof(object)));
        }

        [Test]
        public void Add_Success()
        {
            var controllers = new ControllerCollection(new ActionCollection());
            var controller = new ActionBuilder("Test")
                .Build()
                .Invoker
                .InstanceType;
            controllers.Add(controller);
            CollectionAssert.AreEqual(new[] {controller}, controllers);
        }

        [Test]
        public void Add_Generic()
        {
            var controllers = new ControllerCollection(new ActionCollection());
            var controller = new ActionBuilder("Test")
                .Build()
                .Invoker
                .InstanceType;
            controllers
                .GetType()
                .GetMethods()
                .Single(x => x.Name == nameof(controllers.Add) && x.IsGenericMethod)
                .MakeGenericMethod(controller)
                .Invoke(controllers, new object[0]);
            CollectionAssert.AreEqual(new[] {controller}, controllers);
        }

        [Test]
        public void Clear()
        {
            var actions = new ActionCollection();
            var controllers = new ControllerCollection(actions);
            var controller = new ActionBuilder("Test")
                .Build()
                .Invoker
                .InstanceType;
            controllers.Add(controller);
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
            var controller = new ActionBuilder("Test")
                .AddAttribute(typeof(GetAttribute))
                .AddAttribute(typeof(PathAttribute), "/qwerty")
                .Build()
                .Invoker
                .InstanceType;
            controllers.Add(controller);
            bool methodNotAllowed;
            var action = actions.Get(HttpMethod.Get, "/qwerty", out methodNotAllowed);
            Assert.IsNotNull(action);
        }

        [Test]
        public void GetUsualAction_DefaultPath()
        {
            var actions = new ActionCollection();
            var controllers = new ControllerCollection(actions);
            var controller = new ActionBuilder("Test")
                .AddAttribute(typeof(GetAttribute))
                .Build()
                .Invoker
                .InstanceType;
            controllers.Add(controller);
            bool methodNotAllowed;
            var action = actions.Get(HttpMethod.Get, "/Test", out methodNotAllowed);
            Assert.IsNotNull(action);
        }

        [Test]
        public void GetStatusCodeAction()
        {
            var actions = new ActionCollection();
            var controllers = new ControllerCollection(actions);
            var controller = new ActionBuilder("Test")
                .AddAttribute(typeof(StatusCodeAttribute), HttpStatusCode.NotFound)
                .Build()
                .Invoker
                .InstanceType;
            controllers.Add(controller);
            var action = actions.Get(HttpStatusCode.NotFound);
            Assert.IsNotNull(action);
        }

        [Test]
        public void GetBeforeAction()
        {
            var actions = new ActionCollection();
            var controllers = new ControllerCollection(actions);
            var controller = new ActionBuilder("Test")
                .AddAttribute(typeof(BeforeAttribute))
                .Build()
                .Invoker
                .InstanceType;
            controllers.Add(controller);
            var beforeActions = actions.GetBefore(controller);
            CollectionAssert.IsNotEmpty(beforeActions);
        }
    }
}
