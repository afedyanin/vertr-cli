using Vertr.CommandLine.Models.BackTest;
using Vertr.CommandLine.Models.Helpers;

namespace Vertr.CommandLine.Models.Tests.BackTest
{
    public class BackTestRunnerTests : SystemTestBase
    {
        private const string _csvFilePath = "Data\\SBER_251101_251104.csv";
        private const string _symbol = "SBER";
        private readonly int _maxSteps = 10000;

        [Test]
        public async Task CanIterateBckTestSteps()
        {
            var candles = CsvImporter.LoadCandles(_csvFilePath);
            Assert.That(candles, Is.Not.Null);
            await MarketDataService.LoadData(_symbol, [.. candles]);

            var candleRange = await MarketDataService.GetCandleRange(_symbol);
            Assert.That(candleRange, Is.Not.Null);
            Console.WriteLine($"CandleRange={candleRange}");

            var stepCount = 0;
            var closeTime = candleRange.LastDate;
            var maxSteps = Math.Min(_maxSteps, candleRange.Count);

            var timeIndex = await MarketDataService.GetEnumerable(_symbol);
            Assert.That(timeIndex, Is.Not.Null);

            foreach (var timeStep in timeIndex)
            {
                if (stepCount++ >= maxSteps)
                {
                    closeTime = timeStep;
                    break;
                }

                Console.WriteLine($"Step={stepCount} Time={timeStep:s}");
            }
           
            Console.WriteLine($"CloseTime={closeTime:s}");

            Assert.Pass();
        }
    }
}
