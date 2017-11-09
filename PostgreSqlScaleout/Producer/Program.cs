using System;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Logging;
using Rebus.PostgreSql.Transport;
using Rebus.Routing.TypeBased;

namespace Producer
{
    class Program
    {
        static void Main()
        {
            using (var adapter = new BuiltinHandlerActivator())
            {
                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(LogLevel.Warn))
                    .Transport(t => t.UsePostgreSqlAsOneWayClient("server=localhost; database=rebus2_test; user id=postgres; password=postgres; maximum pool size=30", "messages"))
                    .Routing(r => r.TypeBased().MapAssemblyOf<Job>("consumer"))
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

            var sendTasks = Enumerable.Range(0, numberOfJobs)
                .Select(i => new Job(i, Guid.NewGuid()))
                .Select(async job => await bus.Send(job))
                .ToArray();

            Task.WaitAll(sendTasks);
        }
    }
}
