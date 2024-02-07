using Microsoft.Extensions.DependencyInjection;
using TransactionContext.Configurators;
using TransactionContext.Internals;

namespace TransactionContext
{
    public static class Extensions
    {
        public static void AddTransactionContext(
            this IServiceCollection services,
            Action<IDbProviderConfigurator> configure)
        {
            var configuratior = new DbProviderConfigurator(services);

            configure(configuratior);

            services.AddScoped<ITransactionContext, InternalTransactionContext>();
        }
    }
}
