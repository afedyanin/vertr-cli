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

        var enumerable = _marketDataService.GetEnumerable(_backTestParams.Symbol);
        Debug.Assert(enumerable != null);

        return result;

        /*
        foreach (var timeStep in enumerator!)
        {

        }


        while (current < _backTestParams.To)
        {
            var request = new BackTestExecuteStepRequest
            {
                Time = current,
                Symbol = _backTestParams.Symbol,
                PortfolioId = _backTestParams.PortfolioId,
                CurrencyCode = _backTestParams.CurrencyCode,
            };

            var response = await _mediator.Send(request);

            if (response.HasErrors)
            {
                _logger.LogError(response.Exception, $"Step {current:O}. Error:{response.Message}");
            }

            result.Items[current] = response.Items;
            current += _backTestParams.Step;
        }

        var closeRequest = new BackTestClosePositionRequest
        {
            MarketTime = current,
            Symbol = _backTestParams.Symbol,
            PortfolioId = _backTestParams.PortfolioId,
            CurrencyCode = _backTestParams.CurrencyCode
        };

        var closeResponse = await _mediator.Send(closeRequest);

        if (closeResponse.HasErrors)
        {
            _logger.LogError(closeResponse.Exception, $"Step {current:O}. Error:{closeResponse.Message}");
        }

        result.Items[current] = closeResponse.Items;

        return result;
        */
    }
}
