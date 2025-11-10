using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.Predictor;
using Vertr.CommandLine.Predictor.Client.Convertors;
using Vertr.CommandLine.Predictor.Client.Models;

namespace Vertr.CommandLine.Predictor.Client;

internal class PredictionService : IPredictionService
{
    private readonly IPredictorClient _predictorClient;

    public PredictionService(IPredictorClient predictorClient)
    {
        _predictorClient = predictorClient;
    }

    public async Task<Dictionary<string, object>> Predict(
        DateTime time, 
        string symbol, 
        PredictorType predictor, 
        Dictionary<string, object> marketData)
    {
        var candle = GetLastCandle(marketData);

        if (candle == null)
        {
            return [];
        }

        var response = await _predictorClient.PredictByLastValue(candle.ToRequest());
        var res = response.ToDictionary();
        res[PredictionContextKeys.LastCandle] = candle;

        return res;
    }

    private Candle? GetLastCandle(Dictionary<string, object> marketData)
    {
        marketData.TryGetValue(PredictionContextKeys.Candles, out var candlesEntry);

        var candles = candlesEntry as Candle[];

        if (candles == null)
        {
            return null;
        }

        var lastCandle = candles.OrderBy(c => c.TimeUtc).LastOrDefault();

        return lastCandle;
    }
}
