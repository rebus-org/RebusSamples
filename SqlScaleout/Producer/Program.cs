using System;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using Rebus;
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
                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(LogLevel.Warn))
                    .MessageOwnership(o => o.FromRebusConfigurationSection())
                    .Transport(t => t.UseSqlServerInOneWayClientMode("server=.;initial catalog=rebus_test;integrated security=true")
                                     .EnsureTableIsCreated())
                    .CreateBus()
                    .Start();

                var keepRunning = true;
                
                while (keepRunning)
                {
                    Console.WriteLine(@"a) Send 10 jobs
b) Send 100 jobs
c) Send 1000 jobs

q) Quit");
                    var key = char.ToLower(Console.ReadKey(true).KeyChar);

                    switch (key)
                    {
                        case 'a':
                            Send(10, adapter.Bus);
                            break;
                        case 'b':
                            Send(100, adapter.Bus);
                            break;
                        case 'c':
                            Send(1000, adapter.Bus);
                            break;
                        case 'q':
                            Console.WriteLine("Quitting");
                            keepRunning = false;
                            break;
                    }
                }
            }
        }

        static void Send(int numberOfJobs, IBus bus)
        {
            Console.WriteLine("Publishing {0} jobs", numberOfJobs);

            var jobs = Enumerable.Range(0, numberOfJobs)
                .Select(i => new Job { JobNumber = i });

            Parallel.ForEach(jobs, bus.Send);
        }
    }
}
