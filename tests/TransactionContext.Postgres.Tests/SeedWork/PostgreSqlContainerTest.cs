using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace TransactionContext.Postgres.Tests.SeedWork
{
    public abstract class PostgreSqlContainerTest : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

        protected ITransactionContext TransactionContext { get; private set; }

        protected IDbConnectionFactory DbConnectionFactory { get; private set; }

        public async Task InitializeAsync()
        {
            await _postgreSqlContainer.StartAsync();

            var connectionString = _postgreSqlContainer.GetConnectionString();

            IServiceCollection services = new ServiceCollection();

            services.AddTransactionContext(x => x.UsePostgres(connectionString));

            var provider = services.BuildServiceProvider();

            TransactionContext = provider.GetRequiredService<ITransactionContext>();
            DbConnectionFactory = provider.GetRequiredService<IDbConnectionFactory>();

            await CreateDb();
        }

        public Task DisposeAsync()
        {
            return _postgreSqlContainer.DisposeAsync().AsTask();
        }

        private Task CreateDb()
        {
            const string sql = "CREATE TABLE IF NOT EXISTS customers (id UUID NOT NULL, name VARCHAR NOT NULL, PRIMARY KEY (id))";

            TransactionContext.Add(sql);

            return TransactionContext.Commit();
        }
    }
}
