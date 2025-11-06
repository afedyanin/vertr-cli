using Microsoft.Extensions.DependencyInjection;
using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Application.Tests
{
    public abstract class AppliactionTestBase
    {
        private readonly IServiceProvider _serviceProvider;

        protected IMediator Mediator => _serviceProvider.GetRequiredService<IMediator>();

        protected AppliactionTestBase()
        {
            var services = new ServiceCollection();

            services.AddMediator();

            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
