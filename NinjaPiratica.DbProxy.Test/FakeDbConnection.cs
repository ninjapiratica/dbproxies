using System.Data;
using System.Data.Common;

namespace NinjaPiratica.DbProxy.Test
{
    public class FakeDbConnection : DbConnection
    {
        public FakeDbConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public override string ConnectionString { get; set; }

        public override string Database => string.Empty;

        public override string DataSource => string.Empty;

        public override string ServerVersion => string.Empty;

        public override ConnectionState State => ConnectionState.Open;

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
        {
        }

        public override void Open()
        {
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => new FakeDbTransaction();

        protected override DbCommand CreateDbCommand() => new FakeDbCommand();
    }
}
