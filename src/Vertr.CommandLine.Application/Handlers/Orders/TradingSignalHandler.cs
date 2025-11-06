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
                Price = request.Price == 0 ? 105m : request.Price,
                Quantity = request.QtyLots,
                TradeId = Guid.NewGuid().ToString(),
                TradeComission = 0.21m,
            };

            var response = new TradingSignalResponse()
            {
                Trades = [trade],
            };

            return Task.FromResult(response);
        }
    }
}
