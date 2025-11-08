using Vertr.CommandLine.Models.BackTest;
using Vertr.CommandLine.Models.Helpers;

namespace Vertr.CommandLine.Models.Tests.BackTest
{
    public class BackTestBatchRunnerTests : SystemTestBase
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
                Steps = 3,
                Skip = 10,
                OpenPositionQty = 100,
                ComissionPercent = 0.001m,
            };

        [Test]
        public async Task CanRunBackTest()
        {
            var bt = new BackTestRunner(MarketDataService, Mediator, NullLogger);
            await bt.InitMarketData(_dataSources);
            var res = await bt.Run(_backTestParams);

            var summary = res.GetSummary(_backTestParams.CurrencyCode);
            Console.WriteLine(summary);

            Assert.Pass();
        }
    }
}
