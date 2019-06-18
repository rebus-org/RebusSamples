## PubSub Native

This sample demonstrates how a publisher can publish messages to two subscribers, using the native
capability of the transport to distribute copies to each subscriber.

NOTE: You need to go to each app.config and add a valid Azure Service Bus connection string, before the
sample runs.

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
beneath it will forward received messages to the caller's input queue (`subscriber2` in this case).

Naturally, when someone then calls
```csharp
await bus.Publish(new SomeEvent(...));
```
the event will be sent to the topic, causing a copy of it to be distributed to each subscriber.

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
