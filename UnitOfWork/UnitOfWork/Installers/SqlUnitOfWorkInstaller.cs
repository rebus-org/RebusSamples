using System;
using System.Configuration;
using System.Data.SqlClient;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Rebus.Transport;
#pragma warning disable 1998

namespace UnitOfWork.Installers
{
    public class SqlUnitOfWorkInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings["database"];
            if (connectionStringSettings == null)
            {
                throw new ConfigurationErrorsException("Could not find connection string 'database' in the current app.config!");
            }

            var connectionString = connectionStringSettings.ConnectionString;

            container.Register(
                Component.For<SqlConnection>()
                    .UsingFactoryMethod(k => GetSqlConnection(connectionString), managedExternally: true)
                    .LifestyleTransient(),

                Component.For<SqlTransaction>()
                    .UsingFactoryMethod(GetSqlTransaction, managedExternally: true)
                    .LifestyleTransient()
                );
        }

        static SqlTransaction GetSqlTransaction(IKernel kernel)
        {
            var transactionContext = GetTransactionContext();

            return transactionContext
                .GetOrAdd("current-sql-transaction", () =>
                {
                    var sqlConnection = kernel.Resolve<SqlConnection>();
                    var transaction = sqlConnection.BeginTransaction();

                    transactionContext.OnCommitted(async () => transaction.Commit());
                    
                    return transaction;
                });
        }

        static SqlConnection GetSqlConnection(string connectionString)
        {
            var transactionContext = GetTransactionContext();

            return transactionContext
                .GetOrAdd("current-sql-connection", () =>
                {
                    var sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();

                    transactionContext.OnDisposed(() => sqlConnection.Dispose());

                    return sqlConnection;
                });
        }

        static ITransactionContext GetTransactionContext()
        {
            var transactionContext = AmbientTransactionContext.Current;
            if (transactionContext == null)
            {
                throw new InvalidOperationException("Attempted to get transaction context outside of a message handler!");
            }
            return transactionContext;
        }
    }
}