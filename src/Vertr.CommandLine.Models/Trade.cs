namespace Vertr.CommandLine.Models;
public record class Trade
{
    public Guid PortfolioId { get; init; }

    public string TradeId { get; init; } = string.Empty;

    public DateTime ExecutionTime { get; init; }

    public decimal Price { get; init; }

    public decimal Quantity { get; init; }

    public decimal Comission { get; init; }
}
