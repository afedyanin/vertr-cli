namespace Vertr.CommandLine.Models;

public class BackTestSettings
{
    public DateTime From { get; init; }

    public DateTime To { get; init; }

    public TimeSpan Step { get; init; }

    public required string DataSourceFolderPath { get; init; }

    public decimal InitialAmount { get; init; }

    public decimal Comission { get; init; }

    public required string Predictor { get; set; }

    public long QtyLots { get; set; }
}
