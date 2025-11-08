namespace Vertr.CommandLine.Models.BackTest;

public class BackTestParams
{
    public Guid PortfolioId { get; init; }

    public required string Symbol { get; init; }

    public required string CurrencyCode { get; init; }

    public required string DataSourceFilePath { get; init; }

    public int Steps { get; init; }

    public int Skip { get; init; }

    public decimal OpenPositionQty { get; init; } = 100;

    public decimal ComissionPercent { get; init; } = 0.003m;
}
