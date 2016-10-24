using System;
using System.Reflection;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;

#pragma warning disable 1998

namespace Shared
{
    public class SharedCodez
    {

        public void Run(string inputQueueName)
        {
            var endpointName = string.Format("{0} ({1})", Assembly.GetEntryAssembly().GetName().Name, inputQueueName);

            using (var adapter = new BuiltinHandlerActivator())
            {
                adapter.Handle<string>(async str =>
                {
                    Console.WriteLine("Got message: {0}", str);
                });

                Console.WriteLine("Starting {0} bus", endpointName);

                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseMsmq(inputQueueName))
                    .Subscriptions(s => s.StoreInSqlServer("server=.; database=messagebus; trusted_connection=true", "subscriptions", isCentralized: true))
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
                            adapter.Bus.Subscribe<string>().Wait();
                            break;

                        case 'b':
                            Console.WriteLine("Unsubscribing!");
                            adapter.Bus.Unsubscribe<string>().Wait();
                            break;

                        case 'c':
                            Console.WriteLine("Publishing!");
                            adapter.Bus.Publish(string.Format("Greetings to subscribers from {0}", endpointName)).Wait();
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
