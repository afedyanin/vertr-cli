using Vertr.CommandLine.Models.BackTest;
using Vertr.CommandLine.Models.Helpers;

namespace Vertr.CommandLine.Models.Tests.BackTest
{
    public class BackTestBatchRunnerTests : SystemTestBase
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
        public async Task CanRunBackTest()
        {
            var bt = new BackTestRunner(_backTestParams, MarketDataService, Mediator, NullLogger);
            var res = await bt.Run();

            var summary = res.GetSummary(_backTestParams.CurrencyCode);
            Console.WriteLine(summary);

            Assert.Pass();
        }
    }
}
