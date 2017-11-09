using System.Data;
using System.Data.SqlClient;
using Rebus.Config;
using Rebus.Transports.Showdown.Core;

namespace Rebus.Transports.Showdown.SqlServer
{
    public class Program
    {
        const string QueueName = "test_showdown";
        const string TableName = QueueName;
        const string SqlServerConnectionString = "server=.; initial catalog=rebus2_test; integrated security=sspi";
        const int TableNotFound = 208;

        public static void Main()
        {
            using (var runner = new ShowdownRunner())
            {
                PurgeInputQueue(QueueName);

                Configure.With(runner.Adapter)
                    .Logging(l => l.None())
                    .Transport(t => t.UseSqlServer(SqlServerConnectionString, TableName, QueueName))
                    .Options(o => o.SetMaxParallelism(20))
                    .Start();

                runner.Run(typeof(Program).Namespace).Wait();
            }
        }

        static void PurgeInputQueue(string inputQueueName)
        {
            using (var connection = new SqlConnection(SqlServerConnectionString))
            {
                connection.Open();

                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"DELETE FROM [{TableName}] WHERE [Recipient] = @recipient";
                        command.Parameters.Add("recipient", SqlDbType.NVarChar, 200).Value = inputQueueName;
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
