namespace Vertr.CommandLine.Models.BackTest;

public class BackTestParams
{
    public Guid PortfolioId { get; init; }

    public required string Symbol { get; init; }

    public required string CurrencyCode { get; init; }

    public DateTime From { get; init; }
    
    public DateTime To { get; init; }
    
    public TimeSpan Step { get; init; }
}
