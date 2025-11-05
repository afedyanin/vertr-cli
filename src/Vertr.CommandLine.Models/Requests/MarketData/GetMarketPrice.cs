using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Models.Requests.MarketData;

public class GetMarketPriceRequest : IRequest<GetMarketPriceResponse>
{
    public required string Symbol { get; init; }
    public DateTime Time { get; init; }
}

public class GetMarketPriceResponse : ResponseBase
{
    public decimal Price { get; init; }
}
