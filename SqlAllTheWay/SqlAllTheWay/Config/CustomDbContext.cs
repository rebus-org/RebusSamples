using System;
using System.Threading;
using Microsoft.Data.SqlClient;
using Rebus.SqlServer;
// ReSharper disable ArgumentsStyleLiteral

namespace SqlAllTheWay.Config
{
    public class CustomDbContext : IDisposable
    {
        public static readonly AsyncLocal<DbConnectionWrapper> AsyncLocalDbConnection = new AsyncLocal<DbConnectionWrapper>();
        readonly SqlConnection _connection;
        readonly SqlTransaction _transaction;

        public CustomDbContext()
        {
            _connection = new SqlConnection(DbConfig.ConnectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            // the connection is managed externally in this case because WE commit and dispose it
            var connectionWrapper = new DbConnectionWrapper(_connection, _transaction, managedExternally: true);

            AsyncLocalDbConnection.Value = connectionWrapper;
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            _transaction.Dispose();
            _connection.Dispose();
            AsyncLocalDbConnection.Value = null;
        }
    }
}