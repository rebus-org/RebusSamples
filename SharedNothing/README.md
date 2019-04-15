## SharedNothing

This sample demonstrates how to separate processes can communicate over message queues, without sharing anything - not even a DLL with message types!

The JSON "interoperability mode" is set to "pure JSON" in both ends, meaning that objects are JSON-serialized, but not deserialized by Rebus' built-in
JSON serializer. We then decorate the serializer in the subscriber to "spice it up", in this case adding some special deserialization support for known
types (in this case it's the publisher's `GreetingWasEntered_Publisher` type, which we deserialize into `GreetingWasEntered_Subscriber`).

