using Microsoft.Extensions.DependencyInjection;
using Vertr.CommandLine.Application;
using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Abstracttions;

namespace Vertr.CommandLine.Models.Tests;

public abstract class SystemTestBase
{
    private readonly IServiceProvider _serviceProvider;

    protected IMediator Mediator => _serviceProvider.GetRequiredService<IMediator>();
    protected IMarketDataService MarketDataService => _serviceProvider.GetRequiredService<IMarketDataService>();

    protected SystemTestBase()
    {
        var services = new ServiceCollection();

        services.AddMediator();
        services.AddApplication();

        _serviceProvider = services.BuildServiceProvider();
    }

}
