using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.Orders;

namespace Vertr.CommandLine.Application.Handlers.Orders
{
    public class TradingSignalHandler : IRequestHandler<TradingSignalRequest, TradingSignalResponse>
    {
        private readonly IPortfolioService _portfolioService;
        private readonly IMediator _mediator;

        private readonly decimal _openQtyLots = 10;

        public TradingSignalHandler(
            IPortfolioService portfolioService,
            IMediator mediator)
        {
            _portfolioService = portfolioService;
            _mediator = mediator;
        }

        public async Task<TradingSignalResponse> Handle(TradingSignalRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Direction == Direction.Hold)
            {
                return new TradingSignalResponse()
                {
                    Message = "Ignore signal with Hold direction."
                };
            }

            var position = _portfolioService.GetPosition(request.PortfolioId, request.Symbol);

            // Open position
            if (position == null || position.Qty == 0)
            {
                var openRequest = new PostOrderRequest
                {
                    Symbol = request.Symbol,
                    QtyLots = _openQtyLots * (request.Direction == Direction.Buy ? 1 : -1),
                    MarketTime = request.MarketTime,
                };

                var openResponse = await _mediator.Send(openRequest, cancellationToken);

                return new TradingSignalResponse()
                {
                    Trades = openResponse.Trades,
                };
            }

            // Same direction
            if ((position.Qty > 0 && request.Direction == Direction.Buy) ||
                (position.Qty < 0 && request.Direction == Direction.Sell))
            {
                return new TradingSignalResponse()
                {
                    Message = "Position is already opened with signal direction."
                };
            }

            // Reverse
            var reverseRequest = new PostOrderRequest
            {
                Symbol = request.Symbol,
                QtyLots = position.Qty * (request.Direction == Direction.Buy ? 2 : -2),
                MarketTime = request.MarketTime,
            };

            var reverseResponse = await _mediator.Send(reverseRequest, cancellationToken);

            return new TradingSignalResponse()
            {
                Trades = reverseResponse.Trades,
            };
        }
    }
}
