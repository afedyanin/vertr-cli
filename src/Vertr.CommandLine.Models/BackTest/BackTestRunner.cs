using Microsoft.Extensions.Logging;
using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Models.BackTest;

public class BackTestRunner
{
    private readonly BackTestParams _backTestParams;
    private readonly IMediator _mediator;
    private readonly ILogger _logger;

    public BackTestRunner(
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

            result.Items[current] = response.Items;
            current += _backTestParams.Step;
        }

        return result;
    }
}
