using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Abstracttions;

namespace Vertr.CommandLine.Application.Services
{
    internal class MarketDataService : IMarketDataService
    {
        public Task<Candle[]> GetCandles(string symbol, DateTime time, int count = 1)
        {
            return Task.FromResult<Candle[]>([]);
        }

        public Task<decimal> GetMarketPrice(string symbol, DateTime time)
        {
            return Task.FromResult(decimal.Zero);
        }
    }
}
