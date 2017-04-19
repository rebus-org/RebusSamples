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
                    .UsingFactoryMethod(() => GetUnitOfWork().GetConnection(), managedExternally: true)
                    .LifestyleTransient(),

                Component.For<SqlTransaction>()
                    .UsingFactoryMethod(() => GetUnitOfWork().GetTransaction(), managedExternally: true)
                    .LifestyleTransient()
                );
        }

        static UnitOfWork GetUnitOfWork()
        {
            var transactionContext = AmbientTransactionContext.Current;

            if (transactionContext == null)
            {
                throw new InvalidOperationException("Attempted to get transaction context outside of a message handler!");
            }

            // get unit of work that was stashed in the transaction context
            return transactionContext.GetOrThrow<UnitOfWork>("uow");
        }
    }
}