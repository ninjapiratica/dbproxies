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

        private string[] _connectionStrings = new string[]
        {
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PersonContext;Integrated Security=True",
            "Data Source=.;Initial Catalog=Custom;Integrated Security=True"
        };

        [TestInitialize]
        public void Initialize()
        {
            _proxy = new SqlConnectionProxy(_connectionStrings, connectionOption: ConnectionOption.RoundRobin);
        }

        [TestMethod]
        public async Task Exception()
        {
            var connectionStrings = new List<string>();
            await Assert.ThrowsExceptionAsync<AggregateException>(() => _proxy.RunAsync(async (con) =>
                {
                    connectionStrings.Add(con.ConnectionString);
                    return await Task.FromException<int>(new Exception());
                })
            );

            Assert.AreEqual(1, connectionStrings.Count);
            Assert.AreEqual(_connectionStrings[0], connectionStrings[0]);

            connectionStrings.Clear();
            await Assert.ThrowsExceptionAsync<AggregateException>(() => _proxy.RunAsync(async (con) =>
                {
                    connectionStrings.Add(con.ConnectionString);
                    return await Task.FromException<int>(new Exception());
                })
            );

            Assert.AreEqual(1, connectionStrings.Count);
            Assert.AreEqual(_connectionStrings[1], connectionStrings[0]);

            connectionStrings.Clear();
            await Assert.ThrowsExceptionAsync<AggregateException>(() => _proxy.RunAsync(async (con) =>
                {
                    connectionStrings.Add(con.ConnectionString);
                    return await Task.FromException<int>(new Exception());
                })
            );

            Assert.AreEqual(1, connectionStrings.Count);
            Assert.AreEqual(_connectionStrings[0], connectionStrings[0]);
        }

        [TestMethod]
        public async Task NoException()
        {
            var connectionStrings = new List<string>();
            await _proxy.RunAsync(async (con) =>
            {
                connectionStrings.Add(con.ConnectionString);
                return await Task.FromResult(0);
            });

            Assert.AreEqual(1, connectionStrings.Count);
            Assert.AreEqual(_connectionStrings[0], connectionStrings[0]);

            connectionStrings.Clear();
            await _proxy.RunAsync(async (con) =>
            {
                connectionStrings.Add(con.ConnectionString);
                return await Task.FromResult(0);
            });

            Assert.AreEqual(1, connectionStrings.Count);
            Assert.AreEqual(_connectionStrings[1], connectionStrings[0]);

            connectionStrings.Clear();
            await _proxy.RunAsync(async (con) =>
            {
                connectionStrings.Add(con.ConnectionString);
                return await Task.FromResult(0);
            });

            Assert.AreEqual(1, connectionStrings.Count);
            Assert.AreEqual(_connectionStrings[0], connectionStrings[0]);
        }
    }
}
