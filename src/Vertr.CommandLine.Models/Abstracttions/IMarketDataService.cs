namespace Vertr.CommandLine.Models.Abstracttions;

public interface IMarketDataService
{
    public Task<Candle[]> GetCandles(string symbol, DateTime before, int count = 1);

    public Task<decimal?> GetMarketPrice(string symbol, DateTime time, int shift = 0);

    public IEnumerable<DateTime>? GetEnumerable(string symbol);

    public Task LoadData(string symbol, Candle[] candles);
}
