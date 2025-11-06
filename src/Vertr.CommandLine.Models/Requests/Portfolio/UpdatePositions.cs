using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Models.Requests.Portfolio;

public class UpdatePositionsRequest : IRequest<UpdatePositionsResponse>
{
    public Guid PortfolioId { get; init; }

    public Trade[] Trades { get; init; } = [];
}

public class UpdatePositionsResponse : ResponseBase
{
    public Position[] Positions { get; init; } = [];
}
