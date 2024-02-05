using Microsoft.Extensions.DependencyInjection;

namespace TransactionContext.Configurators
{
    public interface IDbProviderConfigurator
    {
        IServiceCollection Services { get; }
    }
}
