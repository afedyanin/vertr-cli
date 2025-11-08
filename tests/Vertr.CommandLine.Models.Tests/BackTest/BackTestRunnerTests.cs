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
                MaxSteps = 3,
                OpenPositionQty = 100,
                ComissionPercent = 0.003m,
            };

        [Test]
        public async Task CanIterateBckTestSteps()
        {
            var candles = CsvImporter.LoadCandles(_backTestParams.DataSourceFilePath);
            Assert.That(candles, Is.Not.Null);
            await MarketDataService.LoadData(_backTestParams.Symbol, [.. candles]);

            var candleRange = await MarketDataService.GetCandleRange(_backTestParams.Symbol);
            Assert.That(candleRange, Is.Not.Null);
            Console.WriteLine($"CandleRange={candleRange}");

            var stepCount = 0;
            var closeTime = candleRange.LastDate;
            var maxSteps = Math.Min(_backTestParams.MaxSteps, candleRange.Count);

            var timeIndex = await MarketDataService.GetEnumerable(_backTestParams.Symbol);
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

        [Test]
        public async Task CanRunBacktest()
        {
            var logger = NullLoggerFactory.Instance.CreateLogger<BackTestRunnerTests>();
            var bt = new BackTestRunner(_backTestParams, MarketDataService, Mediator, logger);

            var res = await bt.Run();

            foreach(var result in res.DumpAll())
            {
                Console.WriteLine(result);
            }
            Console.WriteLine("\n-----------------");
            Console.WriteLine(res.DumpCloseStep());

            Assert.Pass();
        }
    }
}
