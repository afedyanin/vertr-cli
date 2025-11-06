using System.Text;
using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Application.Tests.Handlers;

public class BackTestExecuteStepHandlerTests : AppliactionTestBase
{
    [Test]
    public async Task CanRunBackTest()
    {
        var request = new BackTestExecuteStepRequest
        {
            Symbol = "SBER",
            PortfolioId = Guid.NewGuid(),
            QtyLots = 1,
            Time = new DateTime(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var res = await Mediator.Send(request);

        Assert.That(res, Is.Not.Null);

        Console.WriteLine(DumpItems(res.Items));
    }

    private static string DumpItems(IDictionary<string, object> items)
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
