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
            Steps = 0,
            Skip = 10, // Ignored in batch run
            OpenPositionQty = 100,
            ComissionPercent = 0.00m,
            Intraday = null // Ignored in batchByDay run
        };

    [Test]
    public async Task CanRunBackTestBatch()
    {
        var bt = new BackTestRunner(MarketDataService, PortfolioService, Mediator, NullLogger);
        await bt.InitMarketData(_dataSources);
        var res = await bt.RunBatch(_backTestParams, 10);
        var summaries = res.Select(r => r.GetSummary(_backTestParams.CurrencyCode));

        var avgTrading = summaries.Average(s => s.TradingAmount);
        var avgComissions = summaries.Average(s => s.Comissions);
        var avgTotal = summaries.Average(s => s.TotalAmount);

        Console.WriteLine($"AVG: Trading={avgTrading:c} Comissions={avgComissions:c} Total={avgTotal:c}");
        Assert.Pass();
    }

    [Test]
    public async Task CanRunBackTestBatchByDay()
    {
        var bt = new BackTestRunner(MarketDataService, PortfolioService, Mediator, NullLogger);
        await bt.InitMarketData(_dataSources);

        var res = await bt.RunBatchByDay(_backTestParams, 20);

        foreach (var kvp in res)
        {
            var summaries = kvp.Value.Select(r => r.GetSummary(_backTestParams.CurrencyCode));
            var avgTrading = summaries.Average(s => s.TradingAmount);
            var avgComissions = summaries.Average(s => s.Comissions);
            var avgTotal = summaries.Average(s => s.TotalAmount);

            Console.WriteLine($"Day={kvp.Key} AVG: Trading={avgTrading:c} Comissions={avgComissions:c} Total={avgTotal:c}");
        }

        Assert.Pass();
    }
}
