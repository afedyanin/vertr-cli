using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.Portfolio;

namespace Vertr.CommandLine.Application.Handlers.Portfolio;

public class UpdatePositionsHandler : IRequestHandler<UpdatePositionsRequest, UpdatePositionsResponse>
{
    private readonly IPortfolioService _portfolioService;

    public UpdatePositionsHandler(IPortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    public Task<UpdatePositionsResponse> Handle(UpdatePositionsRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var positions = _portfolioService.Update(request.PortfolioId, request.Symbol, request.Trades, request.CurrencyCode);

            var response = new UpdatePositionsResponse
            {
                Positions = positions,
            };

            return Task.FromResult(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new UpdatePositionsResponse
            {
                Exception = ex,
                Message = $"Error updating positions for portfolioId={request.PortfolioId} Message={ex.Message}",
            };

            return Task.FromResult(errorResponse);
        }
    }
}
