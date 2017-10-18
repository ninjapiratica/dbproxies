using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DbProxy
{
    public abstract class DbConnectionProxy<TConnection, TException> : IDbProxy<TConnection> where TConnection : DbConnection where TException : DbException
    {
        private int _connectionStringIndex = 0;
        private bool _fallback => ConnectionOption == ConnectionOption.Fallback || ConnectionOption == ConnectionOption.RoundRobinWithFallback;
        private bool _roundRobin => ConnectionOption == ConnectionOption.RoundRobin || ConnectionOption == ConnectionOption.RoundRobinWithFallback;

        public string[] ConnectionStrings { get; }
        public ConnectionOption ConnectionOption { get; }
        public int MaxAttempts { get; }

        public DbConnectionProxy(string connectionString)
            : this(new string[] { connectionString })
        { }

        public DbConnectionProxy(string[] connectionStrings, ConnectionOption connectionOption = ConnectionOption.FirstOnly, int maxAttempts = 1)
        {
            if (connectionStrings == null)
                throw new ArgumentNullException(nameof(connectionStrings));
            else if (connectionStrings.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(connectionStrings), $"{nameof(connectionStrings)} must have at least 1.");
            ConnectionStrings = connectionStrings;

            if (maxAttempts <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxAttempts), maxAttempts, $"{nameof(maxAttempts)} must be greater than or equal to 1.");
            MaxAttempts = maxAttempts;
            
            ConnectionOption = connectionOption;
        }

        #region IDbProxy
        public Task<T> RunAsync<T>(Func<TConnection, Task<T>> function, CancellationToken cancellationToken = default(CancellationToken)) =>
            RunAsync((con, token) => function(con), cancellationToken);

        public async Task<T> RunAsync<T>(Func<TConnection, CancellationToken, Task<T>> function, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));

            try
            {
                var attempts = 0;
                var connectionStringIndex = _connectionStringIndex;
                var exceptions = new List<Exception>();

                do
                {
                    attempts++;

                    try
                    {
                        return await AttemptConnectionAsync(ConnectionStrings[connectionStringIndex], function, cancellationToken);
                    }
                    catch (AggregateException ex)
                    {
                        exceptions.Add(ex);
                    }

                    connectionStringIndex++;
                    if (connectionStringIndex >= ConnectionStrings.Length)
                        connectionStringIndex = 0;
                } while (_fallback && attempts < ConnectionStrings.Length);

                throw new AggregateException(exceptions);
            }
            finally
            {
                if (_roundRobin)
                {
                    _connectionStringIndex++;
                    if (_connectionStringIndex >= ConnectionStrings.Length)
                        _connectionStringIndex = 0;
                }
            }
        }
        #endregion

        private async Task<T> AttemptConnectionAsync<T>(string connectionString, Func<TConnection, CancellationToken, Task<T>> function, CancellationToken cancellationToken)
        {
            var attempts = 0;
            var canRetry = false;
            var exceptions = new List<Exception>();
            do
            {
                attempts++;
                var attempt = await MakeAttemptAsync(connectionString, function, cancellationToken);

                if (attempt.exception != null)
                {
                    canRetry = attempt.canRetry;
                    exceptions.Add(attempt.exception);
                }
                else
                {
                    return attempt.result;
                }
            } while (canRetry && attempts < MaxAttempts);

            throw new AggregateException(exceptions);
        }

        private async Task<(T result, bool canRetry, Exception exception)> MakeAttemptAsync<T>(string connectionString, Func<TConnection, CancellationToken, Task<T>> function, CancellationToken cancellationToken)
        {
            T result = default(T);
            bool canRetry = false;
            Exception exception = null;

            try
            {
                using (var connection = GetConnection(connectionString))
                {
                    result = await function(connection, cancellationToken);
                }
            }
            catch (TException ex)
            {
                exception = ex;
                canRetry = true;
            }
            catch (Exception ex)
            {
                exception = ex;
                canRetry = false;
            }

            return (result, canRetry, exception);
        }

        protected abstract TConnection GetConnection(string connectionString);
    }
}
