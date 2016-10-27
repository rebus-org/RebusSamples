using System.Threading.Tasks;
using Rebus.Config;
using Rebus.Transports.Showdown.Core;
using Rebus.Logging;
using Rebus.Msmq;
using Rebus.Routing.TypeBased;

namespace Rebus.Transports.Showndown.Msmq
{
    public class Program
    {

        private const string Queue = "test.showdown";

        public static void Main()
        {
            using (var runner = new ShowdownRunner(Queue))
            {
                PurgeInputQueue(Queue);

                Configure.With(runner.Adapter)
                         .Logging(l => l.Console())
                         .Transport(t => t.UseMsmq(Queue))
                         .Start();


                runner.Run().Wait();

            }
        }

        static void PurgeInputQueue(string inputQueueName)
        {
            MsmqUtil.PurgeQueue(inputQueueName);
        }
    }
}
