using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Models.Requests.Orders;

public class TradingSignalRequest : IRequest<TradingSignalResponse>
{
    public Guid PortfolioId { get; init; }

    public required string Symbol { get; init; }

    public decimal QtyLots { get; init; }

    public decimal? Price { get; init; }
}

public class TradingSignalResponse : ResponseBase
{
    public Trade[] Trades { get; init; } = [];
}
