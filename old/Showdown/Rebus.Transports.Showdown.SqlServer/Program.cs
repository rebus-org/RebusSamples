using Rebus.Config;
using Rebus.SqlServer.Transport;
using Rebus.Transports.Showdown.Core;

namespace Rebus.Transports.Showdown.SqlServer
{
    public class Program
    {
        const string QueueName = "test_showdown";
        private const string TableName = QueueName;
        const string SqlServerConnectionString = "server=.;initial catalog=rebus_test;integrated security=sspi";


        public static void Main()
        {
            using (var runner = new ShowdownRunner(QueueName))
            {
                PurgeInputQueue(QueueName);

                Configure.With(runner.Adapter)
                         .Logging(l => l.ColoredConsole())
                         .Transport(t => t.UseSqlServer(SqlServerConnectionString, TableName,QueueName))
                         .Start();

                runner.Run().Wait();
            }
        }

        static void PurgeInputQueue(string inputQueueName)
        {
            //purge this later on...
        }
    }
}
