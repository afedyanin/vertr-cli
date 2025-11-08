using Vertr.CommandLine.Models.BackTest;
using Vertr.CommandLine.Models.Helpers;

namespace Vertr.CommandLine.Models.Tests.BackTest;

public class BackTestBatchRunnerTests : SystemTestBase
{
    private static readonly FileDataSource[] _dataSources =
    {
        new FileDataSource
        {
            Symbol = "SBER",
            FilePath = "Data\\SBER_251101_251104.csv",
        }
    };

    private static readonly BackTestParams _backTestParams =
        new BackTestParams
        {
            PortfolioId = Guid.NewGuid(),
            Symbol = "SBER",
            CurrencyCode = "RUB",
            Steps = 30,
            Skip = 10,
            OpenPositionQty = 100,
            ComissionPercent = 0.001m,
        };

    [Test]
    public async Task CanRunBackTestBatch()
    {
        var bt = new BackTestRunner(MarketDataService, Mediator, NullLogger);
        await bt.InitMarketData(_dataSources);
        var res = await bt.RunBatch(_backTestParams, 10);
        var summaries = res.Select(r => r.GetSummary(_backTestParams.CurrencyCode));

        var avgTrading = summaries.Average(s => s.TradingAmount);
        var avgComissions = summaries.Average(s => s.Comissions);
        var avgTotal = summaries.Average(s => s.TotalAmount);

        Console.WriteLine($"AVG: Trading={avgTrading:c} Comissions={avgComissions:c} Total={avgTotal:c}");
        Assert.Pass();
    }
}
