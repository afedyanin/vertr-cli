using System.Text;
using Vertr.CommandLine.Models.BackTest;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Models.Helpers
{
    public static class BackTestResultExtensions
    {
        public static PositionSummary GetSummary(this BackTestResult backTestResult, string currencyCode)
        {
            var lastItems = backTestResult.FinalClosePositionsResult ?? [];

            if (lastItems.Count == 0)
            {
                var timeStep = backTestResult.Items.Keys.OrderBy(k => k).Last();
                backTestResult.Items.TryGetValue(timeStep, out var items);

                if (items != null && items.Count != 0)
                {
                    lastItems = items;
                }
            }

            lastItems.TryGetValue(BackTestContextKeys.Positions, out var positionsObj);
            var positions = positionsObj as Position[];

            return positions.GetSummary(currencyCode);
        }

        public static string DumpLastStep(this BackTestResult? backTestResult)
        {
            if (backTestResult == null)
            {
                return string.Empty;
            }

            return backTestResult.DumpStep(backTestResult.Items.Keys.OrderBy(k => k).Last());
        }

        public static IEnumerable<string> DumpAll(this BackTestResult? backTestResult)
        {
            if (backTestResult == null)
            {
                return [];
            }

            var res = new List<string>();

            foreach (var item in backTestResult.Items)
            {
                res.Add(DumpItems(item.Value));
            }

            return res;
        }

        public static string DumpStep(this BackTestResult? backTestResult, DateTime timeStep)
        {
            if (backTestResult == null)
            {
                return string.Empty;
            }

            backTestResult.Items.TryGetValue(timeStep, out var items);
            return DumpItems(items);
        }

        public static string DumpCloseStep(this BackTestResult? backTestResult)
        {
            return DumpItems(backTestResult?.FinalClosePositionsResult ?? []);
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
