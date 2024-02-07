# TransactionContext (Proof of concept)
TransactionContext is designed to execute statements changing state (also call to stored procedures) in a single transaction and a single request to database (collects SQL statements without instant execution). 

It can be an alternative for Entity Framework Core and TransactionScope in .NET.     

```sh
transactionContext.Add("INSERT INTO Customers (CustomerId) VALUES (@CustomerId)", new { CustomerId = Guid.NewGuid() });
transactionContext.Add("INSERT INTO Orders (OrderId) VALUES (@OrderId)", new { OrderId = Guid.NewGuid() });
```

`transactionContext.Commit()` open connection, create transaction and execute sql statments in a single database request.

```sh
await transactionContext.Commit();
```
