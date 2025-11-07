using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Models.Requests.Orders;

public class PostOrderRequest : IRequest<PostOrderResponse>
{
    public required string Symbol { get; set; }
    public decimal? Price { get; init; }
    public decimal Qty { get; init; }
    public DateTime? MarketTime {  get; init; }
}

public class PostOrderResponse : ResponseBase
{
    public Trade[] Trades { get; init; } = [];
}
