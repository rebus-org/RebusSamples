using System;
using System.IO;
using PubSub.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Persistence.FileSystem;

namespace PubSub.Publisher
{
    class Program
    {
        static readonly string JsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rebus_subscriptions.json");

        static void Main()
        {
            using (var activator = new BuiltinHandlerActivator())
            {
                Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseMsmq("publisher"))
                    .Subscriptions(s => s.UseJsonFile(JsonFilePath))
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
                            activator.Bus.Publish(new StringMessage("Hello there, I'm a publisher!")).Wait();
                            break;

                        case 'b':
                            activator.Bus.Publish(new DateTimeMessage(DateTime.Now)).Wait();
                            break;

                        case 'c':
                            activator.Bus.Publish(new TimeSpanMessage(DateTime.Now - startupTime)).Wait();
                            break;

                        case 'q':
                            goto consideredHarmful;

                        default:
                            Console.WriteLine("There's no option ({0})", keyChar);
                            break;
                    }
                }

            consideredHarmful: ;
                Console.WriteLine("Quitting!");
            }
        }
    }
}
