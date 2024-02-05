using Dapper;
using TransactionContext.Postgres.Tests.SeedWork;
using Xunit;

namespace TransactionContext.Postgres.Tests
{
    public sealed class TransactionContextTests : PostgreSqlContainerTest
    {
        [Fact]
        public async Task BasicTest()
        {
            // Arrange
            const int numberOfCustomers = 100;
            const string sql = "INSERT INTO customers (id, name) VALUES(@id, @name)";

            for (var i = 0; i < numberOfCustomers; i++)
            {
                TransactionContext.Add(sql, new { id = Guid.NewGuid(), Name = $"name{i}" });
            }

            // Act
            await TransactionContext.Commit();

            // Assert
            const string selectSql = "SELECT id, name FROM customers";
            using var connection = DbConnectionFactory.Create();
            var customers = await connection.QueryAsync<Customer>(selectSql);

            Assert.Equal(numberOfCustomers, customers.Count());
        }
    }
}
