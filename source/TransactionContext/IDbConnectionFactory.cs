using System.Data.Common;

namespace TransactionContext
{
    public interface IDbConnectionFactory
    {
        DbConnection Create();
    }
}
