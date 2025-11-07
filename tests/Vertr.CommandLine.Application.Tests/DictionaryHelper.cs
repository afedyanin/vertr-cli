using System.Text;
using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Application.Tests
{
    internal static class DictionaryHelper
    {
        public static string DumpItems(IDictionary<string, object> items)
        {
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
