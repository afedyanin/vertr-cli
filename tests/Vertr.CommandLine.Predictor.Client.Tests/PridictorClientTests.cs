using Vertr.CommandLine.Models.Requests.Predictor;
using Vertr.CommandLine.Predictor.Client.Models;

namespace Vertr.CommandLine.Predictor.Client.Tests
{
    [TestFixture(Category = "Web", Explicit = true)]
    public class PredictorClientTests: ClientTestBase
    {
        [Test]
        public async Task CanPredictByLastValue()
        {
            var canleRequest = new CandleRequest
            {
                TimeUtc = DateTime.UtcNow,
                Open = 77.45m,
                High = 85.13m,
                Low = 71.14m,
                Close = 82.34m,
                Volume = 46654
            };

            var response = await PredictorClient.PredictByLastValue(canleRequest);
            
            Assert.That(response, Is.Not.Null);

            Console.WriteLine(response);
        }

        [Test]
        public async Task CanPredictByLastValueViaPredictionService()
        {
            // TODO: Implement this
            var data = new Dictionary<string, object>();

            var response = await PredictionService.Predict(DateTime.UtcNow, "SBER", PredictorType.LastValue, data);

            Assert.That(response, Is.Not.Null);

            Console.WriteLine(response);
        }
    }
}