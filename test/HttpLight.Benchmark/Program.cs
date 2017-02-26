using System;
using System.Data;
using System.Linq;
using System.Threading;
using HttpLight.Benchmark.Utils;

namespace HttpLight.Benchmark
{
    internal class Program
    {
        private enum IndicatorStatus
        {
            Stopped,
            Started,
            Stopping
        }

        private static IndicatorStatus _indicator = IndicatorStatus.Stopped;
        private static int _indicatorPosition = 0;

        internal static void Main(string[] args)
        {
            var collection = new BenchmarkCollection();
            var runner = new BenchmarkRunner();
            foreach (var benchmark in collection)
            {
                Console.WriteLine(benchmark.Name + " (" + benchmark.Iterations + " iterations) [Y/N]");
                var key = Console.ReadKey();
                ClearLine();
                Console.WriteLine();
                if (key.Key != ConsoleKey.Y)
                    continue;

                var table = new DataTable();
                table.Columns.Add("Case");
                table.Columns.Add("Elapsed, ms");
                table.Columns.Add("Elapsed, ticks");
                table.Columns.Add("Memory, KB");

                StartIndicator();
                var result = runner.Run(benchmark, benchmark.Iterations);

                foreach (var @case in result)
                {
                    var row = table.NewRow();
                    row[0] = @case.Name;
                    row[1] = (long) @case.ElapsedTime.TotalMilliseconds;
                    row[2] = @case.ElapsedTime.Ticks;
                    row[3] = (@case.MaxMemoryUsage / 1024.0).ToString(".000");
                    table.Rows.Add(row);
                }

                StopIndncator();
                ClearLine();

                PrintTable(table);
                Console.WriteLine();
            }

            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }

        private static void PrintTable(DataTable table)
        {
            var columnWidth = table.Columns.Cast<DataColumn>().Select(x => GetColumnWidth(x)).ToArray();
            PrintRow(table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray(), columnWidth);
            PrintRow(columnWidth.Select(x => new string('-', x)).ToArray(), columnWidth);
            foreach (DataRow row in table.Rows)
                PrintRow(row.ItemArray.Select(x => x.ToString()).ToArray(), columnWidth);
        }

        private static int GetColumnWidth(DataColumn column)
        {
            var width = column.ColumnName.Length;
            foreach (DataRow row in column.Table.Rows)
                width = Math.Max(width, row[column].ToString().Length);
            return width;
        }

        private static void PrintRow(string[] values, int[] columnWidth)
        {
            var format = string.Join(" | ", columnWidth.Select((x, i) => "{" + i + ",-" + x + "}"));
            Console.WriteLine(format, values.Select(x => (object) x).ToArray());
        }

        private static void StartIndicator()
        {
            _indicator = IndicatorStatus.Started;
            _indicatorPosition = 0;
            new Thread(IndicatorThread) {IsBackground = true}.Start();
        }

        private static void StopIndncator()
        {
            _indicator = IndicatorStatus.Stopping;
            while (_indicator == IndicatorStatus.Stopping)
            {
                Thread.Sleep(1);
            }
        }

        private static void IndicatorThread()
        {
            while (_indicator == IndicatorStatus.Started)
            {
                Thread.Sleep(100);
                _indicatorPosition++;
                char c;
                switch (_indicatorPosition % 4)
                {
                    case 0:
                        c = '-';
                        break;
                    case 1:
                        c = '\\';
                        break;
                    case 2:
                        c = '|';
                        break;
                    default:
                        c = '/';
                        break;
                }
                ClearLine();
                Console.Write("Processing... " + c);
            }
            _indicator = IndicatorStatus.Started;
        }

        private static void ClearLine()
        {
            Console.CursorLeft = 0;
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.CursorLeft = 0;
        }
    }
}
