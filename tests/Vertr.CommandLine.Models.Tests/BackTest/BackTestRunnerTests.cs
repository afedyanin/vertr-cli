using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Vertr.CommandLine.Models.BackTest;
using Vertr.CommandLine.Models.Helpers;

namespace Vertr.CommandLine.Models.Tests.BackTest
{
    public class BackTestRunnerTests : SystemTestBase
    {
        private static readonly BackTestParams _backTestParams = 
            new BackTestParams
            {
                PortfolioId = Guid.NewGuid(),
                Symbol = "SBER",
                CurrencyCode = "RUB",
                DataSourceFilePath = "Data\\SBER_251101_251104.csv",
                Steps = 3,
                Skip = 10,
                OpenPositionQty = 100,
                ComissionPercent = 0.001m,
            };

        [Test]
        public async Task CanIterateBackTestSteps()
        {
            var candles = CsvImporter.LoadCandles(_backTestParams.DataSourceFilePath);
            Assert.That(candles, Is.Not.Null);
            await MarketDataService.LoadData(_backTestParams.Symbol, [.. candles]);

            var candleRange = await MarketDataService.GetCandleRange(_backTestParams.Symbol);
            Assert.That(candleRange, Is.Not.Null);
            Console.WriteLine($"CandleRange={candleRange}");

            var stepCount = 0;
            var closeTime = candleRange.LastDate;
            var maxSteps = _backTestParams.Steps > 0 ?
                Math.Min(_backTestParams.Steps + _backTestParams.Skip, candleRange.Count) :
                candleRange.Count;
 
            var timeIndex = await MarketDataService.GetEnumerable(_backTestParams.Symbol);
            Assert.That(timeIndex, Is.Not.Null);

            foreach (var timeStep in timeIndex)
            {
                if (stepCount++ >= maxSteps)
                {
                    closeTime = timeStep;
                    break;
                }

                if (stepCount <= _backTestParams.Skip)
                {
                    Console.WriteLine($"Skipping Step={stepCount} Time={timeStep:s}");
                    continue;
                }

                Console.WriteLine($"Step={stepCount} Time={timeStep:s}");
            }
           
            Console.WriteLine($"CloseTime={closeTime:s}");

            Assert.Pass();
        }

        [Test]
        public async Task CanRunBacktest()
        {
            var logger = NullLoggerFactory.Instance.CreateLogger<BackTestRunnerTests>();
            var bt = new BackTestRunner(_backTestParams, MarketDataService, Mediator, logger);

            var res = await bt.Run();

            /*
            foreach(var result in res.DumpAll())
            {
                Console.WriteLine(result);
            }
            Console.WriteLine(res.DumpLastStep());
            Console.WriteLine("-----------------");
            Console.WriteLine(res.DumpCloseStep());

             */

            var summary = res.GetSummary(_backTestParams.CurrencyCode);
            Console.WriteLine(summary);

            Assert.Pass();
        }
    }
}
