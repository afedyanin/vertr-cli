using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Models.Requests.Predictor;

public class GetNextPriceRequest : IRequest<GetNextPriceResponse>
{
    public required string Symbol { get; init; }

    public Candle[] Candles { get; init; } = [];
}

public class GetNextPriceResponse : ResponseBase
{
    public decimal? Price { get; init; }
}
