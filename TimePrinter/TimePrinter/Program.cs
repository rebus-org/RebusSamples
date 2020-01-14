using System;
using System.Threading.Tasks;
using System.Timers;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
#pragma warning disable 1998

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
                    .Routing(r => r.TypeBased().Map<CurrentTimeMessage>(InputQueueName))
                    .Start();

                timer.Elapsed += delegate { bus.Send(new CurrentTimeMessage(DateTimeOffset.Now)).Wait(); };
                timer.Interval = 1000;
                timer.Start();

                Console.WriteLine("Press enter to quit");
                Console.ReadLine();
            }
        }
    }

    class PrintDateTime : IHandleMessages<CurrentTimeMessage>
    {
        public async Task Handle(CurrentTimeMessage message)
        {
            Console.WriteLine("The time is {0}", message.Time);
        }
    }
}
