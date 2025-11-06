
using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Requests.Predictor;

namespace Vertr.CommandLine.Application.Handlers.Predictor
{
    public class GetNextPriceHandler : IRequestHandler<GetNextPriceRequest, GetNextPriceResponse>
    {
        public Task<GetNextPriceResponse> Handle(GetNextPriceRequest request, CancellationToken cancellationToken = default)
        {
            var response = new GetNextPriceResponse()
            {
                // TODO: Implement this
                Price = 105
            };

            return Task.FromResult(response);
        }
    }
}
