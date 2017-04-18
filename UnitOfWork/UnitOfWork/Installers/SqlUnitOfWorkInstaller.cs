using System;
using System.Data.SqlClient;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Rebus.Transport;
// ReSharper disable ArgumentsStyleLiteral
#pragma warning disable 1998

namespace UnitOfWork.Installers
{
    public class SqlUnitOfWorkInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<SqlConnection>()
                    .UsingFactoryMethod(k => GetSqlConnection(), managedExternally: true)
                    .LifestyleTransient(),

                Component.For<SqlTransaction>()
                    .UsingFactoryMethod(GetSqlTransaction, managedExternally: true)
                    .LifestyleTransient()
                );
        }

        static SqlConnection GetSqlConnection()
        {
            return GetTransactionContext()
                .GetOrThrow<UnitOfWork>("uow")
                .GetConnection();
        }

        static SqlTransaction GetSqlTransaction(IKernel kernel)
        {
            return GetTransactionContext()
                .GetOrThrow<UnitOfWork>("uow")
                .GetTransaction();
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