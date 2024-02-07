using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using System.Transactions;
using Testcontainers.PostgreSql;
using TransactionContext.Tests.SeedWork;

namespace TransactionContext.Postgres.Benchmark
{
    [SimpleJob(RunStrategy.ColdStart, iterationCount: 1000)]
    public class BenchmarkTest
    {
        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

        protected IDbConnectionFactory DbConnectionFactory { get; private set; }

        protected IServiceProvider ServiceProvider { get; private set; }

        [GlobalSetup]
        public async Task GlobalSetup()
        {
            await _postgreSqlContainer.StartAsync();

            var connectionString = _postgreSqlContainer.GetConnectionString();

            IServiceCollection services = new ServiceCollection();

            services.AddTransactionContext(x => x.UsePostgres(connectionString));

            ServiceProvider = services.BuildServiceProvider();
            DbConnectionFactory = ServiceProvider.GetRequiredService<IDbConnectionFactory>();

            await CreateDb();
        }

        const int NumberOfCustomers = 10;

        [Benchmark]
        public async Task TransactionContextTest_Single()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var transactionContext = scope.ServiceProvider.GetRequiredService<ITransactionContext>();

                var customerId = Guid.NewGuid();

                transactionContext.Add(CustomerSQL.Insert, new { CustomerId = customerId, Name = "Name" });

                await transactionContext.Commit();
            }
        }

        [Benchmark]
        public async Task TransactionScopeTest_Single()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var connection = DbConnectionFactory.Create())
            {
                var customerId = Guid.NewGuid();

                await connection.ExecuteAsync(CustomerSQL.Insert, new { CustomerId = customerId, Name = "Name" });

                scope.Complete();
            }
        }

        [Benchmark]
        public async Task TransactionContextTest_Multiple()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var transactionContext = scope.ServiceProvider.GetRequiredService<ITransactionContext>();

                for (var i = 0; i < NumberOfCustomers; i++)
                {
                    var customerId = Guid.NewGuid();
                    var orderId = Guid.NewGuid();

                    transactionContext.Add(CustomerSQL.Insert, new { CustomerId = customerId, Name = $"name{i}" });
                    transactionContext.Add(OrderSQL.Insert, new { OrderId = orderId, CustomerId = customerId });

                    transactionContext.Add(OrderItemSQL.Insert, new { OrderItemId = Guid.NewGuid(), OrderId = orderId, Name = $"Name{i}", Amount = i + 1 });
                    transactionContext.Add(OrderItemSQL.Insert, new { OrderItemId = Guid.NewGuid(), OrderId = orderId, Name = $"Name{i}", Amount = i + 1 });
                    transactionContext.Add(OrderItemSQL.Insert, new { OrderItemId = Guid.NewGuid(), OrderId = orderId, Name = $"Name{i}", Amount = i + 1 });
                    transactionContext.Add(OrderItemSQL.Insert, new { OrderItemId = Guid.NewGuid(), OrderId = orderId, Name = $"Name{i}", Amount = i + 1 });
                }

                await transactionContext.Commit();
            }
        }

        [Benchmark]
        public async Task TransactionScopeTest_Multiple()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var connection = DbConnectionFactory.Create())
            {
                for (var i = 0; i < NumberOfCustomers; i++)
                {
                    var customerId = Guid.NewGuid();
                    var orderId = Guid.NewGuid();

                    await connection.ExecuteAsync(CustomerSQL.Insert, new { CustomerId = customerId, Name = $"name{i}" });
                    await connection.ExecuteAsync(OrderSQL.Insert, new { OrderId = orderId, CustomerId = customerId });

                    await connection.ExecuteAsync(OrderItemSQL.Insert, new { OrderItemId = Guid.NewGuid(), OrderId = orderId, Name = $"Name{i}", Amount = i + 1 });
                    await connection.ExecuteAsync(OrderItemSQL.Insert, new { OrderItemId = Guid.NewGuid(), OrderId = orderId, Name = $"Name{i}", Amount = i + 1 });
                    await connection.ExecuteAsync(OrderItemSQL.Insert, new { OrderItemId = Guid.NewGuid(), OrderId = orderId, Name = $"Name{i}", Amount = i + 1 });
                    await connection.ExecuteAsync(OrderItemSQL.Insert, new { OrderItemId = Guid.NewGuid(), OrderId = orderId, Name = $"Name{i}", Amount = i + 1 });
                }

                scope.Complete();
            }
        }

        [GlobalCleanup]
        public Task GlobalCleanup()
        {
            return _postgreSqlContainer.DisposeAsync().AsTask();
        }

        private async Task CreateDb()
        {
            const string customerSql = "CREATE TABLE IF NOT EXISTS Customers (CustomerId UUID NOT NULL, Name VARCHAR NOT NULL, PRIMARY KEY (CustomerId))";
            const string orderSql = "CREATE TABLE IF NOT EXISTS Orders (OrderId UUID NOT NULL, CustomerId UUID NOT NULL, PRIMARY KEY (OrderId))";
            const string orderItemSql = "CREATE TABLE IF NOT EXISTS OrderItems (OrderItemId UUID NOT NULL, OrderId UUID NOT NULL, Name VARCHAR NOT NULL, Amount INT NOT NULL, PRIMARY KEY (OrderItemId))";
            using var connection = DbConnectionFactory.Create();

            await connection.ExecuteAsync(customerSql);
            await connection.ExecuteAsync(orderSql);
            await connection.ExecuteAsync(orderItemSql);
        }
    }
}
