using DbProxy;
using System.Data.OracleClient;

namespace OracleProxy
{
    public class OracleConnectionProxy : DbConnectionProxy<OracleConnection, OracleException>, IOracleProxy
    {
        public OracleConnectionProxy(string connectionString)
            : base(connectionString)
        { }

        public OracleConnectionProxy(string[] connectionStrings, ConnectionOption connectionOption = ConnectionOption.FirstOnly, int maxAttempts = 1)
            : base(connectionStrings, connectionOption, maxAttempts)
        {
        }

        protected override OracleConnection GetConnection(string connectionString) => new OracleConnection(connectionString);
    }
}
