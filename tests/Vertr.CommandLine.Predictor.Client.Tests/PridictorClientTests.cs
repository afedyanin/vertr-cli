using System.Text;
using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Requests.Predictor;
using Vertr.CommandLine.Predictor.Client.Convertors;

namespace Vertr.CommandLine.Predictor.Client.Tests;

[TestFixture(Category = "Web", Explicit = true)]
public class PredictorClientTests: ClientTestBase
{
    private static readonly Candle _candle = new Candle
    {
        TimeUtc = DateTime.UtcNow,
        Open = 77.45m,
        High = 85.13m,
        Low = 71.14m,
        Close = 82.34m,
        Volume = 46654
    };

    [Test]
    public async Task CanPredictByLastValue()
    {
        var response = await PredictorClient.PredictByLastValue(_candle.ToRequest());
        
        Assert.That(response, Is.Not.Null);

        Console.WriteLine(response);
    }

    [Test]
    public async Task CanPredictByLastValueViaPredictionService()
    {
        var data = new Dictionary<string, object>();
        data[PredictionContextKeys.Candles] = new Candle[] { _candle };

        var response = await PredictionService.Predict(DateTime.UtcNow, "SBER", PredictorType.LastValue, data);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Count, Is.GreaterThan(0));
        
        Console.WriteLine(Dump(response));
    }

    private string Dump(Dictionary<string, object> dict)
    {
        var sb =new StringBuilder();

        foreach (var kvp in dict)
        {
            sb.AppendLine($"{kvp.Key} = {kvp.Value}");
        }

        return sb.ToString();
    }
}