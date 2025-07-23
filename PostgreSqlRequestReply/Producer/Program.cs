using Consumer.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Routing.TypeBased;
using System;

namespace Producer;

internal class Program
{
    private const string ConnectionString = "server=localhost; database=rebus2_test; user id=postgres; password=postgres; maximum pool size=30";
    private const string TableName = "producer.input";

    private static void Main()
    {
        using var adapter = new BuiltinHandlerActivator();

        adapter.Handle<Reply>(async reply =>
        {
            await Console.Out.WriteLineAsync($"Got reply '{reply.KeyChar}' (from OS process {reply.OsProcessId})");
        });

        Configure.With(adapter)
            .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
            .Transport(t => t.UsePostgreSql(ConnectionString, TableName, "producer.input"))
            .Routing(r => r.TypeBased().MapAssemblyOf<Job>("consumer.input"))
            .Start();

        Console.WriteLine("Press Q to quit or any other key to produce a job");

        while (true)
        {
            var keyChar = char.ToLower(Console.ReadKey(true).KeyChar);

            switch (keyChar)
            {
                case 'q':
                    goto quit;

                default:
                    adapter.Bus.Send(new Job(keyChar)).Wait();
                    break;
            }
        }

    quit:
        Console.WriteLine("Quitting...");
    }
}
