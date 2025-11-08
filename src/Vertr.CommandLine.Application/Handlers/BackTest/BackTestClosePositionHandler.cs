using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.BackTest;
using Vertr.CommandLine.Models.Requests.Portfolio;

namespace Vertr.CommandLine.Application.Handlers.BackTest
{
    public class BackTestClosePositionHandler : IRequestHandler<BackTestClosePositionRequest, BackTestClosePostionResponse>
    {
        private readonly IPortfolioService _portfolioService;
        private readonly IOrderExecutionService _orderExecutionService;
        private readonly IMarketDataService _marketDataService;
        private readonly IMediator _mediator;

        public BackTestClosePositionHandler(
            IPortfolioService portfolioService,
            IOrderExecutionService orderExecutionService,
            IMarketDataService marketDataService,
            IMediator mediator)
        {
            _portfolioService = portfolioService;
            _orderExecutionService = orderExecutionService;
            _marketDataService = marketDataService;
            _mediator = mediator;
        }

        public async Task<BackTestClosePostionResponse> Handle(
            BackTestClosePositionRequest request, 
            CancellationToken cancellationToken = default)
        {
            var position = _portfolioService.GetPosition(request.PortfolioId, request.Symbol);

            if (position == null || position.Qty == 0)
            {
                return new BackTestClosePostionResponse
                {
                    Message = $"Position with PortfolioId={request.PortfolioId} with Symbol={request.Symbol} is already closed.",
                };
            }

            var marketPrice = await _marketDataService.GetMarketPrice(request.Symbol, request.MarketTime, shift: 0);

            if (marketPrice == null)
            {
                return new BackTestClosePostionResponse()
                {
                    Message = "Cannot get market price to post order. Skipping request."
                };
            }

            var trades = await _orderExecutionService.PostOrder(
                request.Symbol,
                position.Qty * (-1),
                marketPrice.Value,
                request.ComissionPercent);

            if (trades == null || trades.Length <= 0)
            {
                return new BackTestClosePostionResponse
                {
                    Message = $"No trades received for close PortfolioId={request.PortfolioId} with Symbol={request.Symbol}.",
                };
            }

            var updatePositionsRequest = new UpdatePositionsRequest
            {
                Symbol = request.Symbol,
                PortfolioId = request.PortfolioId,
                Trades = trades,
                CurrencyCode = request.CurrencyCode,
            };

            var updatePositionsResponse = await _mediator.Send(updatePositionsRequest, cancellationToken);

            if (updatePositionsResponse.HasErrors)
            {
                return new BackTestClosePostionResponse
                {
                    Exception = updatePositionsResponse.Exception,
                    Message = $"Update positions request failed with error: {updatePositionsResponse.Message}",
                };
            }

            var items = new Dictionary<string, object>();
            items[BackTestContextKeys.Trades] = trades;
            items[BackTestContextKeys.Positions] = updatePositionsResponse.Positions;
            items[BackTestContextKeys.MarketTime] = request.MarketTime;

            return new BackTestClosePostionResponse
            {
                Items = items
            };
        }
    }
}
