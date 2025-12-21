using Rebus.Config;
using Microsoft.Data.SqlClient;
using Rebus.Transports.Showdown.Core;

namespace Rebus.Transports.Showdown.SqlServer
{
	public class Program
	{
		const string QueueName = "test_showdown";
		const string SqlServerConnectionString = "Server=localhost,11433;Initial Catalog=showdowndb;User Id=sa;Password=Showdown!2025;TrustServerCertificate=True";


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
			using var connection = new SqlConnection(SqlServerConnectionString);
			connection.Open();

			using SqlCommand command = connection.CreateCommand();
			command.CommandText = $"IF OBJECT_ID('[{QueueName}]', 'U') IS NOT NULL DROP TABLE [{QueueName}]";
			command.ExecuteNonQuery();
		}
	}
}
