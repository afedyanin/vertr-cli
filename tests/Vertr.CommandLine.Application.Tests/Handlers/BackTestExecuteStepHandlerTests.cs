using Vertr.CommandLine.Models.Helpers;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Application.Tests.Handlers;

public class BackTestExecuteStepHandlerTests : AppliactionTestBase
{
    [Test]
    public async Task CanRunBackTestStep()
    {
        var request = new BackTestExecuteStepRequest
        {
            Symbol = "SBER",
            CurrencyCode = "RUB",
            PortfolioId = Guid.NewGuid(),
            Time = new DateTime(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var res = await Mediator.Send(request);

        Assert.That(res, Is.Not.Null);

        Console.WriteLine(BackTestResultExtensions.DumpItems(res.Items));
    }
}
