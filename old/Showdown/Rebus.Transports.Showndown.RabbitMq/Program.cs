using Rebus.Config;
using Rebus.Logging;
using Rebus.Transports.Showdown.Core;
using Rebus.Routing.TypeBased;

namespace Rebus.Transports.Showndown.RabbitMq
{
    public class Program
    {
        const string SenderInputQueue = "test.showdown.sender";
        const string ReceiverInputQueue = "test.showdown.receiver";
        const string RabbitMqConnectionString = "amqp://localhost";

        public static void Main()
        {
            using (var runner = new ShowdownRunner(ReceiverInputQueue))
            {
                Configure.With(runner.SenderAdapter)
                    .Logging(l => l.ColoredConsole(LogLevel.Warn))
                    .Transport(t => t.UseRabbitMq(RabbitMqConnectionString, SenderInputQueue))
                    .Routing(c => c.TypeBased())
                    .Start();

                Configure.With(runner.ReceiverAdapter)
                    .Logging(l => l.ColoredConsole(LogLevel.Warn))
                    .Transport(t => t.UseRabbitMq(RabbitMqConnectionString, ReceiverInputQueue))
                    .Routing(c => c.TypeBased())
                    .Start();

                runner.Run().Wait();
            }
        }
    }
}
