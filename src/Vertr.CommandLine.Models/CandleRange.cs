namespace Vertr.CommandLine.Models;

public record class CandleRange
{
    public DateTime FirstDate { get; init; }

    public DateTime LastDate { get; init; }

    public int Count { get; init; }
}
