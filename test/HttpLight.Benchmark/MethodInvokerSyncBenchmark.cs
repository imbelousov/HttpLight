using System.ComponentModel;
using HttpLight.Benchmark.Attributes;
using HttpLight.Utils;

namespace HttpLight.Benchmark
{
    [Benchmark("MethodInvoke.Invoke")]
    internal class MethodInvokerSyncBenchmark
    {
        private const int Count = 200000;
        private static readonly string[] Strings = {"1", "2", "3", "4", "5"};

        [Description("[value type] Direct call")]
        public int DirectCall_Value()
        {
            var obj = new TestClass();
            var sum = 0;
            for (var i = 0; i < Count; i++)
                sum = obj.ValueFunc(i, i + 1, i + 2);
            return sum;
        }

        [Description("[value type] MethodInfo.Invoke")]
        public int MethodInfoInvoke_Value()
        {
            var obj = new TestClass();
            var methodInfo = obj.GetType().GetMethod(nameof(obj.ValueFunc));
            var sum = 0;
            for (var i = 0; i < Count; i++)
                sum = (int) methodInfo.Invoke(obj, new object[] {i, i + 1, i + 2});
            return sum;
        }

        [Description("[value type] MethodInvoker.Invoke")]
        public int MethodInvokerInvoke_Value()
        {
            var obj = new TestClass();
            var methodInfo = obj.GetType().GetMethod(nameof(obj.ValueFunc));
            var methodInvoker = new MethodInvoker(methodInfo, obj.GetType());
            var sum = 0;
            for (var i = 0; i < Count; i++)
                sum = (int) methodInvoker.Invoke(obj, new object[] {i, i + 1, i + 2});
            return sum;
        }

        [Description("[reference type] Direct call")]
        public string DirectCall_Reference()
        {
            var obj = new TestClass();
            var result = string.Empty;
            for (var i = 0; i < Count; i++)
                result = obj.ReferenceFunc(Strings[i % Strings.Length]);
            return result;
        }

        [Description("[reference type] MethodInfo.Invoke")]
        public string MethodInfoInvoke_Reference()
        {
            var obj = new TestClass();
            var methodInfo = obj.GetType().GetMethod(nameof(obj.ReferenceFunc));
            var result = string.Empty;
            for (var i = 0; i < Count; i++)
                result = (string) methodInfo.Invoke(obj, new object[] {Strings[i % Strings.Length]});
            return result;
        }

        [Description("[reference type] MethodInvoker.Invoke")]
        public string MethodInvokerInvoke_Reference()
        {
            var obj = new TestClass();
            var methodInfo = obj.GetType().GetMethod(nameof(obj.ReferenceFunc));
            var methodInvoker = new MethodInvoker(methodInfo, obj.GetType());
            var result = string.Empty;
            for (var i = 0; i < Count; i++)
                result = (string) methodInvoker.Invoke(obj, new object[] {Strings[i % Strings.Length]});
            return result;
        }

        private class TestClass
        {
            public int ValueFunc(int x, int y, int z)
            {
                return x + y + z;
            }

            public string ReferenceFunc(string x)
            {
                return x + x;
            }
        }
    }
}
