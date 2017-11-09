## Welcome to the Rebus Samples repository

Check out

* [Time printer](/TimePrinter) - simple program that sends a message to itself every second
* [Email sender](/EmailSender) - simple email sender that functions as a good example on how to queue up work
* [Pub/sub sample](/PubSub) - demonstrates how pub/sub can be wired up
* [Integration sample](/Integration) - demonstrates how calling an external web service can be made more robust
* [Unit of work sample](/UnitOfWork) - demonstrates how a proper unit of work can be hooked into Rebus in all the right places
* [Rabbit MQ topics sample](/RabbitTopics) - demonstrates how Rebus' low-level topics API can be used with the RabbitMQ transport to do pub/sub with wildcards
* [OWIN Web Host](/OwinWebHost) - demonstrates how Rebus can be configured to host an OWIN endpoint
* [Showdown](/Showdown) - sample that can send/receive a bunch of messages and measure the time it takes
* [Sagas](/Sagas) - sample that shows a saga
* [SimpleInjector](/SimpleInjector) - sample that shows how SimpleInjector works with Rebus
* [Logging](/Logging) - sample with various types of logging
* [SqlAllTheWay](/SqlAllTheWay) - sample that demonstrates "exactly once delivery" with SQL transport and user work enlisted in same transaction
* [MessageBus](/MessageBus) - demonstrates how tree totally independent endpoints can do pub/sub with a central database being their only connection
* [RequestReply](/RequestReply) - demonstrates how a client can send a request to a server, which then can reply back to the client

### Scaleout samples

* [Rabbit scaleout](/RabbitScaleout) - demonstrates how work can be easily distributed among a cluster of workers when working with RabbitMQ
* [SQL Server scaleout](/SqlScaleout) - demonstrates how work can be easily distributed among a cluster of workers when working with SQL Server as the transport
* [PostgreSQL scaleout](/PostgreSqlScaleout) - demonstrates how work can be easily distributed among a cluster of workers when working with PostgreSQL as the transport

## Deprecated

* [User context sample](/old/UserContextHeaders) - shows how an ambient user context can be passed along with messages 
* [MSMQ scaleout](/old/MsmqScaleout) - demonstrates how work can be distributed among a cluster of workers when working with MSMQ and Rebus' MSMQ distributor

