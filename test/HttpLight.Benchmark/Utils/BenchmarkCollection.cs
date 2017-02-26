using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HttpLight.Benchmark.Attributes;

namespace HttpLight.Benchmark.Utils
{
    internal class BenchmarkCollection : IEnumerable<BenchmarkInfo>
    {
        private const int DefaultIterations = 100;

        private BenchmarkInfo[] _benchmarks;

        public BenchmarkCollection()
        {
            _benchmarks = LoadBenchmarks().ToArray();
        }

        public BenchmarkInfo Get(int id)
        {
            if (id >= 0 && id < _benchmarks.Length)
                return _benchmarks[id];
            return null;
        }

        public BenchmarkInfo Get(Type type)
        {
            return _benchmarks.SingleOrDefault(x => x.Type == type);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<BenchmarkInfo> GetEnumerator()
        {
            return _benchmarks.AsEnumerable().GetEnumerator();
        }

        private IEnumerable<BenchmarkInfo> LoadBenchmarks()
        {
            return GetType().
                Assembly
                .GetTypes()
                .Select(x => new {Type = x, Attr = x.GetCustomAttributes(typeof(BenchmarkAttribute), false).FirstOrDefault() as BenchmarkAttribute})
                .Where(x => x.Attr != null)
                .Select((x, i) => new BenchmarkInfo
                {
                    Id = i,
                    Name = x.Attr.Name,
                    Type = x.Type,
                    Iterations = x.Attr.Iterations != default(int) ? x.Attr.Iterations : DefaultIterations
                });
        }
    }

    internal class BenchmarkInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public int Iterations { get; set; }
    }
}
