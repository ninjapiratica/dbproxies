using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DbProxy
{
    public interface IDbProxy<S> where S : DbConnection
    {
        Task<T> RunAsync<T>(Func<S, Task<T>> function, CancellationToken cancellationToken = default(CancellationToken));
        Task<T> RunAsync<T>(Func<S, CancellationToken, Task<T>> function, CancellationToken cancellationToken = default(CancellationToken));
    }
}
