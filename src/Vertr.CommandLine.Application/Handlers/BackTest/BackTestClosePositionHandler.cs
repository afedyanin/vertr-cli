using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.BackTest;
using Vertr.CommandLine.Models.Requests.Orders;
using Vertr.CommandLine.Models.Requests.Portfolio;

namespace Vertr.CommandLine.Application.Handlers.BackTest
{
    public class BackTestClosePositionHandler : IRequestHandler<BackTestClosePositionRequest, BackTestClosePostionResponse>
    {
        private readonly IPortfolioService _portfolioService;
        private readonly IMediator _mediator;

        public BackTestClosePositionHandler(
            IPortfolioService portfolioService,
            IMediator mediator)
        {
            _portfolioService = portfolioService;
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

            var closeRequest = new PostOrderRequest
            {
                Symbol = request.Symbol,
                Qty = position.Qty * (-1),
                MarketTime = request.MarketTime,
            };

            var closeResponse = await _mediator.Send(closeRequest, cancellationToken);

            var trades = closeResponse.Trades;

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
            items[BackTestContextKeys.Positions] = updatePositionsResponse.Positions;
            items[BackTestContextKeys.MarketTime] = request.MarketTime;

            return new BackTestClosePostionResponse
            {
                Items = items
            };
        }
    }
}
