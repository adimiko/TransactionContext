using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using TransactionContext.Configurators;
using Microsoft.Data.SqlClient;
using TransactionContext.SqlServer.Internals;

namespace TransactionContext.SqlServer
{
    public static class Extensions
    {
        public static void UseSqlServer(
            this IDbProviderConfigurator configurator,
            string connectionString)
        {
            configurator.Services.AddSingleton<IDbConnectionFactory>(new SqlServerConnectionFactory(connectionString));
            configurator.Services.AddScoped<DbBatch, SqlBatch>();
        }
    }
}
