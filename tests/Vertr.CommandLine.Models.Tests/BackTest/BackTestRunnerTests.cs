using Vertr.CommandLine.Models.BackTest;
using Vertr.CommandLine.Models.Helpers;

namespace Vertr.CommandLine.Models.Tests.BackTest
{
    public class BackTestRunnerTests : SystemTestBase
    {
        private static readonly FileDataSource[] _dataSources = 
        {
            new FileDataSource
            {
                Symbol = "SBER",
                FilePath = "Data\\SBER_251101_251104.csv",
            }
        };

        private static readonly BackTestParams _backTestParams = 
            new BackTestParams
            {
                PortfolioId = Guid.NewGuid(),
                Symbol = "SBER",
                CurrencyCode = "RUB",
                Steps = 0,
                Skip = 10,
                OpenPositionQty = 100,
                ComissionPercent = 0.001m,
            };

        [Test]
        public async Task CanIterateBackTestSteps()
        {
            var candles = CsvImporter.LoadCandles(_dataSources[0].FilePath);
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
        public async Task CanRunBackTest()
        {
            var bt = new BackTestRunner(MarketDataService, PortfolioService, Mediator, Logger);
            await bt.InitMarketData(_dataSources);
            var res = await bt.Run(_backTestParams);

            DumpResults(res);
        }

        private static void DumpResults(BackTestResult backTestResult)
        {
            foreach (var result in backTestResult.DumpAll())
            {
                //Console.WriteLine(result);
            }

            Console.WriteLine("\nLAST STEP:");
            Console.WriteLine(backTestResult.DumpLastStep());
            
            Console.WriteLine("\nCLOSE STEP:");
            Console.WriteLine(backTestResult.DumpCloseStep());

            Console.WriteLine("\nSUMMARY:");
            Console.WriteLine(backTestResult.GetSummary(_backTestParams.CurrencyCode));
        }
    }
}
