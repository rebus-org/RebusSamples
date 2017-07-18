using Migr8;

namespace SqlAllTheWay.Migrations
{
    [Migration(1, "Creates the necessary schema")]
    public class CreateSchema : ISqlMigration
    {
        public string Sql => @"

CREATE TABLE [dbo].[ReceivedStrings] (
	[Text] [nvarchar](100) NOT NULL,
    CONSTRAINT [PK_ReceivedStrings] PRIMARY KEY CLUSTERED (
	    [Text] ASC
    )
)

";
    }
}