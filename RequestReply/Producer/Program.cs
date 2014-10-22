using System;
using Consumer.Messages;
using Rebus.Configuration;
using Rebus.Logging;
using Rebus.Transports.Sql;

namespace Producer
{
    class Program
    {
        static void Main()
        {
            using (var adapter = new BuiltinContainerAdapter())
            {
                adapter.Handle<Reply>(reply => Console.WriteLine("Got reply '{0}' (from OS process {1})", reply.KeyChar, reply.OsProcessId));

                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseSqlServer("server=.; database=rebus; trusted_connection=true", "producer.input", "error").EnsureTableIsCreated())
                    .MessageOwnership(o => o.FromRebusConfigurationSection())
                    .CreateBus()
                    .Start();

                Console.WriteLine("Press Q to quit or any other key to produce a job");
                while (true)
                {
                    var keyChar = char.ToLower(Console.ReadKey(true).KeyChar);

                    switch (keyChar)
                    {
                        case 'q':
                            goto quit;

                        default:
                            adapter.Bus.Send(new Job(keyChar));
                            break;
                    }
                }

            quit:
                Console.WriteLine("Quitting...");
            }
        }
    }
}
