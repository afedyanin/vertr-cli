using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Models.Requests.BackTest;

public class BackTestClosePositionRequest : IRequest<BackTestClosePostionResponse>
{
    public Guid PortfolioId { get; init; }
    public required string Symbol { get; init; }
    public DateTime Time { get; init; }
}


public class BackTestClosePostionResponse : ResponseBase
{
    public Dictionary<string, object> Items { get; init; } = [];
}