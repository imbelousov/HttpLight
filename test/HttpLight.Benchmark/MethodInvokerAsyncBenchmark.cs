using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using HttpLight.Benchmark.Attributes;
using HttpLight.Utils;

namespace HttpLight.Benchmark
{
    [Benchmark("MethodInvoke.InvokeAsync", Iterations = 50)]
    internal class MethodInvokerAsyncBenchmark
    {
        private const int Count = 30000;
        private static readonly string[] Strings = {"1", "2", "3", "4", "5"};

        [Description("[return:void] Direct call")]
        public void DirectCall_Void()
        {
            var tasks = new List<Task>(Count);
            var obj = new TestClass();
            for (var i = 0; i < Count; i++)
            {
                var task = Task.Run(() => obj.VoidFuncAsync(Strings[i % Strings.Length]));
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        [Description("[return:void] MethodInfo.Invoke")]
        public void MethodInfoInvoke_Void()
        {
            var tasks = new List<Task>(Count);
            var obj = new TestClass();
            var methodInfo = obj.GetType().GetMethod(nameof(obj.VoidFuncAsync));
            for (var i = 0; i < Count; i++)
            {
                var task = Task.Run(() => (Task) methodInfo.Invoke(obj, new object[] {Strings[i % Strings.Length]}));
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        [Description("[return:void] MethodInvoker.InvokeAsync")]
        public void MethodInvokerInvokeAsync_Void()
        {
            var tasks = new List<Task>(Count);
            var obj = new TestClass();
            var methodInfo = obj.GetType().GetMethod(nameof(obj.VoidFuncAsync));
            var methodInvoker = new MethodInvoker(methodInfo, obj.GetType());
            for (var i = 0; i < Count; i++)
            {
                var task = (Task) Task.Run(() => methodInvoker.InvokeAsync(obj, new object[] {Strings[i % Strings.Length]}));
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        [Description("[return:void] MethodInvoker.Invoke")]
        public void MethodInvokerInvoke_Void()
        {
            var tasks = new List<Task>(Count);
            var obj = new TestClass();
            var methodInfo = obj.GetType().GetMethod(nameof(obj.VoidFuncAsync));
            var methodInvoker = new MethodInvoker(methodInfo, obj.GetType());
            for (var i = 0; i < Count; i++)
            {
                var task = Task.Run(() => (Task) methodInvoker.Invoke(obj, new object[] {Strings[i % Strings.Length]}));
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        [Description("[return:string] Direct call")]
        public void DirectCall_String()
        {
            var tasks = new List<Task>(Count);
            var obj = new TestClass();
            for (var i = 0; i < Count; i++)
            {
                var task = (Task) Task.Run(() => obj.StringFuncAsync(Strings[i % Strings.Length]));
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        [Description("[return:string] MethodInfo.Invoke")]
        public void MethodInfoInvoke_String()
        {
            var tasks = new List<Task>(Count);
            var obj = new TestClass();
            var methodInfo = obj.GetType().GetMethod(nameof(obj.StringFuncAsync));
            for (var i = 0; i < Count; i++)
            {
                var task = (Task) Task.Run(() => (Task<string>) methodInfo.Invoke(obj, new object[] {Strings[i % Strings.Length]}));
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        [Description("[return:string] MethodInvoker.InvokeAsync")]
        public void MethodInvokerInvokeAsync_String()
        {
            var tasks = new List<Task>(Count);
            var obj = new TestClass();
            var methodInfo = obj.GetType().GetMethod(nameof(obj.StringFuncAsync));
            var methodInvoker = new MethodInvoker(methodInfo, obj.GetType());
            for (var i = 0; i < Count; i++)
            {
                var task = (Task) Task.Run(() => methodInvoker.InvokeAsync(obj, new object[] {Strings[i % Strings.Length]}));
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        [Description("[return:string] MethodInvoker.Invoke")]
        public void MethodInvokerInvoke_String()
        {
            var tasks = new List<Task>(Count);
            var obj = new TestClass();
            var methodInfo = obj.GetType().GetMethod(nameof(obj.StringFuncAsync));
            var methodInvoker = new MethodInvoker(methodInfo, obj.GetType());
            for (var i = 0; i < Count; i++)
            {
                var task = Task.Run(() => (Task) methodInvoker.Invoke(obj, new object[] {Strings[i % Strings.Length]}));
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        private class TestClass
        {
            public async Task VoidFuncAsync(string x)
            {
                await Task.Delay(1);
            }

            public async Task<string> StringFuncAsync(string x)
            {
                await Task.Delay(1);
                return x;
            }
        }
    }
}
