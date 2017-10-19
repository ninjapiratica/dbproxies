namespace NinjaPiratica.SqlProxy.ConsoleTester
{
    class Program
    {
        static string connectionString = "Data Source=.;Initial Catalog=Custom1;Integrated Security=True";

        static void Main(string[] args)
        {
            ISqlProxy proxy = new SqlConnectionProxy(connectionString);

            var t = proxy.RunAsync(async (con) =>
            {
                await con.OpenAsync();
                return 0;
            });

            var result = t.GetAwaiter().GetResult();
        }
    }
}
