using Rebus.Config;
using Rebus.Transports.Showdown.Core;
using Rebus.Msmq;

namespace Rebus.Transports.Showdown.Msmq
{
    public class Program
    {
        const string Queue = "test.showdown";

        public static void Main()
        {
            using (var runner = new ShowdownRunner())
            {
                PurgeInputQueue(Queue);

                Configure.With(runner.Adapter)
                    .Logging(l => l.None())
                    .Transport(t => t.UseMsmq(Queue))
                    .Options(o => o.SetMaxParallelism(20))
                    .Start();

                runner.Run(typeof(Program).Namespace).Wait();
            }
        }

        static void PurgeInputQueue(string inputQueueName)
        {
            MsmqUtil.PurgeQueue(inputQueueName);
        }
    }
}
