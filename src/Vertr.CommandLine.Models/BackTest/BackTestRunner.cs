using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Helpers;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Models.BackTest;

public class BackTestRunner
{
    private readonly IMediator _mediator;
    private readonly IMarketDataService _marketDataService;
    private readonly ILogger _logger;
    private readonly Dictionary<string, CandleRange?> _candleRanges = [];

    public BackTestRunner(
        IMarketDataService marketDataService,
        IMediator mediator,
        ILogger logger)
    {
        _marketDataService = marketDataService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task InitMarketData(FileDataSource[] dataSources)
    {
        _candleRanges.Clear();

        foreach (var dataSource in dataSources)
        {
            var candles = CsvImporter.LoadCandles(dataSource.FilePath);
            await _marketDataService.LoadData(dataSource.Symbol, [.. candles]);
            _candleRanges[dataSource.Symbol] = await _marketDataService.GetCandleRange(dataSource.Symbol);
        }
    }

    public async Task<BackTestResult> Run(BackTestParams backTestParams)
    {
        _candleRanges.TryGetValue(backTestParams.Symbol, out var candleRange);
        Trace.Assert(candleRange != null);

        var result = new BackTestResult();

        var stepCount = 0;
        var closeTime = candleRange.LastDate;
        var maxSteps = backTestParams.Steps > 0 ?
            Math.Min(backTestParams.Steps + backTestParams.Skip, candleRange.Count) :
            candleRange.Count;

        var timeIndex = await _marketDataService.GetEnumerable(backTestParams.Symbol);
        foreach (var timeStep in timeIndex)
        {
            if (stepCount++ >= maxSteps)
            {
                closeTime = timeStep;
                break;
            }

            if (stepCount < backTestParams.Skip)
            {
                continue;
            }

            result.Items[timeStep] = await ExecuteStep(timeStep, backTestParams);
        }

        result.FinalClosePositionsResult = await ClosePositionsStep(closeTime, backTestParams);

        return result;
    }

    private async Task<Dictionary<string, object>> ExecuteStep(DateTime timeStep, BackTestParams backTestParams)
    {
        var request = new BackTestExecuteStepRequest
        {
            Time = timeStep,
            Symbol = backTestParams.Symbol,
            PortfolioId = backTestParams.PortfolioId,
            CurrencyCode = backTestParams.CurrencyCode,
            OpenPositionQty = backTestParams.OpenPositionQty,
            ComissionPercent = backTestParams.ComissionPercent,
        };

        var response = await _mediator.Send(request);

        if (response.HasErrors)
        {
            _logger.LogError(response.Exception, $"Step {timeStep:O}. Error:{response.Message}");
        }

        return response.Items;
    }

    private async Task<Dictionary<string, object>> ClosePositionsStep(DateTime closeDate, BackTestParams backTestParams)
    {
        var closeRequest = new BackTestClosePositionRequest
        {
            MarketTime = closeDate,
            Symbol = backTestParams.Symbol,
            PortfolioId = backTestParams.PortfolioId,
            CurrencyCode = backTestParams.CurrencyCode,
            ComissionPercent = backTestParams.ComissionPercent
        };

        var closeResponse = await _mediator.Send(closeRequest);

        if (closeResponse.HasErrors)
        {
            _logger.LogError(closeResponse.Exception, $"Close positions step. Error:{closeResponse.Message}");
        }

        return closeResponse.Items;
    }
}
