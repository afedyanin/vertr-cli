
using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Models.Requests.MarketData;

public class GetCandlesRequest : IRequest<GetCandlesResponse>
{
    public required string Symbol { get; init; }
    public DateTime Time { get; init; }
    public int Count { get; init; } = 1;
}

public class GetCandlesResponse : ResponseBase
{
    public Candle[] Candles { get; init; } = [];
}
