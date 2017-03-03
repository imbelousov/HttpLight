using System;
using System.Reflection;
#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif
using HttpLight.Utils;
using NUnit.Framework;

namespace HttpLight.Test.UnitTests
{
    [TestFixture]
    public class MethodInvokerTest
    {
        [TestCase(nameof(MethodInvokerTestClass.VoidRefSync), new object[] {"1", "a"}, ExpectedResult = null, TestName = "Reference type, returns void, sync method")]
        [TestCase(nameof(MethodInvokerTestClass.StringRefSync), new object[] {"1", "a"}, ExpectedResult = "1a", TestName = "Reference type, returns string, sync method")]
        [TestCase(nameof(MethodInvokerTestClass.VoidValSync), new object[] {1, 2}, ExpectedResult = null, TestName = "Value type, returns void, sync method")]
        [TestCase(nameof(MethodInvokerTestClass.IntValSync), new object[] {1, 2}, ExpectedResult = 3, TestName = "Value type, returns int, sync method")]
        public object Invoke(string methodName, object[] parameters)
        {
            var instance = new MethodInvokerTestClass();
            var methodInfo = GetMethod(instance, methodName);
            var invoker = new MethodInvoker(methodInfo, instance.GetType());
            var result = invoker.Invoke(instance, parameters);
            return result;
        }

        [Test]
        public void Parameters()
        {
            var instance = new MethodInvokerTestClass();
            var methodInfo = GetMethod(instance, nameof(MethodInvokerTestClass.TestMethod));
            var invoker = new MethodInvoker(methodInfo, instance.GetType());
            Assert.AreEqual(2, invoker.Parameters.Count);
            Assert.AreEqual(typeof(int), invoker.Parameters[0].Type);
            Assert.AreEqual("x", invoker.Parameters[0].Name);
            Assert.AreEqual(0, invoker.Parameters[0].Attributes.Length);
            Assert.AreEqual(typeof(long), invoker.Parameters[1].Type);
            Assert.AreEqual("y", invoker.Parameters[1].Name);
            Assert.AreEqual(1, invoker.Parameters[1].Attributes.Length);
            Assert.IsTrue(invoker.Parameters[1].Attributes[0] is MethodInvokerTestAttribute);
        }

        [TestCase(nameof(MethodInvokerTestClass.VoidRefSync), ExpectedResult = typeof(void), TestName = "void")]
        [TestCase(nameof(MethodInvokerTestClass.StringRefSync), ExpectedResult = typeof(string), TestName = "string")]
#if FEATURE_ASYNC
        [TestCase(nameof(MethodInvokerTestClass.VoidRefAsync), ExpectedResult = typeof(Task), TestName = "Task")]
        [TestCase(nameof(MethodInvokerTestClass.StringRefAsync), ExpectedResult = typeof(Task<string>), TestName = "Task<string>")]
#endif
        public Type ReturnType(string methodName)
        {
            var instance = new MethodInvokerTestClass();
            var methodInfo = GetMethod(instance, methodName);
            var invoker = new MethodInvoker(methodInfo, instance.GetType());
            return invoker.ReturnType;
        }

        [Test]
        public void InstanceType()
        {
            var instance = new MethodInvokerTestClass();
            var methodInfo = GetMethod(instance, nameof(MethodInvokerTestClass.TestMethod));
            var invoker = new MethodInvoker(methodInfo, instance.GetType());
            Assert.AreEqual(typeof(MethodInvokerTestClass), invoker.InstanceType);
        }

#if FEATURE_ASYNC
        [TestCase(nameof(MethodInvokerTestClass.VoidRefAsync), new object[] {"1", "a"}, ExpectedResult = null, TestName = "Reference type, returns void, async method")]
        [TestCase(nameof(MethodInvokerTestClass.StringRefAsync), new object[] {"1", "a"}, ExpectedResult = "1a", TestName = "Reference type, returns string, async method")]
        [TestCase(nameof(MethodInvokerTestClass.VoidValAsync), new object[] {1, 2}, ExpectedResult = null, TestName = "Value type, returns void, async method")]
        [TestCase(nameof(MethodInvokerTestClass.IntValAsync), new object[] {1, 2}, ExpectedResult = 3, TestName = "Value type, returns int, async method")]
        public object InvokeAsync(string methodName, object[] parameters)
        {
            var instance = new MethodInvokerTestClass();
            var methodInfo = GetMethod(instance, methodName);
            var invoker = new MethodInvoker(methodInfo, instance.GetType());
            var task = Task.Run(() => invoker.InvokeAsync(instance, parameters));
            task.Wait();
            return task.Result;
        }

        [Test]
        public void InvokeAsyncSynchronously()
        {
            var instance = new MethodInvokerTestClass();
            var methodInfo = GetMethod(instance, nameof(MethodInvokerTestClass.StringRefAsync));
            var invoker = new MethodInvoker(methodInfo, instance.GetType());
            var task = (Task<string>) invoker.Invoke(instance, new object[] {"1", "a"});
            task.Wait();
            Assert.AreEqual("1a", task.Result);
        }

        [Test]
        public void InvokeSyncAsynchronously()
        {
            var instance = new MethodInvokerTestClass();
            var methodInfo = GetMethod(instance, nameof(MethodInvokerTestClass.StringRefSync));
            var invoker = new MethodInvoker(methodInfo, instance.GetType());
            var task = invoker.InvokeAsync(instance, new object[] {"1", "a"});
            task.Wait();
            Assert.AreEqual("1a", task.Result);
        }

        [TestCase(nameof(MethodInvokerTestClass.VoidRefSync), ExpectedResult = false, TestName = "Sync")]
        [TestCase(nameof(MethodInvokerTestClass.VoidRefAsync), ExpectedResult = true, TestName = "Async")]
        public bool IsAsync(string methodName)
        {
            var instance = new MethodInvokerTestClass();
            var methodInfo = GetMethod(instance, methodName);
            var invoker = new MethodInvoker(methodInfo, instance.GetType());
            return invoker.IsAsync;
        }
#endif

        private MethodInfo GetMethod<T>(T instance, string name)
        {
            return typeof(T).GetMethod(name);
        }
    }

    internal class MethodInvokerTestClass
    {
        public void VoidRefSync(string p1, string p2)
        {
        }

        public string StringRefSync(string p1, string p2)
        {
            return p1 + p2;
        }

        public void VoidValSync(int p1, int p2)
        {
        }

        public int IntValSync(int p1, int p2)
        {
            return p1 + p2;
        }

#if FEATURE_ASYNC
        public Task VoidRefAsync(string p1, string p2)
        {
            return Task.Run(() => { });
        }

        public Task<string> StringRefAsync(string p1, string p2)
        {
            return Task.Run(() => p1 + p2);
        }

        public Task VoidValAsync(int p1, int p2)
        {
            return Task.Run(() => { });
        }

        public Task<int> IntValAsync(int p1, int p2)
        {
            return Task.Run(() => p1 + p2);
        }
#endif

        public string TestMethod(int x, [MethodInvokerTest] long y)
        {
            return null;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal class MethodInvokerTestAttribute : Attribute
    {
    }
}
