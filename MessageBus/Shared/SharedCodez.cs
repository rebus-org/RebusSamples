using System;
using System.Reflection;
using Rebus.Configuration;
using Rebus.Logging;
using Rebus.Transports.Msmq;

namespace Shared
{
    public class SharedCodez
    {
        readonly string _endpointName = Assembly.GetEntryAssembly().GetName().Name;

        public void Run()
        {
            using (var adapter = new BuiltinContainerAdapter())
            {
                adapter.Handle<string>(str => Console.WriteLine("Got message: {0}", str));

                Console.WriteLine("Starting bus in {0}...", _endpointName);

                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseMsmqAndGetInputQueueNameFromAppConfig())
                    .MessageOwnership(o => o.FromRebusConfigurationSection())
                    .Subscriptions(s => s.StoreInXmlFile(@"C:\temp\subs.xml"))
                    .CreateBus()
                    .Start();

                Console.WriteLine(@"-------------------------------
A) Subscribe to System.String
B) Unsubscribe to System.String
C) Publish System.String
Q) Quit
-------------------------------
");

                var keepRunning = true;

                while (keepRunning)
                {
                    var key = Console.ReadKey(true);

                    switch (char.ToLower(key.KeyChar))
                    {
                        case 'a':
                            Console.WriteLine("Subscribing!");
                            adapter.Bus.Subscribe<string>();
                            break;

                        case 'b':
                            Console.WriteLine("Unsubscribing!");
                            adapter.Bus.Unsubscribe<string>();
                            break;

                        case 'c':
                            Console.WriteLine("Publishing!");
                            adapter.Bus.Publish(string.Format("Greetings to subscribers from {0}", _endpointName));
                            break;

                        case 'q':
                            Console.WriteLine("Quitting!");
                            keepRunning = false;
                            break;
                    }
                }

                Console.WriteLine("Stopping the bus....");
            }

        }
    }
}
