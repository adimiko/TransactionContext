using Npgsql;
using System.Data.Common;

namespace TransactionContext.Postgres.Internals
{
    internal sealed class PostgresConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public PostgresConnectionFactory(string connectionString) => _connectionString = connectionString;

        public DbConnection Create() => new NpgsqlConnection(_connectionString);
    }
}
