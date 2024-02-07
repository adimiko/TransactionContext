namespace TransactionContext.Tests.SeedWork
{
    public readonly record struct OrderItem(Guid OrderItemId, Guid OrderId, string Name, int Amount);
}
