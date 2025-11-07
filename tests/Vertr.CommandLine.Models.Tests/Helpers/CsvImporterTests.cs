using Vertr.CommandLine.Models.Helpers;

namespace Vertr.CommandLine.Models.Tests.Helpers
{
    public class CsvImporterTests
    {
        private const string _csvFilePath = "Data\\SBER_251101_251104.csv";

        [Test]
        public void CanLoadCandlesFromCsv()
        {
            var candles = CsvImporter.LoadCandles(_csvFilePath);

            Assert.That(candles, Is.Not.Null);
            Assert.That(candles.Count, Is.GreaterThan(0));
        }
    }
}
