using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.MarketData;

namespace Vertr.CommandLine.Application.Handlers.MarketData;

public class GetCandlesHandler : IRequestHandler<GetCandlesRequest, GetCandlesResponse>
{
    private readonly IMarketDataService _marketDataService;

    public GetCandlesHandler(IMarketDataService marketDataService)
    {
        _marketDataService = marketDataService;
    }

    public async Task<GetCandlesResponse> Handle(GetCandlesRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var candles = await _marketDataService.GetCandles(request.Symbol, request.Time, request.Count);
            var res = new GetCandlesResponse()
            {
                Candles = candles ?? []
            };

            return res;
        }
        catch (Exception ex)
        {
            var res = new GetCandlesResponse()
            {
                Message = ex.Message,
            };

            return res;
        }
    }
}
