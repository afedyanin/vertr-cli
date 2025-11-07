namespace Vertr.CommandLine.Models.Extensions
{
    public static class CandleExtensions
    {
        public static IEnumerator<DateTime> GetTimeEnumerator(this IEnumerable<Candle> orderedCandles)
            => orderedCandles
                .Select(c => c.TimeUtc)
                .GetEnumerator();

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
    }
}
