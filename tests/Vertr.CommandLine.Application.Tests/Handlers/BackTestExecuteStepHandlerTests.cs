using Vertr.CommandLine.Application.Handlers.BackTest;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Application.Tests.Handlers;

public class BackTestExecuteStepHandlerTests : AppliactionTestBase
{
    [Test]
    public async Task CanRunBackTest()
    {
        var handler = new BackTestExecuteStepHandler(Mediator);

        var request = new BackTestExecuteStepRequest
        {
            Symbol = "SBER",
            PortfolioId = Guid.NewGuid(),
            QtyLots = 1,
            Time = new DateTime(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var res = await handler.Handle(request);

        Assert.That(res, Is.Not.Null);
    }
}
