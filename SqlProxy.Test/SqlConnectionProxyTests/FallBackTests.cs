using DbProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace SqlProxy.Test.SqlConnectionProxyTests
{
    [TestClass]
    public class FallBackTests
    {
        private class FakeDbException : DbException { }
        private SqlConnectionProxy _proxy;

        private string _firstDb = "PersonContext";
        private string _secondDb = "Custom";

        [TestInitialize]
        public void Initialize()
        {
            _proxy = new SqlConnectionProxy(new string[] {
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PersonContext;Integrated Security=True",
                "Data Source=.;Initial Catalog=Custom;Integrated Security=True"
            }, connectionOption: ConnectionOption.Fallback);
        }

        [TestMethod]
        public async Task Exception()
        {
            var count = 0;
            var dbs = new List<string>();
            await Assert.ThrowsExceptionAsync<AggregateException>(() => _proxy.RunAsync(async (con) =>
                {
                    dbs.Add(con.Database);
                    count++;
                    return await Task.FromException<int>(new Exception());
                })
            );

            Assert.AreEqual(_proxy.ConnectionStrings.Length, count);
            Assert.AreEqual(_firstDb, dbs[0]);
            Assert.AreEqual(_secondDb, dbs[1]);

            count = 0;
            dbs = new List<string>();
            await Assert.ThrowsExceptionAsync<AggregateException>(() => _proxy.RunAsync(async (con) =>
                {
                    dbs.Add(con.Database);
                    count++;
                    return await Task.FromException<int>(new Exception());
                })
            );

            Assert.AreEqual(_proxy.ConnectionStrings.Length, count);
            Assert.AreEqual(_firstDb, dbs[0]);
            Assert.AreEqual(_secondDb, dbs[1]);
        }

        [TestMethod]
        public async Task NoException()
        {
            var count = 0;
            var dbs = new List<string>();
            await _proxy.RunAsync(async (con) =>
            {
                dbs.Add(con.Database);
                count++;
                return await Task.FromResult(0);
            });

            Assert.AreEqual(1, count);
            Assert.AreEqual(_firstDb, dbs[0]);

            count = 0;
            dbs = new List<string>();
            await _proxy.RunAsync(async (con) =>
            {
                dbs.Add(con.Database);
                count++;
                return await Task.FromResult(0);
            });

            Assert.AreEqual(1, count);
            Assert.AreEqual(_firstDb, dbs[0]);
        }
    }
}
