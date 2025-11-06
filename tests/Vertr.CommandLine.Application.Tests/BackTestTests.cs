using Vertr.CommandLine.Models;

namespace Vertr.CommandLine.Application.Tests;

public class BackTestTests
{
    [Test]
    public async Task CanRunBackTest()
    {
        var portfolioId = Guid.NewGuid();
        var bt = new BackTest("SYMBOL", portfolioId);

        var from = new DateTime(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2025, 11, 5, 23, 59, 59, DateTimeKind.Utc);
        var step = TimeSpan.FromHours(1);

        await bt.Run(from, to, step);

        Assert.Pass();
    }
}
