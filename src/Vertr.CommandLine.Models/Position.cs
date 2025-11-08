namespace Vertr.CommandLine.Models;

public record class Position
{
    public Guid PortfolioId { get; init; }

    public required string Symbol { get; init; }

    public decimal Qty { get; set; }

    public Position ClonePosition()
        => new()
        {
            PortfolioId = PortfolioId,
            Symbol = Symbol, 
            Qty = Qty, 
        };
}
