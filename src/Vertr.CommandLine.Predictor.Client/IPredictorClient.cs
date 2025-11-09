using Refit;
using Vertr.CommandLine.Predictor.Client.Models;

namespace Vertr.CommandLine.Predictor.Client;

public interface IPredictorClient
{
    [Post("/prediction/predict-by-last-value")]
    public Task<PredictionResult> PredictByLastValue(CandleRequest candle);
}
