using System;
using NLog;
using NLog.Config;
using NLog.Targets;
using Rebus.Activation;
using Rebus.Config;
using Rebus.NLog;
using Rebus.Transport.InMem;
using Serilog;

namespace LoggingApp
{
    class Program
    {
        static void Main()
        {
            using (var activator = new BuiltinHandlerActivator())
            {
                ConfigureUsing(activator);

                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
            }

        }

        static void ConfigureUsing(IHandlerActivator activator)
        {
            //ConfigureSerilog(activator);
            ConfigureNLog(activator);
        }

        static void ConfigureSerilog(IHandlerActivator activator)
        {
            // configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Verbose()
                .CreateLogger();

            // configure Rebus
            Configure.With(activator)
                .Logging(l => l.Serilog())
                .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "logging"))
                .Start();
        }

        static void ConfigureNLog(IHandlerActivator activator)
        {
            // configure NLog
            var configuration = new LoggingConfiguration
            {
                LoggingRules = { new LoggingRule("*", LogLevel.Debug, new ConsoleTarget("console")) }
            };

            LogManager.Configuration = configuration;

            // configure Rebus
            Configure.With(activator)
                .Logging(l => l.NLog())
                .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "logging"))
                .Start();
        }
    }
}
