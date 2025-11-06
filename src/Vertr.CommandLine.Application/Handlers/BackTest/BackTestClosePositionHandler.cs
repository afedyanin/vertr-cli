using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Application.Handlers.BackTest
{
    public class BackTestClosePositionHandler : IRequestHandler<BackTestClosePositionRequest, BackTestClosePostionResponse>
    {
        public Task<BackTestClosePostionResponse> Handle(
            BackTestClosePositionRequest request, 
            CancellationToken cancellationToken = default)
        {
            // Get Current position
            // Post order and get trades
            // Update positions
            throw new NotImplementedException();
        }
    }
}
