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
        var result = new BackTestResult();
        
        var candles = CsvImporter.LoadCandles(_backTestParams.DataSourceFilePath);
        Debug.Assert(candles != null);

        await _marketDataService.LoadData(_backTestParams.Symbol, [.. candles]);

        var timeIndex = _marketDataService.GetEnumerable(_backTestParams.Symbol);
        Debug.Assert(timeIndex != null);

        foreach (var timeStep in timeIndex)
        {
            var request = new BackTestExecuteStepRequest
            {
                Time = timeStep,
                Symbol = _backTestParams.Symbol,
                PortfolioId = _backTestParams.PortfolioId,
                CurrencyCode = _backTestParams.CurrencyCode,
            };

            var response = await _mediator.Send(request);

            if (response.HasErrors)
            {
                _logger.LogError(response.Exception, $"Step {timeStep:O}. Error:{response.Message}");
            }

            result.Items[timeStep] = response.Items;
        }

        (DateTime? first, DateTime? last) = _marketDataService.GetTimeRange(_backTestParams.Symbol);
        Debug.Assert(last != null);

        var closeRequest = new BackTestClosePositionRequest
        {
            MarketTime = last.Value,
            Symbol = _backTestParams.Symbol,
            PortfolioId = _backTestParams.PortfolioId,
            CurrencyCode = _backTestParams.CurrencyCode
        };

        var closeResponse = await _mediator.Send(closeRequest);

        if (closeResponse.HasErrors)
        {
            _logger.LogError(closeResponse.Exception, $"Step close. Error:{closeResponse.Message}");
        }

        // Перезатирает, нужно отдельно хранить
        result.Items[last.Value] = closeResponse.Items;

        return result;
    }
}
