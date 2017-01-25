using System;
using System.Threading.Tasks;
using Common;
using Rebus.Activation;
using Rebus.Config;
using SagaDemo.Messages;
using Serilog;

namespace Logger
{
    class Program
    {
        static void Main()
        {
            // configure Serilog to log with colors in a fairly compact format
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole(outputTemplate: "{Timestamp:HH:mm:ss} {Message}{NewLine}{Exception}")
                .CreateLogger();

            using (var activator = new BuiltinHandlerActivator())
            {
                activator.Register(() => new EventLogger());

                var bus = Configure.With(activator)
                    .ConfigureEndpoint(EndpointRole.Subscriber)
                    .Start();

                Task.WaitAll(
                    bus.Subscribe<AmountsCalculated>(),
                    bus.Subscribe<TaxesCalculated>(),
                    bus.Subscribe<PayoutMethodSelected>(),
                    bus.Subscribe<PayoutReady>(),
                    bus.Subscribe<PayoutNotReady>()
                );

                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
            }
        }
    }
}
