using Rebus.Config;
using Rebus.Logging;
using Rebus.RabbitMq;
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
                PurgeInputQueue(Queue);

                Configure.With(runner.Adapter)
                    .Logging(l => l.ColoredConsole(LogLevel.Warn))
                    .Transport(t => t.UseRabbitMq(RabbitMqConnectionString, Queue))
                    .Options(o => o.SetMaxParallelism(20))
                    .Start();

                runner.Run(typeof(Program).Namespace).Wait();
            }
        }

        static void PurgeInputQueue(string inputQueueName)
        {
            using (var transport = new RabbitMqTransport(RabbitMqConnectionString, inputQueueName, new NullLoggerFactory()))
            {
                transport.PurgeInputQueue();
            }
        }
    }
}
