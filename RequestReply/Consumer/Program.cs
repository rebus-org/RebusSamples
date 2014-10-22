using System;
using System.Diagnostics;
using Consumer.Messages;
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
                    adapter.Bus.Reply(new Reply(job.KeyChar, Process.GetCurrentProcess().Id));
                });

                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseSqlServer("server=.; database=rebus; trusted_connection=true", "consumer.input", "error").EnsureTableIsCreated())
                    .CreateBus()
                    .Start();

                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
            }
        }
    }
}
