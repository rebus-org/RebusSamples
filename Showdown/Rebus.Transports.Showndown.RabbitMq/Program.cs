using Rebus.Config;
using Rebus.Logging;
using Rebus.Transports.Showdown.Core;

namespace Rebus.Transports.Showdown.RabbitMq
{
    public class Program
    {
        const string Queue = "test.showdown";
        const string RabbitMqConnectionString = "amqp://localhost";

        public static void Main()
        {
            using (var runner = new ShowdownRunner())
            {
                Configure.With(runner.Adapter)
                    .Logging(l => l.ColoredConsole(LogLevel.Warn))
                    .Transport(t => t.UseRabbitMq(RabbitMqConnectionString, Queue))
                    .Start();

                runner.Run(typeof(Program).Namespace).Wait();
            }
        }
    }
}
