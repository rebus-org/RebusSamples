using Migr8;

namespace UnitOfWork.Migrations
{
    [Migration(1, "Add table for received strings and their hash codes")]
    public class Migration001 : ISqlMigration
    {
        public string Sql => @"

CREATE TABLE [ReceivedStrings] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [Text] NVARCHAR(MAX) NOT NULL,
    [Hash] INT NOT NULL,
    [Remainder] INT NOT NULL,
    CONSTRAINT [PK_ReceivedStrings] PRIMARY KEY CLUSTERED ([Id])
)

";
    }
}