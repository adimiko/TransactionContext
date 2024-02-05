using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TransactionContext.Configurators;
using TransactionContext.Postgres.Internals;

namespace TransactionContext.Postgres
{
    public static class Extensions
    {
        public static void UsePostgres(
            this IDbProviderConfigurator configurator,
            string connectionString)
        {
            configurator.Services.AddSingleton<IDbConnectionFactory>(new PostgresConnectionFactory(connectionString));
            configurator.Services.AddScoped<DbBatch, NpgsqlBatch>();
        }
    }
}
