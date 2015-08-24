using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IntegrationSample.Client.Handlers;
using IntegrationSample.IntegrationService.Messages;
using Rebus.CastleWindsor;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
using Rebus.Transport.Msmq;

namespace IntegrationSample.Client
{
    class Program
    {
        static void Main()
        {
            var container = new WindsorContainer()
                .Register(Component.For<IHandleMessages<GetGreetingReply>>()
                              .ImplementedBy<GetGreetingReplyHandler>()
                              .LifestyleTransient());

            var bus = Configure.With(new CastleWindsorContainerAdapter(container))
                .Logging(l => l.None()) // disable logging to avoid polluting the console
                .Transport(t => t.UseMsmq("IntegrationSample.Client.input"))
                .Routing(d => d.TypeBased().MapAssemblyOf<GetGreetingRequest>("IntegrationSample.IntegrationService.input"))
                .Start();

            Console.WriteLine("Press R to request a greeting and Q to quit...");

            var keepRunning = true;
            do
            {
                var key = Console.ReadKey(true);

                switch (char.ToLower(key.KeyChar))
                {
                    case 'r':
                        bus.Send(new GetGreetingRequest()).Wait();
                        break;

                    case 'q':
                        keepRunning = false;
                        break;
                }
            } while (keepRunning);

            container.Dispose();
        }
    }
}
