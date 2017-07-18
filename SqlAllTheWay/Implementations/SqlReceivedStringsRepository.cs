using System;
using System.Data;
using System.Threading.Tasks;
using SqlAllTheWay.Repositories;
using IDbConnection = Rebus.SqlServer.IDbConnection;

namespace SqlAllTheWay.Implementations
{
    public class SqlReceivedStringsRepository : IReceivedStringsRepository
    {
        readonly IDbConnection _dbConnection;

        public SqlReceivedStringsRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// This method is NOT idempotent, because the value for [Text] is the PK for the table.
        /// This means that we ALWAYS assume that exactly one single INSERT will happen, no matter how many times the same message is retried.
        /// </summary>
        public async Task Insert(string text)
        {
            var connection = _dbConnection;

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO [ReceivedStrings] ([Text]) VALUES (@text);";
                command.Parameters.Add("text", SqlDbType.NVarChar, 100).Value = text;
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}