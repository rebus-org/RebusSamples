## MessageBus

This sample demonstrates how a single, centralized subscription storage in SQL Server can be used to enable pub/sub between any number of endpoints
without the need to map any of them to eachother.

Each endpoint can subscribe/unsubscribe independently by doing a `await bus.Subscribe<string>()`, and since the subscription storage is configured
to be "centralized", the subscription is established directly.

Whenever an endpoint publishes a `System.String`, all the current subscribers will get a copy of the message.

NOTE: The sample requires the presence of a local database named `messagebus`, unless you change the connection string.
