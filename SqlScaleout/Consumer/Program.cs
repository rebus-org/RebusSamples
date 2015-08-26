using System;
using System.Threading.Tasks;
using Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Transport.SqlServer;

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
                    .Transport(t => t.UseSqlServer("server=.; initial catalog=rebus; integrated security=true", "Messages", "consumer"))
                    .Options(o => o.SetNumberOfWorkers(1).SetMaxParallelism(20))
                    .Start();

                Console.WriteLine("Consumer listening - press ENTER to quit");
                Console.ReadLine();
            }
        }
    }
}
