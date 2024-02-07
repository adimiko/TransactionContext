using Dapper;
using TransactionContext.Postgres.Tests.SeedWork;
using TransactionContext.Tests.SeedWork;
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

            for (var i = 0; i < numberOfCustomers; i++)
            {
                var customerId = Guid.NewGuid();
                var orderId = Guid.NewGuid();

                TransactionContext.Add(CustomerSQL.Insert, new { CustomerId = customerId, Name = $"name{i}" });
                TransactionContext.Add(OrderSQL.Insert, new { OrderId = orderId, CustomerId = customerId });
                
                TransactionContext.Add(OrderItemSQL.Insert, new { OrderItemId = Guid.NewGuid(), OrderId = orderId, Name = $"Name{i}", Amount = i + 1 });
                TransactionContext.Add(OrderItemSQL.Insert, new { OrderItemId = Guid.NewGuid(), OrderId = orderId, Name = $"Name{i}", Amount = i + 1 });
                TransactionContext.Add(OrderItemSQL.Insert, new { OrderItemId = Guid.NewGuid(), OrderId = orderId, Name = $"Name{i}", Amount = i + 1 });
                TransactionContext.Add(OrderItemSQL.Insert, new { OrderItemId = Guid.NewGuid(), OrderId = orderId, Name = $"Name{i}", Amount = i + 1 });
            }

            // Act
            await TransactionContext.Commit();

            // Assert
            const string selectCustomersSql = "SELECT CustomerId, Name FROM Customers";
            using var connection = DbConnectionFactory.Create();
            var customers = await connection.QueryAsync<Customer>(selectCustomersSql);

            const string selectOrdersSql = "SELECT OrderId, CustomerId FROM Orders";
            var orders = await connection.QueryAsync<Order>(selectOrdersSql);

            const string selectOrderItemssSql = "SELECT OrderItemId, OrderId, Name, Amount FROM OrderItems";
            var orderItems = await connection.QueryAsync<Order>(selectOrderItemssSql);

            Assert.Equal(numberOfCustomers, customers.Count());
            Assert.Equal(numberOfCustomers, orders.Count());
            Assert.Equal(4 * numberOfCustomers, orderItems.Count());
        }
    }
}
