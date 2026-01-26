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
		const string PostgresqlConnectionString = "server=localhost;port=55432;database=showdowndb;user id=showdown;password=showdownpw;";

		public static void Main()
		{
			using var runner = new ShowdownRunner();
			PurgeInputQueue(QueueName);

			Configure.With(runner.Adapter)
				.Logging(l => l.None())
				.Transport(t => t.UsePostgreSql(PostgresqlConnectionString, TableName, QueueName))
				.Options(o => o.SetMaxParallelism(20))
				.Start();

			runner.Run(typeof(Program).Namespace).Wait();
		}

		static void PurgeInputQueue(string queueName)
		{
			using var connection = new NpgsqlConnection(PostgresqlConnectionString);
			connection.Open();
			using var command = connection.CreateCommand();
			command.CommandText = $"DROP TABLE IF EXISTS \"{TableName}\"";
			command.ExecuteNonQuery();

		}
	}
}
