using System;
using System.Threading.Tasks;
using System.Timers;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
using Rebus.Transport.Msmq;

namespace TimePrinter
{
    class Program
    {
        const string InputQueueName = "my-app.input";

        static void Main()
        {
            using (var activator = new BuiltinHandlerActivator())
            using (var timer = new Timer())
            {
                activator.Register(() => new PrintDateTime());

                var bus = Configure.With(activator)
                                   .Logging(l => l.None())
                                   .Transport(t => t.UseMsmq(InputQueueName))
                                   .Routing(r => r.TypeBased().Map<DateTime>(InputQueueName))
                                   .Start();

                timer.Elapsed += delegate { bus.Send(DateTime.Now).Wait(); };
                timer.Interval = 1000;
                timer.Start();

                Console.WriteLine("Press enter to quit");
                Console.ReadLine();
            }
        }
    }

    class PrintDateTime : IHandleMessages<DateTime>
    {
        public async Task Handle(DateTime currentDateTime)
        {
            Console.WriteLine("The time is {0}", currentDateTime);
        }
    }
}
