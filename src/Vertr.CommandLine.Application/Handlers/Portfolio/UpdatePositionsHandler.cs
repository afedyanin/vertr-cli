using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Requests.Portfolio;

namespace Vertr.CommandLine.Application.Handlers.Portfolio;

public class UpdatePositionsHandler : IRequestHandler<UpdatePositionsRequest, UpdatePositionsResponse>
{
    public Task<UpdatePositionsResponse> Handle(UpdatePositionsRequest request, CancellationToken cancellationToken = default)
    {
        // TODO: Implement this
        var positionMoney = new Position
        {
            PortfolioId = request.PortfolioId,
            Symbol = "RUB",
            Qty = request.Comission,
        };

        var positionSymbol = new Position 
        {
            PortfolioId = request.PortfolioId,
            Symbol = request.Symbol,
            Qty = request.TradeQty,
        };

        var response = new UpdatePositionsResponse
        {
            Positions = [positionMoney, positionSymbol]
        };

        return Task.FromResult(response);
    }
}
