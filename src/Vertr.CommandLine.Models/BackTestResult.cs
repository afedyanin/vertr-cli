namespace Vertr.CommandLine.Models
{
    public class BackTestResult
    {
        public Dictionary<DateTime, Position[]> Positions { get; } = [];
        public Dictionary<DateTime, Trade[]> Trades { get; } = [];
    }
}
