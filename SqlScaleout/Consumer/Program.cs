using System;
using System.Threading;
using Messages;
using Rebus.Configuration;
using Rebus.Logging;
using Rebus.Transports.Sql;

namespace Consumer
{
    class Program
    {
        static void Main()
        {
            using (var adapter = new BuiltinContainerAdapter())
            {
                adapter.Handle<Job>(job =>
                {
                    Console.WriteLine("Processing job {0}", job.JobNumber);
                    Thread.Sleep(200);
                });

                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(LogLevel.Warn))
                    .Transport(t => t.UseSqlServer("server=.;initial catalog=rebus_test;integrated security=true", "consumer", "error")
                                     .EnsureTableIsCreated())
                    .CreateBus()
                    .Start(20);

                Console.WriteLine("Consumer listening - press ENTER to quit");
                Console.ReadLine();
            }
        }
    }
}
