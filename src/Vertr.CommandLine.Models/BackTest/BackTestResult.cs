namespace Vertr.CommandLine.Models.BackTest
{
    public class BackTestResult
    {
        public Dictionary<DateTime, Dictionary<string, object>> Items { get; } = [];
    }
}
