using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.Orders;

namespace Vertr.CommandLine.Application.Handlers.Orders;

public class PostOrderHandler : IRequestHandler<PostOrderRequest, PostOrderResponse>
{
    private readonly IOrderExecutionService _orderExecutionService;

    public PostOrderHandler(IOrderExecutionService orderExecutionService)
    {
        _orderExecutionService = orderExecutionService;
    }

    public async Task<PostOrderResponse> Handle(PostOrderRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var trades = await _orderExecutionService.PostOrder(request.Symbol, request.Qty, request.MarketTime);
            return new PostOrderResponse
            {
                Trades = trades,
            };
        }
        catch (Exception ex)
        {
            return new PostOrderResponse
            {
                Message = $"Post order error. Message={ex.Message}",
                Exception = ex
            };
        }
    }
}
