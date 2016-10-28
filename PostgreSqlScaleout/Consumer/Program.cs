using System;
using System.Threading.Tasks;
using Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;
using Rebus.PostgreSql.Transport;

namespace Consumer
{
    class Program
    {
        static void Main()
        {
            using (var adapter = new BuiltinHandlerActivator())
            {
                adapter.Handle<Job>(async job =>
                {
                    Console.WriteLine("Processing job {0}", job.JobNumber);

                    await Task.Delay(TimeSpan.FromMilliseconds(300));
                });

                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(LogLevel.Warn))
                    .Transport(t => t.UsePostgreSql("server=localhost;port=5433;database=rebus2_test; user id=test; password=test; maximum pool size=30", "messages", "consumer"))
                    .Options(o =>
                    {
                        o.SetNumberOfWorkers(10);
                        o.SetMaxParallelism(20);
                    })
                    .Start();

                Console.WriteLine("Consumer listening - press ENTER to quit");
                Console.ReadLine();
            }
        }
    }
}
