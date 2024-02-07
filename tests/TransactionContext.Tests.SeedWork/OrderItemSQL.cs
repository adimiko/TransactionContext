namespace TransactionContext.Tests.SeedWork
{
    public static class OrderItemSQL
    {
        public const string Insert = "INSERT INTO OrderItems (OrderItemId, OrderId, Name, Amount) VALUES (@OrderItemId, @OrderId, @Name, @Amount)";
    }
}
