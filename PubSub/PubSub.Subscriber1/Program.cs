using System;
using System.Threading.Tasks;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Logging;
using Rebus.Routing.TypeBased;
using Rebus.Transport.Msmq;

namespace PubSub.Subscriber1
{
    class Program
    {
        static void Main()
        {
            using (var activator = new BuiltinHandlerActivator())
            {
                activator.Register(() => new Handler());

                Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseMsmq("subscriber1"))
                    .Routing(r => r.TypeBased().MapAssemblyOf<string>("publisher"))
                    .Start();

                activator.Bus.Subscribe<string>().Wait();
                activator.Bus.Subscribe<DateTime>().Wait();
                activator.Bus.Subscribe<TimeSpan>().Wait();

                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
            }
        }
    }

    class Handler : IHandleMessages<string>, IHandleMessages<DateTime>, IHandleMessages<TimeSpan>
    {
        public async Task Handle(string message)
        {
            Console.WriteLine("Got string: {0}", message);
        }

        public async Task Handle(DateTime message)
        {
            Console.WriteLine("Got DateTime: {0}", message);
        }

        public async Task Handle(TimeSpan message)
        {
            Console.WriteLine("Got TimeSpan: {0}", message);
        }
    }
}
