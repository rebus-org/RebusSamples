using System;
using PubSub.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;

namespace PubSub.Publisher
{
    class Program
    {
        static void Main()
        {
            using var bus = Configure.OneWayClient()
                .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                .Transport(t => t.UseMsmq("publisher"))
                .Subscriptions(s => s.StoreInSqlServer("server=.; database=RebusPubSubCentralized; trusted_connection=true", "Subscriptions", isCentralized: true))
                .Start();

            var startupTime = DateTime.Now;

            while (true)
            {
                Console.WriteLine(@"a) Publish string
b) Publish DateTime
c) Publish TimeSpan
q) Quit");

                var keyChar = char.ToLower(Console.ReadKey(true).KeyChar);
                switch (keyChar)
                {
                    case 'a':
                        bus.Publish(new StringMessage("Hello there, this is a string message from a publisher!"));
                        break;

                    case 'b':
                        bus.Publish(new DateTimeMessage(DateTime.Now));
                        break;

                    case 'c':
                        bus.Publish(new TimeSpanMessage(DateTime.Now - startupTime));
                        break;

                    case 'q':
                        goto consideredHarmful;

                    default:
                        Console.WriteLine($"There's no option '{keyChar}'");
                        break;
                }
            }

        consideredHarmful:;
            Console.WriteLine("Quitting!");
        }
    }
}
