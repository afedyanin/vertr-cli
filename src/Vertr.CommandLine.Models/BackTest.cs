using Microsoft.Extensions.Logging;
using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Models;

public class BackTest
{
    private readonly BackTestParams _backTestParams;
    private readonly IMediator _mediator;
    private readonly ILogger _logger;

    public BackTest(
        BackTestParams backTestParams,
        IMediator mediator,
        ILogger logger)
    {
        _backTestParams = backTestParams;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<BackTestResult> Run()
    {
        var result = new BackTestResult();
        var current = _backTestParams.From;

        while (current <= _backTestParams.To)
        {
            var request = new BackTestExecuteStepRequest
            {
                Time = current,
                Symbol = _backTestParams.Symbol,
                PortfolioId = _backTestParams.PortfolioId,
                QtyLots = _backTestParams.QtyLots,
            };

            var response = await _mediator.Send(request);

            if (response.HasErrors)
            {
                _logger.LogError(response.Exception, $"Step {current:O}. Error:{response.Message}");
            }
            else if (response.Positions.Any())
            {
                var positions = response.Positions;
                result.Positions[current] = positions;
                // result.Trades[current] = response.Trades;
                var positionsString = string.Join(',', positions.Select(p => p.ToString()));
                _logger.LogDebug($"Step {current:O}. Positions:{positions}");
            }
            else
            {
                _logger.LogDebug($"Step {current:O}. Message:{response.Message}");
            }

            current += _backTestParams.Step;
        }

        return result;
    }
}
