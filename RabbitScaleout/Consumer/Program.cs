using System;
using System.Threading;
using Messages;
using Rebus.Configuration;
using Rebus.Logging;
using Rebus.RabbitMQ;

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
                    .Transport(t => t.UseRabbitMq("amqp://localhost", "consumer", "error")
                        .ManageSubscriptions()
                        .SetPrefetchCount(65535))
                    .CreateBus()
                    .Start(20);

                adapter.Bus.Subscribe<Job>();

                Console.WriteLine("Consumer listening - press ENTER to quit");
                Console.ReadLine();
            }
        }
    }
}
