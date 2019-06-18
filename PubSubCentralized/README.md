## PubSub Centralized

This sample demonstrates how a publisher can publish messages to two subscribers, using a central
shared subscription storage to manage subscriptions.

Contrary to [the PubSub sample](/PubSub), it is not required that the publisher/subscribers be
started in any particular order, and no endpoint mappings are needed.

You need a local SQL Server database called RebusPubSubCentralized for this demo to work.

The reason a "subscription storage" is needed, is because MSMQ is used as the transport,
and it does not support pub/sub natively.

### Azure Service Bus

For example, if you're using Azure Service Bus, Rebus will leverage its built-in topic-based
routing for publishing events.

Therefore, when using Azure Service Bus the `.Subscriptions(...)` part of the configuration is not
needed, and the configuration can e.g. be changed into this:
```csharp
Configure.With(activator)
    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
    .Transport(t => t.UseAzureServiceBus(connectionString, "subscriber2"))
    .Start();
```

Rebus will then create topics and bindings as needed, so when you
```csharp
await bus.Subscribe<SomeEvent>();
```
a topic named after the assembly, namespace, and type name will be created, and a subscription
beneath it will forward received messages to the caller's input queue.

Naturally, when someone then calls
```csharp
await bus.Publish(new SomeEvent(...));
```
the event will be send to the topic, causing a copy of it to be distributed to each subscriber.

### RabbitMQ

If you're using RabbitMQ, Rebus will work in a similar fashion as when using Azure Service Bus - the
configuration just needs to be changed into something like
```csharp
Configure.With(activator)
    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
    .Transport(t => t.UseRabbitMq(connectionString, "subscriber2"))
    .Start();
```
and then it will work the same way.
