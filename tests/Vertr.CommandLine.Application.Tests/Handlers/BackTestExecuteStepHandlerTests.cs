using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Application.Tests.Handlers;

public class BackTestExecuteStepHandlerTests
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

        var handler = new BackTestExecuteStepHandler();


        Assert.Pass();
    }
}
