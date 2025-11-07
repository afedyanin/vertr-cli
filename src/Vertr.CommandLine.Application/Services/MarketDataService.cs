using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Extensions;

namespace Vertr.CommandLine.Application.Services
{
    internal class MarketDataService : IMarketDataService
    {
        private readonly Dictionary<string, Candle[]> _storage = [];

        public IEnumerator<DateTime>? GetTimeEnumerator(string symbol)
        {
            if (_storage.TryGetValue(symbol, out var candles))
            {
                return candles.GetTimeEnumerator();
            }

            return null;
        }
        public Task<Candle[]> GetCandles(string symbol, DateTime before, int count = 1)
        {
            // TODO: Implement this
            var candle = new Candle
            {
                TimeUtc = before,
                Open = 101,
                High = 110,
                Low = 98,
                Close = 103,
                Volume = 505,
            };

            return Task.FromResult<Candle[]>([candle]);
        }

        public Task<decimal?> GetMarketPrice(string symbol, DateTime time, int shift = 0)
        {
            // TODO: Implement this
            return Task.FromResult<decimal?>(106);
        }
    }
}
