using System;
using System.Timers;
using Migr8;
using Rebus.Activation;
using Rebus.Bus.Advanced;
using Rebus.Config;
using SqlAllTheWay.Config;
using SqlAllTheWay.Handlers;
using SqlAllTheWay.Implementations;

namespace SqlAllTheWay
{
    class Program
    {
        static void Main()
        {
            Database.Migrate(DbConfig.ConnectionString, Migr8.Migrations.FromThisAssembly());

            using (var activator = new BuiltinHandlerActivator())
            {
                activator.Register(() =>
                {
                    var receivedStringsRepository = new SqlReceivedStringsRepository(DbConfig.GetDbConnection().Result);
                    return new StringMessageHandler(receivedStringsRepository);
                });

                var bus = Configure.With(activator)
                    .Transport(t => t.UseSqlServer(DbConfig.GetDbConnection, "Messages", "all-the-way-baby"))
                    .Options(o =>
                    {
                        // start out with zero workers, therefore not receiving anything before adding a worker later on
                        o.SetNumberOfWorkers(0);
                        o.SetMaxParallelism(1);
                    })
                    .Start();

                var syncBus = bus.Advanced.SyncBus;

                // send some messages as part of our own transaction
                SendSomeMessages(syncBus);

                // add a worker and start processing string messages
                bus.Advanced.Workers.SetNumberOfWorkers(1);

                using (var timer = new Timer(4000))
                {
                    timer.Elapsed += (o, ea) => SendMessage(syncBus);
                    timer.Start();

                    Console.WriteLine("Press ENTER to quit");
                    Console.ReadLine();
                }
            }
        }

        static void SendSomeMessages(ISyncBus bus)
        {
            var now = DateTime.Now;

            bus.SendLocal($"Outside context msg 1, the time is {now}");

            using (var context = new CustomDbContext())
            {

                // send some messages
                bus.SendLocal($"Custom db context msg 1, the time is {now}");
                bus.SendLocal($"Custom db context msg 2, the time is {now}");
                bus.SendLocal($"Custom db context msg 3, the time is {now}");
                bus.SendLocal($"Custom db context msg 4, the time is {now}");
                bus.SendLocal($"Custom db context msg 5, the time is {now}");

                context.Commit();
            }
        }

        static void SendMessage(ISyncBus bus)
        {
            bus.SendLocal($"Hello there, the time is {DateTime.Now}");
        }
    }
}
