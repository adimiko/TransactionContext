using System.Data.Common;
using System.Data;

namespace TransactionContext.Internals
{
    internal sealed class InternalTransactionContext : ITransactionContext
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly DbBatch _dbBatch;

        public InternalTransactionContext(
            IDbConnectionFactory connectionFactory,
            DbBatch dbBatch)
        {
            _connectionFactory = connectionFactory;
            _dbBatch = dbBatch;
        }

        public void Add(string sql, object? parameters = null, CommandType command = CommandType.Text)
        {
            var cmd = _dbBatch.CreateBatchCommand();
            cmd.CommandText = sql;
            cmd.CommandType = command;

            if (parameters != null)
            {
                var properties = parameters.GetType().GetProperties();

                foreach (var property in properties)
                {
                    var parameter = cmd.CreateParameter();

                    parameter.ParameterName = property.Name;
                    parameter.Value = property.GetValue(parameters);

                    cmd.Parameters.Add(parameter);
                }
            }

            _dbBatch.BatchCommands.Add(cmd);
        }

        public async Task Commit(CancellationToken cancellationToken = default)
        {
            using (DbConnection connection = _connectionFactory.Create())
            {
                await connection
                    .OpenAsync(cancellationToken)
                    .ConfigureAwait(false);

                using (DbTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        _dbBatch.Connection = connection;
                        _dbBatch.Transaction = transaction;

                        await _dbBatch
                            .ExecuteNonQueryAsync(cancellationToken)
                            .ConfigureAwait(false);

                        await transaction
                            .CommitAsync(cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        await transaction
                            .RollbackAsync(cancellationToken)
                            .ConfigureAwait(false);

                        throw;
                    }
                }
            }
        }
    }
}
