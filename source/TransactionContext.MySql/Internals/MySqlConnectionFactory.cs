using System.Data.Common;
using MySqlConnector;

namespace TransactionContext.MySql.Internals
{
    internal sealed class MySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public MySqlConnectionFactory(string connectionString) => _connectionString = connectionString;

        public DbConnection Create() => new MySqlConnection(_connectionString);
    }
}
