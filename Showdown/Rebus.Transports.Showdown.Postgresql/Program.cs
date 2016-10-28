using System;
using System.Reflection;
using Rebus.Config;
using Rebus.PostgreSql.Transport;
using Rebus.Transports.Showdown.Core;

namespace Rebus.Transports.Showdown.PostgreSql
{
    public class Program
    {

        const string QueueName = "test_showdown";
        private const string TableName = QueueName;
        const string PostgresqlConnectionString = "server=localhost;port=5433;database=rebus2_test;user id=test; password=test; Maximum Pool Size=30";


        public static void Main()
        {
            using (var runner = new ShowdownRunner())
            {
                PurgeInputQueue(QueueName);

                Configure.With(runner.Adapter)
                    .Logging(l => l.None())
                    .Transport(t => t.UsePostgreSql(PostgresqlConnectionString, TableName, QueueName))
                    .Start();

            


                runner.Run(typeof(Program).Namespace).Wait();
            }
        }

        static void PurgeInputQueue(string queueName)
        {

        }
    }
}
