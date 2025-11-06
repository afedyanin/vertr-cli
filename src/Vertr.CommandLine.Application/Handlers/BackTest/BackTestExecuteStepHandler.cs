using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Requests.BackTest;
using Vertr.CommandLine.Models.Requests.MarketData;
using Vertr.CommandLine.Models.Requests.Orders;
using Vertr.CommandLine.Models.Requests.Portfolio;
using Vertr.CommandLine.Models.Requests.Predictor;

namespace Vertr.CommandLine.Application.Handlers.BackTest;
internal class BackTestExecuteStepHandler : IRequestHandler<BackTestExecuteStepRequest, BackTestExecuteStepResponse>
{
    private readonly IMediator _mediator;

    public BackTestExecuteStepHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<BackTestExecuteStepResponse> Handle(BackTestExecuteStepRequest request, CancellationToken cancellationToken = default)
    {
        var rb = new ResponseBuilder();

        var candlesRequest = new GetCandlesRequest
        {
            Symbol = request.Symbol,
            Time = request.Time,
            Count = 1
        };

        var candlesResponse = await _mediator.Send(candlesRequest, cancellationToken);

        if (candlesResponse.HasErrors)
        {
            return rb
                .WithError(candlesResponse.Exception, $"Market data request failed with error: {candlesResponse.Message}")
                .Build();
        }

        var candles = candlesResponse.Candles;

        if (candles == null || candles.Length <= 0)
        {
            return rb
                .WithMessage($"Cannot find any candles for symbol={request.Symbol} at time={request.Time:O}")
                .Build();
        }

        rb = rb.WithCandle(candles.OrderBy(c => c.TimeUtc).Last());

        var predictionRequest = new GetNextPriceRequest
        {
            Symbol = request.Symbol,
            Candles = candles,
        };

        var predictionResponse = await _mediator.Send(predictionRequest, cancellationToken);

        if (predictionResponse.HasErrors)
        {
            return rb
                .WithError(predictionResponse.Exception, $"Prediction failed with error: {predictionResponse.Message}")
                .Build();
        }

        if (!predictionResponse.Price.HasValue)
        {
            return rb
                .WithMessage($"Prediction value is undefined.")
                .Build();
        }

        rb = rb.WithPredictedPrice(predictionResponse.Price.Value);

        var marketPriceRequest = new GetMarketPriceRequest
        {
            Symbol = request.Symbol,
            Time = request.Time,
        };

        var marketPriceResponse = await _mediator.Send(marketPriceRequest, cancellationToken);

        if (marketPriceResponse.HasErrors)
        {
            return rb
                .WithError(marketPriceResponse.Exception, $"Market price request failed with error: {marketPriceResponse.Message}")
                .Build();
        }

        var direction = GetTradingDirection(marketPriceResponse.Price, predictionResponse.Price.Value, request.PriceThreshold);

        rb = rb
            .WithMarketPrice(marketPriceResponse.Price)
            .WithSignal(direction);

        if (direction == 0)
        {
            return rb
                .WithMessage($"No trade direction selected.")
                .Build();
        }

        var tradingSignalRequest = new TradingSignalRequest
        {
            PortfolioId = request.PortfolioId,
            Symbol = request.Symbol,
            QtyLots = request.QtyLots * direction,
        };

        var tradingSignalResponse = await _mediator.Send(tradingSignalRequest, cancellationToken);

        if (tradingSignalResponse.HasErrors)
        {
            return rb
                .WithError(tradingSignalResponse.Exception, $"Trading signal request failed with error: {tradingSignalResponse.Message}")
                .Build();
        }

        var trades = tradingSignalResponse.Trades;

        if (trades == null || trades.Length <= 0)
        {
            return rb
                .WithMessage($"No trades received.")
                .Build();
        }

        rb = rb.WithTrades(trades);

        var updatePositionsRequest = new UpdatePositionsRequest
        {
            PortfolioId = request.PortfolioId,
            Trades = trades
        };

        var updatePositionsResponse = await _mediator.Send(updatePositionsRequest, cancellationToken);

        if (updatePositionsResponse.HasErrors)
        {
            return rb
                .WithError(updatePositionsResponse.Exception, $"Update positions request failed with error: {updatePositionsResponse.Message}")
                .Build();
        }

        var positions = updatePositionsResponse.Positions;

        if (positions == null || positions.Length <= 0)
        {
            return rb
                .WithMessage($"No positions.")
                .Build();
        }

        return rb.WithPositions(positions).Build();
    }

    private static int GetTradingDirection(decimal marketPrice, decimal predictedNextPrice, decimal treshold)
    {
        if (marketPrice == decimal.Zero || predictedNextPrice == decimal.Zero)
        {
            return 0;
        }

        var delta = (predictedNextPrice - marketPrice) / marketPrice;

        if (Math.Abs(delta) <= treshold)
        {
            return 0;
        }

        return delta > 0 ? 1 : -1;
    }

    private class ResponseBuilder
    {
        private Dictionary<string, object> _items = [];

        private string? _message;

        private Exception? _exception;

        public ResponseBuilder WithMessage(string message)
        {
            _message = message;
            _items[BackTestContextKeys.Message] = message;
            return this;
        }

        public ResponseBuilder WithError(Exception? exception, string message)
        {
            _exception = exception;
            _message = message;
            _items[BackTestContextKeys.Message] = message;

            return this;
        }

        public ResponseBuilder WithCandle(Candle candle)
        {
            _items[BackTestContextKeys.LastCandle] = candle;
            return this;
        }

        public ResponseBuilder WithPredictedPrice(decimal price)
        {
            _items[BackTestContextKeys.PredictedPrice] = price;
            return this;
        }

        public ResponseBuilder WithMarketPrice(decimal price)
        {
            _items[BackTestContextKeys.MarketPrice] = price;
            return this;
        }

        public ResponseBuilder WithSignal(int signal)
        {
            _items[BackTestContextKeys.Signal] = signal;
            return this;
        }

        public ResponseBuilder WithTrades(Trade[] trades)
        {
            _items[BackTestContextKeys.Trades] = trades;
            return this;
        }

        public ResponseBuilder WithPositions(Position[] positions)
        {
            _items[BackTestContextKeys.Positions] = positions;
            return this;
        }

        public BackTestExecuteStepResponse Build()
        {
            return new BackTestExecuteStepResponse
            {
                Items = _items,
                Message = _message,
                Exception = _exception
            };
        }
    }
}
