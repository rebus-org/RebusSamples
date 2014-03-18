## User context headers

The purpose of this sample is to demonstrate how Rebus' hooks and message headers can be used to pass a user context around in a transparent fashion.

Things to take note of in this project:

* A special `AmbientUserContext` can be used to assign an ambient user context to the thread.
* An extension method, `AutomaticallyTransferUserContext`, is used to make the configuration reusable and pretty.
* The actual "magic" happens by hooking into two events: `MessageSent` (for attaching an ambient user context to the outgoing messages), and `MessageContextEstablished` (for detecting the user context header in the message being handled, making it available in the message context)
* A pretty extension method on `IMessageContext` called `GetUserContext` can be used to extract the current user context.
* Castle Windsor is configured to be able to inject the current user context.

Apart from the `while(true) { ... }` thing going on in `Program`, all of this code is very realistic, and I've had good results doing this kind of thing.