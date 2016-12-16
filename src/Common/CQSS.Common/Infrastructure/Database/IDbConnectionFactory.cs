using System.Data;

namespace CQSS.Common.Infrastructure.Database
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create(string name);
    }
}