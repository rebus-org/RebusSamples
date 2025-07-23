using Consumer.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;
using System;
using System.Diagnostics;

namespace Consumer;

internal class Program
{
    private const string ConnectionString = "server=localhost; database=rebus2_test; user id=postgres; password=postgres; maximum pool size=30";
    private const string TableName = "producer.input";

    private static void Main()
    {
        using var adapter = new BuiltinHandlerActivator();

        adapter.Handle<Job>(async (bus, job) =>
        {
            var keyChar = job.KeyChar;
            var processId = Process.GetCurrentProcess().Id;
            var reply = new Reply(keyChar, processId);

            await bus.Reply(reply);
        });

        Configure.With(adapter)
            .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
            .Transport(t => t.UsePostgreSql(ConnectionString, TableName, "consumer.input"))
            .Start();

        Console.WriteLine("Press ENTER to quit");
        Console.ReadLine();
    }
}
