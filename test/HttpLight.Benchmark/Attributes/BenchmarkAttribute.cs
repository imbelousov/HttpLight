using System;

namespace HttpLight.Benchmark.Attributes
{
    internal class BenchmarkAttribute : Attribute
    {
        public string Name { get; }
        public int Iterations { get; set; }

        public BenchmarkAttribute(string name)
        {
            Name = name;
        }
    }
}
