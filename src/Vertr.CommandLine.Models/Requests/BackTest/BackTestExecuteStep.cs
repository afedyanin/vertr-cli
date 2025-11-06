using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Models.Requests.BackTest;

public class BackTestExecuteStepRequest : IRequest<BackTestExecuteStepResponse>
{
    public DateTime Time { get; init; }

    public Guid PortfolioId { get; init; }

    public required string Symbol { get; init; }

    public decimal QtyLots { get; init; } = 10;
}

public class BackTestExecuteStepResponse : ResponseBase
{
    public Position[] Positions { get; init; } = [];
}
