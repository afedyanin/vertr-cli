using System.Text;
using Vertr.CommandLine.Application.Services;
using Vertr.CommandLine.Models;

namespace Vertr.CommandLine.Application.Tests.Services
{
    public class MarketDataServiceTests
    {
        [Test]
        public async Task CanCreateMarketDataService()
        {
            var startTime = new DateTime(2025, 11, 7);
            var step = TimeSpan.FromHours(1);
            var before = startTime.AddHours(5);
            var candels = CreateCandles(13, startTime, step);
            Console.WriteLine($"Base:\n{Dump(candels)}");

            var mds = new MarketDataService();
            await mds.LoadData("SBER", candels);

            var selected = await mds.GetCandles("SBER", before, 3);
            Assert.That(selected, Is.Not.Null);
            Console.WriteLine($"\nSelected before={before.ToString("s")}:\n{Dump(selected)}");

            Assert.Pass();
        }

        [Test]
        public async Task CanEnumerateCandles()
        {
            var startTime = new DateTime(2025, 11, 7);
            var step = TimeSpan.FromHours(1);
            var before = startTime.AddHours(5);
            var candels = CreateCandles(13, startTime, step);
            Console.WriteLine($"Base:\n{Dump(candels)}");

            var mds = new MarketDataService();
            await mds.LoadData("SBER", candels);

            var enumerable = await mds.GetEnumerable("SBER");

            foreach (var time in enumerable)
            {
                Console.WriteLine(time.ToString("s"));
            }
        }

        private static Candle[] CreateCandles(int count, DateTime startDate, TimeSpan step)
        {
            var res = new List<Candle>();
            var current = startDate;

            for (int i = 0; i < count; i++)
            {
                res.Add(new Candle { TimeUtc = current, });
                current = current + step;
            }

            return [.. res.OrderBy(c => c.TimeUtc)];
        }

        private static string Dump(IEnumerable<Candle> candles)
        {
            var sb = new StringBuilder();

            foreach (var candle in candles)
            {
                if (candle == null)
                {
                    continue;
                }

                sb.AppendLine(candle.TimeUtc.ToString("s"));
            }
            return sb.ToString();
        }

    }
}
