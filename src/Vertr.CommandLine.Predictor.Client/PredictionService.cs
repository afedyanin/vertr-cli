using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.Predictor;

namespace Vertr.CommandLine.Predictor.Client;

internal class PredictionService : IPredictionService
{
    public Task<Dictionary<string, object>> Predict(
        DateTime time, 
        string symbol, 
        PredictorType predictor, 
        Dictionary<string, object> marketData)
    {

        // TODO: Implement this
        var res = new Dictionary<string, object>();
        return Task.FromResult(res);
    }
}
