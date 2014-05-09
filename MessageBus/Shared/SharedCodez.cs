using System;
using System.IO;
using System.Reflection;
using Rebus.Configuration;
using Rebus.Logging;
using Rebus.Transports.Msmq;

namespace Shared
{
    public class SharedCodez
    {

        public void Run()
        {
            var endpointName = Assembly.GetEntryAssembly().GetName().Name;
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RebusSamples", "MessageBus");
            var subscriptionsFilePath = Path.Combine(directory, "subscriptions.xml");

            using (var adapter = new BuiltinContainerAdapter())
            {
                adapter.Handle<string>(str => Console.WriteLine("Got message: {0}", str));

                Console.WriteLine("Starting {0} bus - using subscription storage in {1}...", endpointName, subscriptionsFilePath);
                
                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseMsmqAndGetInputQueueNameFromAppConfig())
                    .MessageOwnership(o => o.FromRebusConfigurationSection())
                    .Subscriptions(s => s.StoreInXmlFile(subscriptionsFilePath))
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
                            adapter.Bus.Publish(string.Format("Greetings to subscribers from {0}", endpointName));
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
