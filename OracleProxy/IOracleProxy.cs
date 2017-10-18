using DbProxy;
using System.Data.OracleClient;

namespace OracleProxy
{
    public interface IOracleProxy : IDbProxy<OracleConnection>
    {
    }
}