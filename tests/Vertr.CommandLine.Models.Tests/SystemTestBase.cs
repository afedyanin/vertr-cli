using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Vertr.CommandLine.Application;
using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Tests.BackTest;

namespace Vertr.CommandLine.Models.Tests;

public abstract class SystemTestBase
{
    private readonly IServiceProvider _serviceProvider;

    protected IMediator Mediator => _serviceProvider.GetRequiredService<IMediator>();
    protected IMarketDataService MarketDataService => _serviceProvider.GetRequiredService<IMarketDataService>();

    protected ILogger NullLogger = NullLoggerFactory.Instance.CreateLogger<SystemTestBase>();

    protected SystemTestBase()
    {
        var services = new ServiceCollection();

        services.AddMediator();
        services.AddApplication();

        _serviceProvider = services.BuildServiceProvider();
    }

}
