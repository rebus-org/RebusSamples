using System;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Serialization.Json;

namespace Publisher
{
    class Program
    {
        const string ConnectionString = "server=.; database=SharedNothing; trusted_connection=true";
        const string SubscriptionsTableName = "Subscriptions";

        static void Main()
        {
            using (var activator = new BuiltinHandlerActivator())
            {
                var bus = Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Info))
                    .Transport(t => t.UseMsmq("sharednothing-publisher"))
                    
                    // subscriptions are stored in a central SQL Server
                    .Subscriptions(s => s.StoreInSqlServer(ConnectionString, SubscriptionsTableName, isCentralized: true))

                    // configure serializer to serialize as pure JSONM (i.e. WITHOUT type information inside the serialized format)
                    .Serialization(s => s.UseNewtonsoftJson(JsonInteroperabilityMode.PureJson))
                    
                    .Start();

                while (true)
                {
                    Console.Write("Type greeting > ");
                    var text = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(text)) break;

                    // subscribers subscribe to this topic name - this is how they get bound to this publisher's events
                    const string topic = "GreetingWasEntered";

                    bus.Advanced.Topics.Publish(topic, new GreetingWasEntered_Publisher(text)).Wait();
                }
            }
        }
    }

    public class GreetingWasEntered_Publisher
    {
        public string Text { get; }

        public GreetingWasEntered_Publisher(string text)
        {
            Text = text;
        }
    }
}

