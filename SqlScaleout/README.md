## Scaleout with RabbitMQ

The purpose of this sample is to demonstrate how Rebus can easily distribute work when using RabbitMQ as the transport layer.

In order to run the sample, you need to have access to a RabbitMQ server, which is assumed to be running on the default port on localhost.

If that is not the case, you need to edit the connection string in both the producer and the consumer.

The sample can be run by starting the producer and any number of consumers. Then you'll see how jobs published by the producer will be distributed to all the consumers.