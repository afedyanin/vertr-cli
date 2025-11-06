namespace Vertr.CommandLine.Models;
public class Trade
{
    public Guid PortfolioId { get; init; }

    public string TradeId { get; init; } = string.Empty;

    public DateTime ExecutionTime { get; init; }

    public decimal? Price { get; init; }

    public long Quantity { get; init; }
}
