using System;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Routing.TypeBased;
using Rebus.SqlServer.Transport;

namespace Producer
{
    class Program
    {
        static void Main()
        {
            using var bus = Configure.OneWayClient()
                .Logging(l => l.ColoredConsole(LogLevel.Warn))
                .Transport(t => t.UseSqlServerAsOneWayClient("server=.; initial catalog=rebus; integrated security=true", "Messages"))
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
                        Send(10, bus);
                        break;
                    case 'b':
                        Send(100, bus);
                        break;
                    case 'c':
                        Send(1000, bus);
                        break;
                    case 'q':
                        Console.WriteLine("Quitting");
                        keepRunning = false;
                        break;
                }
            }
        }

        static void Send(int numberOfJobs, IBus bus)
        {
            Console.WriteLine("Publishing {0} jobs", numberOfJobs);

            var sendTasks = Enumerable.Range(0, numberOfJobs)
                .Select(i => new Job(i))
                .Select(job => bus.Send(job))
                .ToArray();

            Task.WaitAll(sendTasks);
        }
    }
}
