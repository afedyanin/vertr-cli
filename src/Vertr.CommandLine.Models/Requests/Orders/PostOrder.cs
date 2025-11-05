using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Models.Requests.Orders;

public class PostOrderRequest : IRequest<PostOrderResponse>
{
    public required string Symbol { get; set; }
    public required decimal Price { get; init; }
    public required long QuantityLots { get; init; }

}

public class PostOrderResponse
{
}
