using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Requests.BackTest;
using Vertr.CommandLine.Models.Requests.MarketData;
using Vertr.CommandLine.Models.Requests.Orders;
using Vertr.CommandLine.Models.Requests.Portfolio;
using Vertr.CommandLine.Models.Requests.Predictor;

namespace Vertr.CommandLine.Application.Handlers.BackTest;
public class BackTestExecuteStepHandler : IRequestHandler<BackTestExecuteStepRequest, BackTestExecuteStepResponse>
{
    private readonly IMediator _mediator;

    public BackTestExecuteStepHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<BackTestExecuteStepResponse> Handle(BackTestExecuteStepRequest request, CancellationToken cancellationToken = default)
    {
        var rb = new BackTestExecuteStepResponseBuilder()
            .WithMarketTime(request.Time);

        var predictionRequest = new PredictionRequest
        {
            Time = request.Time,
            Symbol = request.Symbol,
            Predictor = request.Predictor,
        };

        var predictionResponse = await _mediator.Send(predictionRequest, cancellationToken);

        if (predictionResponse.HasErrors)
        {
            return rb
                .WithError(predictionResponse.Exception, $"Prediction failed with error: {predictionResponse.Message}")
                .Build();
        }

        if (!predictionResponse.PredictedPrice.HasValue)
        {
            return rb
                .WithMessage($"Prediction value is undefined. Message: {predictionResponse.Message}")
                .Build();
        }

        rb = rb
            .WithPredictedPrice(predictionResponse.PredictedPrice.Value)
            .WithCandle(predictionResponse.LastCandle);

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

        if (!marketPriceResponse.Price.HasValue)
        {
            return rb
                .WithMessage($"Market price is undefined.")
                .Build();
        }

        var direction = GetTradingDirection(marketPriceResponse.Price.Value, predictionResponse.PredictedPrice.Value, request.PriceThreshold);

        rb = rb
            .WithMarketPrice(marketPriceResponse.Price.Value)
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
            Direction = direction,
            MarketTime = request.Time,
            OpenPositionQty = request.OpenPositionQty,
        };

        var tradingSignalResponse = await _mediator.Send(tradingSignalRequest, cancellationToken);

        if (tradingSignalResponse.HasErrors)
        {
            return rb
                .WithError(tradingSignalResponse.Exception, $"Trading signal request failed with error: {tradingSignalResponse.Message}")
                .Build();
        }

        var trades = tradingSignalResponse.Trades;

        if (trades.Length <= 0)
        {
            return rb
                .WithMessage($"No trades received. Message={tradingSignalResponse.Message}")
                .Build();
        }

        rb = rb.WithTrades(trades);

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
            return rb
                .WithError(updatePositionsResponse.Exception, $"Update positions request failed with error: {updatePositionsResponse.Message}")
                .Build();
        }

        return rb
            .WithPositions(updatePositionsResponse.Positions)
            .Build();
    }

    private static Direction GetTradingDirection(decimal marketPrice, decimal predictedNextPrice, decimal treshold)
    {
        if (marketPrice == decimal.Zero || predictedNextPrice == decimal.Zero)
        {
            return Direction.Hold;
        }

        var delta = (predictedNextPrice - marketPrice) / marketPrice;

        if (Math.Abs(delta) <= treshold)
        {
            return Direction.Hold;
        }

        return delta > 0 ? Direction.Buy : Direction.Sell;
    }
}
