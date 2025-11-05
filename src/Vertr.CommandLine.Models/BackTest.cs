using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Requests.MarketData;
using Vertr.CommandLine.Models.Requests.Orders;
using Vertr.CommandLine.Models.Requests.Predictor;

namespace Vertr.CommandLine.Models;

public class BackTest
{
    private readonly Guid _portfolioId;
    private readonly IMediator _mediator;
    private readonly string _symbol;
    private readonly decimal _qtyLots = 10;

    public BackTest(
        string symbol,
        Guid portfolioId,
        IMediator mediator)
    {
        _symbol = symbol;
        _portfolioId = portfolioId;
        _mediator = mediator;
    }

    public async Task Run(DateTime from, DateTime to, TimeSpan step)
    {
        var current = from;

        while (current < to)
        {
            await ExecuteStep(current);
            current += step;
        }
    }

    public async Task ExecuteStep(DateTime time, CancellationToken cancellationToken = default)
    {
        var candlesRequest = new GetCandlesRequest
        {
            Symbol = _symbol,
            Time = time,
            Count = 1
        };

        var candlesResponse = await _mediator.Send(candlesRequest, cancellationToken);

        if (candlesResponse.HasErrors)
        {
            throw new InvalidOperationException($"Backtest failed with error: {candlesResponse.ErrorMessage}");
        }

        var candles = candlesResponse.Candles;

        if (candles == null || candles.Length <= 0)
        {
            throw new InvalidOperationException($"Backtest failed. Cannot find any candles for symbol={_symbol} at time={time:O}");
        }

        var predictionRequest = new GetNextPriceRequest
        {
            Symbol = _symbol,
            Candles = candles,
        };

        var predictionResponse = await _mediator.Send(predictionRequest, cancellationToken);

        if (predictionResponse.HasErrors)
        {
            throw new InvalidOperationException($"Backtest failed with error: {predictionResponse.ErrorMessage}");
        }

        if (!predictionResponse.Price.HasValue)
        {
            // Log info
            return;
        }

        var marketPriceRequest = new GetMarketPriceRequest
        {
            Symbol = _symbol,
            Time = time,
        };

        var marketPriceResponse = await _mediator.Send(marketPriceRequest, cancellationToken);

        if (marketPriceResponse.HasErrors)
        {
            throw new InvalidOperationException($"Backtest failed with error: {marketPriceResponse.ErrorMessage}");
        }

        var direction = GetTradingDirection(marketPriceResponse.Price, predictionResponse.Price.Value);

        if (direction == 0)
        {
            // Log info
            return;
        }

        var tradingSignalRequest = new TradingSignalRequest
        {
            PortfolioId = _portfolioId,
            Symbol = _symbol,
            QtyLots = _qtyLots * direction,
        };

        var tradingSignalResponse = await _mediator.Send(tradingSignalRequest, cancellationToken);

        if (tradingSignalResponse.HasErrors)
        {
            throw new InvalidOperationException($"Backtest failed with error: {marketPriceResponse.ErrorMessage}");
        }



        // Handle trades 
        // Collect Position snapshot & calculate performance
        Console.WriteLine($"strategy step: {time:O}");
    }

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
