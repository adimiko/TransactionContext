using System.Data;

namespace TransactionContext
{
    public interface ITransactionContext
    {
        void Add(string sql, object? parameters = null, CommandType command = CommandType.Text);

        Task Commit(CancellationToken cancellationToken = default);
    }
}
