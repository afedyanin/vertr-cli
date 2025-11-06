using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Models.Requests.BackTest;

public class BackTestExecuteStepRequest : IRequest<BackTestExecuteStepResponse>
{
    public DateTime Time { get; init; }

    public Guid PortfolioId { get; init; }

    public required string Symbol { get; init; }

    public decimal QtyLots { get; init; } = 10;

    public decimal PriceThreshold { get; init; }
}

public class BackTestExecuteStepResponse : ResponseBase
{
    public Dictionary<string, object> Items { get; init; } = [];
}

public static class BackTestContextKeys
{
    public const string Signal = "Signal";
    public const string PredictedPrice = "PredictedPrice";
    public const string MarketPrice = "MarketPrice";
    public const string LastCandle = "LastCandle";
    public const string Positions = "Positions";
    public const string Trades = "Trades";
    public const string Message = "Message";
}