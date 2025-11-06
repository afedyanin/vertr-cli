
namespace Vertr.CommandLine.Models.Abstracttions;

public interface IPortfolioService
{
    public Position[] Update(
        Guid portfolioId, 
        string symbol, 
        Trade[] trades, 
        decimal comission, 
        string currencyCode);

    public Position[] GetPositions(Guid portfolioId);
}
