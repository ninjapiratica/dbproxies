using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace DbProxy.Test.SqlConnectionProxyTests
{
    [TestClass]
    public class RetrySqlExceptionTests
    {
        private FakeDbConnectionProxy _proxy;
        private int maxAttempts = 3;

        [TestInitialize]
        public void Initialize()
        {
            _proxy = new FakeDbConnectionProxy(new string[] {
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
                    count++;
                    return await Task.FromException<int>(new FakeDbException());
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
    }
}
