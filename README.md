# TransactionContext (Proof of concept)
TransactionContext is designed to execute statements changing state (also call to stored procedures) in a single transaction and a single request to database (collects SQL statements without instant execution). 

It can be an alternative for Entity Framework Core and TransactionScope in .NET.     
   
## ITransactionContext
```csharp
transactionContext.Add("INSERT INTO Customers (CustomerId) VALUES (@CustomerId)", new { CustomerId = Guid.NewGuid() });
transactionContext.Add("INSERT INTO Orders (OrderId) VALUES (@OrderId)", new { OrderId = Guid.NewGuid() });
```

`transactionContext.Commit()` open connection, create transaction and execute sql statments in a single database request.

```csharp
await transactionContext.Commit();
```

## IDbConnectionFactory
Package also includes a database connection factory.
```csharp
using var connection = dbConnectionFactory.Create()
```

## Databases
### Postgres
#### Add package
```csharp
dotnet add package TransactionContext.Postgres
```
#### Register dependencies
```csharp
services.AddTransactionContext(x => x.UsePostgres(connectionString));
```

### Microsoft SQL Server (preview)
#### Add package
```csharp
dotnet add package TransactionContext.SqlServer
```
#### Register dependencies
```csharp
services.AddTransactionContext(x => x.UseSqlServer(connectionString));
```

### MySql (also MariaDb)
#### Add package
```csharp
dotnet add package TransactionContext.MySql
```
#### Register dependencies
```csharp
services.AddTransactionContext(x => x.UseMySql(connectionString));
```
