using Vertr.CommandLine.Application.Services;
using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.BackTest;
using Vertr.CommandLine.Models.Helpers;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Application.Tests.Handlers;

public class BackTestExecuteStepHandlerTests : AppliactionTestBase
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
            Steps = 3,
            OpenPositionQty = 100,
            ComissionPercent = 0.001m,
        };

    [Test]
    public async Task CanRunBackTestStep()
    {
        var candles = CsvImporter.LoadCandles(_dataSources[0].FilePath);
        Assert.That(candles, Is.Not.Null);
        await MarketDataService.LoadData(_backTestParams.Symbol, [.. candles]);

        var candleRange = await MarketDataService.GetCandleRange(_backTestParams.Symbol);
        Assert.That(candleRange, Is.Not.Null);
        Console.WriteLine($"CandleRange={candleRange}");

        var request = new BackTestExecuteStepRequest
        {
            Symbol = _backTestParams.Symbol,
            CurrencyCode = _backTestParams.CurrencyCode,
            PortfolioId = _backTestParams.PortfolioId,
            ComissionPercent = _backTestParams.ComissionPercent,
            OpenPositionQty= _backTestParams.OpenPositionQty,
            Predictor = Models.Requests.Predictor.PredictorType.LastValue,
            PriceThreshold = 0,
            Time = candleRange.FirstDate,
        };

        var res = await Mediator.Send(request);

        Assert.That(res, Is.Not.Null);

        Console.WriteLine(BackTestResultExtensions.DumpItems(res.Items));
    }
}
