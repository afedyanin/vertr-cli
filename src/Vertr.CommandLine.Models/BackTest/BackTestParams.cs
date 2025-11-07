namespace Vertr.CommandLine.Models.BackTest;

public class BackTestParams
{
    public Guid PortfolioId { get; init; }

    public required string Symbol { get; init; }

    public required string CurrencyCode { get; init; }

    public required string DataSourceFilePath { get; init; }
}
