namespace NinjaPiratica.DbProxy.Test
{
    public class FakeDbConnectionProxy : DbConnectionProxy<FakeDbConnection, FakeDbException>
    {
        public FakeDbConnectionProxy(string connectionString)
            : base(connectionString)
        {
        }

        public FakeDbConnectionProxy(string[] connectionStrings, ConnectionOption connectionOption = ConnectionOption.FirstOnly, int maxAttempts = 1)
            : base(connectionStrings, connectionOption, maxAttempts)
        {
        }

        protected override FakeDbConnection GetConnection(string connectionString) => new FakeDbConnection(connectionString);
    }
}
