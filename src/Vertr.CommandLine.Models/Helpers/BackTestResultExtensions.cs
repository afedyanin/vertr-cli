using System.Text;
using Vertr.CommandLine.Models.BackTest;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Models.Helpers
{
    public static class BackTestResultExtensions
    {
        public static string DumpLastStep(this BackTestResult backTestResult)
        {
            return backTestResult.DumpStep(backTestResult.Items.Keys.OrderBy(k => k).Last());
        }

        public static IEnumerable<string> DumpAll(this BackTestResult backTestResult)
        {
            var res = new List<string>();

            foreach (var item in backTestResult.Items)
            {
                res.Add(DumpItems(item.Value));
            }

            return res;
        }

        public static string DumpStep(this BackTestResult backTestResult, DateTime timeStep)
        {
            backTestResult.Items.TryGetValue(timeStep, out var items);
            return DumpItems(items);
        }

        public static string DumpCloseStep(this BackTestResult backTestResult)
        {
            return DumpItems(backTestResult.FinalClosePositionsResult ?? []);
        }

        internal static string DumpItems(IDictionary<string, object>? items)
        {
            if (items == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach (var item in items)
            {
                if (item.Key == BackTestContextKeys.Trades)
                {
                    var trades = item.Value as Trade[];

                    if (trades is null || trades.Length == 0)
                    {
                        continue;
                    }

                    sb.AppendLine("Trades:");

                    foreach (var trade in trades)
                    {
                        sb.AppendLine($"\t{trade}");
                    }

                    continue;
                }

                if (item.Key == BackTestContextKeys.Positions)
                {
                    var positions = item.Value as Position[];

                    if (positions is null || positions.Length == 0)
                    {
                        continue;
                    }

                    sb.AppendLine("Positions:");

                    foreach (var position in positions)
                    {
                        sb.AppendLine($"\t{position}");
                    }

                    continue;
                }

                sb.AppendLine($"{item.Key}={item.Value}");
            }

            return sb.ToString();
        }
    }
}
