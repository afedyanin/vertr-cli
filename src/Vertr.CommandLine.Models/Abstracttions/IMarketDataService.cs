namespace Vertr.CommandLine.Models.Abstracttions;

public interface IMarketDataService
{
    public Task<Candle[]> GetCandles(string symbol, DateTime before, int count = 1);

    public Task<decimal?> GetMarketPrice(string symbol, DateTime time, int shift = 0);

    public IEnumerator<DateTime>? GetTimeEnumerator(string symbol);
}
