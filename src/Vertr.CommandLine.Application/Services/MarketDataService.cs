using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Abstracttions;

namespace Vertr.CommandLine.Application.Services
{
    internal class MarketDataService : IMarketDataService
    {
        public Task<Candle[]> GetCandles(string symbol, DateTime time, int count = 1)
        {
            // TODO: Implement this
            var candle = new Candle
            {
                TimeUtc = time,
                Open = 101,
                High = 110,
                Low = 98,
                Close = 103,
                Volume = 505,
            };

            return Task.FromResult<Candle[]>([candle]);
        }

        public Task<decimal?> GetMarketPrice(string symbol, DateTime time)
        {
            // TODO: Implement this
            return Task.FromResult<decimal?>(106);
        }
    }
}
