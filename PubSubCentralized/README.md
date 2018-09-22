## PubSub Centralized

This sample demonstrates how a publisher can publish messages to two subscribers, using a central
shared subscription storage to manage subscriptions.

Contrary to [the PubSub sample](/PubSub), it is not required that the publisher/subscribers be
started in any particular order, and no endpoint mappings are needed.

You need a local SQL Server database called RebusPubSubCentralized for this demo to work.