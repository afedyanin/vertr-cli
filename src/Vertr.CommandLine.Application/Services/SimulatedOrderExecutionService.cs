using System.Diagnostics;
using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Abstracttions;

namespace Vertr.CommandLine.Application.Services;

internal class SimulatedOrderExecutionService : IOrderExecutionService
{
    private static readonly decimal _comissionPercent = 0.03m;

    private readonly IMarketDataService _marketDataService;

    public SimulatedOrderExecutionService(IMarketDataService marketDataService)
    {
        _marketDataService = marketDataService;
    }

    public async Task<Trade[]> PostOrder(
        string symbol, 
        decimal qty,
        DateTime? marketTime = null)
    {
        // For simulated execution marketTime must be set.
        Debug.Assert(marketTime != null);
        Debug.Assert(qty != decimal.Zero);

        var marketPrice = await _marketDataService.GetMarketPrice(symbol, marketTime.Value, shift: 1);

        if (marketPrice == null)
        {
            throw new InvalidOperationException($"Cannot get market price for symbol={symbol}");
        }

        var comission = Math.Abs(_comissionPercent * marketPrice.Value * qty);

        var trade = new Trade
        {
            Price = marketPrice.Value,
            Quantity = qty,
            Comission = comission,
        };

        return [trade];
    }
}
