using System;
using System.Threading.Tasks;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Logging;
using Rebus.Routing.TypeBased;
using Rebus.Transport.Msmq;

namespace PubSub.Subscriber2
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
                         .Transport(t => t.UseMsmq("subscriber2"))
                         .Routing(r => r.TypeBased().MapAssemblyOf<string>("publisher"))
                         .Start();

                activator.Bus.Subscribe<string>().Wait();

                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
            }
        }
    }

    class Handler : IHandleMessages<string>
    {
        public async Task Handle(string message)
        {
            Console.WriteLine("Got string: {0}", message);
        }
    }
}
