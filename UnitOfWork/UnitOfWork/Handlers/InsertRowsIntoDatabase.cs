using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Rebus.Handlers;

namespace UnitOfWork.Handlers
{
    public class InsertRowsIntoDatabase : IHandleMessages<string>
    {
        public const int Modulo = 10;
        readonly SqlConnection _connection;
        readonly SqlTransaction _currentTransaction;

        public InsertRowsIntoDatabase(SqlConnection connection, SqlTransaction currentTransaction)
        {
            _connection = connection;
            _currentTransaction = currentTransaction;
        }

        public async Task Handle(string message)
        {
            var hashCode = message.GetHashCode();
            var remainder = hashCode % Modulo;

            // just insert the received message, its hash code, and the remainder into the db

            using (var command = _connection.CreateCommand())
            {
                command.Transaction = _currentTransaction;
                command.CommandText = @"INSERT INTO [ReceivedStrings] ([Text], [Hash], [Remainder]) VALUES (@text, @hash, @remainder)";
                command.Parameters.Add("text", SqlDbType.NVarChar).Value = message;
                command.Parameters.Add("hash", SqlDbType.Int).Value = hashCode;
                command.Parameters.Add("remainder", SqlDbType.Int).Value = remainder;
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}