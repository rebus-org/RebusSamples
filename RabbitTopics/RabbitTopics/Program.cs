using System;
using System.Threading.Tasks;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;

#pragma warning disable 1998

namespace RabbitTopics
{
    class Program
    {
        const LogLevel MinimumLogLevel = LogLevel.Warn;
        const string ConnectionString = "amqp://localhost";

        static async Task Main()
        {
            using var subscriber1 = new BuiltinHandlerActivator();
            using var subscriber2 = new BuiltinHandlerActivator();
            using var subscriber3 = new BuiltinHandlerActivator();

            ConfigureSubscriber(subscriber1, "endpoint1");
            ConfigureSubscriber(subscriber2, "endpoint2");
            ConfigureSubscriber(subscriber3, "endpoint3");

            await subscriber1.Bus.Advanced.Topics.Subscribe("mercedes.#");
            await subscriber2.Bus.Advanced.Topics.Subscribe("mercedes.bmw.#");
            await subscriber3.Bus.Advanced.Topics.Subscribe("mercedes.bmw.vw");

            using var publisherBus = Configure.OneWayClient()
                .Logging(l => l.ColoredConsole(MinimumLogLevel))
                .Transport(t => t.UseRabbitMqAsOneWayClient(ConnectionString))
                .Start();

            var topicsApi = publisherBus.Advanced.Topics;

            await topicsApi.Publish("mercedes.bmw.vw", "This one should be received by all!");
            await topicsApi.Publish("mercedes.bmw.mazda", "This one should be received by 1 & 2");
            await topicsApi.Publish("mercedes.honda", "This one should be received by 1");

            Console.WriteLine("Press ENTER to quit");
            Console.ReadLine();
        }

        static void ConfigureSubscriber(BuiltinHandlerActivator activator, string inputQueueName)
        {
            activator.Handle<string>(async str =>
            {
                Console.WriteLine("Message '{0}' was received by '{1}'", str, inputQueueName);
            });

            Configure.With(activator)
                .Logging(l => l.ColoredConsole(MinimumLogLevel))
                .Transport(t => t.UseRabbitMq(ConnectionString, inputQueueName))
                .Start();
        }
    }
}
