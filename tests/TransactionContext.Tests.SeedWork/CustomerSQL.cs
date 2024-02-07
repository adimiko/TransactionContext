namespace TransactionContext.Tests.SeedWork
{
    public static class CustomerSQL
    {
        public const string Insert = "INSERT INTO Customers (CustomerId, Name) VALUES (@CustomerId, @Name)";
    }
}
