using CQSS.Common.Extension;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CQSS.Common.Infrastructure.Database
{
    public class DefaultDbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection Create(string name)
        {
            var connectionString = ConfigurationManager.AppSettings[name] ?? string.Empty;
            if (string.IsNullOrEmpty(connectionString))
                throw new ConfigurationErrorsException("获取 ConnectionString 失败，无法在配置文件的 AppSettings 中找到节点 {0}".FormatWith(name));

            var connection = new SqlConnection(connectionString);
            connection.Open();

            return connection;
        }
    }
}