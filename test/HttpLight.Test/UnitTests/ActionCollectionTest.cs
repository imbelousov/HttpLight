using HttpLight.Test.Utils;
using NUnit.Framework;

namespace HttpLight.Test.UnitTests
{
    [TestFixture]
    public class ActionCollectionTest
    {
        [Test]
        public void UsualAction_Success()
        {
            var actions = new ActionCollection();
            var action = new ActionBuilder("Test").Build();
            actions.Add(HttpMethod.Get, "/path", action);
            bool methodNotAllowed;
            var result = actions.Get(HttpMethod.Get, "/path", out methodNotAllowed);
            Assert.AreEqual(action, result);
        }

        [Test]
        public void StatusCodeAction_Success()
        {
            var actions = new ActionCollection();
            var action = new ActionBuilder("Test").Build();
            actions.Add(HttpStatusCode.NotFound, action);
            var result = actions.Get(HttpStatusCode.NotFound);
            Assert.AreEqual(action, result);
        }

        [Test]
        public void UsualAction_TwoEntries()
        {
            var actions = new ActionCollection();
            var action1 = new ActionBuilder("Test1").Build();
            var action2 = new ActionBuilder("Test2").Build();
            actions.Add(HttpMethod.Get, "/path1", action1);
            actions.Add(HttpMethod.Get, "/path2", action2);
            bool methodNotAllowed;
            var result1 = actions.Get(HttpMethod.Get, "/path1", out methodNotAllowed);
            var result2 = actions.Get(HttpMethod.Get, "/path2", out methodNotAllowed);
            Assert.AreEqual(action1, result1);
            Assert.AreEqual(action2, result2);
            Assert.AreNotEqual(result1, result2);
        }

        [Test]
        public void StatusCodeAction_TwoEntries()
        {
            var actions = new ActionCollection();
            var action1 = new ActionBuilder("Test1").Build();
            var action2 = new ActionBuilder("Test2").Build();
            actions.Add(HttpStatusCode.NotFound, action1);
            actions.Add(HttpStatusCode.InternalServerError, action2);
            var result1 = actions.Get(HttpStatusCode.NotFound);
            var result2 = actions.Get(HttpStatusCode.InternalServerError);
            Assert.AreEqual(action1, result1);
            Assert.AreEqual(action2, result2);
            Assert.AreNotEqual(result1, result2);
        }

        [Test]
        public void UsualAction_Override()
        {
            var actions = new ActionCollection();
            Action action1, action2;
            action1 = action2 = new ActionBuilder("Test").Build();
            actions.Add(HttpMethod.Get, "/path", action1);
            actions.Add(HttpMethod.Get, "/path", action2);
            bool methodNotAllowed;
            var result = actions.Get(HttpMethod.Get, "/path", out methodNotAllowed);
            Assert.AreEqual(action2, result);
        }

        [Test]
        public void StatusCodeAction_Override()
        {
            var actions = new ActionCollection();
            Action action1, action2;
            action1 = action2 = new ActionBuilder("Test").Build();
            actions.Add(HttpStatusCode.NotFound, action1);
            actions.Add(HttpStatusCode.NotFound, action2);
            var result = actions.Get(HttpStatusCode.NotFound);
            Assert.AreEqual(action2, result);
        }

        [Test]
        public void UsualAction_PathUndefined()
        {
            var actions = new ActionCollection();
            bool methodNotAllowed;
            var result = actions.Get(HttpMethod.Get, "/path", out methodNotAllowed);
            Assert.IsNull(result);
            Assert.IsFalse(methodNotAllowed);
        }

        [Test]
        public void UsualAction_MethodNotAllowed()
        {
            var actions = new ActionCollection();
            var action = new ActionBuilder("Test").Build();
            actions.Add(HttpMethod.Get, "/path", action);
            bool methodNotAllowed;
            var result = actions.Get(HttpMethod.Post, "/path", out methodNotAllowed);
            Assert.IsNull(result);
            Assert.IsTrue(methodNotAllowed);
        }

        [Test]
        public void StatusCodeAction_Undefined()
        {
            var actions = new ActionCollection();
            var result = actions.Get(HttpStatusCode.NotFound);
            Assert.IsNull(result);
        }

        [TestCase("/path", "/PaTh", TestName = "Case insensitive")]
        [TestCase("path", "/path", TestName = "Leading slash missed 1")]
        [TestCase("/path", "path", TestName = "Leading slash missed 2")]
        [TestCase("path", "path", TestName = "Leading slash missed 3")]
        public void UsualAction_Path(string addPath, string getPath)
        {
            var actions = new ActionCollection();
            var action = new ActionBuilder("Test").Build();
            actions.Add(HttpMethod.Get, addPath, action);
            bool methodNotAllowed;
            var result = actions.Get(HttpMethod.Get, getPath, out methodNotAllowed);
            Assert.AreEqual(action, result);
        }

        [Test]
        public void UsualAction_Clear()
        {
            var actions = new ActionCollection();
            var action = new ActionBuilder("Test").Build();
            actions.Add(HttpMethod.Get, "/path", action);
            actions.Clear();
            bool methodNotAllowed;
            var result = actions.Get(HttpMethod.Get, "/path", out methodNotAllowed);
            Assert.IsNull(result);
        }

        [Test]
        public void StatusCodeAction_Clear()
        {
            var actions = new ActionCollection();
            var action = new ActionBuilder("Test").Build();
            actions.Add(HttpStatusCode.NotFound, action);
            actions.Clear();
            var result = actions.Get(HttpStatusCode.NotFound);
            Assert.IsNull(result);
        }
    }
}
