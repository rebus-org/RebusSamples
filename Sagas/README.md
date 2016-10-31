## Sagas

This sample demonstrates how sagas can be made with Rebus.

Sagas (also known in the literature as "[process managers](http://www.enterpriseintegrationpatterns.com/patterns/messaging/ProcessManager.html)") are stateful services. You can think of them as state machines whose transitions are driven by messages.

With Rebus, you build a saga by creating a handler that is derived from the generic `Saga&lt;TSagaData&gt;` class, closing it with the type of your saga data. The "saga data" is the chunk of state, which gets automatically saved between handling messages, and thus represents the state of the state machine.

Sagas don't do anything that you could not have built yourself with a database and ordinary message handlers. They just save you the time it takes to handle

* persistence
* correlation
* concurrency

in a meaningful way that is also robust.

### How to run the demo

The `Common` project has an extension method, `ConfigureEndpoint`, which is used throughout in ordert to configure each Rebus endpoint. The method accepts an `EndpointRole` parameter,
which is used by the extension method to decide which things to configure for that particular endpoint.

Unless you change this method, you will need a local SQL Server with a database named `rebus_sagademo` and MSMQ installed.

You should then start the following three console applications:

* Client
* SagaDemo
* Logger

### What does the demo show?

The demo shows an imaginary scenario in a domain where we have cases. 

A case is identified by its case number.

A case is about some amount of money that needs to be paid out at some point in time, but the payout can only be made when the following things have happened:

* the amounts to be paid out have been calculated
* the taxes part has been deducted
* the recipient of the payout has indicated which way she/he wishes to receive the payout

When these three things have happened (i.e. we have received `AmountsCalculated`, `TaxesCalculated`, and `PayoutMethodSelected` events for a case), we must publish an appropriate `PayoutReady` event.

One possible sequence could be this:

                                    +----------+
                                    | SagaDemo |
                                    +----------+
                                         |
                                         |
     AmountsCalculated (case: 123)       |
    ------------------------------------>I (process begins)
                                         I
                                         I 
     TaxesCalculated (case: 123)         I 
    ------------------------------------>I
                                         I
                                         I
     PayoutMethodSelected (case: 123)    I
    ------------------------------------>I   PayoutReady (case: 123)
                                         I--------------------------->
                                         X (process terminates)
                                         |
                                         |
    
where it can be seen that a process of sorts is kicked off when the first event arrives, and then the process receives the other two neessary events, and finally the `PayoutReady` is published when all three events have been received for that particular case number.

In the real world, messages can be delayed for different reasons. When you want to build robust systems, you should NOT rely on the order of events (too much), so we want to make the process infifferent to the order in which the three events are received.

This means that we must support a state machine with transitions for all possible permutations of the tree events. We could draw it like this:

                                                           PayoutMethodSelected
                                               -------------------------------------------
                                               |                                         |
                                          +-------+                          +-------+   |
                       AmountsCalculated  | A: 1  |  TaxesCalculated         | A: 1  |   | PayoutMethodSelected
                 ------------------------>| T: 0  |------------------------->| T: 1  |---)------------------------
                 |                        | P: 0  |                      |   | P: 0  |   |                        |
                 |                        +-------+   AmountsCalculated  |   +-------+   |                        |
                 |                                   ---------------------               |                        |
                 |                                   |                                   |                        V
             +-------+                    +-------+---                       +-------+   |                     +-------+
      Start  | A: 0  | TaxesCalculated    | A: 0  | PayoutMethodSelected     | A: 0  |   |  AmountsCalculated  | A: 1  |  End
    O------->| T: 0  |------------------->| T: 1  |------------------------->| T: 1  |---)---------------------| T: 1  |------>Ø
             | P: 0  |                    | P: 0  |                      |   | P: 1  |   |                     | P: 1  |
             +-------+                    +-------+    TaxesCalculated   |   +-------+   |                     +-------+
                 |                                   ---------------------       ________|                        A
                 |                                   |                           V                                |
                 |                        +-------+---                       +-------+                            |
                 |  PayoutMethodSelected  | A: 0  |  AmountsCalculated       | A: 1  |   TaxesCalculated          |
                 ------------------------>| T: 0  |------------------------->| T: 0  |----------------------------
                                          | P: 1  |                          | P: 1  |
                                          +-------+                          +-------+

with an ASCII-adaptation of the formal UML State Diagram notation, or we could simply spot the pattern that we apparently remember which of the three events we have seen, finishing the state machine when we have received all three. If we accept that the state of the
state machine is represented by the `(A, T, P)` tuple shown in the box, we could represent the machine like this:

         AmountsCalculated
          TaxesCalculated
        PayoutMethodSelected
               -----
               |   |
               |   |
               |   V
             +-------+
      Start  | A: 0  |  End
    O------->| T: 0  |------>Ø
             | P: 0  |
             +-------+

Now, obviously we will have to track the three events for each case number that the process runs for. This means that an instance of the state machine must be stored for each case number.

With Rebus, this saga's state could be coded like this:

    public class PayoutSagaData : SagaData
    {
        public string CaseNumber { get; set; }

        public bool AmountsCalculated { get; set; }
        public bool TaxesCalculated { get; set; }
        public bool PayoutMethodSelected { get; set; }

        public bool IsDone => AmountsCalculated
                              && TaxesCalculated
                              && PayoutMethodSelected;
    }

and then the saga could be defined like this:

    public class PayoutSaga : Saga<PayoutSagaData>, 
		IAmInitiatedBy<AmountsCalculated>, 
		IAmInitiatedBy<TaxesCalculated>, 
		IAmInitiatedBy<PayoutMethodSelected>
    {
        protected override void CorrelateMessages(ICorrelationConfig<PayoutSagaData> config)
        {
            config.Correlate<AmountsCalculated>(m => m.CaseNumber, d => d.CaseNumber);
            config.Correlate<TaxesCalculated>(m => m.CaseNumber, d => d.CaseNumber);
            config.Correlate<PayoutMethodSelected>(m => m.CaseNumber, d => d.CaseNumber);
        }

		// (....)
	}

Note how the `CorrelateMessages` method sets up correlation between incoming messages by specifying which field of the saga data's state to retrieve an instance by,
and how `IAmInitiatedBy` is used instead of the ordinary `IHandleMessages` meaning that all three messages will result in a new instance of the saga if an existing instance could not be found.