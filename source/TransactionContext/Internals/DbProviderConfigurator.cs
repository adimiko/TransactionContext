using Microsoft.Extensions.DependencyInjection;
using TransactionContext.Configurators;

namespace TransactionContext.Internals
{
    internal sealed class DbProviderConfigurator : IDbProviderConfigurator
    {
        public IServiceCollection Services { get; private set; }

        internal DbProviderConfigurator(IServiceCollection services) => Services = services;
    }
}
