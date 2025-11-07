using Vertr.CommandLine.Models.Requests.Orders;

namespace Vertr.CommandLine.Models.Abstracttions;
public interface IOrderExecutionService
{
    public Task<Trade[]> PostOrder(
        string symbol, 
        decimal qty,
        DateTime? marketTime = null);
}
