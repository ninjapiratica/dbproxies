using DbProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlProxy.Test.SqlConnectionProxyTests
{
    [TestClass]
    public class RoundRobinTests
    {
        private SqlConnectionProxy _proxy;

        private string _firstDb = "PersonContext";
        private string _secondDb = "Custom";

        [TestInitialize]
        public void Initialize()
        {
            _proxy = new SqlConnectionProxy(new string[] {
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PersonContext;Integrated Security=True",
                "Data Source=.;Initial Catalog=Custom;Integrated Security=True"
            }, connectionOption: ConnectionOption.RoundRobin);
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

            Assert.AreEqual(1, count);
            Assert.AreEqual(_firstDb, dbs[0]);

            count = 0;
            dbs = new List<string>();
            await Assert.ThrowsExceptionAsync<AggregateException>(() => _proxy.RunAsync(async (con) =>
                {
                    dbs.Add(con.Database);
                    count++;
                    return await Task.FromException<int>(new Exception());
                })
            );

            Assert.AreEqual(1, count);
            Assert.AreEqual(_secondDb, dbs[0]);

            count = 0;
            dbs = new List<string>();
            await Assert.ThrowsExceptionAsync<AggregateException>(() => _proxy.RunAsync(async (con) =>
                {
                    dbs.Add(con.Database);
                    count++;
                    return await Task.FromException<int>(new Exception());
                })
            );

            Assert.AreEqual(1, count);
            Assert.AreEqual(_firstDb, dbs[0]);
        }

        [TestMethod]
        public async Task NoException()
        {
            var count = 0;
            var db = string.Empty;
            await _proxy.RunAsync(async (con) =>
            {
                db = con.Database;
                count++;
                return await Task.FromResult(0);
            });

            Assert.AreEqual(1, count);
            Assert.AreEqual(_firstDb, db);

            count = 0;
            db = string.Empty;
            await _proxy.RunAsync(async (con) =>
            {
                db = con.Database;
                count++;
                return await Task.FromResult(0);
            });

            Assert.AreEqual(1, count);
            Assert.AreEqual(_secondDb, db);

            count = 0;
            db = string.Empty;
            await _proxy.RunAsync(async (con) =>
            {
                db = con.Database;
                count++;
                return await Task.FromResult(0);
            });

            Assert.AreEqual(1, count);
            Assert.AreEqual(_firstDb, db);
        }
    }
}
