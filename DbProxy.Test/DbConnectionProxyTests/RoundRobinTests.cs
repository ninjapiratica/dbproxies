using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbProxy.Test.SqlConnectionProxyTests
{
    [TestClass]
    public class RoundRobinTests
    {
        private FakeDbConnectionProxy _proxy;

        private string[] _connectionStrings = {
            "FakeConnectionString",
            "FakeConnectionString1"
        };

        [TestInitialize]
        public void Initialize()
        {
            _proxy = new FakeDbConnectionProxy(_connectionStrings, connectionOption: ConnectionOption.RoundRobin);
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
