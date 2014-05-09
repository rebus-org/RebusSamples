## MessageBus

This sample is NOT your typical .NET service bus sample  - it's a POC that three independent endpoints can pub/sub the same messages without knowing of each other's existence beforehand, using only a shared central subscription storage in order to register for messages of a particular type.