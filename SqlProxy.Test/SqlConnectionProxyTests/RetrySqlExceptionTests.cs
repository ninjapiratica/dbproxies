using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SqlProxy.Test.SqlConnectionProxyTests
{
    [TestClass]
    public class RetrySqlExceptionTests
    {
        private class FakeDbException : DbException { }
        private SqlConnectionProxy _proxy;
        private int maxAttempts = 3;

        [TestInitialize]
        public void Initialize()
        {
            _proxy = new SqlConnectionProxy(new string[] {
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PersonContext;Integrated Security=True"
            }, maxAttempts: maxAttempts);
        }

        [TestMethod]
        public async Task GenericException()
        {
            var count = 0;
            await Assert.ThrowsExceptionAsync<AggregateException>(() => _proxy.RunAsync(async (con) =>
                {
                    count++;
                    return await Task.FromException<int>(new Exception());
                })
            );

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task SqlException()
        {
            var count = 0;
            await Assert.ThrowsExceptionAsync<AggregateException>(() => _proxy.RunAsync(async (con) =>
                {
                    var exception = Instantiate<SqlException>();

                    count++;
                    return await Task.FromException<int>(exception);
                })
            );

            Assert.AreEqual(maxAttempts, count);
        }

        [TestMethod]
        public async Task NoException()
        {
            var count = 0;
            await _proxy.RunAsync(async (con) =>
            {
                count++;
                return await Task.FromResult(0);
            });

            Assert.AreEqual(1, count);
        }

        private T Instantiate<T>() where T : class => FormatterServices.GetUninitializedObject(typeof(T)) as T;
    }
}
