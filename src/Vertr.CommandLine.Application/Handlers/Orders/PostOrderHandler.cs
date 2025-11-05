using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Requests.Orders;

namespace Vertr.CommandLine.Application.Handlers.Orders;

internal class PostOrderHandler : IRequestHandler<PostOrderRequest, PostOrderResponse>
{
    public Task<PostOrderResponse> Handle(PostOrderRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
