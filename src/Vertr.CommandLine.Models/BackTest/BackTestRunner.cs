using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Helpers;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Models.BackTest;

public class BackTestRunner
{
    private readonly BackTestParams _backTestParams;
    private readonly IMediator _mediator;
    private readonly IMarketDataService _marketDataService;
    private readonly ILogger _logger;

    public BackTestRunner(
        BackTestParams backTestParams,
        IMarketDataService marketDataService,
        IMediator mediator,
        ILogger logger)
    {
        _backTestParams = backTestParams;
        _marketDataService = marketDataService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<BackTestResult> Run()
    {
        var candles = CsvImporter.LoadCandles(_backTestParams.DataSourceFilePath);
        await _marketDataService.LoadData(_backTestParams.Symbol, [.. candles]);
        var timeIndex = await _marketDataService.GetEnumerable(_backTestParams.Symbol);
        var candleRange = await _marketDataService.GetCandleRange(_backTestParams.Symbol);
        Trace.Assert(candleRange != null);

        var result = new BackTestResult();

        var stepCount = 0;
        var closeTime = candleRange.LastDate;
        var maxSteps = _backTestParams.MaxSteps > 0 ?
            Math.Min(_backTestParams.MaxSteps, candleRange.Count) : 
            candleRange.Count;

        foreach (var timeStep in timeIndex)
        {
            if (stepCount++ >= maxSteps)
            {
                closeTime = timeStep;
                break;
            }
            result.Items[timeStep] = await ExecuteStep(timeStep);
        }

        result.FinalClosePositionsResult = await ClosePositionsStep(closeTime);

        return result;
    }

    private async Task<Dictionary<string, object>> ExecuteStep(DateTime timeStep)
    {
        var request = new BackTestExecuteStepRequest
        {
            Time = timeStep,
            Symbol = _backTestParams.Symbol,
            PortfolioId = _backTestParams.PortfolioId,
            CurrencyCode = _backTestParams.CurrencyCode,
            OpenPositionQty = _backTestParams.OpenPositionQty,
            ComissionPercent = _backTestParams.ComissionPercent,
        };

        var response = await _mediator.Send(request);

        if (response.HasErrors)
        {
            _logger.LogError(response.Exception, $"Step {timeStep:O}. Error:{response.Message}");
        }

        return response.Items;
    }

    private async Task<Dictionary<string, object>> ClosePositionsStep(DateTime closeDate)
    {
        var closeRequest = new BackTestClosePositionRequest
        {
            MarketTime = closeDate,
            Symbol = _backTestParams.Symbol,
            PortfolioId = _backTestParams.PortfolioId,
            CurrencyCode = _backTestParams.CurrencyCode,
            ComissionPercent = _backTestParams.ComissionPercent
        };

        var closeResponse = await _mediator.Send(closeRequest);

        if (closeResponse.HasErrors)
        {
            _logger.LogError(closeResponse.Exception, $"Close positions step. Error:{closeResponse.Message}");
        }

        return closeResponse.Items;
    }
}
