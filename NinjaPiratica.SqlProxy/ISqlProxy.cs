using NinjaPiratica.DbProxy;
using System.Data.SqlClient;

namespace NinjaPiratica.SqlProxy
{
    public interface ISqlProxy : IDbProxy<SqlConnection>
    {
    }
}