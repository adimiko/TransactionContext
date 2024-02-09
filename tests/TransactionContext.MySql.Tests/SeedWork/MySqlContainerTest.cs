using Dapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using Testcontainers.MySql;
using Xunit;

namespace TransactionContext.MySql.Tests.SeedWork
{
    public abstract class MySqlContainerTest : IAsyncLifetime
    {
        private readonly MySqlContainer _container = new MySqlBuilder().Build();

        protected ITransactionContext TransactionContext { get; private set; }

        protected IDbConnectionFactory DbConnectionFactory { get; private set; }

        public async Task InitializeAsync()
        {
            await _container.StartAsync();

            var connectionString = _container.GetConnectionString();

            IServiceCollection services = new ServiceCollection();

            services.AddTransactionContext(x => x.UseMySql(connectionString));

            var provider = services.BuildServiceProvider();

            TransactionContext = provider.GetRequiredService<ITransactionContext>();
            DbConnectionFactory = provider.GetRequiredService<IDbConnectionFactory>();

            await CreateDb();
        }

        public Task DisposeAsync()
        {
            return _container.DisposeAsync().AsTask();
        }

        private Task CreateDb()
        {
            const string customerSql = "CREATE TABLE IF NOT EXISTS Customers(CustomerId CHAR(36), Name VARCHAR(100))";
            const string orderSql = "CREATE TABLE IF NOT EXISTS Orders(OrderId CHAR(36), CustomerId CHAR(36))";
            const string orderItemSql = "CREATE TABLE IF NOT EXISTS OrderItems(OrderItemId CHAR(36), OrderId CHAR(36), Name VARCHAR(100), Amount INT)";

            TransactionContext.Add(customerSql);
            TransactionContext.Add(orderSql);
            TransactionContext.Add(orderItemSql);

            return TransactionContext.Commit();
        }
    }
}
