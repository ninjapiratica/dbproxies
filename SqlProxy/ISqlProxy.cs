using DbProxy;
using System.Data.SqlClient;

namespace SqlProxy
{
    public interface ISqlProxy : IDbProxy<SqlConnection>
    {
    }
}