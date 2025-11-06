using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Models.Requests.Portfolio;

public class UpdatePositionsRequest : IRequest<UpdatePositionsResponse>
{
    public Guid PortfolioId { get; init; }

    public required string Symbol { get; init; }

    public Trade[] Trades { get; init; } = [];

    public required string CurrencyCode { get; init; } = "RUB";
}

public class UpdatePositionsResponse : ResponseBase
{
    public Position[] Positions { get; init; } = [];
}
