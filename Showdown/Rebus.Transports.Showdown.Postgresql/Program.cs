using Npgsql;
using NpgsqlTypes;
using Rebus.Config;
using Rebus.PostgreSql.Transport;
using Rebus.Transports.Showdown.Core;

namespace Rebus.Transports.Showdown.PostgreSql
{
    public class Program
    {
        const string QueueName = "test_showdown";
        const string TableName = QueueName;
        const string PostgresqlConnectionString = "server=localhost;database=rebus2_test;user id=postgres; password=postgres; Maximum Pool Size=30";
        const string TableNotFound = "42P01";

        public static void Main()
        {
            using (var runner = new ShowdownRunner())
            {
                PurgeInputQueue(QueueName);

                Configure.With(runner.Adapter)
                    .Logging(l => l.None())
                    .Transport(t => t.UsePostgreSql(PostgresqlConnectionString, TableName, QueueName))
                    .Options(o => o.SetMaxParallelism(20))
                    .Start();

                runner.Run(typeof(Program).Namespace).Wait();
            }
        }

        static void PurgeInputQueue(string queueName)
        {
            using (var connection = new NpgsqlConnection(PostgresqlConnectionString))
            {
                connection.Open();
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $@"DELETE FROM ""{TableName}"" WHERE ""recipient"" = @recipient;";
                        command.Parameters.Add("recipient", NpgsqlDbType.Text, 200).Value = queueName;
                        command.ExecuteNonQuery();
                    }
                }
                catch (PostgresException postgresException) when (postgresException.SqlState == TableNotFound)
                {
                }
            }
        }
    }
}
