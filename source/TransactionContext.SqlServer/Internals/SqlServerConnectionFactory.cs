using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace TransactionContext.SqlServer.Internals
{
    internal sealed class SqlServerConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlServerConnectionFactory(string connectionString) => _connectionString = connectionString;

        public DbConnection Create() => new SqlConnection(_connectionString);
    }
}
