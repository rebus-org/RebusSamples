using System;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;
using Rebus.RabbitMq;
#pragma warning disable 1998

namespace RabbitTopics
{
    class Program
    {
        const LogLevel MinimumLogLevel = LogLevel.Warn;
        const string ConnectionString = "amqp://localhost";

        static void Main()
        {
            using (var publisher = new BuiltinHandlerActivator())
            using (var subscriber1 = new BuiltinHandlerActivator())
            using (var subscriber2 = new BuiltinHandlerActivator())
            using (var subscriber3 = new BuiltinHandlerActivator())
            {
                ConfigureSubscriber(subscriber1, "endpoint1");
                ConfigureSubscriber(subscriber2, "endpoint2");
                ConfigureSubscriber(subscriber3, "endpoint3");

                subscriber1.Bus.Advanced.Topics.Subscribe("mercedes.#").Wait();
                subscriber2.Bus.Advanced.Topics.Subscribe("mercedes.bmw.#").Wait();
                subscriber3.Bus.Advanced.Topics.Subscribe("mercedes.bmw.vw").Wait();

                var publisherBus = Configure.With(publisher)
                    .Logging(l => l.ColoredConsole(MinimumLogLevel))
                    .Transport(t => t.UseRabbitMqAsOneWayClient(ConnectionString))
                    .Start();

                var topicsApi = publisherBus.Advanced.Topics;

                topicsApi.Publish("mercedes.bmw.vw", "This one should be received by all!").Wait();
                topicsApi.Publish("mercedes.bmw.mazda", "This one should be received by 1 & 2").Wait();
                topicsApi.Publish("mercedes.honda", "This one should be received by 1").Wait();

                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
            }
        }

        static void ConfigureSubscriber(BuiltinHandlerActivator activator, string inputQueueName)
        {
            activator.Handle<string>(async str =>
            {
                Console.WriteLine("{0} => '{1}'", str, inputQueueName);
            });

            Configure.With(activator)
                .Logging(l => l.ColoredConsole(MinimumLogLevel))
                .Transport(t => t.UseRabbitMq(ConnectionString, inputQueueName))
                .Start();
        }
    }
}
