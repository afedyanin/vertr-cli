using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Helpers;

namespace Vertr.CommandLine.Application.Services
{
    internal class MarketDataService : IMarketDataService
    {
        private readonly Dictionary<string, Candle[]> _storage = [];

        public IEnumerable<DateTime>? GetEnumerable(string symbol)
        {
            if (_storage.TryGetValue(symbol, out var candles))
            {
                return candles.GetTimeEnumerable();
            }

            return null;
        }
        public Task<Candle[]> GetCandles(string symbol, DateTime before, int count = 1)
        {
            Candle[] res = [];
            
            if (_storage.TryGetValue(symbol, out var candles))
            {
                res = [.. candles.GetEqualOrLessThanBefore(before, count)];
            }

            return Task.FromResult(res);
        }

        public Task<decimal?> GetMarketPrice(string symbol, DateTime time, int shift = 0)
        {
            if (_storage.TryGetValue(symbol, out var candles))
            {
                var candle = candles.GetShifted(time, 1);

                if (candle != null)
                {
                    return Task.FromResult<decimal?>(candle.Close);
                }
            }

            return Task.FromResult<decimal?>(null);
        }

        public Task LoadData(string symbol, Candle[] candles)
        {
            _storage[symbol] = candles.OrderBy(c => c.TimeUtc).ToArray();
            return Task.CompletedTask;
        }

        public (DateTime?, DateTime?) GetTimeRange(string symbol)
        {
            if (_storage.TryGetValue(symbol, out var candles))
            {
                var from = candles.First().TimeUtc;
                var to = candles.Last().TimeUtc;

                return (from, to);
            }

            return (null, null);
        }
    }
}
