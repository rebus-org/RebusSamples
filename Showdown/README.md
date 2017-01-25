## Showdown

The Showdown is a simple test that can be run for different transports.

It will first send a bunch of messages.

And then it will receive all those messages.

And then it will print how long it took to send/receive.

## Example

Running it with the MSMQ transport on my machine looks like this:

	----------------------------------------------------------------------
	Running showdown: Rebus.Transports.Showndown.Msmq
	----------------------------------------------------------------------
	Stopping all workers in receiver
	Sending 100000 messages from sender to receiver
	Sent 3258 messages. Received 0 messages.
	Sent 8062 messages. Received 0 messages.
	Sent 12866 messages. Received 0 messages.
	Sent 17679 messages. Received 0 messages.
	Sent 22518 messages. Received 0 messages.
	Sent 27350 messages. Received 0 messages.
	Sent 31668 messages. Received 0 messages.
	Sent 35715 messages. Received 0 messages.
	Sent 40412 messages. Received 0 messages.
	Sent 45009 messages. Received 0 messages.
	Sent 49673 messages. Received 0 messages.
	Sent 54554 messages. Received 0 messages.
	Sent 59428 messages. Received 0 messages.
	Sent 64364 messages. Received 0 messages.
	Sent 69272 messages. Received 0 messages.
	Sent 74143 messages. Received 0 messages.
	Sent 79026 messages. Received 0 messages.
	Sent 83929 messages. Received 0 messages.
	Sent 88817 messages. Received 0 messages.
	Sent 93740 messages. Received 0 messages.
	Sent 98654 messages. Received 0 messages.
	Sending 100000 messages took 210,9 s (474,2 msg/s)
	Starting receiver with 15 workers
	Sent 100000 messages. Received 13678 messages.
	Sent 100000 messages. Received 32318 messages.
	Sent 100000 messages. Received 50994 messages.
	Sent 100000 messages. Received 69978 messages.
	Sent 100000 messages. Received 89203 messages.
	Receiving 100000 messages took 52,9 s (1889,4 msg/s)
	Press any key to continue . . .
