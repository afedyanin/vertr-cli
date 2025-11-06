using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Requests.Orders;

namespace Vertr.CommandLine.Application.Handlers.Orders
{
    public class TradingSignalHandler : IRequestHandler<TradingSignalRequest, TradingSignalResponse>
    {
        public Task<TradingSignalResponse> Handle(TradingSignalRequest request, CancellationToken cancellationToken = default)
        {
            // TODO: Implement this 
            var trade = new Trade
            {
                ExecutionTime = DateTime.UtcNow,
                PortfolioId = request.PortfolioId,
                Price = request.Price ?? decimal.Zero,
                Quantity = request.QtyLots,
                TradeId = Guid.NewGuid().ToString(),
                Comission = 0.21m,
            };

            var response = new TradingSignalResponse()
            {
                Trades = [trade],
            };

            return Task.FromResult(response);
        }
    }
}
