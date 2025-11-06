namespace Vertr.CommandLine.Models;
public record class Trade
{
    public string TradeId { get; init; } = string.Empty;

    public DateTime ExecutionTime { get; init; }

    public decimal Price { get; init; }

    public decimal Quantity { get; init; }

    public decimal TradeComission { get; init; }
}
