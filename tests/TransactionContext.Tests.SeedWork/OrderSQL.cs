namespace TransactionContext.Tests.SeedWork
{
    public static class OrderSQL
    {
        public const string Insert = "INSERT INTO Orders (OrderId, CustomerId) VALUES (@OrderId, @CustomerId)";
    }
}
