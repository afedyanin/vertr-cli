using System.Reflection.Metadata;

namespace Vertr.CommandLine.Models.Helpers;

public static class CandleExtensions
{
    public static IEnumerable<DateTime> GetTimeEnumerable(
        this IEnumerable<Candle>? orderedCandles)
    {
        if (orderedCandles == null)
        {
            return Enumerable.Empty<DateTime>();
        }

        return orderedCandles.Select(c => c.TimeUtc);
    }
 
    public static IEnumerable<Candle> GetEqualOrLessThanBefore(this IEnumerable<Candle> orderedCandles, DateTime before, int count = 1)
        => orderedCandles
            .Where(c => c.TimeUtc <= before)
            .TakeLast(count);

    public static IEnumerable<Candle> GetEqualOrGreatherThanAfter(this IEnumerable<Candle> orderedCandles, DateTime after, int count = 1)
        => orderedCandles
            .Where(c => c.TimeUtc >= after)
            .Take(count);

    public static Candle? GetShifted(this IEnumerable<Candle> orderedCandles, DateTime time, int shift = 0)
    {
        var count = Math.Abs(shift) + 1;

        var filtered = shift <= 0 ?
            orderedCandles.GetEqualOrLessThanBefore(time, count).ToArray() :
            orderedCandles.GetEqualOrGreatherThanAfter(time, count).ToArray();

        // shift is out of range
        if (filtered.Length < count)
        {
            return null;
        }

        return shift <=0 ? filtered.First() : filtered.Last();
    }

    public static CandleRange? GetRange(this IEnumerable<Candle>? candles, string symbol)
    {
        if (candles == null || !candles.Any())
        {
            return null;
        }

        return new CandleRange
        {
            Symbol = symbol,
            FirstDate = candles.First().TimeUtc,
            LastDate = candles.Last().TimeUtc,
            Count = candles.Count()
        };
    }

    public static Dictionary<DateOnly, CandleRange> GetRanges(this IEnumerable<Candle>? candles, string symbol)
    {
        if (candles == null || !candles.Any())
        {
            return [];
        }

        return candles
           .GroupBy(c => DateOnly.FromDateTime(c.TimeUtc))
           .Select(group => new CandleRange
           {
               Symbol = symbol,
               FirstDate = group.OrderBy(item => item.TimeUtc).First().TimeUtc,
               LastDate = group.OrderByDescending(item => item.TimeUtc).First().TimeUtc,
               Count = group.Count()
           })
           .ToDictionary(c => DateOnly.FromDateTime(c.FirstDate));
    }
}
