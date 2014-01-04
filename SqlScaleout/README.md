## Scaleout with SQL Server

The purpose of this sample is to demonstrate how Rebus can easily distribute work when using SQL Server as the transport layer.

In order to run the sample, you need to have access to a SQL Server, which is assumed to be running locally as the default instance, containing a database called 'rebus_test'. A table, 'RebusMessages', will automatically be created in the database.

Naturally, you're free to edit the connection string in both the producer and the consumer.

The sample can be run by starting the producer and any number of consumers. Then you'll see how jobs published by the producer will be distributed to all the consumers.

Please note that since the SQL Server transport is based on polling the message table, it will back off its polling when no messages are found. Therefore, when sending a small batch of messages when the consumers are idle, some consumers might miss the fact that messages were available, which may seem like the consumers aren't properly competing.
