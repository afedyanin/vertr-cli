namespace Vertr.CommandLine.Models;

public class BackTestParams
{
    public Guid PortfolioId { get; init; }

    public required string Symbol { get; init; }

    public decimal QtyLots { get; init; } = 10;

    public DateTime From { get; init; }
    
    public DateTime To { get; init; }
    
    public TimeSpan Step { get; init; }
}
