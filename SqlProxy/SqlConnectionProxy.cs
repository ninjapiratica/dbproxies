using DbProxy;
using System.Data.SqlClient;

namespace SqlProxy
{
    public class SqlConnectionProxy : DbConnectionProxy<SqlConnection, SqlException>, ISqlProxy
    {
        public SqlConnectionProxy(string connectionString)
            : base(connectionString)
        { }

        public SqlConnectionProxy(string[] connectionStrings, ConnectionOption connectionOption = ConnectionOption.FirstOnly, int maxAttempts = 1)
            : base(connectionStrings, connectionOption, maxAttempts)
        {
        }

        protected override SqlConnection GetConnection(string connectionString) => new SqlConnection(connectionString);
    }
}
