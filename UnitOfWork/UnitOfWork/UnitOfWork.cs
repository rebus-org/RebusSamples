using System;
using System.Configuration;
using System.Data.SqlClient;

namespace UnitOfWork
{
    class UnitOfWork : IDisposable
    {
        readonly string _connectionString;

        SqlConnection _currentConnection;
        SqlTransaction _currentTransaction;

        public UnitOfWork()
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings["database"];
            if (connectionStringSettings == null)
            {
                throw new ConfigurationErrorsException("Could not find connection string 'database' in the current app.config!");
            }

            _connectionString = connectionStringSettings.ConnectionString;
        }

        public SqlConnection GetConnection()
        {
            EnsureInitialized();

            return _currentConnection;
        }

        public SqlTransaction GetTransaction()
        {
            EnsureInitialized();

            return _currentTransaction;
        }

        public void Commit()
        {
            _currentTransaction?.Commit();
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _currentConnection?.Dispose();
        }

        void EnsureInitialized()
        {
            if (_currentConnection != null) return;

            lock (this)
            {
                if (_currentConnection != null) return;

                _currentConnection = new SqlConnection(_connectionString);
                _currentConnection.Open();

                _currentTransaction = _currentConnection.BeginTransaction();
            }
        }
    }
}