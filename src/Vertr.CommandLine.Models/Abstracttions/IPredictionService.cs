namespace Vertr.CommandLine.Models.Abstracttions;

public interface IPredictionService
{
    public decimal GetNextPrice(Candle[] candles);
}
