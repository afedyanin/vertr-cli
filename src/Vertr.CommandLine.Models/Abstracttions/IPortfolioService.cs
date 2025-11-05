
namespace Vertr.CommandLine.Models.Abstracttions;

public interface IPortfolioService
{
    public Trade[] GetTrades(Guid portfolioId);

    public Position[] GetPositions(Guid portfolioId);
}
