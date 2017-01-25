## Scaleout with PostgreSQL

The purpose of this sample is to demonstrate how Rebus can easily distribute work when using PostgreSQL as the transport layer.

In order to run the sample, you need to have access to a PostgreSQL, which is assumed to be running locally as the default instance, containing a database called 'rebus2_test'.  A table, 'messages', will automatically be created in the database. I have PostgreSQL running on port 5433, but the standard port is 5432, so please bear this in mind.

Naturally, you're free to edit the connection string in both the producer and the consumer.

The sample can be run by starting the producer and any number of consumers. Then you'll see how jobs published by the producer will be distributed to all the consumers.

Unless you change it, each consumer will have one single thread processing messages, but `SetMaxParallelism` is used to enable up to 20 messages to be processes in parallel.

Please note that since the SQL Server transport is based on polling the message table, it will back off its polling when no messages are found. Therefore, when sending a small batch of messages when the consumers are idle, some consumers might miss the fact that messages were available, which may seem like the consumers aren't properly competing.
