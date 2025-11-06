using Vertr.CommandLine.Common.Mediator;
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
        var candlesRequest = new GetCandlesRequest
        {
            Symbol = request.Symbol,
            Time = request.Time,
            Count = 1
        };

        var candlesResponse = await _mediator.Send(candlesRequest, cancellationToken);

        if (candlesResponse.HasErrors)
        {
            return HandleError($"Market data request failed with error: {candlesResponse.Message}", candlesResponse.Exception);
        }

        var candles = candlesResponse.Candles;

        if (candles == null || candles.Length <= 0)
        {
            return HandleEmptyResponse($"Cannot find any candles for symbol={request.Symbol} at time={request.Time:O}");
        }

        var predictionRequest = new GetNextPriceRequest
        {
            Symbol = request.Symbol,
            Candles = candles,
        };

        var predictionResponse = await _mediator.Send(predictionRequest, cancellationToken);

        if (predictionResponse.HasErrors)
        {
            return HandleError($"Prediction failed with error: {predictionResponse.Message}", predictionResponse.Exception);
        }

        if (!predictionResponse.Price.HasValue)
        {
            return HandleEmptyResponse("Prediction value undefined.");
        }

        var marketPriceRequest = new GetMarketPriceRequest
        {
            Symbol = request.Symbol,
            Time = request.Time,
        };

        var marketPriceResponse = await _mediator.Send(marketPriceRequest, cancellationToken);

        if (marketPriceResponse.HasErrors)
        {
            return HandleError($"Market price request failed with error: {marketPriceResponse.Message}", marketPriceResponse.Exception);
        }

        var direction = GetTradingDirection(marketPriceResponse.Price, predictionResponse.Price.Value);

        if (direction == 0)
        {
            return HandleEmptyResponse("No trade direction selected.");
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
            return HandleError($"Trading signal request failed with error: {tradingSignalResponse.Message}", tradingSignalResponse.Exception);
        }

        var trades = tradingSignalResponse.Trades;

        if (trades == null || trades.Length <= 0)
        {
            return HandleEmptyResponse("No trades received.");
        }

        var updatePositionsRequest = new UpdatePositionsRequest
        {
            PortfolioId = request.PortfolioId,
            Trades = trades
        };

        var updatePositionsResponse = await _mediator.Send(updatePositionsRequest, cancellationToken);

        if (updatePositionsResponse.HasErrors)
        {
            return HandleError($"Update positions request failed with error: {updatePositionsResponse.Message}", updatePositionsResponse.Exception);
        }

        var snapshot = updatePositionsResponse.Positions;

        if (snapshot == null || snapshot.Length <= 0)
        {
            return HandleEmptyResponse("No positions.");
        }

        var stepResponse = new BackTestExecuteStepResponse
        {
            Positions = snapshot,
        };

        return stepResponse;
    }

    private static BackTestExecuteStepResponse HandleError(string message, Exception? ex)
        => new BackTestExecuteStepResponse
        {
            Message = message,
            Exception = ex
        };

    private static BackTestExecuteStepResponse HandleEmptyResponse(string? message = null)
        => new BackTestExecuteStepResponse
        {
            Message = message,
        };

    private int GetTradingDirection(decimal marketPrice, decimal predictedNextPrice)
    {
        // TODO: Use treshold
        if (predictedNextPrice > marketPrice)
        {
            return 1; // Buy

        }
        else
        {
            return -1; // Sell
        }
    }
}
