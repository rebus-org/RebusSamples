## MessageBus

This sample is NOT your typical .NET service bus sample  - it's a POC that three independent endpoints 
can pub/sub the same messages without knowing of each other's existence beforehand, using only a shared 
central subscription storage in order to register for messages of a particular type.

A single centralized subscription storage is provided by the XML file subscription storage, which will 
use a file beneath the current user's local app data to store subscriptions - e.g. something like 
`C:\Users\WhateverYourNameIs\AppData\Local\RebusSamples\MessageBus\subscriptions.xml`.

Each endpoint can subscribe/unsubscribe independently by doing a `bus.Subscribe<string>()` which
underneath the covers sends a subscription request to the owner of `System.String`. Each endpoint
believes that it is the owner of `System.String`, which will cause itself to receive the subscription
request, thereby persisting the subscription in the XML file.

Whenever an endpoint publishes a `System.String`, all the current subscribers will get a copy of
the message.
