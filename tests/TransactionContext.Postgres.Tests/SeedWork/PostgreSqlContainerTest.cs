﻿using Microsoft.Extensions.DependencyInjection;
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
            const string customerSql = "CREATE TABLE IF NOT EXISTS Customers (CustomerId UUID NOT NULL, Name VARCHAR NOT NULL, PRIMARY KEY (CustomerId))";
            const string orderSql = "CREATE TABLE IF NOT EXISTS Orders (OrderId UUID NOT NULL, CustomerId UUID NOT NULL, PRIMARY KEY (OrderId))";
            const string orderItemSql = "CREATE TABLE IF NOT EXISTS OrderItems (OrderItemId UUID NOT NULL, OrderId UUID NOT NULL, Name VARCHAR NOT NULL, Amount INT NOT NULL, PRIMARY KEY (OrderItemId))";

            TransactionContext.Add(customerSql);
            TransactionContext.Add(orderSql);
            TransactionContext.Add(orderItemSql);

            return TransactionContext.Commit();
        }
    }
}
