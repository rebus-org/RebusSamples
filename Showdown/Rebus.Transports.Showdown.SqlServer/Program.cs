using System.Data.SqlClient;
using Rebus.Config;
using Rebus.Transports.Showdown.Core;

namespace Rebus.Transports.Showdown.SqlServer
{
    public class Program
    {
        const string QueueName = "test_showdown";
        const string SqlServerConnectionString = "server=.; initial catalog=rebus2_test; integrated security=sspi";
        const int TableNotFound = 208;

        public static void Main()
        {
            using (var runner = new ShowdownRunner())
            {
                PurgeInputQueue();

                Configure.With(runner.Adapter)
                    .Logging(l => l.None())
                    .Transport(t => t.UseSqlServer(SqlServerConnectionString, QueueName))
                    .Options(o => o.SetMaxParallelism(20))
                    .Start();

                runner.Run(typeof(Program).Namespace).Wait();
            }
        }

        static void PurgeInputQueue()
        {
            using (var connection = new SqlConnection(SqlServerConnectionString))
            {
                connection.Open();

                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"DROP TABLE [{QueueName}]";
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException sqlException) when (sqlException.Number == TableNotFound)
                {
                }
            }
        }
    }
}
