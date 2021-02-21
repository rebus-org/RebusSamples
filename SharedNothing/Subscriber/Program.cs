using System;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Serialization;
using Rebus.Serialization.Json;
#pragma warning disable 1998

namespace Subscriber
{
    class Program
    {
        const string ConnectionString = "server=.; database=rebussamples; trusted_connection=true";
        const string SubscriptionsTableName = "Subscriptions";

        static void Main()
        {
            using (var activator = new BuiltinHandlerActivator())
            {
                activator.Handle<GreetingWasEntered_Subscriber>(async message =>
                {
                    Console.WriteLine($"Got this greeting: {message.Text}");
                });

                var bus = Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Info))
                    .Transport(t => t.UseMsmq("sharednothing-subscriber"))

                    // subscriptions are stored in a central SQL Server
                    .Subscriptions(s => s.StoreInSqlServer(ConnectionString, SubscriptionsTableName, isCentralized: true))
                    
                    // configure serializer to serialize as pure JSONM (i.e. WITHOUT type information inside the serialized format)
                    .Serialization(s => s.UseNewtonsoftJson(JsonInteroperabilityMode.PureJson))

                    // we extend the default JSON serialization with some extra deserialization help
                    .Options(o => o.Decorate<ISerializer>(c => new CustomMessageDeserializer(c.Get<ISerializer>())))

                    .Start();

                // the publisher publishes to this topic name - this is how this subscriber gets bound to it
                const string topic = "GreetingWasEntered";

                bus.Advanced.Topics.Subscribe(topic).Wait();

                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
            }
        }
    }

    public class GreetingWasEntered_Subscriber
    {
        public string Text { get; }

        public GreetingWasEntered_Subscriber(string text)
        {
            Text = text;
        }
    }
}