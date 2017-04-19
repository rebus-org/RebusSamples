using System;
using System.Timers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Migr8;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers.Reordering;
using Rebus.Pipeline;
using Rebus.UnitOfWork;
using UnitOfWork.Handlers;
// ReSharper disable ArgumentsStyleNamedExpression

namespace UnitOfWork
{
    class Program
    {
        static void Main()
        {
            Database.Migrate("database", Migr8.Migrations.FromThisAssembly());

            using (var container = new WindsorContainer())
            {
                // run Windsor installers - they're in the UnitOfWork.Installers namespace
                container.Install(FromAssembly.This());

                Configure.With(new CastleWindsorContainerAdapter(container))
                    .Transport(t => t.UseMsmq("uow.test"))
                    .Options(o =>
                    {
                        // run the potentially failing handler last to demonstrate
                        // that the uow is not committed, even though the error
                        // happes after the insert
                        o.SpecifyOrderOfHandlers()
                            .First<InsertRowsIntoDatabase>()
                            .Then<FailSometimes>();

                        o.EnableUnitOfWork(Create, commitAction: Commit, cleanupAction: Dispose);
                    })
                    .Start();

                using (var timer = new Timer(1000))
                {
                    timer.Elapsed += (o, ea) => SendStringToSelf(container);
                    timer.Start();

                    Console.WriteLine("Press ENTER to quit");
                    Console.ReadLine();
                }
            }
        }

        static UnitOfWork Create(IMessageContext context)
        {
            var unitOfWork = new UnitOfWork();

            // stash current unit of work in the transaction context's items
            context.TransactionContext.Items["uow"] = unitOfWork;

            return unitOfWork;
        }

        static void Commit(IMessageContext context, UnitOfWork uow) => uow.Commit();

        static void Dispose(IMessageContext context, UnitOfWork uow) => uow.Dispose();

        static void SendStringToSelf(IWindsorContainer container)
        {
            var bus = container.Resolve<IBus>();
            var message = $"The time is {DateTime.Now:T}";
            bus.SendLocal(message).Wait();
        }
    }
}
