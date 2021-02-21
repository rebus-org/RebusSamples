using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Rebus.SqlServer;
using Rebus.SqlServer.Transport;
using Rebus.Transport;
// ReSharper disable ArgumentsStyleLiteral

namespace SqlAllTheWay.Config
{
    public static class DbConfig
    {
        public static string ConnectionString => ConfigurationManager.ConnectionStrings["db"]?.ConnectionString ??
                                                 throw new ConfigurationErrorsException("Could not find 'db' connection string in the current application configuration file");

        public static async Task<IDbConnection> GetDbConnection()
        {
            return
                // if CustomDbContext started the connection, use it:
                GetCustomDbConnectionOrNull()

                // if we are in a Rebus message handler and we already provided a connection, use it:
                ?? await GetCurrentlyOngoingRebusDbConnectionOrNull()

                // otherwise create a new connection, letting Rebus manage it from now on:
                ?? await OpenNewDbConnection();
        }

        static IDbConnection GetCustomDbConnectionOrNull()
        {
            return CustomDbContext.AsyncLocalDbConnection.Value;
        }

        static async Task<IDbConnection> GetCurrentlyOngoingRebusDbConnectionOrNull()
        {
            var context = AmbientTransactionContext.Current;
            if (context == null) return null;

            return await context.GetOrThrow<Task<IDbConnection>>(SqlServerTransport.CurrentConnectionKey);
        }

        static async Task<IDbConnection> OpenNewDbConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();

            return new DbConnectionWrapper(connection, transaction, managedExternally: false);
        }
    }
}