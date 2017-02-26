using System.Reflection;
#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif
using HttpLight.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpLight.Test
{
    [TestClass]
    public class MethodInvokerTest
    {
        [TestMethod]
        public void VoidRefSync()
        {
            var instance = new MethodInvokerTestClass();
            var invoker = new MethodInvoker(GetMethod(instance, nameof(instance.VoidRefSync)), instance.GetType());
#if FEATURE_ASYNC
            Assert.IsFalse(invoker.IsAsync);
#endif
            var result = invoker.Invoke(instance, new object[] {"1", "a"});
            Assert.IsNull(result);
        }

        [TestMethod]
        public void StringRefSync()
        {
            var instance = new MethodInvokerTestClass();
            var invoker = new MethodInvoker(GetMethod(instance, nameof(instance.StringRefSync)), instance.GetType());
#if FEATURE_ASYNC
            Assert.IsFalse(invoker.IsAsync);
#endif
            var result = (string) invoker.Invoke(instance, new object[] {"1", "a"});
            Assert.AreEqual("1a", result);
        }

        [TestMethod]
        public void VoidValSync()
        {
            var instance = new MethodInvokerTestClass();
            var invoker = new MethodInvoker(GetMethod(instance, nameof(instance.VoidValSync)), instance.GetType());
#if FEATURE_ASYNC
            Assert.IsFalse(invoker.IsAsync);
#endif
            var result = invoker.Invoke(instance, new object[] {1, 2});
            Assert.IsNull(result);
        }

        [TestMethod]
        public void IntValSync()
        {
            var instance = new MethodInvokerTestClass();
            var invoker = new MethodInvoker(GetMethod(instance, nameof(instance.IntValSync)), instance.GetType());
#if FEATURE_ASYNC
            Assert.IsFalse(invoker.IsAsync);
#endif
            var result = (int) invoker.Invoke(instance, new object[] {1, 2});
            Assert.AreEqual(3, result);
        }

#if FEATURE_ASYNC
        [TestMethod]
        public void VoidRefAsync()
        {
            var instance = new MethodInvokerTestClass();
            var invoker = new MethodInvoker(GetMethod(instance, nameof(instance.VoidRefAsync)), instance.GetType());
            Assert.IsTrue(invoker.IsAsync);
            var task = Task.Run(() => invoker.InvokeAsync(instance, new object[] {"1", "a"}));
            task.Wait();
            var result = task.Result;
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void StringRefAsync()
        {
            var instance = new MethodInvokerTestClass();
            var invoker = new MethodInvoker(GetMethod(instance, nameof(instance.StringRefAsync)), instance.GetType());
            Assert.IsTrue(invoker.IsAsync);
            var task = Task.Run(() => invoker.InvokeAsync(instance, new object[] {"1", "a"}));
            task.Wait();
            var result = (string) task.Result;
            Assert.AreEqual("1a", result);
        }

        [TestMethod]
        public void VoidValAsync()
        {
            var instance = new MethodInvokerTestClass();
            var invoker = new MethodInvoker(GetMethod(instance, nameof(instance.VoidValAsync)), instance.GetType());
            Assert.IsTrue(invoker.IsAsync);
            var task = Task.Run(() => invoker.InvokeAsync(instance, new object[] {1, 2}));
            task.Wait();
            var result = task.Result;
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void IntValAsync()
        {
            var instance = new MethodInvokerTestClass();
            var invoker = new MethodInvoker(GetMethod(instance, nameof(instance.IntValAsync)), instance.GetType());
            Assert.IsTrue(invoker.IsAsync);
            var task = Task.Run(() => invoker.InvokeAsync(instance, new object[] {1, 2}));
            task.Wait();
            var result = (int) task.Result;
            Assert.AreEqual(3, result);
        }
#endif

        private MethodInfo GetMethod<T>(T instance, string name)
        {
            return typeof(T).GetMethod(name);
        }
    }

    public class MethodInvokerTestClass
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
    }
}
