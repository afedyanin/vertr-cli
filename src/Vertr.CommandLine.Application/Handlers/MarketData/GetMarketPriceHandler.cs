using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.MarketData;

namespace Vertr.CommandLine.Application.Handlers.MarketData
{
    public class GetMarketPriceHandler : IRequestHandler<GetMarketPriceRequest, GetMarketPriceResponse>
    {
        private readonly IMarketDataService _marketDataService;

        public GetMarketPriceHandler(IMarketDataService marketDataService)
        {
            _marketDataService = marketDataService;
        }

        public async Task<GetMarketPriceResponse> Handle(GetMarketPriceRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var price = await _marketDataService.GetMarketPrice(request.Symbol, request.Time);
                
                var res = new GetMarketPriceResponse()
                {
                     Price = price,
                };

                return res;
            }
            catch (Exception ex)
            {
                var res = new GetMarketPriceResponse()
                {
                    Message = ex.Message,
                };

                return res;
            }
        }
    }
}
