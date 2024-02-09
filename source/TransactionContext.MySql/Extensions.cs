using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using System.Data.Common;
using TransactionContext.Configurators;
using TransactionContext.MySql.Internals;

namespace TransactionContext.MySql
{
    public static class Extensions
    {
        public static void UseMySql(
            this IDbProviderConfigurator configurator,
            string connectionString)
        {
            configurator.Services.AddSingleton<IDbConnectionFactory>(new MySqlConnectionFactory(connectionString));
            configurator.Services.AddScoped<DbBatch, MySqlBatch>();
        }
    }
}
