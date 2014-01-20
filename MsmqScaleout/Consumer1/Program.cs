using System;
using System.Threading;
using Messages;
using Rebus.Configuration;
using Rebus.Logging;
using Rebus.Transports.Msmq;

namespace Consumer1
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
                    .Transport(t => t.UseMsmqAndGetInputQueueNameFromAppConfig())
                    .CreateBus()
                    .Start();

                Console.WriteLine("Consumer 1 listening - press ENTER to quit");
                Console.ReadLine();
            }
        }
    }
}
