using Microsoft.Extensions.DependencyInjection;
using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Application.Tests
{
    internal abstract class AppliactionTestBase
    {
        private readonly IServiceProvider _serviceProvider;

        protected AppliactionTestBase()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IMediator, Mediatr>();
            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
