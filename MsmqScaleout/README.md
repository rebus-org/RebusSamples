## Scaleout with MSMQ

The purpose of this sample is to demonstrate how Rebus can distribute work when using MSMQ as the transport layer.

In order to run the sample, please make sure you INITIALLY run the processes in the following order:

* Consumer1
* Consumer2
* Distributor
* Producer

thus ensuring that all the necessary queues are in place. When this has been done once, the order does not matter anymore.

Work is distributed by having each consumer use its own input queue, "consumer1" and "consumer2" respectively, and then the distributor is configured by calling `AddDestinationQueue` with these two queue names.

The distributor itself uses "distributor" as its input queue, which means that the producer must send its work to that queue.