using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Xunit;

namespace TransactionContext.SqlServer.Tests.SeedWork
{
    public abstract class SqlServerContainerTest : IAsyncLifetime
    {
        private readonly MsSqlContainer _sqlServerContainer = new MsSqlBuilder().Build();

        protected ITransactionContext TransactionContext { get; private set; }

        protected IDbConnectionFactory DbConnectionFactory { get; private set; }

        public async Task InitializeAsync()
        {
            await _sqlServerContainer.StartAsync();

            var connectionString = _sqlServerContainer.GetConnectionString();

            IServiceCollection services = new ServiceCollection();

            services.AddTransactionContext(x => x.UseSqlServer(connectionString));

            var provider = services.BuildServiceProvider();

            TransactionContext = provider.GetRequiredService<ITransactionContext>();
            DbConnectionFactory = provider.GetRequiredService<IDbConnectionFactory>();

            await CreateDb();
        }

        public Task DisposeAsync()
        {
            return _sqlServerContainer.DisposeAsync().AsTask();
        }

        private Task CreateDb()
        {
            const string customerSql = "IF OBJECT_ID(N'Customers', N'U') IS NULL CREATE TABLE Customers (CustomerId UNIQUEIDENTIFIER NOT NULL, Name VARCHAR(100) NOT NULL, PRIMARY KEY (CustomerId))";
            const string orderSql = "IF OBJECT_ID(N'Orders', N'U') IS NULL CREATE TABLE Orders (OrderId UNIQUEIDENTIFIER NOT NULL, CustomerId UNIQUEIDENTIFIER NOT NULL, PRIMARY KEY (OrderId))";
            const string orderItemSql = "IF OBJECT_ID(N'OrderItems', N'U') IS NULL CREATE TABLE OrderItems (OrderItemId UNIQUEIDENTIFIER NOT NULL, OrderId UNIQUEIDENTIFIER NOT NULL, Name VARCHAR(100) NOT NULL, Amount INT NOT NULL, PRIMARY KEY (OrderItemId))";

            TransactionContext.Add(customerSql);
            TransactionContext.Add(orderSql);
            TransactionContext.Add(orderItemSql);

            return TransactionContext.Commit();
        }
    }
}
