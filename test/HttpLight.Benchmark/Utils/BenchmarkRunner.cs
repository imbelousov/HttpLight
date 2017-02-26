using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace HttpLight.Benchmark.Utils
{
    internal class BenchmarkRunner
    {
        public IEnumerable<BenchmarkCase> Run(BenchmarkInfo benchmark, int iterations = 100)
        {
            Debug.Assert(iterations > 0);
            var instance = Activator.CreateInstance(benchmark.Type);
            var objectMethods = typeof(object).GetMethods(BindingFlags.Instance | BindingFlags.Public).Select(x => x.Name).ToList();
            var methods = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(x => !objectMethods.Contains(x.Name));
            foreach (var method in methods)
            {
                var cases = new List<BenchmarkCase>();
                for (var i = 0; i < iterations; i++)
                {
                    var @case = new BenchmarkCase();
                    var nameAttribute = method.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
                    @case.Name = nameAttribute != null ? nameAttribute.Description : method.Name;
                    var measuringHandle = BeginMeasureMaxMemory();
                    GC.Collect();
                    var sw = Stopwatch.StartNew();
                    method.Invoke(instance, new object[0]);
                    sw.Stop();
                    EndMeasureMaxMemory(measuringHandle);
                    @case.ElapsedTime = sw.Elapsed;
                    @case.ElapsedTicks = sw.ElapsedTicks;
                    @case.MaxMemoryUsage = measuringHandle.MaxUsage;
                    cases.Add(@case);
                }
                yield return new BenchmarkCase
                {
                    Name = cases.First().Name,
                    ElapsedTime = TimeSpan.FromMilliseconds(cases.Average(x => x.ElapsedTime.TotalMilliseconds)),
                    ElapsedTicks = (long) cases.Average(x => x.ElapsedTicks),
                    MaxMemoryUsage = (long) cases.Average(x => x.MaxMemoryUsage)
                };
            }
        }

        private MemoryMeasuringHandle BeginMeasureMaxMemory()
        {
            var initnalMemoryUsage = GetMemoryUsage();
            var handle = new MemoryMeasuringHandle {Enabled = true, MaxUsage = 0L};
            new Thread(() =>
            {
                for (var i = 0; handle.Enabled; i++)
                {
                    if (i % 20 == 19)
                        Thread.Sleep(1);
                    handle.MaxUsage = Math.Max(handle.MaxUsage, GetMemoryUsage() - initnalMemoryUsage);
                }
            }).Start();
            return handle;
        }

        private long EndMeasureMaxMemory(MemoryMeasuringHandle handle)
        {
            handle.Enabled = false;
            return handle.MaxUsage;
        }

        private long GetMemoryUsage()
        {
            return Process.GetCurrentProcess().PrivateMemorySize64;
        }

        private class MemoryMeasuringHandle
        {
            public bool Enabled { get; set; }
            public long MaxUsage { get; set; }
        }
    }

    internal class BenchmarkCase
    {
        public string Name { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public long ElapsedTicks { get; set; }
        public long MaxMemoryUsage { get; set; }
    }
}
